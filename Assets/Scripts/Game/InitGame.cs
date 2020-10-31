using System;
using Singleton;
using UnityEngine;

namespace Game
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
            toolbox.InitGame = this;
            toolbox.mWorldSaver.InitWorld();
            toolbox.mWorldManager.InitCamera(mainCamera);
            toolbox.mWorldManager.InitSpawnPoint();
        }
    }
}