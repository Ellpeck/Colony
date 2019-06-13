using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildInstruction : MonoBehaviour {

    public Image icon;
    public Building buildingToPlace;

    private void Start() {
        this.icon.sprite = this.buildingToPlace.GetComponent<Selectable>().menuSprite;
    }

    public void OnClick() {
        var inst = Instantiate(this.buildingToPlace, WorldGenerator.Instance.decorations);
        inst.SetMode(true, true);
        SelectionManager.Instance.placingBuilding = inst;
    }

}