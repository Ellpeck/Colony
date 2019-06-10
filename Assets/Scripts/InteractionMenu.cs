using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionMenu : MonoBehaviour {

    public GameObject panel;
    public Building[] placeableBuildings;
    public BuildInstruction instruction;

    private void Start() {
        SelectionManager.Instance.onSelectionChanged += this.OnSelectionChanged;

        foreach (var building in this.placeableBuildings) {
            var inst = Instantiate(this.instruction, this.panel.transform);
            inst.buildingToPlace = building;
        }
    }

    private void OnSelectionChanged() {
        var allSelected = SelectionManager.Instance.selectedObjects;
        this.panel.SetActive(allSelected.Find(selectable => selectable.GetComponent<Person>()));
    }

}