using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour {

    public static SelectionManager Instance { get; private set; }

    public LayerMask objectLayers;
    public List<Selectable> selectedObjects;
    public Selectable hoveringObject;
    public Building placingBuilding;
    public UnityEvent onSelectionChanged;

    private new Camera camera;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        this.camera = Camera.main;
    }

    private void LateUpdate() {
        var pos = Input.mousePosition;
        Vector2 worldPos = this.camera.ScreenToWorldPoint(pos);
        var pointerBlocked = EventSystem.current.IsPointerOverGameObject();
        var canInteract = !pointerBlocked && WorldGenerator.Instance.IsDiscovered(worldPos);
        if (!canInteract) {
            if (this.hoveringObject)
                this.hoveringObject.OnStopHover();
            this.hoveringObject = null;
        }

        if (this.placingBuilding) {
            if (Input.GetMouseButtonDown(1)) {
                Destroy(this.placingBuilding.gameObject);
                this.placingBuilding = null;
                return;
            }
            this.placingBuilding.SetHidden(pointerBlocked);

            var valid = canInteract && this.placingBuilding.IsValidPosition();
            this.placingBuilding.SetCanBuild(valid);
            if (valid && Input.GetMouseButtonDown(0)) {
                this.placingBuilding.SetMode(false, false);
                this.placingBuilding.UpdateGraph();
                foreach (var obj in this.selectedObjects) {
                    if (obj.canBuild)
                        obj.GetComponent<Commandable>().MoveTo(this.placingBuilding.gameObject, true);
                }
                this.placingBuilding = null;
            } else {
                var ground = WorldGenerator.Instance.ground;
                var placePos = ground.GetCellCenterWorld(ground.WorldToCell(worldPos));
                this.placingBuilding.transform.position = placePos;
            }
            return;
        }

        if (canInteract) {
            var left = Input.GetMouseButtonDown(0);
            var control = Input.GetKey(KeyCode.LeftControl);

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

                if (left)
                    this.Select(selectable, !control); 
            } else {
                if (this.hoveringObject) {
                    this.hoveringObject.OnStopHover();
                    this.hoveringObject = null;
                }

                if (left && !control)
                    this.Select(null, true);
            }
        }
    }

    public void SetPlacingBuilding(Building building) {
        if (this.placingBuilding)
            Destroy(this.placingBuilding.gameObject);
        this.placingBuilding = building;
        building.SetMode(true, true);
    }

    public IEnumerable<T> GetAllSelected<T>() where T : MonoBehaviour {
        foreach (var selected in this.selectedObjects) {
            var comp = selected.GetComponent<T>();
            if (comp)
                yield return comp;
        }
    }

    public T GetLastSelected<T>() where T : MonoBehaviour {
        for (var i = this.selectedObjects.Count - 1; i >= 0; i--) {
            var comp = this.selectedObjects[i].GetComponent<T>();
            if (comp)
                return comp;
        }
        return null;
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

        this.onSelectionChanged.Invoke();
    }

}