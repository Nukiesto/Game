using System.Collections.Generic;
using Game.ChunkSystem;
using SavingSystem;
using Singleton;
using UnityEngine;

namespace Game.ItemSystem
{
    public class ItemManager : MonoBehaviour
    { 
        private List<Item> _items = new List<Item>();

        private void AddItem(Item item)
        {
            _items.Add(item);
        }
        private void Start()
        {
            var worldSaver = Toolbox.Instance.mWorldSaver;
            worldSaver.OnSaveEvent += OnSave;
        }

        private void OnSave(WorldSavingSystem.WorldSaving worldSaving)
        {
            //Debug.Log("Invoked");
            worldSaving.WorldDataUnit.AddItems(GetItemsData());
        }

        private List<WorldSaver.ItemUnitData> GetItemsData()
        {
            var list = new List<WorldSaver.ItemUnitData>();
            var chunkManager = ChunkManager.Instance;
            foreach (var item in _items)
            {
                if (item != null)
                {
                    var pos = item.gameObject.transform.position;
                    var chunk = chunkManager.GetChunk(pos);
                    if (chunk != null)
                    {
                        var posChunk = chunk.posChunk;
                        var data = new WorldSaver.ItemUnitData()
                        {
                            Name = item.data.Name,
                            X = pos.x,
                            Y = pos.y,
                            ChunkX = posChunk.x,
                            ChunkY = posChunk.y
                        };
                        list.Add(data); 
                    }
                }
            }

            return list;
        }
        public void RemoveItem(Item entity)
        {
            _items.Remove(entity);
        }
        public Item CreateItem(Vector3 pos, ItemData.Data data)
        {
            var item = PoolManager.GetObject("Item", pos, Quaternion.identity).GetComponent<Item>();

            item.itemManager = this;
            item.data = data;
            item.InitSprite();
        
            AddItem(item);
        
            return item;
        }        
    }
}
