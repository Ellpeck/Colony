using System;
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

    public Building[] placeableBuildings;
    public BuildInstruction buildInstruction;

    public Sprite[] resourceSprites;

    public Resource[] Resources { get; private set; }

    private void Awake() {
        this.Resources = new[] {this.wood, this.stone, this.iron, this.gold, this.food};
        Instance = this;
    }

    public void Add(Resource.Type type, int amount) {
        foreach (var res in this.Resources) {
            if (res.type == type) {
                res.amount += amount;
                break;
            }
        }
    }

    public int Take(Resource.Type type, int amount) {
        foreach (var res in this.Resources) {
            if (res.type == type) {
                var taken = Mathf.Min(amount, res.amount);
                res.amount -= taken;
                return taken;
            }
        }
        return 0;
    }

    public int GetResourceAmount(Resource.Type type) {
        foreach (var res in this.Resources) {
            if (res.type == type)
                return res.amount;
        }
        return 0;
    }

    public bool HasResource(Resource resource) {
        return this.GetResourceAmount(resource.type) >= resource.amount;
    }

    [Serializable]
    public class Data {

        public int[] resources;

        public Data(ResourceManager manager) {
            this.resources = new int[manager.Resources.Length];
            for (var i = 0; i < this.resources.Length; i++)
                this.resources[i] = manager.GetResourceAmount((Resource.Type) i);
        }

        public void Load(ResourceManager manager) {
            for (var i = 0; i < this.resources.Length; i++)
                manager.Resources[i].amount = this.resources[i];
        }

    }

}