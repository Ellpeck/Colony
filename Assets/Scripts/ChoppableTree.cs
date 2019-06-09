using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppableTree : MonoBehaviour {

    public int woodAmount;

    public int Chop() {
        this.woodAmount--;
        if (this.woodAmount <= 0) {
            Destroy(this.gameObject);
            AstarPath.active.Scan();
        }
        return 1;
    }

}