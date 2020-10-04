using System.Collections.Generic;
using UnityEngine;

public class ChunkBlockController : MonoBehaviour
{
    private List<BlockUnit> blocks;
    [HideInInspector] public ChunkUnit chunk;

    private void Awake()
    {
        blocks = new List<BlockUnit>();
    }
    public BlockUnit AddUnit(BlockData data, Vector2Int posChunk)
    {
        //Debug.Log("Data: " + data + " ;posChunk: " + posChunk);
        if (data != null)
        {
            BlockUnit block = new BlockUnit(data, posChunk);

            //Debug.Log("BlockUnit: " + block);
            //Debug.Log(blocks);
            blocks.Add(block);
            return block;
        }
        return null;
    }
    public BlockUnit AddUnit(BlockData data, int x, int y)
    {
        return AddUnit(data, new Vector2Int(x, y));
    }  

    public BlockUnit GetBlock(Vector2Int posBlock)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            BlockUnit block = blocks[i];
            if (block.posChunk == posBlock)
            {
                return block;
            }
        }
        return null;
    }
    public BlockUnit GetBlock(int x, int y)
    {
        return GetBlock(new Vector2Int(x, y));
    }

    public void DeleteUnit(BlockUnit block)
    {
        //Debug.Log("Unit Removed: " + block.data.name);
        blocks.Remove(block);
    }
    public void DeleteBlock(Vector2Int posBlock)
    {
        DeleteUnit(GetBlock(posBlock));
    }
    public void DeleteBlock(int x, int y)
    {
        DeleteUnit(GetBlock(x, y));
    }
}
public class BlockUnit
{
    public BaseBlockMemory memory;
    public BaseBlockScript script;
    public Vector2Int posChunk;//Расположение относительно чанка
    public BlockData data;

    public BlockUnit(BlockData data, Vector2Int posChunk)
    {
        memory = data.memory;
        script = data.script;
        this.posChunk = posChunk;
        this.data = data;
    }

    public ItemData.Data GetItem()
    {
        return data.item;
    }
}
