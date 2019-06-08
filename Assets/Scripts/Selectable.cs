using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {

    public SpriteRenderer outline;
    public Color selectedColor;
    public Color hoverColor;

    public bool IsSelected { get; private set; }

    public void OnHover() {
        if (this.outline && !this.IsSelected) {
            this.outline.gameObject.SetActive(true);
            this.outline.color = this.hoverColor;
        }
    }

    public void OnStopHover() {
        if (this.outline && !this.IsSelected) {
            this.outline.gameObject.SetActive(false);
        }
    }

    public void OnSelect() {
        if (this.outline) {
            this.outline.gameObject.SetActive(true);
            this.outline.color = this.selectedColor;
        }
        this.IsSelected = true;
    }

    public void OnDeselect() {
        if (this.outline) {
            this.outline.gameObject.SetActive(false);
        }
        this.IsSelected = false;
    }

}