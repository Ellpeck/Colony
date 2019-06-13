using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePersonButton : MonoBehaviour {

    public Slider progressBar;

    public void OnClick() {
        foreach (var selected in SelectionManager.Instance.GetAllSelected<TownCenter>())
            selected.CreatePerson();
    }

    private void Update() {
        var last = SelectionManager.Instance.GetLastSelected<TownCenter>();
        if (last && last.CreatePersonTimer > 0) {
            this.progressBar.value = 1 - last.CreatePersonTimer / last.personCreationTime;
        } else {
            this.progressBar.value = 0;
        }
    }

}