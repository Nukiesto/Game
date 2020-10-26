using UnityEngine;

[System.Serializable]
public class BaseBlockMemory : ScriptableObject
{
    public virtual MemoryUnit memoryUnit { get; set; }
    public class MemoryUnit
    {
        
    }

    public virtual void SavingMemoryUnit(){}

    public virtual void SetMemoryUnit(MemoryUnit unit, ChunkUnit chunkUnit)
    {
        
    }
}
