﻿using System.Collections.Generic;
 using UnityEngine;

 [System.Serializable]
[CreateAssetMenu(menuName = "BlockMemories/Chest", fileName = nameof(ChestMemory))]
public class ChestMemory : BaseBlockMemory
{
    public class ChestMemoryUnit : MemoryUnit
    {
        public List<ChestSlotUnitSave> Items;
    }

    public override MemoryUnit memoryUnit { get; set; }
    public List<ChestSlotUnit> items = new List<ChestSlotUnit>();
    public void WriteItems(List<ItemUnit> items)
    {
        this.items.Clear();
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var unit = new ChestSlotUnit()
            {
                Data = item.data, 
                Count = item.Count
            };
            this.items.Add(unit);
        }
    }
    public void Debugging()
    {
        for (var i = 0; i < items.Count; i++)
        {
            if (items[i].Data != null)
            {
                Debug.Log("Data: " + items[i].Data.Name + " ;Count: " + items[i].Count);
            }
        }
    }

    public override void SavingMemoryUnit()
    {
        var list = new List<ChestSlotUnitSave>();
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var itemSave = new ChestSlotUnitSave()
            {
                NameItem = item.Data?.Name ?? "",
                Count = item.Count
            };
            list.Add(itemSave);    
        }
        memoryUnit = new ChestMemoryUnit(){Items = list};
    }
    public override void SetMemoryUnit(MemoryUnit unit, ChunkUnit chunkUnit)
    {
        var im = unit as ChestMemoryUnit;
        var dataBase = chunkUnit.chunkManager.dataBase;
        items = new List<ChestSlotUnit>();
        var itemsLoaded = im.Items;
        Debug.Log(itemsLoaded.Count);
        for (var i = 0; i < itemsLoaded.Count; i++)
        {
            var item = itemsLoaded[i];
            Debug.Log(item != null ? item.NameItem : "");
            var slotUnit = new ChestSlotUnit()
            {
                Data = DataBase.Instance.GetItem(item != null ? item.NameItem : ""),
                Count = item != null ? item.Count : 0
            };
            items.Add(slotUnit);
        }
    }

    
}
public class ChestSlotUnitSave
{
 public string NameItem;
 public int Count;
}
public class ChestSlotUnit
{
 public ItemData.Data Data;
 public int Count;
}

