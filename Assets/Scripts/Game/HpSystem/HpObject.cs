using EasyButtons;
using Photon.Pun;
using UnityEngine;

namespace Game.HpSystem
{
    public class HpObject : MonoBehaviour
    {
        [SerializeField] private float maxHp = 100;
        [SerializeField] private bool updateBar = true;
        [SerializeField] private PhotonView photonView;
        
        [HideInInspector] public HpBase hpBase;
        
        public BarManager barManager;
        private bool _isGodMode;
        
        public float CurrentHp { get; private set; }
        public float MaxHp { get => maxHp; }

        private void Start()
        {
            CurrentHp = maxHp;

            if (photonView.IsMine)
            {
                if (barManager != null) barManager.InitAllBar(maxHp);
            }
        }

        public void Adjust(float value)
        {
            CurrentHp += value;
            UpdateBar();
            if (!_isGodMode && CurrentHp <= 0)
                Death();
        }

        private void UpdateBar()
        {
            if (updateBar)
                barManager.EnterAllValue(CurrentHp);
        }

        public void SetGodMode(bool value)
        {
            _isGodMode = value;
            //barManager.SetActiveAllBars(!value);
        }

        public void SetHp(float value)
        {
            CurrentHp = value > maxHp ? maxHp : value;
            UpdateBar();
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
