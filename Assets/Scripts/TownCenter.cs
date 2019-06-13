using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownCenter : MonoBehaviour {

    public float personCreationTime;
    public int maxPersonQueue;
    public Resource requiredResource;
    public Transform personSpawnPoint;
    public GameObject createPersonButton;

    public float CreatePersonTimer { get; private set; }
    public float QueuedPersonAmount { get; private set; }

    private Building building;

    private void Start() {
        this.building = this.GetComponent<Building>();
    }

    public void GetInteractionItems(List<GameObject> items) {
        if (this.building.IsFinished)
            items.Add(Instantiate(this.createPersonButton));
    }

    public void CreatePerson() {
        if (this.QueuedPersonAmount < this.maxPersonQueue && ResourceManager.Instance.HasResource(this.requiredResource)) {
            ResourceManager.Instance.Take(this.requiredResource.type, this.requiredResource.amount);
            this.QueuedPersonAmount++;
        }
    }

    private void Update() {
        if (this.CreatePersonTimer > 0) {
            if (TownStats.Instance.GetVillagerAmount() < TownStats.Instance.villagerLimit) {
                this.CreatePersonTimer -= Time.deltaTime;
                if (this.CreatePersonTimer <= 0) {
                    var gen = WorldGenerator.Instance;
                    Instantiate(gen.person, this.personSpawnPoint.position, Quaternion.identity, gen.people);
                }
            }
        } else if (this.QueuedPersonAmount > 0) {
            this.CreatePersonTimer = this.personCreationTime;
            this.QueuedPersonAmount--;
        }
    }

}