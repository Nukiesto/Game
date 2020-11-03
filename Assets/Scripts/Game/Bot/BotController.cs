using Game.Entity;
using Game.HpSystem;
using UnityEngine;

namespace Game.Bot
{
    public class BotController : MonoBehaviour
    {
        [SerializeField] private EntityType type;
        [SerializeField] private HpEntity hpEntity;
        [HideInInspector] public EntityManager EntityManager;
        
        public EntityType Type { get; private set; }

        private void Awake()
        {
            SetEntity(type);
        }

        private void Start()
        {
            hpEntity.OnDeathEvent += Death;
        }

        private void Death()
        {
            Debug.Log("Success");
            hpEntity.FillHp();
            gameObject.SetActive(false);
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
