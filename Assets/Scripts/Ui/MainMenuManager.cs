﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

    public GameObject main;
    public GameObject newGame;
    public TMP_InputField newGameName;
    public SceneAsset inGameScene;
    public GameObject loadGame;
    public Transform loadGameContent;
    public SavedGameButton saveButtonPrefab;

    public void OnNewGameButton() {
        this.main.SetActive(false);
        this.newGame.SetActive(true);
    }

    public void OnLoadGameButton() {
        this.main.SetActive(false);
        this.loadGame.SetActive(true);

        var saves = new DirectoryInfo(SaveManager.GetSaveFolder());
        foreach (var dir in saves.EnumerateDirectories()) {
            var guid = new Guid(dir.Name);
            var summary = SaveManager.LoadSummary(guid);
            if (summary != null) {
                var button = Instantiate(this.saveButtonPrefab, this.loadGameContent);
                button.Populate(this, guid, summary);
            }
        }
    }

    public void OnQuitButton() {
        Application.Quit();
    }

    public void OnNewGameSubmit() {
        if (this.newGameName.text.Length <= 0)
            return;
        var data = GameStartData.Create();
        data.saveId = Guid.NewGuid();
        data.saveName = this.newGameName.text;
        this.LoadInGame();
    }

    public void OnLoadGameSubmit(Guid saveId, SaveData summary) {
        var data = GameStartData.Create();
        data.saveId = saveId;
        data.saveName = summary.saveName;
        data.seed = summary.seed;
        data.isLoadedGame = true;
        this.LoadInGame();
    }

    private void LoadInGame() {
        SceneManager.LoadScene(this.inGameScene.name);
    }

    public void OnBack() {
        this.main.SetActive(true);
        this.newGame.SetActive(false);
        this.loadGame.SetActive(false);
    }

}