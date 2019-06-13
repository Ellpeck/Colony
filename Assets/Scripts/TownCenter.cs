using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownCenter : MonoBehaviour {

    public float personCreationTime;
    public Transform personSpawnPoint;
    public GameObject createPersonButton;

    public float CreatePersonTimer { get; private set; }

    private Building building;

    private void Start() {
        this.building = this.GetComponent<Building>();
    }

    public void GetInteractionItems(List<GameObject> items) {
        if (this.building.IsFinished)
            items.Add(Instantiate(this.createPersonButton));
    }

    public void CreatePerson() {
        if (this.CreatePersonTimer <= 0) {
            this.CreatePersonTimer = this.personCreationTime;
        }
    }

    private void Update() {
        if (this.CreatePersonTimer > 0) {
            this.CreatePersonTimer -= Time.deltaTime;
            if (this.CreatePersonTimer <= 0) {
                var gen = WorldGenerator.Instance;
                Instantiate(gen.person, this.personSpawnPoint.position, Quaternion.identity, gen.people);
            }
        }
    }

}