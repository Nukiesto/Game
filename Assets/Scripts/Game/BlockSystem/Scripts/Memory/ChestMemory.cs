using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Game.ItemSystem;
using Newtonsoft.Json;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName = "BlockMemories/Chest", fileName = nameof(ChestMemory))]
public class ChestMemory : BaseBlockMemory
{
    public List<ChestSlotUnitSave> itemsToSave;
    [JsonIgnore]
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

    public override BaseBlockMemory GetMemoryUnit()
    {
        itemsToSave = new List<ChestSlotUnitSave>();
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var itemSave = new ChestSlotUnitSave()
            {
                NameItem = item?.Data?.Name ?? "",
                Count = item?.Count ?? 0
            };
            itemsToSave.Add(itemSave);
        }

        //Debug.Log("ChestItemsCount: " + items.Count);
        //Debug.Log("ChestSaveItemsCount: " + itemsToSave.Count);
        //Debug.Log("ChestSaveItemsCount: " + itemsToSave.Count);
        //m.ItemConverterParameters = new object[] { list };
        return this;
    }

    public override void SetMemoryUnit(string memory)
    {
        memory = memory.Replace("]", "");
        memory = memory.Replace("[", "");
        
        var rg = new Regex(@"^{.itemsToSave.:(.*?)}$");
        
        var result = rg.Match(memory).Groups[1].Value;
        
        var words = result.Split(new char[] { ','});
        //var wordSum = "";
        var wordsM = new string[32];
        var j = 0;
        var m = 0;
        var wordsMSum = "";
        var itemsLoaded = new List<ChestSlotUnitSave>();
        for (var i = 0; i < words.Length - 2; i++)
        {
            wordsM[j] += (m==1 ? ",": "") + words[i];
           //Debug.Log("1i: " + j + "; " + words[j]);
            m++;
            if (m>1)
            {
                m = 0;
                //Debug.Log("2i: " + j + "; " + wordsM[j]);
                //wordsMSum += wordsM[j];
                itemsLoaded.Add(JsonConvert.DeserializeObject<ChestSlotUnitSave>(wordsM[j]));
                j++;
            }
        }
        //Debug.Log(memory);
        //Debug.Log("Result: " + result);
        //Debug.Log(words.Length + "; " + wordsMSum);
        //Debug.Log("MemLoaded: " + memory);
        //var itemsLoaded = JsonConvert.DeserializeObject<List<ChestSlotUnitSave>>(wordsMSum);
        
        //Debug.Log(string.Join(", ", itemsLoaded.ToArray()));
        if (itemsLoaded != null)
        {
            var dataBase = DataBase.Instance;
            items.Clear();
            //Debug.Log(itemsLoaded.Count);
            for (var i = 0; i < itemsLoaded.Count; i++)
            {
                var item = itemsLoaded[i];
                //Debug.Log(item != null ? item.NameItem : "");
                var slotUnit = new ChestSlotUnit()
                {
                    Data = dataBase.GetItem(item != null ? item.NameItem : ""),
                    Count = item != null ? item.Count : 0
                };
                items.Add(slotUnit);
            }
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
 public string NameItem;
 public ItemData.Data Data;
 public int Count;
}

