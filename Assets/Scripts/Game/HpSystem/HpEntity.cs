using Game.Bot;
using UnityEngine;

namespace Game.HpSystem
{
    public class HpEntity : HpBase
    {
        [Header("Entity")]
        [SerializeField] private BotController botController;
    }
}