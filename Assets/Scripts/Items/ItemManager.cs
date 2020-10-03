using UnityEngine;

public static class ItemManager 
{
    public static void CreateItem(Vector3 pos, ItemData.Data data)
    {
        ItemUnit item = PoolManager.GetObject("Item", pos, Quaternion.identity).GetComponent<ItemUnit>();

        item.data = data;
    }        
}
