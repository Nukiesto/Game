using SimpleLocalizator;
using System;
using UnityEngine;

public enum ItemType
{ 
    tool,
    block,
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

        [Header("Наименование")]
        public TranslateString name;
        public TranslateString description;

        [Header("Ресурсы")]
        public Sprite sprite;
    }
}

