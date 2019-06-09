using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Commandable : MonoBehaviour {

    private static readonly int Walking = Animator.StringToHash("Walking");

    public float movementSpeed;
    public float maxWaypointDistance;
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
    private Selectable destinationSelectable;

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
            this.destinationSelectable = SelectionManager.Instance.hoveringObject;
            this.seeker.StartPath(this.body.position, pos, this.OnPathCalculated);

            if (this.onCommandReceived != null)
                this.onCommandReceived(pos, this.destinationSelectable);
        }
    }

    private void FixedUpdate() {
        if (this.currentPath != null) {
            Vector2 waypoint;
            while (true) {
                var path = this.currentPath.vectorPath;
                waypoint = path[this.currentWaypoint];
                var dist = Vector2.Distance(this.body.position, waypoint);
                if (dist <= this.maxWaypointDistance) {
                    this.currentWaypoint++;
                    if (this.currentWaypoint >= path.Count) {
                        if (this.onTargetReached != null)
                            this.onTargetReached(this.destinationSelectable);
                        this.currentPath = null;
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

    private void OnPathCalculated(Path path) {
        this.currentPath = path;
        this.currentWaypoint = 0;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, this.maxWaypointDistance);
    }

    public delegate void OnTargetReached(Selectable destinationSelectable);

    public delegate void OnCommandReceived(Vector2 destination, Selectable destinationSelectable);

}