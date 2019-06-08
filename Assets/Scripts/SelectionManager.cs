using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour {

    public GameObject selectedObject;
    public LayerMask objectLayers;

    private new Camera camera;

    private void Start() {
        this.camera = Camera.main;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            var pos = Input.mousePosition;
            var ray = this.camera.ScreenPointToRay(pos);
            var trace = Physics2D.GetRayIntersection(ray, Mathf.Infinity, this.objectLayers);
            this.selectedObject = trace ? trace.transform.gameObject : null;
        }
    }

}