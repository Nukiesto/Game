using System;
using Game.Misc;
using UnityEngine;

namespace Game.HpSystem
{
    [RequireComponent(typeof(HpObject))]
    public class HpBase : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private HpObject hpObject;
        [SerializeField] private Movement movement;
        
        public delegate void OnDeath();

        public event OnDeath OnDeathEvent;

        private void Awake()
        {
            hpObject.hpBase = this;
        }

        private void Start()
        {
            movement.OnToFallEvent += OnToFall;
        }

        private void OnToFall(float hp)
        {
            hpObject.Adjust(hp);
        }
        public void Death()
        {
            OnDeathEvent?.Invoke();
        }

        public void FillHp()
        {
            hpObject.FillHp();
        }

        public void SetGodMode(bool value)
        {
            hpObject.SetGodMode(value);
        }

        public void SetHp(float hp)
        {
            hpObject.SetHp(hp);
        }

        public float GetHp()
        {
            return hpObject.CurrentHp;
        }
    }
}