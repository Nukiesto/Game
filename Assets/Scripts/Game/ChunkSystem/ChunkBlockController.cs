using System.Collections.Generic;
using UnityEngine;

public class ChunkBlockController : MonoBehaviour
{
    private ChunkUnit _chunkUnit;
    private List<BlockUnit> _blocks;
    
    private void Awake()
    {
        _blocks = new List<BlockUnit>();
    }

    public void SetChunk(ChunkUnit chunkUnit)
    {
        _chunkUnit = chunkUnit;
    }
    public BlockUnit AddUnit(BlockData data, Vector2Int posLocal, BlockLayer layer)
    {
        if (data != null)
        {
            var block = new BlockUnit(data, posLocal, layer);
            
            _blocks.Add(block);
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
        for (var i = 0; i < _blocks.Count; i++)
        {
            var block = _blocks[i];
            if (block.PosChunk == posBlock && block.layer == layer)
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

    public void DeleteBlock(BlockUnit block)
    {
        //Debug.Log("Unit Removed: " + block.data.name);
        _blocks.Remove(block);
    }
    public void DeleteBlock(Vector2Int posBlock, BlockLayer layer)
    {
        DeleteBlock(GetBlock(posBlock, layer));
    }
    public void DeleteBlock(int x, int y, BlockLayer layer)
    {
        DeleteBlock(GetBlock(x, y, layer));
    }

    public void Clear()
    {
        _blocks.Clear();
    }
}
public class BlockUnit
{
    public BaseBlockMemory Memory;
    public BaseBlockScript Script;
    public Vector2Int PosChunk;//Расположение относительно чанка
    public BlockData data;
    public BlockLayer layer;

    public BlockUnit(BlockData data, Vector2Int posChunk, BlockLayer layer)
    {
        Memory = data.memory;
        Script = data.script;
        PosChunk = posChunk;
        this.data = data;
        this.layer = layer;
    }

    public ItemData.Data GetItem()
    {
        //Debug.Log(data.Item.sprite);
        return data.Item;
    }
}
