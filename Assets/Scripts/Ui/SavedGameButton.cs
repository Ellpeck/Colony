using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SavedGameButton : MonoBehaviour {

    public TextMeshProUGUI saveName;
    public TextMeshProUGUI lastPlayed;
    public TextMeshProUGUI seed;

    private MainMenuManager manager;
    private Guid saveId;
    private SaveData summary;

    public void OnPress() {
        this.manager.OnLoadGameSubmit(this.saveId, this.summary);
    }

    public void Populate(MainMenuManager manager, Guid saveId, SaveData summary) {
        this.manager = manager;
        this.saveId = saveId;
        this.summary = summary;
        this.saveName.text = summary.saveName;
        this.lastPlayed.text = summary.lastPlayed.ToString();
        this.seed.text = summary.seed.ToString();
    }

}