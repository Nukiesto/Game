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

    [HideInInspector] public ItemData.Data Item;

    private void Awake()
    {
        InitItem();
    }
    public void InitItem()
    {
        Item = new ItemData.Data
        {
            sprite = tile.sprite         
        };
        Debug.Log(tile.sprite);
    }
    public void TryInitItem()
    {
        if (Item == null)
        {
            InitItem();
        }
    }
}