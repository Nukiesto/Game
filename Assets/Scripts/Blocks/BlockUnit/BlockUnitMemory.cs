using UnityEngine;

public class BlockUnitMemory : MonoBehaviour
{
    private BaseBlockMemory memory;
    private BlockUnit block;

    public void SetMemory(BlockData data)
    {
        memory = data.memory;
    }
    public void Remove()
    {
        memory = null;        
    }

    public void SetBlockUnit(BlockUnit _block)
    {
        block = _block;
    }
}

