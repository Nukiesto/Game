using UnityEngine;

[CreateAssetMenu(menuName = "BlockMemories/Null", fileName = nameof(NullMemory))]
public class NullMemory : BaseBlockMemory
{
    public override MemoryUnit memoryUnit { get; set; }
}