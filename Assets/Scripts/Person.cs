using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

    public float chopSpeed;

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