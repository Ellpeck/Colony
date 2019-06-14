using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipProvider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public TextTooltip tooltip;
    [TextArea] public string text;

    private Canvas canvas;
    private TextTooltip instance;

    private void Start() {
        this.canvas = this.GetComponentInParent<Canvas>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        this.instance = Instantiate(this.tooltip, this.canvas.transform);
        this.instance.SetText(this.text);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (this.instance)
            this.instance.FadeOut();
    }

}