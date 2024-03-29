﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Selectable : MonoBehaviour {

    public SpriteRenderer outline;
    public Color selectedColor;
    public Color hoverColor;
    public string menuName;
    public Sprite menuSprite;
    public bool canBuild;

    public OnSelection onSelection;
    public GetInteractionItems getInteractionItems;

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
        this.onSelection.Invoke(true);
    }

    public void OnDeselect() {
        if (this.outline) {
            this.outline.gameObject.SetActive(false);
        }
        this.IsSelected = false;
        this.onSelection.Invoke(false);
    }

    [Serializable]
    public class OnSelection : UnityEvent<bool> {

    }

    [Serializable]
    public class GetInteractionItems : UnityEvent<List<GameObject>> {

    }

}