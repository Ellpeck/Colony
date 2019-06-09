﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour {

    public Type type;

    public static Building GetClosestBuilding(Vector3 position, params Type[] types) {
        Building closest = null;
        var closestDistance = float.MaxValue;
        foreach (var building in FindObjectsOfType<Building>()) {
            if (!types.Contains(building.type))
                continue;
            var dist = (position - building.transform.position).sqrMagnitude;
            if (dist < closestDistance) {
                closestDistance = dist;
                closest = building;
            }
        }
        return closest;
    }

    public enum Type {

        TownCenter

    }

}