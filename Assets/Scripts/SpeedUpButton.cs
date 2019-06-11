using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpButton : MonoBehaviour {

    public float speedUpFactor;

    public void OnSelect() {
        Time.timeScale *= this.speedUpFactor;
    }

    public void OnDeselect() {
        Time.timeScale /= this.speedUpFactor;
    }

}