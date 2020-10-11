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
    public BlockUnit AddUnit(BlockData data, Vector2Int posLocal, BlockLayer layer)
    {
        //Debug.Log("Data: " + data + " ;posLocal: " + posLocal);
        if (data != null)
        {
            BlockUnit block = new BlockUnit(data, posLocal, layer);

            //Debug.Log("BlockUnit: " + block);
            //Debug.Log(blocks);
            blocks.Add(block);
            return block;
        }
        return null;
    }
    public BlockUnit AddUnit(BlockData data, Vector3Int posLocal, BlockLayer layer)
    {
        return AddUnit(data, new Vector2Int(posLocal.x, posLocal.y), layer);
    }
    public BlockUnit AddUnit(BlockData data, int x, int y, BlockLayer layer)
    {
        return AddUnit(data, new Vector2Int(x, y), layer);
    }  

    public BlockUnit GetBlock(Vector2Int posBlock, BlockLayer layer)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            BlockUnit block = blocks[i];
            if (block.posChunk == posBlock && block.layer == layer)
            {
                return block;
            }
        }
        return null;
    }
    public BlockUnit GetBlock(int x, int y, BlockLayer layer)
    {
        return GetBlock(new Vector2Int(x, y), layer);
    }

    public void DeleteUnit(BlockUnit block)
    {
        //Debug.Log("Unit Removed: " + block.data.name);
        blocks.Remove(block);
    }
    public void DeleteBlock(Vector2Int posBlock, BlockLayer layer)
    {
        DeleteUnit(GetBlock(posBlock, layer));
    }
    public void DeleteBlock(int x, int y, BlockLayer layer)
    {
        DeleteUnit(GetBlock(x, y, layer));
    }

    public void Clear()
    {
        blocks.Clear();
    }
}
public class BlockUnit
{
    public BaseBlockMemory memory;
    public BaseBlockScript script;
    public Vector2Int posChunk;//Расположение относительно чанка
    public BlockData data;
    public BlockLayer layer;

    public BlockUnit(BlockData data, Vector2Int posChunk, BlockLayer layer)
    {
        memory = data.memory;
        script = data.script;
        this.posChunk = posChunk;
        this.data = data;
        this.layer = layer;
    }

    public ItemData.Data GetItem()
    {
        //Debug.Log(data.Item.sprite);
        return data.Item;
    }
}
