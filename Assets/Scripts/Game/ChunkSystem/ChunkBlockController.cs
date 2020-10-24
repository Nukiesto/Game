using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    #region AddUnit

    public BlockUnit AddUnit(BlockData data, Vector2Int posLocal, BlockLayer layer, Tilemap tilemap, bool toStartCor = false)
    {
        if (data != null)
        {
            var block = new BlockUnit(data, posLocal, layer, tilemap, _chunkUnit);
            
            _blocks.Add(block);
            if (block.Script != null)
            {
                block.Script.StartScript();
                if (toStartCor)
                {
                    var rot = block.Script.CoroutineToInit();
                    if (rot != null)
                    {
                        StartCoroutine(rot);
                    } 
                }
            }
            return block;
        }
        return null;
    }
    public BlockUnit AddUnit(BlockData data, Vector3Int posLocal, BlockLayer layer, Tilemap tilemap, bool toStartCor = false)
    {
        return AddUnit(data, new Vector2Int(posLocal.x, posLocal.y), layer, tilemap, toStartCor);
    }
    public BlockUnit AddUnit(BlockData data, int x, int y, BlockLayer layer, Tilemap tilemap, bool toStartCor = false)
    {
        return AddUnit(data, new Vector2Int(x, y), layer, tilemap, toStartCor);
    } 

    #endregion
    #region GetBlock

    public BlockUnit GetBlock(Vector2Int posBlock, BlockLayer layer)
    {
        for (var i = 0; i < _blocks.Count; i++)
        {
            var block = _blocks[i];
            if (block.PosChunk == posBlock && block.Layer == layer)
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

    #endregion
    #region DeleteBlock

    public void DeleteBlock(Vector2Int posBlock, BlockLayer layer)
    {
        DeleteUnit(GetBlock(posBlock, layer));
    }
    public void DeleteBlock(int x, int y, BlockLayer layer)
    {
        DeleteUnit(GetBlock(x, y, layer));
    }

    #endregion
    #region Misc

    public void DeleteUnit(BlockUnit block)
    {
        //Debug.Log("Unit Removed: " + block.data.name);
        var rot = block.Script.CoroutineToInit();
        if (rot != null)
        {
            StopCoroutine(rot);
        } 
        _blocks.Remove(block);
    }
    public void Clear()
    {
        _blocks.Clear();
    }

    #endregion
}
public class BlockUnit
{
    public BaseBlockMemory Memory;
    public BaseBlockScript Script;
    public Vector2Int PosChunk;//Расположение относительно чанка
    public BlockData Data;
    public BlockLayer Layer;
    public Tilemap Tilemap;
    public ChunkUnit ChunkUnit;
    
    public BlockUnit(BlockData data, Vector2Int posChunk, BlockLayer layer, Tilemap tilemap, ChunkUnit chunkUnit)
    {
        Memory = data.memory;
        Script = data.script;
        PosChunk = posChunk;
        Data = data;
        Layer = layer;
        Tilemap = tilemap;
        ChunkUnit = chunkUnit;
        if (Script != null)
        {
            Script.blockUnit = this;
        }

    }

    public ItemData.Data GetItem()
    {
        return Data.Item;
    }
}
