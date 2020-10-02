using UnityEngine;

public class BlockUnitUpdate : MonoBehaviour
{
    //Delegats
    private delegate void UpdateBlock(BlockMemoryPackToUpdaterBase packData);
    private UpdateBlock DelUpdate;

    //Components
    private BlockUnitMemory memory;

    //DataPacks
    private BlockMemoryPackToUpdaterBase packData;

    public void Update()
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
    public void SetMemoryPack(BlockMemoryPackToUpdaterBase packData)
    {
        this.packData = packData;
    }
}

public abstract class BlockMemoryPackToUpdaterBase
{

}