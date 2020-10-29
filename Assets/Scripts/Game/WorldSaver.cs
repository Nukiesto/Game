using Singleton;
using UnityEngine;

public class WorldSaver : MonoBehaviour
{
    public void SaveWorld()
    {
        var toolbox = Toolbox.Instance;
        var entities = toolbox.mEntityManager.GetEntitiesData();
        var items = toolbox.mItemManager.GetItemsData();
    }

    public void LoadWorld()
    {
        
    }
    public struct EntityUnitData
    {
        public float X;
        public float Y;
        public EntityType EntityType;
    }
    public struct ItemUnitData
    {
        public float X;
        public float Y;
        public string Name;
    }
}