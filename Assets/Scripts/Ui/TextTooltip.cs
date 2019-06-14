using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextTooltip : MonoBehaviour {

    private TextMeshProUGUI text;

    private void Awake() {
        this.text = this.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void LateUpdate() {
        this.transform.position = Input.mousePosition;
    }

    public void SetText(string text) {
        this.text.text = text;
    }

}