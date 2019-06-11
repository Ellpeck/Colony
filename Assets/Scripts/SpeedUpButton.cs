using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpButton : MonoBehaviour {

    public float speedUpFactor;
    private bool speedingUp;

    public void OnSelect() {
        Time.timeScale = this.speedUpFactor;
    }

    public void OnDeselect() {
        Time.timeScale = 1;
    }

}