using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartData : MonoBehaviour {

    public Guid saveId;
    public string saveName;
    public int seed;
    public bool isLoadedGame;

    public static GameStartData Create() {
        var obj = new GameObject("GameStartData");
        var data = obj.AddComponent<GameStartData>();
        DontDestroyOnLoad(obj);
        return data;
    }

}