using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSource : MonoBehaviour {

    public Resource.Type type;
    public int amount;

    public static ResourceSource GetClosest(Vector3 position, Resource.Type type) {
        ResourceSource closest = null;
        var closestDistance = float.MaxValue;
        foreach (var source in FindObjectsOfType<ResourceSource>()) {
            if (source.type != type)
                continue;
            var dist = (position - source.transform.position).sqrMagnitude;
            if (dist < closestDistance) {
                closestDistance = dist;
                closest = source;
            }
        }
        return closest;
    }

    public void Mine() {
        this.amount--;
        if (this.amount <= 0)
            Destroy(this.gameObject);
    }

    private void OnDestroy() {
        if (AstarPath.active)
            AstarPath.active.Scan();
    }

}