using UnityEngine;

[CreateAssetMenu(menuName = "Block/BlocksData", fileName = "BlockData")]
public class BlockData : ScriptableObject
{
    [Header("Основные параметры")]
    public Sprite sprite;
    public BaseBlockScript script;
    public BaseBlockMemory memory;
    public bool isSolid;
    public bool HasScript() => script != null;
    public bool HasMemory() => memory != null;
}