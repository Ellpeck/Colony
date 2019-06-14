using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTooltip : MonoBehaviour {

    public Image icon;
    public TextMeshProUGUI displayName;
    public TextMeshProUGUI description;
    public TextMeshProUGUI rate;

    private void LateUpdate() {
        this.transform.position = Input.mousePosition;
    }

}