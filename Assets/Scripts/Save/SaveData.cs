using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData {

    public SerializableVec3 cameraPosition;
    public int seed;

    public SaveData(SerializableVec3 cameraPosition, int seed) {
        this.cameraPosition = cameraPosition;
        this.seed = seed;
    }

}