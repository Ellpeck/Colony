using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInfoOverlay : MonoBehaviour {

    public GameObject panel;
    public TextMeshProUGUI nameText;
    public Image icon;

    [Space] public GameObject personInfo;
    public GameObject personInfoNothing;
    public TextMeshProUGUI personInfoAmount;
    public Image personInfoIcon;

    [Space] public GameObject resourceInfo;
    public TextMeshProUGUI resourceInfoAmount;
    public Image resourceInfoIcon;

    private Person selectedPerson;
    private ResourceSource selectedSource;

    public void OnSelectionChanged() {
        var selectables = SelectionManager.Instance.selectedObjects;
        Selectable selectable;
        if (selectables.Count <= 0 || !(selectable = selectables[selectables.Count - 1])) {
            this.panel.SetActive(false);
            this.selectedPerson = null;
            this.selectedSource = null;
            return;
        }

        this.panel.SetActive(true);
        this.nameText.text = selectable.menuName;
        this.icon.sprite = selectable.menuSprite;

        this.selectedPerson = selectable.GetComponent<Person>();
        this.personInfo.SetActive(this.selectedPerson);
        this.selectedSource = selectable.GetComponent<ResourceSource>();
        this.resourceInfo.SetActive(this.selectedSource);
    }

    private void LateUpdate() {
        if (this.selectedPerson) {
            var carrying = this.selectedPerson.CarryingResource != null;
            this.personInfoAmount.gameObject.SetActive(carrying);
            this.personInfoIcon.gameObject.SetActive(carrying);
            this.personInfoNothing.SetActive(!carrying);
            if (carrying) {
                this.personInfoAmount.text = this.selectedPerson.CarryingResource.amount.ToString();
                this.personInfoIcon.sprite = ResourceManager.Instance.resourceSprites[(int) this.selectedPerson.CarryingResource.type];
            }
        }

        if (this.selectedSource) {
            this.resourceInfoAmount.text = this.selectedSource.amount.ToString();
            this.resourceInfoIcon.sprite = ResourceManager.Instance.resourceSprites[(int) this.selectedSource.type];
        }
    }

}