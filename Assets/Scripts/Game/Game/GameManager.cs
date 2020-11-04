using System;
using UnityEngine;

namespace Game.Game
{
    public enum GameMode
    {
        Survival,
        Sandbox
    }
    public class GameManager : MonoBehaviour
    {
        private GameMode _gameMode = GameMode.Survival;
        public delegate void OnChangeGameMode(GameMode gameMode);

        public event OnChangeGameMode OnChangeGameModeEvent;
        
        public void SetGameMode(GameMode gameMode)
        {
            _gameMode = gameMode;
            OnChangeGameModeEvent?.Invoke(gameMode);
        }

        public GameMode GetGameMode()
        {
            return _gameMode;
        }
    }
}
