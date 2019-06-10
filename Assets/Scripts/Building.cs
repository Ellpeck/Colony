using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour {

    public Type type;
    public List<Resource.Type> storeableTypes;
    public int discoveryRadius;
    public bool isGhost;

    private SpriteRenderer[] renderers;
    private PolygonCollider2D collision;

    private void Awake() {
        this.renderers = this.GetComponentsInChildren<SpriteRenderer>();
        this.collision = this.GetComponentInChildren<PolygonCollider2D>();
    }

    public bool IsValidPosition() {
        var filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.SetLayerMask(WorldGenerator.Instance.objectCollisionLayers);
        return Physics2D.OverlapCollider(this.collision, filter, new Collider2D[1]) <= 0;
    }

    public void SetGhost(bool ghost) {
        this.isGhost = ghost;

        if (!ghost) {
            this.SetGhostColor(Color.white);
            WorldGenerator.Instance.Discover(this.transform.position, this.discoveryRadius);
        }
    }

    public void SetGhostColor(Color color) {
        foreach (var rend in this.renderers) {
            rend.color = color;
        }
    }

    public static Building GetClosest(Vector3 position, BuildingFilter filter = null) {
        Building closest = null;
        var closestDistance = float.MaxValue;
        foreach (var building in FindObjectsOfType<Building>()) {
            if (building.isGhost)
                continue;
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