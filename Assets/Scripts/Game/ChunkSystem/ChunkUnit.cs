using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum BlockLayer
{
    front = 0,
    back = 1
}
[RequireComponent(typeof(ChunkBlockController))]
public class ChunkUnit : MonoBehaviour
{
    //Tilemaps
    [Header("Tilemaps")]
    [SerializeField] private Tilemap tilemap_BackWorld;
    [SerializeField] private Tilemap tilemap_FrontWorld;

    private Dictionary<BlockLayer, Tilemap> dic_tile;
    //Components
    private ChunkBlockController controller;

    //Other
    private static readonly int chunkSize = GameConstants.chunkSize;
    [HideInInspector] public ChunkManager chunkManager;

    //Position Cash
    private Vector3 posObj;
    private Vector3Int posObjGlobal;

    private void Start()
    {
        InitMethods();

        BuildChunk();

        //BuildFillChunk();
    }

    #region InitMethods
    private void InitMethods()
    {
        InitComponents();
        InitTilemaps();
        InitPosition();
    }
    private void InitComponents()
    {
        controller = GetComponent<ChunkBlockController>();
        controller.chunk = this;

    }
    private void InitTilemaps()
    {
        dic_tile = new Dictionary<BlockLayer, Tilemap>
        {
            { BlockLayer.front, tilemap_FrontWorld },
            { BlockLayer.back, tilemap_BackWorld }
        };
    }
    private void InitPosition()
    {
        posObj = transform.position;
        posObjGlobal = new Vector3Int(Mathf.FloorToInt(posObj.x), Mathf.FloorToInt(posObj.y), 0);
    }
    #endregion

    private void BuildFillChunk()
    {
        WorldGenerator generator = chunkManager.generator;

        BlockData dirt = generator.dirt;

        //Building
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                Vector3Int pos = Vector3Int.zero;

                pos.x = i;
                pos.y = j;
                //Debug.Log(tilemap_BackWorld);
                SetBlock(pos, dirt, true, tilemap_BackWorld, BlockLayer.back);

                SetBlock(pos, dirt, true, tilemap_FrontWorld);
                
                //Debug.Log("BlockBack: " + i + "; " + j + "; " + controller.GetBlock(new Vector2Int(i, j), BlockLayer.back));
                //Debug.Log("BlockFront: " + i + "; " + j + "; " + controller.GetBlock(new Vector2Int(i, j), BlockLayer.front));
            }
        }
    }
    private void BuildChunk()
    {
        Vector3Int pos = Vector3Int.zero;
        Vector2Int posGlobal = Vector2Int.zero;

        WorldGenerator generator = chunkManager.generator;

        BlockData dirt = generator.dirt;
        BlockData sand = generator.sand;
        BlockData stone = generator.stone;
        BlockData[] ores = generator.ores;

        int terrainDestruct = 2;

        BlockData[,] chunkFront = new BlockData[chunkSize, chunkSize];
        BlockData[,] chunkBack = new BlockData[chunkSize, chunkSize];

        int worldHeight = generator.worldHeight;

        Vector2Int chunkCoord = chunkManager.ChunkPosInWorld(this);
        int chunk_level = chunkCoord.y * chunkSize;//Высота от начала мира до начала чанка

        int surface_level = worldHeight - Random.Range(12, 18);//Высота от начала мира до поверхности земли

        //int height_surface = 0;//высота от начала чанка до поверхности земли


        //Generating Enviroment
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                int height_dirt = Random.Range(4 - 1, 8 + 1);//Толщина земляного покрова

                //height_surface = 0;
                //Debug.Log("i: " + i + " ;j: " + j);

                if (chunk_level + j < surface_level)
                {
                    chunkFront[i, j] = stone;
                }
                else
                {
                    if (chunk_level + j < surface_level + height_dirt)
                    {
                        chunkFront[i, j] = dirt;
                    }
                }
            }
            //height_surface = 0;
            surface_level = worldHeight - Random.Range(12, 18) + Random.Range(-terrainDestruct - 1, terrainDestruct + 1);
        }
        //Clearing
        for (int i = 1; i < chunkSize - 1; i += 1)
        {
            for (int j = 1; j < chunkSize - 1; j += 1)
            {
                if (chunkFront[i, j] == dirt)
                {
                    if ((chunkFront[i - 1, j] == null)
                    && (chunkFront[i + 1, j] == null)
                    && (chunkFront[i, j - 1] == null))
                    {
                        chunkFront[i, j] = null;
                    }
                }
            }
        }
        chunkBack = chunkFront;

        //Building
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                pos.x = i;
                pos.y = j;

                //posGlobal.x = posObjGlobal.x + i;
                //posGlobal.y = posObjGlobal.y + j;

                //chunkFront[i, j] = dirt;
                //chunkBack[i, j]  = dirt;

                SetBlock(pos, chunkBack[i, j], true, tilemap_BackWorld, BlockLayer.back);

                SetBlock(pos, chunkFront[i, j], true, tilemap_FrontWorld);
            }
        }
    }

    #region SetBlock
    //Local
    public bool SetBlock(Vector3Int pos, BlockData data, bool checkCollisions, Tilemap tilemap, BlockLayer layer = BlockLayer.front)
    {
        bool hasBlock = checkCollisions && HasBlock(pos, tilemap);//controller.GetBlock(new Vector2Int(pos.x, pos.y)) != null;

        if (InBounds(pos) && !hasBlock)
        {
            if (data != null)
            {
                tilemap.SetTile(pos, data.tile);
                controller.AddUnit(data, pos, layer);
                //Debug.Log("True Placed; InBounds:" + InBounds(pos) + " ;Collisions: " + hasBlock);
                return true;
            }
        }
        return false;
    }
    //Global
    public bool SetBlock(Vector3Int pos, BlockData data, bool checkCollisions, BlockLayer layer = BlockLayer.front)
    {
        Tilemap tilemap = GetTileMapOfLayer(layer);
        return SetBlock(pos, data, checkCollisions, tilemap, layer);
    }   

    public bool SetBlock(Vector3 pos, BlockData data, bool checkCollisions, BlockLayer layer = BlockLayer.front)
    {
        Tilemap tilemap = GetTileMapOfLayer(layer);
        Vector3Int posInt = tilemap.WorldToCell(pos);

        return SetBlock(posInt, data, checkCollisions, tilemap);
    }
    #endregion
    #region DeleteBlock
    //Local
    public void DeleteBlock(Vector3Int pos, Tilemap tilemap, BlockLayer layer)
    {
        if (InBounds(pos) && (tilemap.GetTile(pos) != null))
        {
            BlockUnit blockUnit = controller.GetBlock(pos.x, pos.y, layer);
            if (blockUnit.data.isBreackable)
            {
                //Очистка
                controller.DeleteUnit(blockUnit);
                tilemap.SetTile(pos, null);

                #region CreateItem
                if (blockUnit.data.toCreateItem)
                {
                    Vector3 posCreateItem = new Vector3//Создание предмета в центре блока
                    {
                        x = posObj.x + Mathf.Floor(pos.x) + 0.5f,
                        y = posObj.y + Mathf.Floor(pos.y) + 0.5f
                    };
                    ItemManager.CreateItem(posCreateItem, blockUnit.GetItem());
                }
            }            
            #endregion
        }
    }
    //Global
    public void DeleteBlock(Vector3 pos, BlockLayer layer = BlockLayer.front)
    {
        Tilemap tilemap = GetTileMapOfLayer(layer);//Получение тайлмапа
        Vector3Int blockPos = tilemap.WorldToCell(pos);//Получение расположения

        DeleteBlock(blockPos, tilemap, layer);
    } 
    #endregion

    private Tilemap GetTileMapOfLayer(BlockLayer layer)
    {
        return dic_tile[layer];
    }

    private bool InBounds(Vector3Int pos)
    {
        //Debug.Log("pos.x: " + pos.x + " ;pos.y: " + pos.y + "chunkSize: " + chunkSize);
        return pos.x >= 0 && pos.x < chunkSize && pos.y >= 0 && pos.y < chunkSize;
    }
    public void Clear()
    {
        controller.Clear();
        tilemap_BackWorld.ClearAllTiles();
        tilemap_FrontWorld.ClearAllTiles();
    }
    public BlockUnit GetBlockUnit(Vector2Int pos, BlockLayer layer)
    {       
        return controller.GetBlock(pos, layer);
    }
    #region HasBlock
    //Global
    public bool HasBlock(Vector3 pos, BlockLayer layer = BlockLayer.front)
    {
        Tilemap tilemap = GetTileMapOfLayer(layer);
        return HasBlock(tilemap.WorldToCell(pos), layer);
    }
    //Local 
    public bool HasBlock(Vector3Int pos, Tilemap tilemap)
    {
        return (tilemap.HasTile(pos));
    }
    public bool HasBlock(Vector3Int pos, BlockLayer layer = BlockLayer.front)
    {
        Tilemap tilemap = GetTileMapOfLayer(layer);
        return tilemap.HasTile(pos);
    }
    #endregion

    #region Debugging
    private void DebugClick(Vector3 pos, BlockLayer layer)
    {
        Tilemap tilemap = GetTileMapOfLayer(layer);
        Vector3Int toSetBlockPos = tilemap.WorldToCell(pos);

        Debug.Log("ClickPos: " + tilemap.WorldToCell(pos) + " ;In Bounds: " + InBounds(toSetBlockPos));
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
    #endregion 
}