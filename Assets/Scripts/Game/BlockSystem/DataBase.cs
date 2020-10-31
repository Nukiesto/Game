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
    private List<ItemData.Data> _itemsList;
    
    public Dictionary<string, BlockData> Blocks => _blocks;
    public Dictionary<string, ItemData.Data> Items  => _items;
    public List<ItemData.Data> ItemsList => _itemsList;
    
    private static DataBase _instance;
    public static DataBase Instance {
        get {
            InitInstance();
            return _instance;
        }
    }

    private static void InitInstance() {
        if (_instance==null) {
            _instance = (DataBase)Resources.Load ("DataBase");
            //if (_instance == null) {
            //    _instance = CreateInstance<DataBase>();
            //    #if UNITY_EDITOR
			//		Extensions.WriteAsset(_instance);
			//		CreateDefaultLabels();
            //   #endif
            //   //Debug.Log ("DataBase: loaded instance from resources is null, created instance");
            //}
        }
    }
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
        _itemsList = new List<ItemData.Data>();
        var items = data.items;
        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];
            if (item != null)
            {
                _items.Add(item.Name, item);
                _itemsList.Add(item);
            }
        }
        var items1 = data.itemsBlocks;
        for (var i = 0; i < items1.Count; i++)
        {
            var item = items1[i];
            if (item != null)
            {
                _items.Add(item.Name, item);
                _itemsList.Add(item);
            }
        }
    }

    private void OnEnable()
    {
        Refresh();
    }

    public BlockData GetBlock(string name)
    {
        if (name == "")
            return null;
        if (_blocks.TryGetValue(name, out var block))
        {
            return block;
        }
        return null;
    }
    public ItemData.Data GetItem(string name)
    {
        if (name == "")
            return null;
        if (_items.TryGetValue(name, out var item))
        {
            return item;
        }
        return null;
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
