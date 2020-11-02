using EasyButtons;
using UnityEngine;

namespace Game.Misc.Misc
{
    public class HpObject : MonoBehaviour
    {
        [SerializeField] private float maxHp = 100;
        [SerializeField] private bool autoDestroy;

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
            if (autoDestroy && CurrentHp <= 0)
            {
                Destroy(gameObject);
            }
        }
        public void UpdateBar()
        {
            barManager.EnterAllValue(CurrentHp);
        }

        [Button]
        public void Test()
        {
            Adjust(-5);
        }
        [Button]
        public void Destroying()
        {
            Destroy(gameObject);
        }
    }
}
