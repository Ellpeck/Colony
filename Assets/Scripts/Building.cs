using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour {

    private static readonly int Ghost = Animator.StringToHash("IsGhost");
    private static readonly int CanBuild = Animator.StringToHash("CanBuild");
    private static readonly int Hidden = Animator.StringToHash("Hidden");

    public string savedPrefabName;
    public Resource.Type[] storeableTypes;
    public int discoveryRadius;
    public Sprite unfinishedSprite;
    public List<Resource> requiredResources;
    public int villagerLimitIncrease;
    [TextArea] public string description;
    public SpriteRenderer mainRenderer;

    public bool IsGhost { get; private set; }
    public bool IsFinished { get; private set; }

    private PathableObject pathableObject;
    private Sprite finishedSprite;
    private Animator animator;
    public Selectable Selectable { get; private set; }

    private void Awake() {
        this.finishedSprite = this.mainRenderer.sprite;
        this.pathableObject = this.GetComponentInChildren<PathableObject>();
        this.animator = this.GetComponentInChildren<Animator>();
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

    public void SetCanBuild(bool can) {
        this.animator.SetBool(CanBuild, can);
    }

    public void SetHidden(bool hidden) {
        this.animator.SetBool(Hidden, hidden);
    }

    public void SetMode(bool ghost, bool finished) {
        this.IsGhost = ghost;
        this.IsFinished = finished;

        this.mainRenderer.sprite = finished ? this.finishedSprite : this.unfinishedSprite;
        if (!ghost) {
            if (finished) {
                WorldGenerator.Instance.Discover(this.transform.position, this.discoveryRadius);
                TownStats.Instance.villagerLimit += this.villagerLimitIncrease;
            }
        }
        this.animator.SetBool(Ghost, ghost);
    }

    private void OnDestroy() {
        if (!this.IsGhost && this.IsFinished)
            TownStats.Instance.villagerLimit -= this.villagerLimitIncrease;
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

    [Serializable]
    public class Data {

        public string prefabName;
        public SerializableVec3 position;
        public bool isFinished;
        public List<Resource> requiredResources;

        private Data(Building building) {
            this.prefabName = building.savedPrefabName;
            this.position = building.transform.position;
            this.isFinished = building.IsFinished;
            this.requiredResources = building.requiredResources;
        }

        public void Load(Building building) {
            building.requiredResources = this.requiredResources;
            building.transform.position = this.position;
            building.SetMode(false, this.isFinished);
        }

        public static Data Save(Building building) {
            return building.IsGhost ? null : new Data(building);
        }

    }

}