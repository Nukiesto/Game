using Game.ItemSystem;
using UnityEngine;

namespace Game.Game
{
    public static class GameCond
    {
        public static Camera MainCamera;
        public static Inventory Inventory;

        public static Camera GetMainCamera()
        {
            return IsGame ? MainCamera : null;
        }
        public static Inventory GetInventory()
        {
            //Debug.Log(IsGame);
            return IsGame ? Inventory : null;
        }
        public static bool IsGame;
    }
}
