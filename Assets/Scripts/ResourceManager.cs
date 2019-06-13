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

}