using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour {

    private static readonly int Out = Animator.StringToHash("FadeOut");
    private Animator animator;

    protected virtual void Awake() {
        this.animator = this.GetComponent<Animator>();
    }

    private void LateUpdate() {
        this.transform.position = Input.mousePosition;
    }

    public void FadeOut() {
        this.animator.SetTrigger(Out);
        Destroy(this.gameObject, 0.1F);
    }

}