using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {

    public static CameraController Instance { get; private set; }
    public float closestZoom;
    public float farthestZoom;
    public float keyMoveSpeed;
    public bool invertZoom;

    private new Camera camera;
    private Vector3 lastMousePos;

    private void Start() {
        Instance = this;
        this.camera = this.GetComponent<Camera>();
    }

    public void CenterCameraOn(Vector2 position) {
        var trans = this.camera.transform;
        trans.position = new Vector3(position.x, position.y, trans.position.z);
    }

    private void Update() {
        var mousePos = Input.mousePosition;
        var posDelta = this.lastMousePos - mousePos;
        if (posDelta != Vector3.zero) {
            if (Input.GetMouseButton(2) && !EventSystem.current.IsPointerOverGameObject()) {
                var worldDelta = this.camera.ScreenToWorldPoint(this.lastMousePos) - this.camera.ScreenToWorldPoint(mousePos);
                this.camera.transform.position += worldDelta;
            }

            this.lastMousePos = mousePos;
        }

        var zoomDelta = Input.mouseScrollDelta.y;
        var newZoom = this.camera.orthographicSize - (this.invertZoom ? -zoomDelta : zoomDelta);
        this.camera.orthographicSize = Mathf.Clamp(newZoom, this.closestZoom, this.farthestZoom);

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        this.camera.transform.position += new Vector3(horizontal, vertical) * this.keyMoveSpeed * this.camera.orthographicSize;
    }

}