using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableVec3 {

    public float x;
    public float y;
    public float z;

    public SerializableVec3(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z; 
    }

    public static implicit operator Vector3(SerializableVec3 vec) {
        return new Vector3(vec.x, vec.y, vec.z);
    }

    public static implicit operator SerializableVec3(Vector3 vec) {
        return new SerializableVec3(vec.x, vec.y, vec.z);
    }

}