using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TilemapCollider2D))]
[RequireComponent(typeof(TilemapRenderer))]
public class ChunkUnit : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private BlockData blockInit;
    [SerializeField] private GameObject blockPrefab;

    //Components
    private Tilemap tilemap;
    private PoolManagerLocal pool;
    //Other
    private int chunkSize;
    [HideInInspector]public ChunkManager chunkManager;
    //Data
    private BlockUnit[,] blocks;
    private Vector3 posObj;
    private void Start()
    {
        InitComponents();     
        chunkSize = GameConstants.chunkSize;
        blocks = new BlockUnit[chunkSize, chunkSize];
        posObj = transform.position;
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

        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                pos.x = i;
                pos.y = j;

                if (Random.Range(0, 2) == 1)
                    SetBlock(pos, blockInit, true);              
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
            tilemap.SetTile(pos, data.tile);
            Vector3 posSet = posObj + new Vector3(pos.x + 0.5f, pos.y + 0.5f);
            GameObject block = pool.GetObject("Block", posSet, Quaternion.identity);
            BlockUnit blockUnit = block.GetComponent<BlockUnit>();
            blockUnit.SetData(data);
            blocks[pos.x, pos.y] = blockUnit;
            block.transform.position = posObj + new Vector3(pos.x + 0.5f, pos.y + 0.5f);
            block.transform.parent = transform;
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
            BlockUnit blockUnit = blocks[blockPos.x, blockPos.y];
            if (createItem)
            {
                Vector3 posCreateItem = new Vector3
                {
                    x = Mathf.Floor(pos.x),
                    y = Mathf.Floor(pos.y)
                };
                ItemManager.CreateItem(posCreateItem, blockUnit.data.item);
            }
            blockUnit.gameObject.GetComponent<PoolObject>().ReturnToPool();
            blocks[blockPos.x, blockPos.y] = null;

            tilemap.SetTile(blockPos, null);                                          
        }           
    }
    private bool InBounds(Vector3Int pos)
    {
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
        pool = GetComponent<PoolManagerLocal>();
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
