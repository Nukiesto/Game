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

        public void SetGameMode(GameMode gameMode)
        {
            _gameMode = gameMode;
        }

        public GameMode GetGameMode()
        {
            return _gameMode;
        }
    }
}
