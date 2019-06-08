using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {

    public static ResourceManager Instance { get; private set; }

    public Resource wood;
    public Resource stone;
    public Resource iron;
    public Resource gold;
    public Resource food;
    
    public Resource[] Resources { get; private set; }

    private void Awake() {
        this.Resources = new[] {this.wood, this.stone, this.iron, this.gold, this.food};
        Instance = this;
    }

}