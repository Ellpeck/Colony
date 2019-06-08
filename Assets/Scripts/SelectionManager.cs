using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour {

    public LayerMask objectLayers;
    public Selectable selectedObject;
    public Selectable hoveringObject;

    private new Camera camera;

    private void Start() {
        this.camera = Camera.main;
    }

    private void Update() {
        if (this.selectedObject && Input.GetMouseButtonDown(0)) {
            this.selectedObject.OnDeselect();
            this.selectedObject = null;
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
                this.selectedObject = selectable;
                this.selectedObject.OnSelect();
            }
        } else {
            if (this.hoveringObject) {
                this.hoveringObject.OnStopHover();
                this.hoveringObject = null;
            }
        }
    }

}