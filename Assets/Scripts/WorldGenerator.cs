﻿using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class WorldGenerator : MonoBehaviour {

    public static WorldGenerator Instance { get; private set; }

    public int size;
    public Tilemap ground;
    public Tilemap darkness;
    public Transform resources;
    public Transform buildings;
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
    public int townCenterSpawnRadius;
    public int personSpawnTries;
    public int personSpawnRadius;
    public float personCount;
    public Person person;
    public Building townCenter;

    public ClutterGenerator[] clutterGenerators;

    public int Seed { get; private set; }
    private string[] nameArray;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        this.nameArray = this.names.text.Split('\n');
    }

    public void Generate(int seed, bool loading) {
        this.Seed = seed;
        this.StartCoroutine(this.GenerateMap(loading));
    }

    public void GenerateDarkness(DarknessData data) {
        if (data == null) {
            for (var x = -this.darknessBorder; x < this.size + this.darknessBorder; x++) {
                for (var y = -this.darknessBorder; y < this.size + this.darknessBorder; y++) {
                    this.darkness.SetTile(new Vector3Int(x, y, 0), this.darknessTile);
                }
            }
        } else {
            data.Load(this.size, this.darknessBorder, this.darkness, this.darknessTile);
        }
    }

    private IEnumerator GenerateMap(bool loading) {
        Physics2D.autoSyncTransforms = true;
        Random.InitState(this.Seed);

        for (var x = 0; x < this.size; x++) {
            for (var y = 0; y < this.size; y++) {
                var noise = (Mathf.PerlinNoise(this.Seed + x / this.perlinScale1, this.Seed + y / this.perlinScale1) +
                             Mathf.PerlinNoise(this.Seed + x / this.perlinScale2, this.Seed + y / this.perlinScale2) * 3F) / 4F;
                Tile tile;
                if (noise <= this.waterHeight)
                    tile = this.water;
                else if (noise <= this.sandHeight)
                    tile = this.sand;
                else {
                    var treeNoise = (Mathf.PerlinNoise(this.Seed + x / this.treePerlinScale1, this.Seed + y / this.treePerlinScale1) +
                                     Mathf.PerlinNoise(this.Seed + x / this.treePerlinScale2, this.Seed + y / this.treePerlinScale2) * 2F) / 3F;
                    if (treeNoise <= this.treeHeight) {
                        if (!loading && Random.Range(0, this.treeDensity) == 0) {
                            var treePos = this.ground.GetCellCenterWorld(new Vector3Int(x, y, 0));
                            Instantiate(this.tree, treePos, Quaternion.identity, this.resources);
                        }
                        tile = this.darkGrass;
                    } else
                        tile = this.grass;
                }
                this.ground.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        yield return new WaitForEndOfFrame(); // tilemap collider updates in LateUpdate, so wait

        if (!loading) {
            foreach (var gen in this.clutterGenerators)
                gen.Spawn(this);

            var townCenterInst = Instantiate(this.townCenter, this.buildings);
            for (var x = -this.townCenterSpawnRadius; x <= this.townCenterSpawnRadius; x++) {
                for (var y = -this.townCenterSpawnRadius; y <= this.townCenterSpawnRadius; y++) {
                    var pos = this.ground.GetCellCenterWorld(new Vector3Int(this.size / 2 + x, this.size / 2 + y, 0));
                    townCenterInst.transform.position = pos;
                    if (townCenterInst.IsValidPosition()) {
                        townCenterInst.SetMode(false, true);
                        CameraController.Instance.CenterCameraOn(pos);
                        goto townCenterSpawned;
                    }
                }
            }
            townCenterSpawned:

            var peopleSpawned = 0;
            for (var i = 0; i < this.personSpawnTries; i++) {
                var randomPos = townCenterInst.transform.position + new Vector3(
                                    Random.Range(-this.personSpawnRadius, this.personSpawnRadius),
                                    Random.Range(-this.personSpawnRadius, this.personSpawnRadius));
                if (!Physics2D.OverlapCircle(randomPos, 0.5F, this.objectCollisionLayers)) {
                    this.CreatePerson(randomPos);
                    peopleSpawned++;
                    if (peopleSpawned >= this.personCount)
                        break;
                }
            }
        }

        var worldCenter = this.ground.CellToWorld(new Vector3Int(this.size / 2, this.size / 2, 0));
        foreach (var graph in AstarPath.active.graphs) {
            if (graph is GridGraph grid) {
                grid.center = worldCenter;
                grid.SetDimensions(this.size, this.size, grid.nodeSize);
            }
        }
        AstarPath.active.Scan();

        Physics2D.autoSyncTransforms = false;
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

    public bool IsOutOfBounds(PolygonCollider2D collider) {
        foreach (var point in collider.points) {
            var local = this.ground.WorldToCell(collider.transform.position + (Vector3) point);
            if (this.IsOutOfBounds(local))
                return true;
        }
        return false;
    }

    private bool IsOutOfBounds(Vector3Int position) {
        return position.x < 0 || position.y < 0 || position.x >= this.size || position.y >= this.size;
    }

    public void CreatePerson(Vector2 position) {
        var newPerson = Instantiate(this.person, position, Quaternion.identity, this.people);
        var selectable = newPerson.GetComponent<Selectable>();
        selectable.menuName = this.nameArray[Random.Range(0, this.nameArray.Length)];
    }

}