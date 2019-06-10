using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour {

    public static SelectionManager Instance { get; private set; }

    public LayerMask objectLayers;
    public List<Selectable> selectedObjects;
    public Selectable hoveringObject;

    private new Camera camera;

    private void Start() {
        Instance = this;
        this.camera = Camera.main;
    }

    private void Update() {
        var pos = Input.mousePosition;
        if (EventSystem.current.IsPointerOverGameObject()
            || !WorldGenerator.Instance.IsDiscovered(this.camera.ScreenToWorldPoint(pos))) {
            if (this.hoveringObject)
                this.hoveringObject.OnStopHover();
            this.hoveringObject = null;
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
    }

}