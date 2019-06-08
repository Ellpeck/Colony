using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

    public float chopSpeed;
    public float maxTargetDistance;

    private Commandable commandable;
    private ChoppableTree interactingTree;

    private void Start() {
        this.commandable = this.GetComponent<Commandable>();
        this.commandable.onTargetReached += this.OnGoalReached;
        this.commandable.onCommandReceived += this.OnCommandReceived;
    }

    private void OnGoalReached(Selectable destination) {
        if (destination == null)
            return;
        if (Vector2.Distance(this.transform.position, destination.transform.position) > this.maxTargetDistance)
            return;
        this.interactingTree = destination.GetComponent<ChoppableTree>();
        if (this.interactingTree)
            this.StartCoroutine(this.ChopWood());
    }

    private void OnCommandReceived(Vector2 destination, Selectable destinationSelectable) {
        this.interactingTree = null;
    }

    private IEnumerator ChopWood() {
        while (this.interactingTree) {
            this.interactingTree.Chop();
            yield return new WaitForSeconds(this.chopSpeed);
        }
    }

}