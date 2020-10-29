using System;
using System.Collections.Generic;
using Game.Bot;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    private List<BotController> _entities = new List<BotController>();
    
    public void AddEntity(BotController entity)
    {
        _entities.Add(entity);
    }
    public List<WorldSaver.EntityUnitData> GetEntitiesData()
    {
        var list = new List<WorldSaver.EntityUnitData>();
        var chunkManager = ChunkManager.Instance;
        foreach (var entity in _entities)
        {
            if (entity != null)
            {
                var pos = entity.gameObject.transform.position;
                var chunk = chunkManager.GetChunk(pos);
                if (chunk != null)
                {
                    var posChunk = chunk.posChunk;
                    var data = new WorldSaver.EntityUnitData()
                    {
                        EntityType = entity.Type,
                        X = pos.x,
                        Y = pos.y,
                        ChunkX = posChunk.x,
                        ChunkY = posChunk.y
                    };
                    list.Add(data);
                }
            }
        }

        return list;
    }
    public void RemoveEntity(BotController entity)
    {
        _entities.Remove(entity);
    }
    
    public BotController Create(Vector3 pos, EntityType type)
    {
        var bot = PoolManager.GetObject("Entity", pos, Quaternion.identity).GetComponent<BotController>();
        
        bot.EntityManager = this;
        bot.SetEntity(type);
        
        AddEntity(bot);
        
        return bot;
    }
}