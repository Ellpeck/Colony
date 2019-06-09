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
    private GameObject pausedInteraction;
    private ResourceSource interactingSource;
    private bool stopCurrentInteraction;

    private void Start() {
        this.commandable = this.GetComponent<Commandable>();
        this.commandable.onTargetReached += this.OnTargetReached;
        this.commandable.onCommandReceived += this.OnCommandReceived;
    }

    public bool IsBusy() {
        return this.commandable.IsBusy() || this.interactingSource;
    }

    private void OnTargetReached(GameObject destination) {
        if (destination == null)
            return;
        if (Vector2.Distance(this.transform.position, destination.transform.position) > this.maxTargetDistance)
            return;

        // interact with building
        var building = destination.GetComponent<Building>();
        if (building) {
            if (this.CarryingResource != null && building.storeableTypes.Contains(this.CarryingResource.type)) {
                ResourceManager.Instance.Add(this.CarryingResource.type, this.CarryingResource.amount);
                this.CarryingResource = null;
            }
        }

        // continue a paused interaction (from carrying items to a building)
        if (this.pausedInteraction != null) {
            this.commandable.MoveTo(this.pausedInteraction);
            this.pausedInteraction = null;
            return;
        }

        // interact with resource source
        this.interactingSource = destination.GetComponent<ResourceSource>();
        if (this.interactingSource)
            this.StartCoroutine(this.InteractWithSource());
    }

    private void OnCommandReceived(Vector2 destination, GameObject destinationObj) {
        this.stopCurrentInteraction = true;
        this.pausedInteraction = null;
    }

    private IEnumerator InteractWithSource() {
        this.stopCurrentInteraction = false;
        while (!this.stopCurrentInteraction) {
            var type = this.interactingSource.type;
            if (!this.Carry(type, 1)) {
                // pause the interaction and carry items to a building because we are full
                this.StoreCarrying();
                this.pausedInteraction = this.interactingSource.gameObject;
            } else {
                // mine the resource
                this.interactingSource.Mine();
            }

            yield return new WaitForSeconds(this.chopSpeed);

            // if the resource was depleted, find a new resource close by
            if (!this.interactingSource) {
                var next = ResourceSource.GetClosest(this.transform.position, type);
                if (next && Vector2.Distance(this.transform.position, next.transform.position) <= this.maxNextSourceDistance)
                    this.commandable.MoveTo(next.gameObject);
            }
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