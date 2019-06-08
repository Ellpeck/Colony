using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour {

    public LayerMask objectLayers;
    public List<Selectable> selectedObjects;
    public Selectable hoveringObject;

    private new Camera camera;

    private void Start() {
        this.camera = Camera.main;
    }

    private void Update() {
        if (this.selectedObjects.Count > 0 && Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftControl)) {
            foreach (var obj in this.selectedObjects)
                obj.OnDeselect();
            this.selectedObjects.Clear();
        }

        var pos = Input.mousePosition;
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
                this.selectedObjects.Add(selectable);
                selectable.OnSelect();
            }
        } else {
            if (this.hoveringObject) {
                this.hoveringObject.OnStopHover();
                this.hoveringObject = null;
            }
        }
    }

}