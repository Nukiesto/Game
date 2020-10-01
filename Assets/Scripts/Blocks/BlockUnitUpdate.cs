using UnityEngine;

public class BlockUnitUpdate : MonoBehaviour
{
    //Delegats
    private delegate void UpdateBlock(BlockMemoryPackUpdaterBase packData);
    private UpdateBlock DelUpdate;
    private BlockUnitMemory memory;
    private BlockMemoryPackUpdaterBase packData;

    private void Update()
    {
        DelUpdate?.Invoke(packData);
    }
    public void SetScript(BlockData data)
    {
        DelUpdate = data.script.UpdateBlock;
    }
    public void Remove()
    {
        DelUpdate = null;
    }

    public void SetMemory(BlockUnitMemory memory)
    {
        this.memory = memory;
    }
    public void SetMemoryPack(BlockMemoryPackUpdaterBase packData)
    {
        this.packData = packData;
    }
}

public abstract class BlockMemoryPackUpdaterBase
{

}
