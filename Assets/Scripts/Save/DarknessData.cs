using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class DarknessData {

    public bool[,] hasDarkness;

    public DarknessData(int size, int border, Tilemap darkness) {
        this.hasDarkness = new bool[size + border * 2, size + border * 2];
        for (var x = -border; x < size + border; x++) {
            for (var y = -border; y < size + border; y++) {
                this.hasDarkness[x + border, y + border] = darkness.GetTile(new Vector3Int(x, y, 0));
            }
        }
    }

    public void Load(int size, int border, Tilemap darkness, Tile tile) {
        for (var x = -border; x < size + border; x++) {
            for (var y = -border; y < size + border; y++) {
                if (this.hasDarkness[x + border, y + border])
                    darkness.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

}