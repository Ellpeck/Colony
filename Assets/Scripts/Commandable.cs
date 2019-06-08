using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Commandable : MonoBehaviour {

    private static readonly int Walking = Animator.StringToHash("Walking");

    public float movementSpeed;
    public float maxWaypointDistance;

    private Selectable selectable;
    private Seeker seeker;
    private Rigidbody2D body;
    private Animator animator;
    private new Camera camera;

    private Path currentPath;
    private int currentWaypoint;
    private bool facingLeft;

    private void Start() {
        this.selectable = this.GetComponent<Selectable>();
        this.seeker = this.GetComponent<Seeker>();
        this.body = this.GetComponent<Rigidbody2D>();
        this.animator = this.GetComponentInChildren<Animator>();
        this.camera = Camera.main;
    }

    private void Update() {
        if (this.selectable.IsSelected && Input.GetMouseButtonDown(1)) {
            var pos = this.camera.ScreenToWorldPoint(Input.mousePosition);
            this.seeker.StartPath(this.body.position, pos, this.OnPathCalculated);
        }
    }

    private void FixedUpdate() {
        if (this.currentPath != null) {
            Vector2 waypoint = this.currentPath.vectorPath[this.currentWaypoint];
            var dir = (waypoint - this.body.position).normalized;

            if (this.facingLeft != dir.x < 0) {
                this.transform.Rotate(0, 180, 0);
                this.facingLeft = dir.x < 0;
            }

            this.body.velocity = dir * this.movementSpeed;
            this.animator.SetBool(Walking, true);

            var dist = Vector2.Distance(this.body.position, waypoint);
            if (dist <= this.maxWaypointDistance) {
                this.currentWaypoint++;
                if (this.currentWaypoint >= this.currentPath.vectorPath.Count) {
                    this.currentPath = null;
                }
            }
        } else {
            this.body.velocity = Vector2.zero;
            this.animator.SetBool(Walking, false);
        }
    }

    private void OnPathCalculated(Path path) {
        this.currentPath = path;
        this.currentWaypoint = 0;
    }

}