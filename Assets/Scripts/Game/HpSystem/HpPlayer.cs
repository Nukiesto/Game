using Game.Player;
using UnityEngine;

namespace Game.HpSystem
{
    public class HpPlayer : HpBase
    {
        [Header("Player")] 
        [SerializeField] private PlayerController playerController;
    }
}