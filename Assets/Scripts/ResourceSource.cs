using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceSource : MonoBehaviour {

    public Resource.Type type;
    public int amount;

    public static ResourceSource GetClosest(Vector3 position, float maxDistance, params Resource.Type[] types) {
        ResourceSource closest = null;
        var closestDistance = float.MaxValue;
        foreach (var source in FindObjectsOfType<ResourceSource>()) {
            if (!types.Contains(source.type) || !WorldGenerator.Instance.IsDiscovered(source.transform.position))
                continue;
            var dist = (position - source.transform.position).sqrMagnitude;
            if (dist < closestDistance && dist < maxDistance * maxDistance) {
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

}