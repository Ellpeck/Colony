using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour {

    public Type type;
    public List<Resource.Type> storeableTypes;
    public int discoveryRadius;

    private void Start() {
        WorldGenerator.Instance.Discover(this.transform.position, this.discoveryRadius);
    }

    public static Building GetClosest(Vector3 position, BuildingFilter filter = null) {
        Building closest = null;
        var closestDistance = float.MaxValue;
        foreach (var building in FindObjectsOfType<Building>()) {
            if (filter != null && !filter(building))
                continue;
            var dist = (position - building.transform.position).sqrMagnitude;
            if (dist < closestDistance) {
                closestDistance = dist;
                closest = building;
            }
        }
        return closest;
    }

    public delegate bool BuildingFilter(Building building);

    public enum Type {

        TownCenter

    }

}