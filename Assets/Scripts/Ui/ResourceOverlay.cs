using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceOverlay : MonoBehaviour {

    public TextMeshProUGUI[] texts;
    public TextMeshProUGUI personText;

    private void Update() {
        for (var i = 0; i < this.texts.Length; i++) {
            var res = ResourceManager.Instance.Resources[i];
            this.texts[i].text = res.amount.ToString();
        }
        var stats = TownStats.Instance;
        this.personText.text = stats.GetVillagerAmount() + "/" + stats.villagerLimit;
    }

}