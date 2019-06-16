using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData {

    public DateTime lastPlayed;
    public string saveName;
    public int seed;

    public SaveData(string saveName, DateTime lastPlayed, int seed) {
        this.saveName = saveName;
        this.lastPlayed = lastPlayed;
        this.seed = seed;
    }

}