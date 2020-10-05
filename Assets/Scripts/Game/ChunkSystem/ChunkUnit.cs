using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TilemapCollider2D))]
[RequireComponent(typeof(TilemapRenderer))]
[RequireComponent(typeof(ChunkBlockController))]
public class ChunkUnit : MonoBehaviour
{
    //Components
    private Tilemap tilemap;
    private TilemapCollider2D tilemapCollider;
    private ChunkBlockController controller;

    //Other
    private static readonly int chunkSize = GameConstants.chunkSize;
    [HideInInspector]public ChunkManager chunkManager;

    //Data
    private Vector3 posObj;
    private Vector3Int posObjGlobal;
    private void Start()
    {
        InitComponents();

        posObj = transform.position;
        posObjGlobal = new Vector3Int(Mathf.FloorToInt(posObj.x), Mathf.FloorToInt(posObj.y), 0);
        controller.chunk = this;

        BuildChunk();       
    }
    //private void OnBecameInvisible()
    //{
    //    //Debug.Log(this);
    //    _chunkManager.BecameInvisible(this);
    //    SetActive(false);
    //}
    private void BuildChunk()
    {
        Vector3Int pos = Vector3Int.zero;
        Vector2Int posGlobal = Vector2Int.zero;

        WorldGenerator generator = chunkManager.generator;
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                pos.x = i;
                pos.y = j;

                posGlobal.x = posObjGlobal.x + i;
                posGlobal.y = posObjGlobal.y + j;


                //Debug.Log(posGlobal);
                BlockData block = generator.GetBlock(posGlobal);
                //block?.InitItem();
                SetBlock(pos, block, true);      
            }
        }
    }
    private void DebugClick(Vector3 pos) {
        Vector3Int toSetBlockPos = tilemap.WorldToCell(pos);

        Debug.Log("ClickPos: " + tilemap.WorldToCell(pos) + " ;In Bounds: " + InBounds(toSetBlockPos));
    }
    public void SetBlock(Vector3Int pos, BlockData data, bool checkCollisions)
    {
        if (InBounds(pos) && !(!checkCollisions && !OnBlock(pos)))
        {
            if (data != null)
            {
                tilemap.SetTile(pos, data.tile);
            }            
            controller.AddUnit(data, pos.x, pos.y);
            
            //Debug.Log(pos);
        }
    }
    public void SetBlock(Vector3 pos, BlockData data, bool checkCollisions)
    {
        Vector3Int posInt = tilemap.WorldToCell(pos);

        SetBlock(posInt, data, checkCollisions);
    }
    public void DeleteBlock(Vector3 pos, bool createItem)
    {
        Vector3Int blockPos = tilemap.WorldToCell(pos);
        if (InBounds(blockPos) && (tilemap.GetTile(blockPos) != null))
        {
            BlockUnit blockUnit = controller.GetBlock(blockPos.x, blockPos.y);
            if (createItem)
            {
                Vector3 posCreateItem = new Vector3
                {
                    x = Mathf.Floor(pos.x) + 0.5f,
                    y = Mathf.Floor(pos.y) + 0.5f
                };
                //ItemUnit item = 
                ItemManager.CreateItem(posCreateItem, blockUnit.GetItem());
                //item.sprite.sprite = blockUnit.data.tile.sprite;
            }
            tilemapCollider.ProcessTilemapChanges();
            controller.DeleteUnit(blockUnit);

            tilemap.SetTile(blockPos, null);                                          
        }           
    }
    private bool InBounds(Vector3Int pos)
    {
        //Debug.Log("pos.x: " + pos.x + " ;pos.y: " + pos.y + "chunkSize: " + chunkSize);
        return pos.x >= 0 && pos.x < chunkSize && pos.y >= 0 && pos.y < chunkSize;
    }
    public bool OnBlock(Vector3 pos)
    {
        return OnBlock(tilemap.WorldToCell(pos));
    }
    public bool OnBlock(Vector3Int pos)
    {
        return (tilemap.HasTile(pos));
    }
    private void InitComponents()
    {
        tilemap = GetComponent<Tilemap>();
        controller = GetComponent<ChunkBlockController>();
        tilemapCollider = GetComponent<TilemapCollider2D>();
    }
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        int size = chunkSize;

        Gizmos.color = Color.red;
        
        Gizmos.DrawLine(pos, pos + new Vector3(size, 0));
        Gizmos.DrawLine(pos + new Vector3(0, size), pos + new Vector3(size, size));
        Gizmos.DrawLine(pos, pos + new Vector3(0, size));
        Gizmos.DrawLine(pos + new Vector3(size, 0), pos + new Vector3(size, size));
    }
#endif
}
