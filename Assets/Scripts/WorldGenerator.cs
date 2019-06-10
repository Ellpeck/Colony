﻿using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour {

    public static WorldGenerator Instance { get; private set; }

    public int size;
    public Tilemap ground;
    public Tilemap darkness;
    public Transform decorations;
    public Transform people;
    public TextAsset names;

    [Space] public Tile darknessTile;
    public int darknessBorder;

    [Space] public float perlinScale1;
    public float perlinScale2;
    public Tile grass;
    public Tile darkGrass;
    public float sandHeight;
    public Tile sand;
    public float waterHeight;
    public Tile water;

    [Space] public float treePerlinScale1;
    public float treePerlinScale2;
    public float treeHeight;
    public int treeDensity;
    public GameObject tree;

    [Space] public LayerMask objectCollisionLayers;
    public int objectSpawnTries;
    public int objectSpawnRadius;
    public float personCount;
    public Person person;
    public GameObject townCenter;

    [Space] public int berryBushVeinAmount;
    public int berryBushVeinSize;
    public int berryBushAmountPerVein;
    public GameObject berryBush;

    private int seed;
    private string[] nameArray;

    private void Start() {
        Instance = this;
        this.seed = Random.Range(0, 100000);
        this.nameArray = this.names.text.Split('\n');
        this.StartCoroutine(this.GenerateMap());
    }

    private IEnumerator GenerateMap() {
        Random.InitState(this.seed);

        for (var x = -this.darknessBorder; x < this.size + this.darknessBorder; x++) {
            for (var y = -this.darknessBorder; y < this.size + this.darknessBorder; y++) {
                this.darkness.SetTile(new Vector3Int(x, y, 0), this.darknessTile);
            }
        }

        for (var x = 0; x < this.size; x++) {
            for (var y = 0; y < this.size; y++) {
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
                        if (Random.Range(0, this.treeDensity) == 0) {
                            var treePos = this.ground.GetCellCenterWorld(new Vector3Int(x, y, 0));
                            Instantiate(this.tree, treePos, Quaternion.identity, this.decorations);
                        }
                        tile = this.darkGrass;
                    } else
                        tile = this.grass;
                }
                this.ground.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        yield return null; // tilemap collider updates in LateUpdate, so wait a frame

        for (var i = 0; i < this.berryBushVeinAmount; i++) {
            var center = this.GetPosAroundCenter(this.size / 2 - this.berryBushVeinSize);
            for (var j = 0; j < this.berryBushAmountPerVein; j++) {
                var pos = center + new Vector3Int(
                              Random.Range(-this.berryBushVeinSize, this.berryBushVeinSize),
                              Random.Range(-this.berryBushVeinSize, this.berryBushVeinSize), 0);
                var worldPos = this.ground.GetCellCenterWorld(pos);
                if (!Physics2D.OverlapCircle(worldPos, 0.5F, this.objectCollisionLayers)) {
                    Instantiate(this.berryBush, worldPos, Quaternion.identity, this.decorations);
                }
            }
        }

        var townCenterColl = this.townCenter.GetComponent<BoxCollider2D>();
        for (var i = 0; i < this.objectSpawnTries; i++) {
            var pos = this.ground.GetCellCenterWorld(this.GetPosAroundCenter(this.objectSpawnRadius));
            if (!Physics2D.OverlapBox(pos, townCenterColl.size, 0, this.objectCollisionLayers)) {
                Instantiate(this.townCenter, pos, Quaternion.identity, this.decorations);
                CameraController.Instance.CenterCameraOn(pos);
                break;
            }
        }

        var peopleSpawned = 0;
        for (var i = 0; i < this.objectSpawnTries; i++) {
            var pos = this.ground.GetCellCenterWorld(this.GetPosAroundCenter(this.objectSpawnRadius));
            if (!Physics2D.OverlapCircle(pos, 0.5F, this.objectCollisionLayers)) {
                this.CreatePerson(pos);
                peopleSpawned++;
                if (peopleSpawned >= this.personCount)
                    break;
            }
        }

        var worldCenter = this.ground.CellToWorld(new Vector3Int(this.size / 2, this.size / 2, 0));
        foreach (var graph in AstarPath.active.graphs) {
            var grid = graph as GridGraph;
            if (grid != null) {
                grid.center = worldCenter;
                grid.SetDimensions(this.size, this.size, grid.nodeSize);
            }
        }
        AstarPath.active.Scan();
    }

    private Vector3Int GetPosAroundCenter(int radius) {
        var x = Random.Range(-radius, radius) + this.size / 2;
        var y = Random.Range(-radius, radius) + this.size / 2;
        return new Vector3Int(x, y, 0);
    }

    public void Discover(Vector2 position, int radius) {
        var local = this.darkness.WorldToCell(position);
        for (var x = -radius; x <= radius; x++) {
            for (var y = -radius; y <= radius; y++) {
                var offset = local + new Vector3Int(x, y, 0);
                if (Vector3Int.Distance(local, offset) < radius) {
                    this.darkness.SetTile(offset, null);
                }
            }
        }
    }

    public bool IsDiscovered(Vector2 position) {
        var local = this.darkness.WorldToCell(position);
        return !this.darkness.GetTile(local);
    }

    public Person CreatePerson(Vector2 position) {
        var newPerson = Instantiate(this.person, position, Quaternion.identity, this.people);
        var selectable = newPerson.GetComponent<Selectable>();
        selectable.menuName = this.nameArray[Random.Range(0, this.nameArray.Length)];
        return newPerson;
    }

}