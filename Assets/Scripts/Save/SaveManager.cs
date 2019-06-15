using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour {

    public static SaveManager Instance;

    public string saveName;
    public Transform resources;
    public Transform buildings;
    public Transform people;
    public GameObject[] prefabs;

    private BinaryFormatter formatter;

    private void Awake() {
        Instance = this;
        this.formatter = new BinaryFormatter();
    }

    public void Save(WorldGenerator generator) {
        var data = new SaveData(CameraController.Instance.transform.position, WorldGenerator.Instance.Seed);
        this.Save("Summary", data);

        var darknessData = new DarknessData(generator.size, generator.darknessBorder, generator.darkness);
        this.Save("Darkness", darknessData);

        var resourceData = new List<ResourceSource.Data>();
        foreach (var res in this.resources.GetComponentsInChildren<ResourceSource>())
            resourceData.Add(new ResourceSource.Data(res));
        this.Save("Resources", resourceData);

        var buildingData = new List<Building.Data>();
        foreach (var building in this.buildings.GetComponentsInChildren<Building>()) {
            var dat = Building.Data.Save(building);
            if (dat != null)
                buildingData.Add(dat);
        }
        this.Save("Buildings", buildingData);

        var personData = new List<Person.Data>();
        foreach (var person in this.people.GetComponentsInChildren<Person>())
            personData.Add(new Person.Data(person));
        this.Save("People", personData);

        var resourceManagerData = new ResourceManager.Data(ResourceManager.Instance);
        this.Save("ResourceManager", resourceManagerData);
    }

    public (SaveData summary, DarknessData darkness) Load() {
        var summary = this.Load<SaveData>("Summary");
        if (summary == null)
            return (null, null);
        CameraController.Instance.transform.position = summary.cameraPosition;

        var darknessData = this.Load<DarknessData>("Darkness");

        var resourceData = this.Load<List<ResourceSource.Data>>("Resources");
        foreach (var res in resourceData) {
            var inst = this.InstantiatePrefab<ResourceSource>(res.prefabName, this.resources);
            res.Load(inst);
        }

        var buildingData = this.Load<List<Building.Data>>("Buildings");
        foreach (var building in buildingData) {
            var inst = this.InstantiatePrefab<Building>(building.prefabName, this.buildings);
            building.Load(inst);
        }

        var personData = this.Load<List<Person.Data>>("People");
        foreach (var person in personData) {
            var inst = this.InstantiatePrefab<Person>(person.prefabName, this.people);
            person.Load(inst);
        }

        var resourceManagerData = this.Load<ResourceManager.Data>("ResourceManager");
        resourceManagerData.Load(ResourceManager.Instance);

        return (summary, darknessData);
    }

    private T InstantiatePrefab<T>(string name, Transform parent) {
        var prefab = Array.Find(this.prefabs, obj => obj.name == name);
        var inst = Instantiate(prefab, parent);
        return inst.GetComponent<T>();
    }

    private void Save(string fileName, object data) {
        var file = this.GetPath(fileName);
        if (file.Directory != null && !file.Directory.Exists)
            file.Directory.Create();
        if (file.Exists)
            file.Delete();

        using (var stream = file.OpenWrite()) {
            this.formatter.Serialize(stream, data);
        }
    }

    private T Load<T>(string fileName) {
        var file = this.GetPath(fileName);
        if (file.Directory == null || !file.Directory.Exists)
            return default;
        if (!file.Exists)
            return default;

        using (var stream = file.OpenRead()) {
            var data = this.formatter.Deserialize(stream);
            return (T) data;
        }
    }

    private FileInfo GetPath(string fileName) {
        var path = Path.Combine(Application.persistentDataPath, "Saves", this.saveName, fileName);
        return new FileInfo(path);
    }

}