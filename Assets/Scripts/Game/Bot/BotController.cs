using System;
using Singleton;
using UnityEngine;

namespace Game.Bot
{
    public class BotController : MonoBehaviour
    {
        [SerializeField] private EntityType type;
        [HideInInspector] public EntityManager EntityManager;
        
        public EntityType Type { get; private set; }

        private void Awake()
        {
            SetEntity(type);
        }

        public void SetEntity(EntityType entityType)
        {
            Type = entityType;
        }
        
        private void OnDisable()
        {
            EntityManager?.RemoveEntity(this);
        }
    }
   
}

public enum EntityType
{
    TestBot
}
