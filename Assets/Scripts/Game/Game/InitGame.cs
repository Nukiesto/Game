using Game.ItemSystem;
using Singleton;
using UnityEngine;

namespace Game.Game
{
    public class InitGame : MonoBehaviour
    {
        public Camera mainCamera;
        public Inventory inventory;
        
        private void Awake()
        {
            GameCond.Inventory = inventory;
            GameCond.MainCamera = mainCamera;
            GameCond.IsGame = true;
        }

        private void Start()
        {
            var toolbox = Toolbox.Instance;
            toolbox.mWorldManager.SceneStarted();
        }
    }
}