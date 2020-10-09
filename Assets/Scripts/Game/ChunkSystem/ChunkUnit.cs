using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum BlockLayer
{
    front,
    back
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
    [HideInInspector]public ChunkManager chunkManager;

    //Position Cash
    private Vector3 posObj;
    private Vector3Int posObjGlobal;

    private void Start()
    {
        InitMethods();

        BuildChunk();       
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
        BlockData[,] chunkBack  = new BlockData[chunkSize, chunkSize];

        //int meter;
        //int a = posGlobal.y + Random.Range(12, 18);
        ////Generating Enviroment
        //for (int i = 0; i < chunkSize; i++)
        //{
        //    for (meter = a; meter < chunkSize; meter++)
        //    {
        //        if (meter < Random.Range(19, 26))
        //        {
        //            chunkFront[i, meter - posGlobal.y] = dirt;
        //        }
        //        else
        //        {
        //            chunkFront[i, meter - posGlobal.y] = stone;
        //        }
        //    }
        //    a = posGlobal.y + Random.Range(12, 18) + Random.Range(-terrainDestruct - 1, terrainDestruct + 1);
        //}
        ////Clearing
        //for (int i = 1; i < chunkSize - 1; i += 1)
        //{
        //    for (int j = 1; j < chunkSize - 1; j += 1)
        //    {
        //        if (chunkFront[i, j] == dirt)
        //        {
        //            if ((chunkFront[i - 1, j] == null)
        //            && (chunkFront[i + 1, j] == null)
        //            && (chunkFront[i, j - 1] == null))
        //            {
        //                chunkFront[i, j] = null;
        //            }
        //        }
        //    }
        //}
        //chunkBack = chunkFront;

        //Building
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                pos.x = i;
                pos.y = j;

                posGlobal.x = posObjGlobal.x + i;
                posGlobal.y = posObjGlobal.y + j;

                chunkFront[i, j] = dirt;
                chunkBack[i, j]  = dirt;

                SetBlock(pos, chunkBack[i, j], true, tilemap_BackWorld);

                SetBlock(pos, chunkFront[i, j], true, tilemap_FrontWorld);               
            }
        }
    }

    #region SetBlock
    //Local
    public void SetBlock(Vector3Int pos, BlockData data, bool checkCollisions, Tilemap tilemap)
    {
        if (InBounds(pos) && !(!checkCollisions && !HasBlock(pos, tilemap)))
        {
            if (data != null)
            {
                tilemap.SetTile(pos, data.tile);
            }
            controller.AddUnit(data, pos);
        }
    }
    //Global
    public void SetBlock(Vector3 pos, BlockData data, bool checkCollisions, BlockLayer layer = BlockLayer.front)
    {
        Tilemap tilemap = GetTileOfLayer(layer);
        Vector3Int posInt = tilemap.WorldToCell(pos);

        SetBlock(posInt, data, checkCollisions, tilemap);
    }
    #endregion
    #region DeleteBlock
    //Local
    public void DeleteBlock(Vector3Int pos, Tilemap tilemap)
    {
        if (InBounds(pos) && (tilemap.GetTile(pos) != null))
        {
            BlockUnit blockUnit = controller.GetBlock(pos.x, pos.y);

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
            #endregion
        }
    }
    //Global
    public void DeleteBlock(Vector3 pos, BlockLayer layer = BlockLayer.front)
    {
        Tilemap tilemap = GetTileOfLayer(layer);//Получение тайлмапа
        Vector3Int blockPos = tilemap.WorldToCell(pos);//Получение расположения

        DeleteBlock(blockPos, tilemap);
    } 
    #endregion

    private Tilemap GetTileOfLayer(BlockLayer layer)
    {
        return dic_tile[layer];
    }

    private bool InBounds(Vector3Int pos)
    {
        //Debug.Log("pos.x: " + pos.x + " ;pos.y: " + pos.y + "chunkSize: " + chunkSize);
        return pos.x >= 0 && pos.x < chunkSize && pos.y >= 0 && pos.y < chunkSize;
    }

    #region HasBlock
    //Global
    public bool HasBlock(Vector3 pos, BlockLayer layer = BlockLayer.front)
    {
        Tilemap tilemap = GetTileOfLayer(layer);
        return HasBlock(tilemap.WorldToCell(pos), layer);
    }
    //Local 
    public bool HasBlock(Vector3Int pos, Tilemap tilemap)
    {
        return (tilemap.HasTile(pos));
    }
    public bool HasBlock(Vector3Int pos, BlockLayer layer = BlockLayer.front)
    {
        Tilemap tilemap = GetTileOfLayer(layer);
        return tilemap.HasTile(pos);
    }
    #endregion

    #region Debugging
    private void DebugClick(Vector3 pos, BlockLayer layer)
    {
        Tilemap tilemap = GetTileOfLayer(layer);
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
