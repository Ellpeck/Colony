using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResourceInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Resource.Type type;
    public TextMeshProUGUI amount;
    public ResourceTooltip tooltip;
    [TextArea] public string description;

    private Canvas canvas;

    private int accumCounter;
    private int lastAmount;
    private float minuteCounter;
    private int accumLastMinute;
    private ResourceTooltip tooltipInstance;

    private void Start() {
        this.canvas = this.GetComponentInParent<Canvas>();
        this.lastAmount = ResourceManager.Instance.GetResourceAmount(this.type);
    }

    private void Update() {
        var currAmount = ResourceManager.Instance.GetResourceAmount(this.type);
        this.amount.text = currAmount.ToString();

        this.minuteCounter += Time.deltaTime;
        if (this.minuteCounter >= 60) {
            this.accumLastMinute = this.accumCounter;
            this.accumCounter = 0;
            this.minuteCounter = 0;
        }

        if (currAmount != this.lastAmount) {
            this.accumCounter += currAmount - this.lastAmount;
            this.lastAmount = currAmount;
        }

        if (this.tooltipInstance) {
            this.tooltipInstance.rate.text = this.accumLastMinute.ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!this.tooltipInstance) {
            this.tooltipInstance = Instantiate(this.tooltip, this.canvas.transform);
            this.tooltipInstance.displayName.text = this.type.ToString();
            this.tooltipInstance.description.text = this.description;
            this.tooltipInstance.icon.sprite = ResourceManager.Instance.resourceSprites[(int) this.type];
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (this.tooltipInstance)
            Destroy(this.tooltipInstance.gameObject);
    }

}