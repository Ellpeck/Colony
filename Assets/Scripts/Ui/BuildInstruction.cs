using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildInstruction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Image icon;
    public Building buildingToPlace;
    public CreationTooltip tooltip;

    private Canvas canvas;
    private Selectable selectable;
    private CreationTooltip tooltipInstance;

    private void Start() {
        this.canvas = this.GetComponentInParent<Canvas>();
        this.selectable = this.buildingToPlace.GetComponent<Selectable>();
        this.icon.sprite = this.selectable.menuSprite;
    }

    [UsedImplicitly]
    public void OnClick() {
        var inst = Instantiate(this.buildingToPlace, WorldGenerator.Instance.decorations);
        inst.SetMode(true, true);
        SelectionManager.Instance.placingBuilding = inst;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!this.tooltipInstance) {
            this.tooltipInstance = Instantiate(this.tooltip, this.canvas.transform);
            this.tooltipInstance.icon.sprite = this.selectable.menuSprite;
            this.tooltipInstance.displayName.text = this.selectable.menuName;
            this.tooltipInstance.description.text = this.buildingToPlace.description;
            foreach (var requirement in this.tooltipInstance.requirements) {
                var req = this.buildingToPlace.requiredResources.Find(res => res.type == requirement.type);
                if (req != null) {
                    requirement.text.text = req.amount.ToString();
                    requirement.gameObject.SetActive(true);
                } else {
                    requirement.gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (this.tooltipInstance)
            Destroy(this.tooltipInstance.gameObject);
    }

}