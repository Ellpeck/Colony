using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePathfindingGraph : MonoBehaviour {

    public bool updateOnDestroy;
    public float boundExpand = 1;

    private new Collider2D collider;

    private void Start() {
        this.collider = this.GetComponent<Collider2D>();
    }

    public void UpdateGraph() {
        if (AstarPath.active) {
            var bounds = this.collider.bounds;
            bounds.Expand(this.boundExpand);
            AstarPath.active.UpdateGraphs(bounds);
        }
    }

    private void OnDestroy() {
        if (this.updateOnDestroy)
            this.UpdateGraph();
    }

}