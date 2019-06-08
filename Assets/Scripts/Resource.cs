using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Resource {

    [SerializeField] private int amount;

    public void Add(int amount) {
        this.amount += amount;
    }

    public int Consume(int amount) {
        var consumed = Math.Min(this.amount, amount);
        this.amount -= consumed;
        return consumed;
    }

    public int GetAmount() {
        return this.amount;
    }

    public override string ToString() {
        return this.amount.ToString();
    }

}