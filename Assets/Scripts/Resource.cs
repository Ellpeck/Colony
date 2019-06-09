using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Resource {

    public Type type;
    public int amount;

    public enum Type {

        Wood,
        Stone,
        Iron,
        Gold,
        Food

    }

}