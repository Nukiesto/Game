using Game.ChunkSystem;
using Game.Game;
using Photon.Pun;
using SavingSystem;
using Singleton;
using UnityEngine;

namespace Game.World
{
    public class WorldManager : MonoBehaviour
    {
        public Vector3 SpawnPoint { get; private set; }
        
        private bool _loadedPointIsInit;
        public Vector3 loadedPoint;

        public delegate void OnStartScene();

        public event OnStartScene OnStartSceneEvent;
        
        public bool TryGetLoadedPoint(out Vector3 tryLoadedPoint)
        {
            tryLoadedPoint = loadedPoint;
            return _loadedPointIsInit;
        }

        private SceneManager _sceneManager;
        private Toolbox _toolbox;
        public bool IsGame => _sceneManager.CurrentScene == GameScene.Game;

        private void Start()
        {
            _toolbox = Toolbox.Instance;
            _sceneManager = _toolbox.mSceneManager;
            
            var worldSaver = Toolbox.Instance.mWorldSaver;
            worldSaver.OnLoadEvent += OnLoad;

            OnStartSceneEvent += InitSpawnPoint;
        }
        
        private void OnLoad(WorldSavingSystem.WorldSaving worldSaving)
        {
            InitLoadedPoint();
        }
        public void InitSpawnPoint()
        {
            var pos = Vector3.zero;
            if (PhotonNetwork.IsMasterClient)
            {
                var worldSaving = _toolbox.mWorldSaver.worldSaving;
                worldSaving.LoadPlayerData();
                var playerData = worldSaving.PlayerData;
                
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
                }
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
            Debug.Log("InitSpawnPoint");
        }

        private void InitLoadedPoint()
        {
            var worldSaving = _toolbox.mWorldSaver.worldSaving;
            worldSaving.LoadPlayerData();
            var playerData = worldSaving.PlayerData;
            var pos = Vector3.zero;

            if (playerData == null) return;
            
            pos.x = playerData.x;
            pos.y = playerData.y;
            _loadedPointIsInit = true;
            loadedPoint = pos;
            //Debug.Log("InitLoadedPoint");
        }

        public void MoveCameraToPoint(Vector3 pos)
        {
            var posCamera = pos;
            posCamera.z = -10;
            GameCond.MainCamera.transform.position = posCamera;
        }

        public void SetSpawnPoint(Vector3 pos)
        {
            SpawnPoint = pos;
        }

        public void SceneStarted()
        {
            OnStartSceneEvent?.Invoke();
        }
    }
}