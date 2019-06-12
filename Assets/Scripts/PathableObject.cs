using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathableObject : MonoBehaviour {

    public bool updateOnDestroy;
    public float boundExpand = 1;

    public PolygonCollider2D Collider { get; private set; }

    private void Awake() {
        this.Collider = this.GetComponent<PolygonCollider2D>();
    }

    public void UpdateGraph() {
        if (AstarPath.active) {
            var bounds = this.Collider.bounds;
            bounds.Expand(this.boundExpand);
            AstarPath.active.UpdateGraphs(bounds);
        }
    }

    public Vector2 GetPathPoint(Vector2 from) {
        var closest = this.Collider.bounds.ClosestPoint(from);
        return closest;
    }

    private void OnDestroy() {
        if (this.updateOnDestroy)
            this.UpdateGraph();
    }

}