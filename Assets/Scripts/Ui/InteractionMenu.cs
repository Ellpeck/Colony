using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionMenu : MonoBehaviour {

    public GameObject panel;
    private Animator animator;

    private void Start() {
        this.animator = this.panel.GetComponent<Animator>();
    }

    public void OnSelectionChanged() {
        var selectable = SelectionManager.Instance.GetLastSelected<Selectable>();
        if (selectable) {
            var items = new List<GameObject>();
            selectable.getInteractionItems.Invoke(items);

            if (items.Count > 0) {
                foreach (Transform child in this.panel.transform)
                    Destroy(child.gameObject);
                foreach (var interaction in items) {
                    interaction.transform.SetParent(this.panel.transform, false);
                }
            }

            this.animator.SetBool("Out", items.Count <= 0);
        } else {
            this.animator.SetBool("Out", true);
        }
    }

}