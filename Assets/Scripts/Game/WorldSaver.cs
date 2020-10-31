﻿using System;
using System.Collections;
using Game;
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
        if (worldSaving != null)
        {
            worldSaving.Clear();
            worldSaving.WorldDataUnit.toGenerateWorld = false;
        
            var toolbox = Toolbox.Instance;
        
            var entities = toolbox.mEntityManager.GetEntitiesData();
            var items = toolbox.mItemManager.GetItemsData();
            
            //Debug.Log("Entities:" + entities.Count);
            //Debug.Log("Items:" + items.Count);
            
            //Player
            var player = PlayerController.Instance;
            var savePos = player.transform.position;
            var playerData = new WorldSavingSystem.PlayerData();
            var worldManager = toolbox.mWorldManager;
            playerData.x = savePos.x;
            playerData.y = savePos.y;
            playerData.spawnX = worldManager.SpawnPoint.x;
            playerData.spawnY = worldManager.SpawnPoint.y;
            playerData.spawnPointInited = true;
            
            var inventory = toolbox.InitGame.inventory;
            playerData.items = inventory.GetItems();
            
            worldSaving.PlayerData = playerData;
            worldSaving.AddPlayerData();

            ChunkManager.Instance.AddDataToWorldSaving(worldSaving);
        
            worldSaving.WorldDataUnit.AddEntities(entities);
            worldSaving.WorldDataUnit.AddItems(items);
            
            worldSaving.SaveWorld();
        }
    }
    public void LoadWorld()
    {
        var toolbox = Toolbox.Instance;
        
        worldSaving.LoadWorld();
        var chunkManager = ChunkManager.Instance;
        
        var world = worldSaving.WorldDataUnit;
        chunkManager.generator.worldWidth = world.width;
        chunkManager.generator.worldHeight = world.height;
        
        chunkManager.LoadWorld(worldSaving);
        
        toolbox.mWorldManager.InitLoadedPoint();

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