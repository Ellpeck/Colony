using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InteractionMenu : MonoBehaviour {

    public GameObject panel;

    public void OnSelectionChanged() {
        foreach (Transform child in this.panel.transform)
            Destroy(child.gameObject);

        var items = new List<GameObject>();
        foreach (var selectable in SelectionManager.Instance.selectedObjects)
            selectable.getInteractionItems.Invoke(items);

        var any = false;
        foreach (var interaction in items) {
            interaction.transform.SetParent(this.panel.transform, false);
            any = true;
        }
        this.panel.SetActive(any);
    }

}