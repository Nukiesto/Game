using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Block/DataBase", fileName = "DataBase")]
public class DataBase : ScriptableObject
{
    private Dictionary<string, BlockData> _blocks = new Dictionary<string, BlockData>();
    private Dictionary<string, ItemData.Data> _items = new Dictionary<string, ItemData.Data>();
    private ItemData.Data[] _itemsArray;
    
    public Dictionary<string, BlockData> Blocks => _blocks;
    public Dictionary<string, ItemData.Data> Items  => _items;
    public ItemData.Data[] ItemsArray => _itemsArray;
    
    [SerializeField] private Data data;
    private void RefreshBlocks()
    {
        _blocks.Clear();
        var blocks = data.blocks;
        data.itemsBlocks.Clear();
        for (var i = 0; i < blocks.Length; i++)
        {
            var block = blocks[i];
            _blocks.Add(block.nameBlock, block);
            data.itemsBlocks.Add(block.Item);
        }
    }
    private void RefreshItems()
    {
        _items.Clear();
        _itemsArray = new ItemData.Data[data.items.Length + data.itemsBlocks.Count];
        var a = 0;
        var items = data.items;
        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];
            _items.Add(item.Name, item);
            _itemsArray[a] = item;
            a++;
        }
        var items1 = data.itemsBlocks;
        for (var i = 0; i < items1.Count; i++)
        {
            var item = items1[i];
            _items.Add(item.Name, item);
            _itemsArray[a] = item;
            a++;
        }
    }

    private void OnEnable()
    {
        Refresh();
    }

    public BlockData GetBlock(string name)
    {  
        return _blocks[name];
    }
    public ItemData.Data GetItem(string name)
    {  
        return _items[name];
    }
    public void Refresh()
    {
        RefreshBlocks();
        RefreshItems();
    }
    [Serializable]
    private class Data
    {
        public BlockData[] blocks;
        public ItemData.Data[] items;
        public List<ItemData.Data> itemsBlocks = new List<ItemData.Data>();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DataBase))]
public class LabelsDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //base.OnInspectorGUI();

        DataBase dataBase = (DataBase)target;
        EditorUtility.SetDirty(target);

        if (GUILayout.Button("Refresh"))
        {
            dataBase.Refresh();
        }
    }
}
#endif
