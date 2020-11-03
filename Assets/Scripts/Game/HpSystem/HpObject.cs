using EasyButtons;
using UnityEngine;

namespace Game.HpSystem
{
    public class HpObject : MonoBehaviour
    {
        [SerializeField] private float maxHp = 100;
        [SerializeField] private bool updateBar = true;
        
        [HideInInspector] public HpBase hpBase;
        
        public BarManager barManager;
        public float CurrentHp { get; private set; }
        public float MaxHp { get => maxHp; }

        private void Start()
        {
            CurrentHp = maxHp;
            if (barManager != null) barManager.InitAllBar(maxHp);
        }

        public void Adjust(float value)
        {
            CurrentHp += value;
            UpdateBar();
            if (CurrentHp <= 0)
                Death();
        }
        public void UpdateBar()
        {
            if (updateBar)
                barManager.EnterAllValue(CurrentHp);
        }

        [Button]
        private void DebugHp()
        {
            Debug.Log("HP: " + CurrentHp);
        }
        
        [Button]
        public void Test()
        {
            Adjust(-5);
        }
        [Button]
        private void Death()
        {
            hpBase.Death();
        }

        public void FillHp()
        {
            CurrentHp = maxHp;
            UpdateBar();
        }
    }
}
