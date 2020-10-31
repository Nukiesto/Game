using System;
using Singleton;
using UnityEngine;

namespace Game
{
    public class WorldManager : MonoBehaviour
    {
        public Vector3 SpawnPoint { get; private set; }
        
        private bool _loadedPointIsInit;
        public Vector3 loadedPoint;

        public bool TryGetLoadedPoint(out Vector3 loadedPoint)
        {
            loadedPoint = this.loadedPoint;
            return _loadedPointIsInit;
        }
        public Camera MainCamera { get; private set; }

        private GameSceneManager _sceneManager;
        private Toolbox _toolbox;
        public bool IsGame => _sceneManager.CurrentScene == GameScene.Game;

        private void Start()
        {
            _toolbox = Toolbox.Instance;
            _sceneManager = _toolbox.mGameSceneManager;
        }

        public void InitCamera(Camera camera)
        {
            MainCamera = camera;
        }
        
        public void InitSpawnPoint()
        {
            var worldSaving = _toolbox.mWorldSaver.worldSaving;
            worldSaving.LoadPlayerData();
            var playerData = worldSaving.PlayerData;
            var pos = Vector3.zero;

            if (playerData != null && playerData.spawnPointInited)
            {
                pos = new Vector3(playerData.spawnX, playerData.spawnY, 0);
                //Debug.Log("InitSpawnPoint: " + playerData.spawnX + "; " + playerData.spawnY);
            }
            else
            {
                var chunkManager = ChunkManager.Instance;
                var generator = chunkManager.generator;
                var posZero = chunkManager.posZero;
                pos.x = posZero.x + generator.worldWidth / 2;
                pos.y = posZero.y + generator.worldHeight - 4;
                
                //Debug.Log("InitSpawnPoint");
            }
            
            SpawnPoint = pos;
            //Debug.Log("InitSpawnPoint");
        }
        public void InitLoadedPoint()
        {
            var worldSaving = _toolbox.mWorldSaver.worldSaving;
            worldSaving.LoadPlayerData();
            var playerData = worldSaving.PlayerData;
            var pos = Vector3.zero;
        
            if (playerData != null)
            {
                pos.x = playerData.x;
                pos.y = playerData.y;
                _loadedPointIsInit = true;
                loadedPoint = pos;
                //Debug.Log("InitLoadedPoint");
            }
        }

        public void MoveCameraToPoint(Vector3 pos)
        {
            var posCamera = pos;
            posCamera.z = -10;
            GameCond.MainCamera.transform.position = posCamera;
        }
    }
}