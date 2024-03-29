﻿using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Commandable : MonoBehaviour {

    private static readonly int Walking = Animator.StringToHash("Walking");

    public float movementSpeed;
    public float maxWaypointDistance;
    public GameObject waypointMarker;
    public int discoveryRadius;
    public OnTargetReached onTargetReached;
    public OnCommandReceived onCommandReceived;

    private Selectable selectable;
    private Seeker seeker;
    private Rigidbody2D body;
    private Animator animator;
    private new Camera camera;

    private Path currentPath;
    private int currentWaypoint;
    private bool facingLeft;
    private GameObject destination;
    private GameObject currentWaypointMarker;
    private Vector2 lastDiscoveryPosition;

    private void Start() {
        this.selectable = this.GetComponent<Selectable>();
        this.selectable.onSelection.AddListener(this.OnSelection);
        this.seeker = this.GetComponent<Seeker>();
        this.body = this.GetComponent<Rigidbody2D>();
        this.animator = this.GetComponentInChildren<Animator>();
        this.camera = Camera.main;

        WorldGenerator.Instance.Discover(this.transform.position, this.discoveryRadius);
    }

    public bool IsBusy() {
        return this.currentPath != null;
    }

    private void OnSelection(bool selected) {
        if (this.currentWaypointMarker)
            this.currentWaypointMarker.SetActive(selected);
    }

    private void Update() {
        if (this.selectable.IsSelected && Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject()) {
            if (!SelectionManager.Instance.IsBusy()) {
                var dest = SelectionManager.Instance.hoveringObject;
                if (dest) {
                    this.MoveTo(dest.gameObject, true);
                } else {
                    this.MoveTo(this.camera.ScreenToWorldPoint(Input.mousePosition), null, true);
                }
            }
        }

        Vector2 pos = this.transform.position;
        if ((pos - this.lastDiscoveryPosition).sqrMagnitude > 1) {
            this.lastDiscoveryPosition = pos;
            WorldGenerator.Instance.Discover(pos, this.discoveryRadius);
        }
    }

    public void MoveTo(GameObject destination, bool fromPlayer = false) {
        Vector2 goal;
        var pathable = destination.GetComponentInChildren<PathableObject>();
        if (pathable) {
            goal = pathable.GetPathPoint(this.transform.position);
        } else {
            goal = destination.transform.position;
        }
        this.MoveTo(goal, destination, fromPlayer);
    }

    public void MoveTo(Vector2 pos, GameObject destination, bool fromPlayer = false) {
        this.seeker.StartPath(this.body.position, pos, this.OnPathCalculated);

        if (this.currentWaypointMarker)
            Destroy(this.currentWaypointMarker);
        this.currentWaypointMarker = Instantiate(this.waypointMarker, pos, Quaternion.identity);
        this.currentWaypointMarker.SetActive(this.selectable.IsSelected);

        this.destination = destination;
        this.onCommandReceived.Invoke(pos, destination, fromPlayer);
    }

    private void FixedUpdate() {
        if (this.currentPath != null) {
            var path = this.currentPath.vectorPath;
            if (path.Count <= 0) {
                this.OnGoalReached();
                return;
            }

            Vector2 waypoint;
            while (true) {
                waypoint = path[this.currentWaypoint];
                var dist = Vector2.Distance(this.body.position, waypoint);
                if (dist <= this.maxWaypointDistance) {
                    this.currentWaypoint++;
                    if (this.currentWaypoint >= path.Count) {
                        this.OnGoalReached();
                        return;
                    }
                } else {
                    break;
                }
            }

            var dir = (waypoint - this.body.position).normalized;
            if (this.facingLeft != dir.x < 0) {
                this.transform.Rotate(0, 180, 0);
                this.facingLeft = dir.x < 0;
            }
            this.body.velocity = dir * this.movementSpeed;
            this.animator.SetBool(Walking, true);
        } else {
            this.body.velocity = Vector2.zero;
            this.animator.SetBool(Walking, false);
        }
    }

    private void OnGoalReached() {
        if (this.currentWaypointMarker)
            Destroy(this.currentWaypointMarker);
        this.currentPath = null;
        this.onTargetReached.Invoke(this.destination);
    }

    private void OnPathCalculated(Path path) {
        this.currentPath = path;
        this.currentWaypoint = 0;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, this.maxWaypointDistance);
    }

    [Serializable]
    public class OnTargetReached : UnityEvent<GameObject> {

    }

    [Serializable]
    public class OnCommandReceived : UnityEvent<Vector2, GameObject, bool> {

    }

}