using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour {

    public Resource.Type[] storeableTypes;
    public int discoveryRadius;
    public Sprite unfinishedSprite;
    public List<Resource> requiredResources;
    public int villagerLimitIncrease;
    [TextArea] public string description;
    public SpriteRenderer mainRenderer;

    public bool IsGhost { get; private set; }
    public bool IsFinished { get; private set; }

    private SpriteRenderer[] renderers;
    private PathableObject pathableObject;
    private Sprite finishedSprite;
    public Selectable Selectable { get; private set; }

    private void Awake() {
        this.renderers = this.GetComponentsInChildren<SpriteRenderer>();
        this.finishedSprite = this.mainRenderer.sprite;
        this.pathableObject = this.GetComponentInChildren<PathableObject>();
        this.Selectable = this.GetComponent<Selectable>();
    }

    public bool IsValidPosition() {
        if (WorldGenerator.Instance.IsOutOfBounds((PolygonCollider2D) this.pathableObject.Collider))
            return false;

        var filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.SetLayerMask(WorldGenerator.Instance.objectCollisionLayers);
        return Physics2D.OverlapCollider(this.pathableObject.Collider, filter, new Collider2D[1]) <= 0;
    }

    public void SetMode(bool ghost, bool finished) {
        this.IsGhost = ghost;
        this.IsFinished = finished;

        this.mainRenderer.sprite = finished ? this.finishedSprite : this.unfinishedSprite;
        if (!ghost) {
            this.SetGhostColor(Color.white);
            if (finished) {
                WorldGenerator.Instance.Discover(this.transform.position, this.discoveryRadius);
                TownStats.Instance.villagerLimit += this.villagerLimitIncrease;
            }
        }
    }

    private void OnDestroy() {
        if (!this.IsGhost && this.IsFinished)
            TownStats.Instance.villagerLimit -= this.villagerLimitIncrease;
    }

    public void SetGhostColor(Color color) {
        foreach (var rend in this.renderers) {
            rend.color = color;
        }
    }

    public void UpdateGraph() {
        this.pathableObject.UpdateGraph();
    }

    public bool FeedResource(Resource.Type type) {
        var rightOne = false;
        foreach (var resource in this.requiredResources) {
            if (resource.type == type) {
                rightOne = true;

                resource.amount--;
                if (resource.amount <= 0)
                    this.requiredResources.Remove(resource);
                break;
            }
        }
        if (this.requiredResources.Count <= 0)
            this.SetMode(false, true);
        return rightOne;
    }

    public static Building GetClosest(Vector3 position, BuildingFilter filter = null, bool finished = true, float maxDistance = 0) {
        Building closest = null;
        var closestDistance = float.MaxValue;
        foreach (var building in FindObjectsOfType<Building>()) {
            if (building.IsGhost || finished != building.IsFinished)
                continue;
            if (filter != null && !filter(building))
                continue;
            var dist = (position - building.transform.position).sqrMagnitude;
            if (dist < closestDistance && (maxDistance == 0 || dist < maxDistance * maxDistance)) {
                closestDistance = dist;
                closest = building;
            }
        }
        return closest;
    }

    public delegate bool BuildingFilter(Building building);

}