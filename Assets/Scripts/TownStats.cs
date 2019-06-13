using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownStats : MonoBehaviour {

    public static TownStats Instance;
    public Transform people;

    public float chopSpeed;
    public float buildSpeed;
    public int villagerLimit;

    private void Awake() {
        Instance = this;
    }

    public int GetVillagerAmount() {
        return this.people.childCount;
    }

}