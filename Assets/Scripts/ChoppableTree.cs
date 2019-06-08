using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppableTree : MonoBehaviour {

    public int woodAmount;

    public void Chop() {
        this.woodAmount--;
        if (this.woodAmount <= 0)
            Destroy(this.gameObject);
    }

}