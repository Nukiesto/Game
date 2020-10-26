using SimpleLocalizator;
using System;
using UnityEngine;

public enum ItemType
{ 
    Tool,
    Block,
}

[CreateAssetMenu(menuName = "Items/Data", fileName = "ItemData")]
public class ItemData : ScriptableObject
{
    public Data data;

    [Serializable]
    public class Data
    {
        [Header("Основные параметры")]
        public ItemType type;
        public int maxCount;
        public bool showInSandboxPanel = true;
        
        [Header("Наименование")]
        public string Name;
        public TranslateString name;
        public TranslateString description;
        public BlockData block;

        [Header("Ресурсы")]
        public Sprite sprite;
    }
}

