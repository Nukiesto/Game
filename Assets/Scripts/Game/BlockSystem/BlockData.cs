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
    public bool toPlaceBack = true;
    public bool isInteractable = false;
    public bool mustHaveDownerBlock = false;
    public bool showInSandboxPanel = true;
    
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
    public Tile[] tileVariables;

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
    }
    public void InitItem()
    {
        Item = new Data()
        {
            showInSandboxPanel = showInSandboxPanel,
            type = ItemType.Block,
            sprite = tile?.sprite,
            description = descriptionTranslations,
            Name = nameBlock,
            name = nameTranslations,
            block = this,
            maxCount = 64
        };
    }
}