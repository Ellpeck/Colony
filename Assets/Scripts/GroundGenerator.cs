using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundGenerator : MonoBehaviour {

    public Tilemap ground;
    public Transform decorations;
    public Transform people;
    public GameObject person;

    [Space] public int width;
    public int height;

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

    [Space] public LayerMask personCollisionLayers;
    public int personSpawnTries;
    public int personSpawnRadius;
    public float personCount;

    private int seed;

    private void Start() {
        this.seed = Random.Range(0, 100000);
        this.StartCoroutine(this.GenerateMap());
    }

    private IEnumerator GenerateMap() {
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

        var peopleSpawned = 0;
        for (var i = 0; i < this.personSpawnTries; i++) {
            var x = Random.Range(-this.personSpawnRadius, this.personSpawnRadius) + this.width / 2;
            var y = Random.Range(-this.personSpawnRadius, this.personSpawnRadius) + this.height / 2;
            var cell = this.ground.GetCellCenterWorld(new Vector3Int(x, y, 0));
            if (!Physics2D.OverlapCircle(cell, 0.5F, this.personCollisionLayers)) {
                Instantiate(this.person, cell, Quaternion.identity, this.people);
                peopleSpawned++;
                if (peopleSpawned >= this.personCount)
                    break;
            }
        }

        AstarPath.active.Scan();
    }

}