using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float closestZoom;
    public float farthestZoom;
    public bool invertZoom;

    private new Camera camera;
    private Vector3 lastMousePos;

    private void Start() {
        this.camera = this.GetComponent<Camera>();
    }

    private void Update() {
        var mousePos = Input.mousePosition;
        var posDelta = this.lastMousePos - mousePos;
        if (posDelta != Vector3.zero) {
            var worldDelta = this.camera.ScreenToWorldPoint(this.lastMousePos) - this.camera.ScreenToWorldPoint(mousePos);
            if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
                this.camera.transform.position += worldDelta;
            }

            this.lastMousePos = mousePos;
        }

        var zoomDelta = Input.mouseScrollDelta.y;
        var newZoom = this.camera.orthographicSize - (this.invertZoom ? -zoomDelta : zoomDelta);
        this.camera.orthographicSize = Mathf.Clamp(newZoom, this.closestZoom, this.farthestZoom);
    }

}