using System.Collections;
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
    [SerializeField] internal Tilemap tilemap_BackWorld;
    [SerializeField] internal Tilemap tilemap_FrontWorld;

    internal Dictionary<BlockLayer, Tilemap> dic_tile;

    //Components
    private ChunkBlockController controller;

    //Other
    private static readonly int chunkSize = GameConstants.chunkSize;
    [HideInInspector] public ChunkManager chunkManager;
    private ChunkBuilder chunkBuilder;

    //Position Cash
    private Vector3 posObj;

    private void Awake()
    {
        Init();   
    }
    private void Start()
    {
        Init();
        chunkBuilder = new ChunkBuilder(this, chunkManager);
        chunkBuilder.Build();
        StartCoroutine(ToBuildGrass());
    }
    IEnumerator ToBuildGrass()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            chunkBuilder.BuildingGrass();
        }
    }
    private void Init()
    {
        //Controller
        controller = GetComponent<ChunkBlockController>();
        controller.chunk = this;
        //TileMapDic
        dic_tile = new Dictionary<BlockLayer, Tilemap>
        {
            { BlockLayer.front, tilemap_FrontWorld },
            { BlockLayer.back, tilemap_BackWorld }
        };
        //Pos
        posObj = transform.position;
        //posObjGlobal = new Vector3Int(Mathf.FloorToInt(posObj.x), Mathf.FloorToInt(posObj.y), 0);
    }

    #region SetBlock
    //Local
    public bool SetBlock(Vector3Int pos, BlockData data, bool checkCollisions, Tilemap tilemap, BlockLayer layer = BlockLayer.front)
    {
        bool hasBlock = checkCollisions && HasBlock(pos, tilemap);
        if (InBounds(pos) && !hasBlock)
        {
            if (data != null)
            {
                tilemap.SetTile(pos, data.tile);
                controller.AddUnit(data, pos, layer);               
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
                    //Debug.Log("ItemCreated: " + posCreateItem);
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
    #region GetBlockUnit
    public BlockUnit GetBlockUnit(Vector2Int pos, BlockLayer layer)
    {
        return controller?.GetBlock(pos, layer);
    }
    public BlockUnit GetBlockUnit(Vector3 pos, BlockLayer layer)
    {
        Tilemap tilemap = GetTileMapOfLayer(layer);//Получение тайлмапа
        Vector3Int blockPos = tilemap.WorldToCell(pos);//Получение расположения

        return controller.GetBlock(new Vector2Int(blockPos.x, blockPos.y), layer);
    } 
    #endregion
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
        return tilemap.HasTile(pos);
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

    internal class ChunkBuilder 
    {
        private ChunkManager chunkManager;
        private ChunkUnit chunkUnit;
        private WorldGenerator generator;

        private ChunkUnit chunkUpper;
        private Vector2Int chunkPos;
        private BlockData[,] chunkFront = new BlockData[chunkSize, chunkSize];
        private BlockData[,] chunkBack = new BlockData[chunkSize, chunkSize];

        private int worldHeight;
        private int chunk_level;//Высота от начала мира до начала чанка

        public ChunkBuilder(ChunkUnit chunkUnit, ChunkManager chunkManager)
        {
            this.chunkUnit = chunkUnit;
            this.chunkManager = chunkManager;
            Init();
        }
        private void Init()
        {
            generator = chunkManager.generator;
            worldHeight = generator.worldHeight;
            chunk_level = chunkPos.y * chunkSize;//Высота от начала мира до начала чанка

            Vector2Int chunkCoord = chunkManager.ChunkPosInWorld(chunkUnit);
            chunkUpper = chunkManager.GetUpperChunk(chunkCoord);

            chunkPos = chunkManager.ChunkPosInWorld(chunkUnit);
        }
        public void GenerateChunk()
        {          
            BlockData dirt = generator.dirt;
            BlockData sand = generator.sand;
            BlockData stone = generator.stone;
            BlockData[] ores = generator.ores;

            int terrainDestruct = 2;
            int surface_level;//Высота от вершины мира до поверхности земли
            
            //Generating Enviroment
            for (int i = 0; i < chunkSize; i++)
            {
                surface_level = worldHeight - Random.Range(12, 18) + Random.Range(-terrainDestruct - 1, terrainDestruct + 1);
                //Debug.Log(surface_level);
                for (int j = 0; j < chunkSize; j++)
                {
                    int height_dirt = Random.Range(3, 9);//Толщина земляного покрова

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
            }
            //Clearing
            for (int i = 1; i < chunkSize - 1; i++)
            {
                for (int j = 1; j < chunkSize - 1; j++)
                {
                    //Debug.Log(chunkFront[i, j]);
                    if (chunkFront[i, j] != null)
                    {
                        //Debug.Log("BlockHas");
                        if ((chunkFront[i - 1, j] == null)
                        && (chunkFront[i + 1, j] == null)
                        && (chunkFront[i, j - 1] == null))
                        {
                            //Debug.Log("BlockCleared");
                            chunkFront[i, j] = null;
                        }
                    }
                }
            }
            chunkBack = chunkFront;                      
        }
        private void Rebuild(BlockData[,] chunkFront, BlockData[,] chunkBack)
        {
            this.chunkFront = chunkFront;
            this.chunkBack = chunkBack;
            Building();
        }
        public void Build()
        {
            GenerateChunk();
            Building();
        }
        private void Building()
        {
            Vector3Int pos = Vector3Int.zero;
            for (int i = 0; i < chunkSize; i++)
            {
                for (int j = 0; j < chunkSize; j++)
                {
                    pos.x = i;
                    pos.y = j;
                    chunkUnit.SetBlock(pos, chunkFront[i, j], true, chunkUnit.tilemap_FrontWorld);
                    
                    chunkUnit.SetBlock(pos, chunkBack[i, j], true, chunkUnit.tilemap_BackWorld, BlockLayer.back);
                }
            }
        }
        public void BuildFillChunk()
        {
            WorldGenerator generator = chunkManager.generator;

            BlockData dirt = generator.dirt;

            Vector3Int pos = Vector3Int.zero;
            //Building
            for (int i = 0; i < chunkSize; i++)
            {
                for (int j = 0; j < chunkSize; j++)
                {                    
                    pos.x = i;
                    pos.y = j;

                    chunkUnit.SetBlock(pos, dirt, true, chunkUnit.tilemap_BackWorld, BlockLayer.back);

                    chunkUnit.SetBlock(pos, dirt, true, chunkUnit.tilemap_FrontWorld);
                }
            }
        }
        public void BuildingGrass()
        {
            if (chunk_level > worldHeight - 30)//Если уровень чанка идет по уровню земли
            {
                Vector3Int pos = Vector3Int.zero;
                for (int i = 0; i < chunkSize; i++)
                {
                    for (int j = 0; j < chunkSize; j++)
                    {
                        pos.x = i;
                        pos.y = j;

                        SetGrass(pos);
                    }
                }
            }            
        }
        private void SetGrass(Vector3Int pos)
        {
            int i = pos.x, j = pos.y;
            if (chunkFront[i, j] == generator.dirt)
            {
                if (chunk_level + chunkSize == worldHeight && j == chunkSize)//Если блок под вершиной мира
                {
                    chunkUnit.tilemap_FrontWorld.SetTile(pos, chunkFront[i, j].tileVariables[0]);
                    return;
                }
            }
            if ((j + 1) <= chunkSize - 1)//Если над блоком пусто
            {
                if (chunkFront[i, j + 1] == null)
                {
                    chunkUnit.tilemap_FrontWorld.SetTile(pos, chunkFront[i, j].tileVariables[0]);
                    return;
                }
            }
            if (j == chunkSize - 1)//Если блок под низом другого чанка
            {
                //Debug.Log("Block: " + chunkUpper?.HasBlock(new Vector3Int(i, 0, 0), BlockLayer.front));
                if (!chunkUpper.HasBlock(new Vector3Int(i, 0, 0), BlockLayer.front))
                {
                    chunkUnit.tilemap_FrontWorld.SetTile(pos, chunkFront[i, j].tileVariables[0]);
                    return;
                }
            }              
        }
    }
}

