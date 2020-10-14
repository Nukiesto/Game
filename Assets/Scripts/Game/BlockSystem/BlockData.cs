﻿using SimpleLocalizator;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ItemData;

[CreateAssetMenu(menuName = "Block/BlocksData", fileName = "BlockData")]
public class BlockData : ScriptableObject
{
    [Header("Основные параметры")]
    public bool isSolid = true;
    public bool toCreateItem = true;
    public bool isBreackable = true;
    public string nameBlock;
    [HideInInspector] public int id { get; private set; }
    [Header("")]
    [Range(0, 1000)]
    public int hp;

    [Header("Компоненты")]
    public BaseBlockScript script;
    public BaseBlockMemory memory;

    [Header("Ресурсы")]
    public Tile tile;

    [Header("Предмет")]
    public TranslateString nameTranslations;
    public TranslateString descriptionTranslations;
    [HideInInspector] public Data Item { get; private set; }

    private void OnEnable()
    { 
        InitItem();
    }
    public void OnValidate()
    {
        InitItem();
        //Debug.Log(nameTranslations);
        //Debug.Log(descriptionTranslations);
    }
    public void InitItem()
    {
        //Debug.Log(Item);
        if (Item == null)
            Item = new Data();

        Item.type = ItemType.block;
        Item.sprite = tile?.sprite;
        Item.description = descriptionTranslations;
        Item.name = nameTranslations;
        Item.block = this;
        Item.maxCount = 64;
        
        //Debug.Log(tile.sprite);
    }
}