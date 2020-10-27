using UnityEngine;

public static class ItemManager
{
    public static Item CreateItem(Vector3 pos, ItemData.Data data)
    {
        var item = PoolManager.GetObject("Item", pos, Quaternion.identity).GetComponent<Item>();

        item.data = data;
        item.InitSprite();

        return item;
    }        
}
