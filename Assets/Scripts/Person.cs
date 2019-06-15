using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Person : MonoBehaviour {

    public string savedPrefabName;
    public float maxTargetDistance;
    public int maxCarryAmount;
    public float maxNextSourceDistance;

    public Resource CarryingResource { get; private set; }
    private Commandable commandable;
    private ResourceSource interactingSource;
    private Resource.Type? resourceToBeGathered;
    private Building constructingBuilding;
    private bool shouldContructBuilding;
    private bool isWaitingForBuildingResources;
    private float actionTimer;

    private void Awake() {
        this.commandable = this.GetComponent<Commandable>();
    }

    public void GetInteractionItems(List<GameObject> items) {
        var manager = ResourceManager.Instance;
        foreach (var building in manager.placeableBuildings) {
            var inst = Instantiate(manager.buildInstruction);
            inst.buildingToPlace = building;
            items.Add(inst.gameObject);
        }
    }

    private void Update() {
        if (this.commandable.IsBusy())
            return;
        if (this.resourceToBeGathered != null) {
            if (!this.interactingSource) {
                // if the resource we are gathering has depleted, find a new resource close by
                var next = ResourceSource.GetClosest(this.transform.position, this.maxNextSourceDistance, this.resourceToBeGathered.Value);
                if (next) {
                    this.MoveTo(next.gameObject);
                } else {
                    this.resourceToBeGathered = null;
                }
            } else if (this.IsInRange(this.interactingSource.gameObject)) {
                // gather the resource
                this.actionTimer += Time.deltaTime;
                if (this.actionTimer >= TownStats.Instance.chopSpeed) {
                    this.actionTimer = 0;
                    if (!this.Carry(this.interactingSource.type, 1)) {
                        // carry items to a building because we are full
                        var closest = Building.GetClosest(this.transform.position, building =>
                            building.storeableTypes.Contains(this.CarryingResource.type));
                        if (closest)
                            this.MoveTo(closest.gameObject);
                    } else {
                        // mine the resource
                        this.interactingSource.Mine();
                    }
                }
            } else {
                this.MoveTo(this.interactingSource.gameObject);
            }
        } else if (this.shouldContructBuilding) {
            // if the building we're constructing is finished, then our work is done
            if (!this.constructingBuilding || this.constructingBuilding.IsFinished) {
                if (this.constructingBuilding && this.constructingBuilding.storeableTypes.Length > 0) {
                    // if there's anything to gather after building, do it
                    var next = ResourceSource.GetClosest(this.transform.position, this.maxNextSourceDistance, this.constructingBuilding.storeableTypes);
                    if (next)
                        this.MoveTo(next.gameObject);
                    this.shouldContructBuilding = false;
                } else {
                    // otherwise find the next building to work on
                    var next = Building.GetClosest(this.transform.position, null, false, this.maxNextSourceDistance);
                    if (next) {
                        this.MoveTo(next.gameObject);
                    } else {
                        this.shouldContructBuilding = false;
                    }
                }
                this.constructingBuilding = null;
            }
            // construct building if it is in range
            else if (this.IsInRange(this.constructingBuilding.gameObject)) {
                this.actionTimer += Time.deltaTime;
                if (this.actionTimer >= TownStats.Instance.buildSpeed) {
                    this.actionTimer = 0;
                    // if the building can be fed the current resource
                    if (this.CarryingResource != null && this.constructingBuilding.FeedResource(this.CarryingResource.type)) {
                        // deposit an item in it
                        this.Deposit(1);
                    } else {
                        // otherwise, find a building that has the required resources
                        bool Filter(Building building) {
                            foreach (var res in this.constructingBuilding.requiredResources) {
                                if (!building.storeableTypes.Contains(res.type))
                                    continue;
                                if (ResourceManager.Instance.GetResourceAmount(res.type) > 0)
                                    return true;
                            }
                            return false;
                        }

                        var closest = Building.GetClosest(this.transform.position, Filter);
                        if (closest) {
                            this.MoveTo(closest.gameObject);
                            this.isWaitingForBuildingResources = false;
                        } else {
                            this.isWaitingForBuildingResources = true;
                        }
                    }
                }
            } else {
                this.MoveTo(this.constructingBuilding.gameObject);
            }
        }
    }

    public string GetActivity() {
        if (this.resourceToBeGathered != null)
            return "Gathering " + this.resourceToBeGathered;
        if (this.constructingBuilding) {
            if (this.isWaitingForBuildingResources)
                return "Waiting for resources";
            return "Constructing " + this.constructingBuilding.Selectable.menuName;
        }
        return this.commandable.IsBusy() ? "On their way" : "Doing nothing";
    }

    public void MoveTo(GameObject destination) {
        this.commandable.MoveTo(destination);
    }

    public bool IsBusy() {
        return this.commandable.IsBusy()
               || this.resourceToBeGathered != null
               || this.shouldContructBuilding && !this.isWaitingForBuildingResources;
    }

    private bool IsInRange(GameObject destination) {
        Vector2 position;
        var pathable = destination.GetComponentInChildren<PathableObject>();
        if (pathable) {
            position = pathable.GetPathPoint(this.transform.position);
        } else {
            position = destination.transform.position;
        }
        return Vector2.Distance(this.transform.position, position) <= this.maxTargetDistance;
    }

    public void OnTargetReached(GameObject destination) {
        if (destination == null || !this.IsInRange(destination))
            return;

        // interact with building
        var building = destination.GetComponent<Building>();
        if (building) {
            if (building.IsFinished) {
                // store the resource we are currently holding
                if (this.CarryingResource != null && building.storeableTypes.Contains(this.CarryingResource.type)) {
                    ResourceManager.Instance.Add(this.CarryingResource.type, this.CarryingResource.amount);
                    this.CarryingResource = null;
                }

                // if a building is currently being constructed, pick up the required ingredients
                if (this.constructingBuilding != null) {
                    foreach (var res in this.constructingBuilding.requiredResources) {
                        if (!building.storeableTypes.Contains(res.type))
                            continue;
                        var taken = ResourceManager.Instance.Take(res.type, Mathf.Min(res.amount, this.maxCarryAmount));
                        if (taken > 0) {
                            this.CarryingResource = new Resource {
                                type = res.type,
                                amount = taken
                            };
                            break;
                        }
                    }
                }
            } else {
                this.shouldContructBuilding = true;
                this.constructingBuilding = building;
            }
            return;
        }

        // interact with resource source
        var source = destination.GetComponent<ResourceSource>();
        if (source) {
            this.resourceToBeGathered = source.type;
            this.interactingSource = source;
        }
    }

    public void OnCommandReceived(Vector2 destination, GameObject destinationObj, bool fromPlayer) {
        if (fromPlayer) {
            this.resourceToBeGathered = null;
            this.interactingSource = null;
            this.shouldContructBuilding = false;
            this.constructingBuilding = null;
        }
    }

    private bool Carry(Resource.Type type, int amount) {
        if (this.CarryingResource != null) {
            if (this.CarryingResource.type != type)
                return false;
            if (this.CarryingResource.amount >= this.maxCarryAmount)
                return false;
        } else {
            this.CarryingResource = new Resource {
                type = type
            };
        }
        this.CarryingResource.amount += amount;
        return true;
    }

    private bool Deposit(int amount) {
        if (this.CarryingResource == null)
            return false;
        this.CarryingResource.amount--;
        if (this.CarryingResource.amount <= 0)
            this.CarryingResource = null;
        return true;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, this.maxTargetDistance);
    }

    [Serializable]
    public class Data {

        public string prefabName;
        public SerializableVec3 position;
        public Resource carryingResource;
        public Resource.Type? resourceToBeGathered;
        public bool shouldConstructBuilding;

        public Data(Person person) {
            this.prefabName = person.savedPrefabName;
            this.position = person.transform.position;
            this.carryingResource = person.CarryingResource;
            this.resourceToBeGathered = person.resourceToBeGathered;
            this.shouldConstructBuilding = person.shouldContructBuilding;
        }

        public void Load(Person person) {
            person.transform.position = this.position;
            person.CarryingResource = this.carryingResource;
            person.resourceToBeGathered = this.resourceToBeGathered;
            person.shouldContructBuilding = this.shouldConstructBuilding;
        }

    }

}