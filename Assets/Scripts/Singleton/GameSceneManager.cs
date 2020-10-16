﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameScene
{
    Game,
    MainMenu,
    undefined
}

public class GameSceneManager : MonoBehaviour
{
    private Dictionary<GameScene, string> gameScenes;
    public GameScene CurrentScene { get; private set; }

    private void Awake()
    {
        Init();
    }
    private void OnEnable()
    {
        SetSceneActive();
    }
    private void Init()
    {
        Debug.Log("Initiatialized");    
        gameScenes = new Dictionary<GameScene, string>
        {
            { GameScene.MainMenu, "MainMenu" },
            { GameScene.Game, "Game" }
        };
    }
    public void SetScene(GameScene type)
    {
        SceneManager.LoadSceneAsync(gameScenes[type]);
    }
    public void SetSceneAlt(GameScene type)
    {
        SceneManager.LoadSceneAsync(gameScenes[type]);
        CurrentScene = type;
    }
    public void SetSceneCurrent()
    {
        SceneManager.LoadSceneAsync(gameScenes[CurrentScene]);
    }
    private void SetSceneActive()
    {
        Scene scene = SceneManager.GetActiveScene();
        GameScene gameScene = gameScenes.Where(x => x.Value == scene.name).First().Key;
    }
}