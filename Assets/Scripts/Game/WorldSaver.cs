using System;
using SavingSystem;
using Singleton;
using UnityEngine;

public class WorldSaver : MonoBehaviour
{
    public WorldSavingSystem.WorldSaving worldSaving;
    public void InitWorld()
    {
        worldSaving = new WorldSavingSystem.WorldSaving(WorldSavingSystem.WorldsList);
        if (worldSaving.LoadWorldName(WorldSavingSystem.CurrentWorld))
        {
            if (!worldSaving.WorldDataUnit.toGenerateWorld)
            {
                LoadWorld();
            }
        }
    }
    public void SaveWorld()
    {
        worldSaving.Clear();
        worldSaving.WorldDataUnit.toGenerateWorld = false;
        
        var toolbox = Toolbox.Instance;
        
        var entities = toolbox.mEntityManager.GetEntitiesData();
        var items = toolbox.mItemManager.GetItemsData();
        var player = PlayerController.Instance;
        //Debug.Log("Entities:" + entities.Count);
        //Debug.Log("Items:" + items.Count);
        
        
        //Player
        var savePos = player.transform.position;
        var playerData = new WorldSavingSystem.PlayerData();
        playerData.x = savePos.x;
        playerData.y = savePos.y;
        worldSaving.PlayerData = playerData;
        worldSaving.AddPlayerData();
        
        ChunkManager.Instance.AddDataToWorldSaving(worldSaving);
        
        worldSaving.WorldDataUnit.AddEntities(entities);
        worldSaving.WorldDataUnit.AddItems(items);
        
        worldSaving.SaveWorld();
    }
    public void LoadWorld()
    {
        worldSaving.LoadWorld();
        
        ChunkManager.Instance.LoadWorld(worldSaving);
        
        //Player
        worldSaving.LoadPlayerData();
        var player = PlayerController.Instance;
        var playerData = worldSaving.PlayerData;
        if (playerData != null)
        {
            var newPos = new Vector3();
            newPos.x = playerData.x;
            newPos.y = playerData.y;
            player.transform.position = newPos;
            Camera.current.transform.position = newPos;
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
        if (Toolbox.Instance.mGameSceneManager.CurrentScene == GameScene.Game)
        {
            Debug.Log("OnQuitSaved");
            SaveWorld();  
        }
    }
}