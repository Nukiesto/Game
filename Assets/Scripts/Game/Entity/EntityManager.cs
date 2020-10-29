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
        foreach (var entity in _entities)
        {
            var pos = entity.gameObject.transform.position;
            var data = new WorldSaver.EntityUnitData()
            {
                EntityType = entity.Type,
                X = pos.x,
                Y = pos.y
            };
            list.Add(data);
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
        bot.SetEntity(type);
        AddEntity(bot);
        
        return bot;
    }
}