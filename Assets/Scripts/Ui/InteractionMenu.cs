using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionMenu : MonoBehaviour {

    public GameObject panel;

    public void OnSelectionChanged() {
        foreach (Transform child in this.panel.transform)
            Destroy(child.gameObject);

        var selectable = SelectionManager.Instance.GetLastSelected<Selectable>();
        if (selectable) {
            var items = new List<GameObject>();
            selectable.getInteractionItems.Invoke(items);

            foreach (var interaction in items) {
                interaction.transform.SetParent(this.panel.transform, false);
                this.panel.SetActive(true);
            }
        } else {
            this.panel.SetActive(false);
        }
    }

}