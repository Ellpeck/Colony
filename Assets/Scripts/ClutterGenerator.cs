using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Clutter Generator")]
public class ClutterGenerator : ScriptableObject {

    public int veinAmount;
    public int veinSize;
    public int amountPerVein;
    public GameObject objectToSpawn;

    public void Spawn(WorldGenerator generator) {
        for (var i = 0; i < this.veinAmount; i++) {
            var center = new Vector3Int(
                Random.Range(this.veinSize, generator.size - this.veinSize),
                Random.Range(this.veinSize, generator.size - this.veinSize), 0);
            for (var j = 0; j < this.amountPerVein; j++) {
                var pos = center + new Vector3Int(
                              Random.Range(-this.veinSize, this.veinSize),
                              Random.Range(-this.veinSize, this.veinSize), 0);
                var worldPos = generator.ground.GetCellCenterWorld(pos);
                if (!Physics2D.OverlapCircle(worldPos, 0.5F, generator.objectCollisionLayers)) {
                    Instantiate(this.objectToSpawn, worldPos, Quaternion.identity, generator.resources);
                }
            }
        }
    }

}