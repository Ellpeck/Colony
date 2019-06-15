using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class InGameManagement : MonoBehaviour {

    public bool shouldSave;

    private void Start() {
        var (summary, darknessData) = SaveManager.Instance.Load();
        if (summary == null) {
            WorldGenerator.Instance.Generate(Random.Range(0, 100000), false, darknessData);
        } else {
            WorldGenerator.Instance.Generate(summary.seed, true, darknessData);
        }
    }

    private void Update() {
        if (this.shouldSave) {
            this.shouldSave = false;
            SaveManager.Instance.Save(WorldGenerator.Instance);
        }
    }

}