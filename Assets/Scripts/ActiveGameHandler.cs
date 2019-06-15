using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveGameHandler : MonoBehaviour {

    public static ActiveGameHandler Instance { get; private set; }

    public Guid saveId;
    public string saveName;
    public SaveData savedSummary;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this.gameObject);
        }
    }

}