using System;
using System.Collections.Generic;
using SimpleLocalizator;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ItemData;

[CreateAssetMenu(menuName = "Block/BlocksData", fileName = "BlockData")]
public class BlockData : ScriptableObject
{
    [Header("Основные параметры")]
    [Tooltip("Твёрдый?")]public bool isSolid = true;
    [Tooltip("Создавать предмет при уничтожении?")]public bool toCreateItem = true;
    [Tooltip("Ломаемый?")]public bool isBreackable = true;
    [Tooltip("Размещаем на фоне?")]public bool toPlaceBack = true;
    [Tooltip("Функциональный?")]public bool isInteractable = false;
    [Tooltip("Должен иметь опору?")]public bool mustHaveDownerBlock = false;
    [Tooltip("Показывать в панели песочницы")]public bool showInSandboxPanel = true;
    [Tooltip("Препятствие света?")]public bool isLightObstacle = true;
    [Tooltip("Источник света?")]public bool isLightSource = false;
    [Tooltip("Включать свет в старте?")]public bool isLightOnStart = true;
    
    public string nameBlock;
    [HideInInspector] public int Id { get; private set; }
    [Header("")]
    [Range(0, 1000)]
    public int hp;
    
    [Header("Компоненты")]
    public BaseBlockScript script;
    public BaseBlockMemory memory;

    [Header("Основной тайл")]
    public Tile tile;
    public Tile[] tileVariables;
    [Header("Световой тайл")]
    public Tile tileLightSource;
    public Tile[] tileLightSourceVariables;
    [Header("Тайл с анимацией")]
    public AnimatedTile tileAnimation;
    public AnimatedTile[] tileAnimationVariables;

    [Header("Предмет")] 
    public bool itemDropOverride;
    public ItemData itemDrop;

    public bool itemCraftable;
    public ItemCraftUnit itemCraft;
    public TranslateString nameTranslations;
    public TranslateString descriptionTranslations;
    [HideInInspector] public Data Item { get; private set; }

    private void OnEnable()
    { 
        InitItem();
        itemCraft.Init();
    }
    public void OnValidate()
    {
        InitItem();
        itemCraft.Init();
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
            maxCount = 64,
            itemCraft = itemCraft,
            craftable = itemCraftable
        };
    }
}
[Serializable]
public struct ItemCraft
{
    public List<ItemCraftUnit> items;
}
[Serializable]
public struct ItemCraftUnit
{
    public BlockData blockData;
    public ItemData itemData;
    
    [HideInInspector]public Data item;
    public int count;

    public void Init()
    {
        if (blockData != null)
        {
            item = blockData.Item;
            //Debug.Log(item.Name);
        }

        if (itemData != null)
        {
            item = itemData.data;
            //Debug.Log(item.Name);
        }
        
        //Debug.Log(item);
        //
    }
}