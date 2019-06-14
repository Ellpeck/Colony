using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreationTooltip : MonoBehaviour {

    public Image icon;
    public TextMeshProUGUI displayName;
    public TextMeshProUGUI description;
    public ConstructionRequirement[] requirements;

    private void Update() {
        this.transform.position = Input.mousePosition;
    }

}
