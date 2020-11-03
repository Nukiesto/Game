using System.Collections;
using Game.ChunkSystem;
using Game.Game;
using SavingSystem;
using Singleton;
using UnityEngine;

namespace Game
{
    public class WorldSaver : MonoBehaviour
    {
        public WorldSavingSystem.WorldSaving worldSaving;

        public delegate void OnLoad(WorldSavingSystem.WorldSaving worldSaving);

        public event OnLoad OnLoadEvent;
    
        public delegate void OnSave(WorldSavingSystem.WorldSaving worldSaving);

        public event OnLoad OnSaveEvent;
        public void InitWorld()
        {
            worldSaving = new WorldSavingSystem.WorldSaving(WorldSavingSystem.WorldsList);
            
            if (!worldSaving.LoadWorldName(WorldSavingSystem.CurrentWorld)) return;
            
            if (!worldSaving.WorldDataUnit.toGenerateWorld)
            {
                LoadWorld();
            }
        }
        public void SaveWorld()
        {
            //if (worldSaving == null) return;
        
            worldSaving.Clear();
            worldSaving.WorldDataUnit.toGenerateWorld = false;
        
            OnSaveEvent?.Invoke(worldSaving);

            worldSaving.SaveWorld();
        }

        private void LoadWorld()
        {
            worldSaving.LoadWorld();
        
            OnLoadEvent?.Invoke(worldSaving);

            StartCoroutine(WriteInventory());
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