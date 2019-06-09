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

    private void Update() {
        var selectables = SelectionManager.Instance.selectedObjects;
        if (selectables.Count <= 0) {
            this.panel.SetActive(false);
            return;
        }
        
        var selectable = selectables[selectables.Count - 1];
        if (!selectable) {
            this.panel.SetActive(false);
            return;
        }

        this.panel.SetActive(true);
        this.nameText.text = selectable.menuName;
        this.icon.sprite = selectable.menuSprite;

        var person = selectable.GetComponent<Person>();
        if (person) {
            this.personInfo.SetActive(true);
            var carrying = person.CarryingResource != null;
            this.personInfoAmount.gameObject.SetActive(carrying);
            this.personInfoIcon.gameObject.SetActive(carrying);
            this.personInfoNothing.SetActive(!carrying);
            if (carrying) {
                this.personInfoAmount.text = person.CarryingResource.amount.ToString();
                this.personInfoIcon.sprite = ResourceManager.Instance.resourceSprites[(int) person.CarryingResource.type];
            }
        } else {
            this.personInfo.SetActive(false);
        }

        var resource = selectable.GetComponent<ResourceSource>();
        if (resource) {
            this.resourceInfo.SetActive(true);
            this.resourceInfoAmount.text = resource.amount.ToString();
            this.resourceInfoIcon.sprite = ResourceManager.Instance.resourceSprites[(int) resource.type];
        } else {
            this.resourceInfo.SetActive(false);
        }
    }

}