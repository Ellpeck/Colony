using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceOverlay : MonoBehaviour {

    public TextMeshProUGUI[] texts;

    private void Update() {
        for (var i = 0; i < this.texts.Length; i++)
            this.texts[i].text = ResourceManager.Instance.Resources[i].ToString();
    }

}