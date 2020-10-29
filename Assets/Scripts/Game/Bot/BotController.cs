using System;
using Singleton;
using UnityEngine;

namespace Game.Bot
{
    public class BotController : MonoBehaviour
    {
        [SerializeField] private EntityType type;
        public EntityType Type { get; private set; }

        private void Awake()
        {
            SetEntity(type);
        }

        public void SetEntity(EntityType entityType)
        {
            Type = entityType;
        }

        private void OnEnable()
        {
            Toolbox.Instance.mEntityManager.AddEntity(this);
        }

        private void OnDisable()
        {
            Toolbox.Instance.mEntityManager.RemoveEntity(this);
        }
    }
   
}

public enum EntityType
{
    TestBot
}
