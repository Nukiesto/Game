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
        Debug.Log(nameTranslations);
        Debug.Log(descriptionTranslations);
    }
    public void InitItem()
    {        
        Item = new Data
        {
            type = ItemType.block,
            sprite = tile.sprite,
            description = descriptionTranslations,
            name = nameTranslations,
            block = this,
            maxCount = 64
        };
        Debug.Log(tile.sprite);
    }
}