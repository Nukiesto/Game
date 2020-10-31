using System.Collections.Generic;
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
    [SerializeField] private Dictionary<GameScene, string> gameScenes;

    [SerializeField] private GameScene currentScene;
    public GameScene CurrentScene { get => currentScene; private set => currentScene = value; }

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
        //Debug.Log("Initiatialized");    
        gameScenes = new Dictionary<GameScene, string>
        {
            { GameScene.MainMenu, "MainMenu" },
            { GameScene.Game, "Game" }
        };
    }
    public void SetScene(GameScene type)
    {
        SceneManager.LoadSceneAsync(gameScenes[type]);
        CurrentScene = type;
        GameCond.IsGame = type == GameScene.Game;
    }
    public void SetSceneAlt(GameScene type)
    {
        SceneManager.LoadSceneAsync(gameScenes[type]);
        CurrentScene = type;
        GameCond.IsGame = type == GameScene.Game;
    }
    public void SetSceneCurrent()
    {
        SceneManager.LoadSceneAsync(gameScenes[CurrentScene]);
    }
    private void SetSceneActive()
    {
        var scene = SceneManager.GetActiveScene();
        var gameScene = gameScenes.Where(x => x.Value == scene.name).First().Key;
    }
}
