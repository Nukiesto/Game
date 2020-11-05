using System.Collections;
using System.Linq;
using Game.ChunkSystem;
using Game.Game;
using Photon.Pun;
using SavingSystem;
using Singleton;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class WorldSaver : MonoBehaviour
    {
        public WorldSavingSystem.WorldSaving worldSaving;

        public delegate void OnLoad(WorldSavingSystem.WorldSaving worldSaving);

        public event OnLoad OnLoadEvent;
    
        public delegate void OnSave(WorldSavingSystem.WorldSaving worldSaving);

        public event OnSave OnSaveEvent;

        private void Start()
        {
            var worldManager = Toolbox.Instance.mWorldManager;
            worldManager.OnStartSceneEvent += InitWorld;
        }

        public void InitWorld()
        {
            var manager = Toolbox.Instance.mMultiPlayerManager;
            if (!manager.IsOfflineGame && !PhotonNetwork.IsMasterClient)
            {
                //Debug.Log("Did`nt created world in not master client");
                return;
            }
            
            worldSaving = new WorldSavingSystem.WorldSaving(WorldSavingSystem.WorldsList);
            if (!worldSaving.LoadWorldName(WorldSavingSystem.CurrentWorld)) return;
        
            if (!worldSaving.WorldDataUnit.toGenerateWorld)
            {
                ChunkManager.Instance.BuildChunks(false);
                LoadWorld();
            }
            else
            {
                ChunkManager.Instance.BuildChunks(true);
            }
        }
        public void SaveWorld()
        {
            if (worldSaving != null)
            {
                worldSaving.Clear();
                worldSaving.WorldDataUnit.toGenerateWorld = false;
            
                OnSaveEvent?.Invoke(worldSaving);
            
                worldSaving.SaveWorld();
            }
        }

        private void LoadWorld()
        {
            worldSaving.LoadWorld();
        
            OnLoadEvent?.Invoke(worldSaving);

            StartCoroutine(WriteInventory());
        }

        public void InsertStartOnSave(OnSave method)
        {
            var list = OnSaveEvent?.GetInvocationList().ToList();

            if (list != null)
            {
                OnSaveEvent = null;
            
                list.Insert(0, method);

                foreach (var item in list)
                {
                    //Debug.Log(item.Method.Name);
                    OnSaveEvent += (OnSave)item;
                }
            }
        }
        private IEnumerator WriteInventory()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                //yield return new WaitForSeconds(1f);
                var playerData = worldSaving.PlayerData;
                //Debug.Log(GameCond.GetInventory());
                if (playerData != null)
                {
                    GameCond.GetInventory().WriteItems(playerData.items);
                }
                yield break;
            }
        }
        public struct EntityUnitData
        {
            public float X;
            public float Y;
            public EntityType EntityType;
            public int ChunkX;
            public int ChunkY;
        }
        public struct ItemUnitData
        {
            public float X;
            public float Y;
            public string Name;
            public int ChunkX;
            public int ChunkY;
        }

        private void OnApplicationQuit()
        {
            if (GameCond.IsGame)
            {
                Debug.Log("OnQuitSaved");
                SaveWorld();  
            }
        }
    }
}