using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour {

    public static SelectionManager Instance { get; private set; }

    public LayerMask objectLayers;
    public List<Selectable> selectedObjects;
    public Selectable hoveringObject;
    public Color ghostColor;
    public Color invalidGhostColor;
    public Building placingBuilding;
    public OnSelectionChanged onSelectionChanged;

    private new Camera camera;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        this.camera = Camera.main;
    }

    private void Update() {
        var pos = Input.mousePosition;
        Vector2 worldPos = this.camera.ScreenToWorldPoint(pos);
        if (EventSystem.current.IsPointerOverGameObject() || !WorldGenerator.Instance.IsDiscovered(worldPos)) {
            if (this.hoveringObject)
                this.hoveringObject.OnStopHover();
            this.hoveringObject = null;
            if (this.placingBuilding)
                this.placingBuilding.SetGhostColor(new Color(0, 0, 0, 0));
            return;
        }

        if (this.placingBuilding) {
            var valid = this.placingBuilding.IsValidPosition();
            this.placingBuilding.SetGhostColor(valid ? this.ghostColor : this.invalidGhostColor);
            if (valid && Input.GetMouseButtonDown(0)) {
                this.placingBuilding.SetMode(false, false);
                this.placingBuilding.UpdateGraph();
                foreach (var obj in this.selectedObjects) {
                    var person = obj.GetComponent<Person>();
                    if (person)
                        person.MoveTo(this.placingBuilding.gameObject);
                }
                this.placingBuilding = null;
            } else {
                var ground = WorldGenerator.Instance.ground;
                var placePos = ground.GetCellCenterWorld(ground.WorldToCell(worldPos));
                this.placingBuilding.transform.position = placePos;
            }
            return;
        }

        if (this.selectedObjects.Count > 0 && Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftControl)) {
            this.Select(null, true);
        }

        var ray = this.camera.ScreenPointToRay(pos);
        var trace = Physics2D.GetRayIntersection(ray, Mathf.Infinity, this.objectLayers);
        if (trace) {
            var selectable = trace.transform.gameObject.GetComponent<Selectable>();
            if (selectable != this.hoveringObject) {
                if (this.hoveringObject)
                    this.hoveringObject.OnStopHover();
                this.hoveringObject = selectable;
                this.hoveringObject.OnHover();
            }

            if (Input.GetMouseButtonDown(0)) {
                this.Select(selectable, false);
            }
        } else {
            if (this.hoveringObject) {
                this.hoveringObject.OnStopHover();
                this.hoveringObject = null;
            }
        }
    }

    public bool IsBusy() {
        return this.placingBuilding;
    }

    public void Select(Selectable selectable, bool clearOthers) {
        if (clearOthers) {
            foreach (var obj in this.selectedObjects)
                obj.OnDeselect();
            this.selectedObjects.Clear();
        }

        if (selectable) {
            this.selectedObjects.Add(selectable);
            selectable.OnSelect();
        }

        if (this.onSelectionChanged != null)
            this.onSelectionChanged();
    }

    public delegate void OnSelectionChanged();

}