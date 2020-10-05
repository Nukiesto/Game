using UnityEngine;

public static class ItemManager 
{
    public static ItemUnit CreateItem(Vector3 pos, ItemData.Data data)
    {
        ItemUnit item = PoolManager.GetObject("Item", pos, Quaternion.identity).GetComponent<ItemUnit>();

        item.data = data;
        item.InitSprite();

        return item;
    }        
}
