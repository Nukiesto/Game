﻿using UnityEngine;

[CreateAssetMenu(menuName = "Block/BlocksData", fileName = "BlockData")]
public class BlockData : ScriptableObject
{
    [Header("Основные параметры")]
    public bool isSolid = true;
    public bool toCreateItem = true;
    public bool isBreackable = true;

    [Header("Компоненты")]
    public BaseBlockScript script;
    public BaseBlockMemory memory;

    [Header("Ресурсы")]
    public Texture2D texture;

    [Header("Предмет")]
    public ItemData.Data data;

    private void Awake()
    {
        data.texture = texture;   
    }

    public bool HasScript() => script != null;
    public bool HasMemory() => memory != null;
}