using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public abstract class BaseBlockScript : ScriptableObject
{
    protected Vector3Int Pos;
    public BlockUnit blockUnit;

    public delegate IEnumerator coroutineToInit();

    public coroutineToInit CoroutineToInit;
    public virtual void UpdateBlock() {}
    
    public virtual void StartScript() {}

    protected void Init()
    {
        Pos = new Vector3Int(blockUnit.PosChunk.x, blockUnit.PosChunk.y, 0);
    }
    protected Tile GetTile()
    {
        return (Tile)blockUnit.Tilemap.GetTile(Pos);
    }
    protected void SetTile(Tile tile)
    {
        blockUnit.Tilemap.SetTile(Pos, tile);
    }
}
