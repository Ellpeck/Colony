﻿using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GroundGenerator : MonoBehaviour {

    public Tilemap ground;
    public Tilemap decorations;

    public int width;
    public int height;
    public float perlinScale1;
    public float perlinScale2;
    public float perlinScale2Importance;
    public Tile grass;
    public Tile darkGrass;
    public float sandHeight;
    public Tile sand;
    public float waterHeight;
    public Tile water;
    public float treePerlinScale1;
    public float treePerlinScale2;
    public float treeHeight;
    public int treeDensity;
    public Tile tree;

    private int seed;

    private void Start() {
        this.seed = Random.Range(0, 100000);
        this.GenerateMap();
    }

    private void Update() {
        for (var x = 0; x < this.width; x++) {
            for (var y = 0; y < this.height; y++) {
                this.decorations.SetTile(new Vector3Int(x, y, 0), null);
            }
        }
        this.GenerateMap();
    }

    private void GenerateMap() {
        Random.InitState(this.seed);
        
        for (var x = 0; x < this.width; x++) {
            for (var y = 0; y < this.height; y++) {
                var noise = (Mathf.PerlinNoise(this.seed + x / this.perlinScale1, this.seed + y / this.perlinScale1) +
                             Mathf.PerlinNoise(this.seed + x / this.perlinScale2, this.seed + y / this.perlinScale2) * 3F) / 4F;
                Tile tile;
                if (noise <= this.waterHeight)
                    tile = this.water;
                else if (noise <= this.sandHeight)
                    tile = this.sand;
                else {
                    var treeNoise = (Mathf.PerlinNoise(this.seed + x / this.treePerlinScale1, this.seed + y / this.treePerlinScale1) +
                                     Mathf.PerlinNoise(this.seed + x / this.treePerlinScale2, this.seed + y / this.treePerlinScale2) * 2F) / 3F;
                    if (treeNoise <= this.treeHeight) {
                        if (Random.Range(0, this.treeDensity) == 0)
                            this.decorations.SetTile(new Vector3Int(x, y, 0), this.tree);
                        tile = this.darkGrass;
                    } else
                        tile = this.grass;
                }
                this.ground.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

}