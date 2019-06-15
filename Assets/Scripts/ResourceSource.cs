using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceSource : MonoBehaviour {

    public string savedPrefabName;
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
    
    [Serializable]
    public class Data {

        public string prefabName;
        public SerializableVec3 position;
        public int type;
        public int amount;

        public Data(ResourceSource source) {
            this.prefabName = source.savedPrefabName;
            this.position = source.transform.position;
            this.type = (int) source.type;
            this.amount = source.amount;
        }

        public void Load(ResourceSource source) {
            source.transform.position = this.position;
            source.type = (Resource.Type) this.type;
            source.amount = this.amount;
        }

    }
}