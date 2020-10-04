using UnityEngine;
using UnityEngine.Tilemaps;

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
    public Tile tile;

    [Header("Предмет")]
    public ItemData.Data item;    
}