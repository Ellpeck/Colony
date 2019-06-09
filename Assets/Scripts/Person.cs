using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

    public float chopSpeed;
    public float maxTargetDistance;
    public int maxCarryAmount;

    private Commandable commandable;
    private ChoppableTree interactingTree;
    private Resource carryingResource;
    private GameObject pausedInteraction;

    private void Start() {
        this.commandable = this.GetComponent<Commandable>();
        this.commandable.onTargetReached += this.OnTargetReached;
        this.commandable.onCommandReceived += this.OnCommandReceived;
    }

    private void OnTargetReached(GameObject destination) {
        if (destination == null)
            return;
        if (Vector2.Distance(this.transform.position, destination.transform.position) > this.maxTargetDistance)
            return;

        var building = destination.GetComponent<Building>();
        if (building && building.type == Building.Type.TownCenter) {
            ResourceManager.Instance.Add(this.carryingResource.type, this.carryingResource.amount);
            this.carryingResource = null;
        }

        if (this.pausedInteraction != null) {
            this.commandable.MoveTo(this.pausedInteraction);
            this.pausedInteraction = null;
            return;
        }

        this.interactingTree = destination.GetComponent<ChoppableTree>();
        if (this.interactingTree)
            this.StartCoroutine(this.ChopWood());
    }

    private void OnCommandReceived(Vector2 destination, GameObject destinationObj) {
        this.interactingTree = null;
    }

    private IEnumerator ChopWood() {
        while (this.interactingTree) {
            if (!this.Carry(Resource.Type.Wood, 1)) {
                this.pausedInteraction = this.interactingTree.gameObject;
                this.StoreCarrying();
            } else {
                this.interactingTree.Chop();
            }
            yield return new WaitForSeconds(this.chopSpeed);
        }
    }

    private bool Carry(Resource.Type type, int amount) {
        if (this.carryingResource != null) {
            if (this.carryingResource.type != type)
                return false;
            if (this.carryingResource.amount >= this.maxCarryAmount)
                return false;
        } else {
            this.carryingResource = new Resource {
                type = type
            };
        }
        this.carryingResource.amount += amount;
        return true;
    }

    private void StoreCarrying() {
        if (this.carryingResource == null)
            return;
        var closest = Building.GetClosestBuilding(this.transform.position, Building.Type.TownCenter);
        if (closest)
            this.commandable.MoveTo(closest.gameObject);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, this.maxTargetDistance);
    }

}