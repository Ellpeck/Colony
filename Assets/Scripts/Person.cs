using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

    public float chopSpeed;
    public float maxTargetDistance;
    public int maxCarryAmount;
    public float maxNextSourceDistance;

    public Resource CarryingResource { get; private set; }
    private Commandable commandable;
    private ResourceSource interactingSource;
    private Resource.Type? resourceToBeGathered;
    private float chopTimer;

    private void Start() {
        this.commandable = this.GetComponent<Commandable>();
        this.commandable.onTargetReached += this.OnTargetReached;
        this.commandable.onCommandReceived += this.OnCommandReceived;
    }

    private void Update() {
        if (!this.commandable.IsBusy() && this.resourceToBeGathered != null) {
            if (!this.interactingSource) {
                // if the resource we are gathering has depleted, find a new resource close by
                var next = ResourceSource.GetClosest(this.transform.position, this.resourceToBeGathered.Value, this.maxNextSourceDistance);
                if (next) {
                    this.commandable.MoveTo(next.gameObject);
                } else {
                    this.resourceToBeGathered = null;
                }
            } else if (this.IsInRange(this.interactingSource.transform.position)) {
                // gather the resource
                this.chopTimer += Time.deltaTime;
                if (this.chopTimer >= this.chopSpeed) {
                    this.chopTimer = 0;
                    if (!this.Carry(this.interactingSource.type, 1)) {
                        // carry items to a building because we are full
                        this.StoreCarrying();
                    } else {
                        // mine the resource
                        this.interactingSource.Mine();
                    }
                }
            } else {
                this.commandable.MoveTo(this.interactingSource.gameObject);
            }
        }
    }

    public bool IsBusy() {
        return this.commandable.IsBusy() || this.resourceToBeGathered != null;
    }

    private bool IsInRange(Vector2 position) {
        return Vector2.Distance(this.transform.position, position) <= this.maxTargetDistance;
    }

    private void OnTargetReached(GameObject destination) {
        if (destination == null || !this.IsInRange(destination.transform.position))
            return;

        // interact with building
        var building = destination.GetComponent<Building>();
        if (building) {
            if (this.CarryingResource != null && building.storeableTypes.Contains(this.CarryingResource.type)) {
                ResourceManager.Instance.Add(this.CarryingResource.type, this.CarryingResource.amount);
                this.CarryingResource = null;
            }
        }

        // interact with resource source
        var source = destination.GetComponent<ResourceSource>();
        if (source) {
            this.resourceToBeGathered = source.type;
            this.interactingSource = source;
        }
    }

    private void OnCommandReceived(Vector2 destination, GameObject destinationObj, bool fromPlayer) {
        if (fromPlayer) {
            this.resourceToBeGathered = null;
            this.interactingSource = null;
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

    private void StoreCarrying() {
        if (this.CarryingResource == null)
            return;
        Building.BuildingFilter filter = building => building.storeableTypes.Contains(this.CarryingResource.type);
        var closest = Building.GetClosest(this.transform.position, filter);
        if (closest)
            this.commandable.MoveTo(closest.gameObject);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, this.maxTargetDistance);
    }

}