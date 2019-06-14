using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PeopleInfo : MonoBehaviour {

    public TextMeshProUGUI text;

    private void Update() {
        var inst = TownStats.Instance;
        this.text.text = inst.GetVillagerAmount() + "/" + inst.villagerLimit;
    }

}