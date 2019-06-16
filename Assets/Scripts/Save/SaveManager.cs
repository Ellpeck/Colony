using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

public class SaveManager : MonoBehaviour {

    private static readonly BinaryFormatter Formatter = new BinaryFormatter();

    public Transform resources;
    public Transform buildings;
    public Transform people;
    public GameObject[] prefabs;
    public float saveInterval;

    private Guid saveId;
    private string saveName;
    private float saveTimer;

    private void Start() {
        var worldGen = WorldGenerator.Instance;

        var data = FindObjectOfType<GameStartData>();
        if (data) {
            Destroy(data.gameObject);
            
            this.saveId = data.saveId;
            this.saveName = data.saveName;

            if (data.isLoadedGame) {
                this.Load();
                worldGen.Generate(data.seed, true);
                return;
            }
        } else {
            this.saveId = Guid.Empty;
            this.saveName = "Editor";
        }

        worldGen.GenerateDarkness(null);
        worldGen.Generate(Random.Range(0, 100000), false);
    }

    private void Update() {
        this.saveTimer += Time.unscaledDeltaTime;
        if (this.saveTimer >= this.saveInterval) {
            this.saveTimer = 0;
            this.Save();
            Debug.Log("Saved game");
        }
    }

    private void Save() {
        var generator = WorldGenerator.Instance;
        var summary = new SaveData(this.saveName, DateTime.Now, generator.Seed);
        Save(this.saveId, "Summary", summary);

        var darknessData = new DarknessData(generator.size, generator.darknessBorder, generator.darkness);
        Save(this.saveId, "Darkness", darknessData);

        var resourceData = new List<ResourceSource.Data>();
        foreach (var res in this.resources.GetComponentsInChildren<ResourceSource>())
            resourceData.Add(new ResourceSource.Data(res));
        Save(this.saveId, "Resources", resourceData);

        var buildingData = new List<Building.Data>();
        foreach (var building in this.buildings.GetComponentsInChildren<Building>()) {
            var dat = Building.Data.Save(building);
            if (dat != null)
                buildingData.Add(dat);
        }
        Save(this.saveId, "Buildings", buildingData);

        var personData = new List<Person.Data>();
        foreach (var person in this.people.GetComponentsInChildren<Person>())
            personData.Add(new Person.Data(person));
        Save(this.saveId, "People", personData);

        var resourceManagerData = new ResourceManager.Data(ResourceManager.Instance);
        Save(this.saveId, "ResourceManager", resourceManagerData);
    }

    private void Load() {
        var resourceData = Load<List<ResourceSource.Data>>(this.saveId, "Resources");
        foreach (var res in resourceData) {
            var inst = this.InstantiatePrefab<ResourceSource>(res.prefabName, this.resources);
            res.Load(inst);
        }

        var buildingData = Load<List<Building.Data>>(this.saveId, "Buildings");
        foreach (var building in buildingData) {
            var inst = this.InstantiatePrefab<Building>(building.prefabName, this.buildings);
            building.Load(inst);
        }

        var personData = Load<List<Person.Data>>(this.saveId, "People");
        foreach (var person in personData) {
            var inst = this.InstantiatePrefab<Person>(person.prefabName, this.people);
            person.Load(inst);
        }

        var resourceManagerData = Load<ResourceManager.Data>(this.saveId, "ResourceManager");
        resourceManagerData.Load(ResourceManager.Instance);

        var darknessData = Load<DarknessData>(this.saveId, "Darkness");
        WorldGenerator.Instance.GenerateDarkness(darknessData);
    }

    public static SaveData LoadSummary(Guid saveId) {
        return Load<SaveData>(saveId, "Summary");
    }

    private T InstantiatePrefab<T>(string name, Transform parent) {
        var prefab = Array.Find(this.prefabs, obj => obj.name == name);
        var inst = Instantiate(prefab, parent);
        return inst.GetComponent<T>();
    }

    private static void Save(Guid saveId, string fileName, object data) {
        var file = GetPath(saveId, fileName);
        if (file.Directory != null && !file.Directory.Exists)
            file.Directory.Create();
        if (file.Exists)
            file.Delete();

        using (var stream = file.OpenWrite()) {
            Formatter.Serialize(stream, data);
        }
    }

    private static T Load<T>(Guid saveId, string fileName) {
        var file = GetPath(saveId, fileName);
        if (file.Directory == null || !file.Directory.Exists)
            return default;
        if (!file.Exists)
            return default;

        using (var stream = file.OpenRead()) {
            var data = Formatter.Deserialize(stream);
            return (T) data;
        }
    }

    private static FileInfo GetPath(Guid saveId, string fileName) {
        return new FileInfo(Path.Combine(GetSaveFolder(), saveId.ToString(), fileName));
    }

    public static string GetSaveFolder() {
        return Path.Combine(Application.persistentDataPath, "Saves");
    }

}