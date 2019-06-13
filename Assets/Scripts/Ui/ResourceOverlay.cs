using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceOverlay : MonoBehaviour {

    public TextMeshProUGUI[] texts;

    private void Update() {
        for (var i = 0; i < this.texts.Length; i++) {
            var res = ResourceManager.Instance.Resources[i];
            this.texts[i].text = res.amount.ToString();
        }
    }

}