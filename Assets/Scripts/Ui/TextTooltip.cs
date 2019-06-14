using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextTooltip : Tooltip {

    private TextMeshProUGUI text;

    protected override void Awake() {
        base.Awake();
        this.text = this.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetText(string text) {
        this.text.text = text;
    }

}