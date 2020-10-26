using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public enum BlockLayer
{
    Front = 0,
    Back = 1
}

[RequireComponent(typeof(ChunkUnit))]
[RequireComponent(typeof(ChunkBlockController))]
public class ChunkUnit : MonoBehaviour
{
    #region Fields

    #region Tilemaps

    [Header("Tilemaps")] [SerializeField] internal Tilemap tilemapBackWorld;
    [SerializeField] internal Tilemap tilemapFrontWorld;
    [SerializeField] internal Tilemap tilemapFrontWorldLightObstacle;
    internal Dictionary<BlockLayer, Tilemap> DicTile;

    #endregion

    #region Components

    private ChunkBlockController _controller;
    [HideInInspector]public ChunkBuilder chunkBuilder;

    #endregion

    #region Other

    private static readonly int chunkSize = GameConstants.ChunkSize;
    [HideInInspector] public ChunkManager chunkManager;
    private Vector3 _posObj;
    private ChunkUnit _chunkDowner;
    private ChunkUnit _chunkUpper;
    public bool ToGenerate { private get; set; }

    #endregion

    #endregion

    #region UnityEvents

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        Init();
        if (chunkBuilder == null)
        {
            chunkBuilder = new ChunkBuilder(this, chunkManager);
        }
        
        //Debug.Log(ToGenerate);
        if (ToGenerate)
        {
            
            chunkBuilder.GenerateBuild();
        }     
        StartCoroutine(ToBuildGrass());
        
        var chunkCoord = chunkManager.ChunkPosInWorld(this);
        _chunkDowner = chunkManager.GetDownerChunk(chunkCoord);
        _chunkUpper = chunkManager.GetUpperChunk(chunkCoord);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            chunkBuilder.BuildingGrass();
        }
    }

    #endregion
    
    public IEnumerator ToBuildGrass()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            //Debug.Log("BuildingGrass");
            chunkBuilder.BuildingGrass();
            yield break;
        }
    }

    private void Init()
    {
        //Controller
        _controller = GetComponent<ChunkBlockController>();
        _controller.SetChunk(this);
        //TileMapDic
        DicTile = new Dictionary<BlockLayer, Tilemap>
        {
            {BlockLayer.Front, tilemapFrontWorld},
            {BlockLayer.Back, tilemapBackWorld}
        };
        //Pos
        _posObj = transform.position;
    }

    #region SetBlock

    //Local
    public bool SetBlock(Vector3Int pos, BlockData data, bool checkCollisions, Tilemap tilemap,
        BlockLayer layer = BlockLayer.Front, bool toStartCor = false)
    {
        var hasBlock = checkCollisions && HasBlock(pos, tilemap);
        if (layer == BlockLayer.Back && !(data?.toPlaceBack ?? false))
        {
            return false;
        }
        if (InBounds(pos) && !hasBlock)
            if (data != null)
            {
                //Debug.Log(layer);
                tilemap.SetTile(pos, data.tile);
                if (layer == BlockLayer.Front)
                {
                    tilemapFrontWorldLightObstacle.SetTile(pos, data.tile);
                }
                _controller.AddUnit(data, pos, layer, tilemap, toStartCor);
                return true;
            }

        return false;
    }

    //Global
    public bool SetBlock(Vector3Int pos, BlockData data, bool checkCollisions, BlockLayer layer = BlockLayer.Front, bool toStartCor = false)
    {
        var tilemap = GetTileMapOfLayer(layer);
        return SetBlock(pos, data, checkCollisions, tilemap, layer, toStartCor);
    }

    public bool SetBlock(Vector3 pos, BlockData data, bool checkCollisions, BlockLayer layer = BlockLayer.Front, bool toStartCor = false)
    {
        var tilemap = GetTileMapOfLayer(layer);
        var posInt = tilemap.WorldToCell(pos);

        return SetBlock(posInt, data, checkCollisions, tilemap, layer, toStartCor);
    }

    #endregion
    public void SetMemory(Vector3Int pos, BaseBlockMemory.MemoryUnit memory, BlockLayer layer = BlockLayer.Front)
    {
        var block = _controller.GetBlock(pos.x, pos.y, layer);
        if (block != null)
        {
            var mem = (BaseBlockMemory)ScriptableObject.CreateInstance(typeof(BaseBlockMemory));;
            Debug.Log(mem);
            if (mem != null)
            {
                mem.SetMemoryUnit(memory, this);
                block.Memory = mem; 
            }
        }
    }
    
    #region DeleteBlock

    public bool CanBreakBlock(Vector3 pos, BlockLayer layer = BlockLayer.Front)
    {
        var tilemap = GetTileMapOfLayer(layer);
        var blockPos = tilemap.WorldToCell(pos);
        
        if (InBounds(blockPos) && tilemap.GetTile(blockPos) != null)
        {
            var blockUnit = _controller.GetBlock(blockPos.x, blockPos.y, layer);
            var blockUpper = GetUpperBlockUnit(blockPos, layer);

            if (blockUnit.Data.isBreackable && blockUpper != null ? !blockUpper.Data.mustHaveDownerBlock : true)
            {
                return true;
            }
        }

        return false;
    }
    //Local
    public void DeleteBlock(Vector3Int pos, Tilemap tilemap, BlockLayer layer)
    {
        if (InBounds(pos) && tilemap.GetTile(pos) != null)
        {
            var blockUnit = _controller.GetBlock(pos.x, pos.y, layer);
            var blockUpper = GetUpperBlockUnit(pos, layer);
           
            if (blockUnit.Data.isBreackable && blockUpper != null ? !blockUpper.Data.mustHaveDownerBlock : true )
            {
                //Clear
                _controller.DeleteUnit(blockUnit);
                tilemap.SetTile(pos, null);
                if (layer == BlockLayer.Front)
                {
                    tilemapFrontWorldLightObstacle.SetTile(pos, null);
                }

                #region CreateItem

                if (blockUnit.Data.toCreateItem)
                {
                    var posCreateItem = new Vector3 //Создание предмета в центре блока
                    {
                        x = _posObj.x + Mathf.Floor(pos.x) + 0.5f,
                        y = _posObj.y + Mathf.Floor(pos.y) + 0.5f
                    };
                    //Debug.Log("ItemCreated: " + posCreateItem);
                    ItemManager.CreateItem(posCreateItem, blockUnit.GetItem());
                }
                
                #endregion
            }
        }
    }

    //Global
    public void DeleteBlock(Vector3 pos, BlockLayer layer = BlockLayer.Front)
    {
        var tilemap = GetTileMapOfLayer(layer); //Получение тайлмапа
        var blockPos = tilemap.WorldToCell(pos); //Получение расположения

        DeleteBlock(blockPos, tilemap, layer);
    }

    #endregion

    #region Other
    
    private Tilemap GetTileMapOfLayer(BlockLayer layer)
    {
        return DicTile[layer];
    }
    
    private bool InBounds(Vector3Int pos)
    {
        //Debug.Log("pos.x: " + pos.x + " ;pos.y: " + pos.y + "chunkSize: " + chunkSize);
        return pos.x >= 0 && pos.x < chunkSize && pos.y >= 0 && pos.y < chunkSize;
    }

    public void Clear()
    {
        //Debug.Log("Cleared");
        _controller.Clear();
        tilemapBackWorld.ClearAllTiles();
        tilemapFrontWorld.ClearAllTiles();
        tilemapFrontWorldLightObstacle.ClearAllTiles();
    }

    #endregion

    #region GetBlockUnit

    public BlockUnit GetBlockUnit(Vector2Int pos, BlockLayer layer)
    {
        return _controller?.GetBlock(pos, layer);
    }

    public BlockUnit GetBlockUnit(Vector3 pos, BlockLayer layer)
    {
        var tilemap = GetTileMapOfLayer(layer); //Получение тайлмапа
        var blockPos = tilemap.WorldToCell(pos); //Получение расположения

        return _controller.GetBlock(new Vector2Int(blockPos.x, blockPos.y), layer);
    }

    public BlockUnit GetDownerBlockUnit(Vector2Int pos, BlockLayer layer)
    {
        var posBlock = new Vector2Int(pos.x, chunkSize - 1);
        
        if (pos.y == 0)
        {
            return _chunkDowner.GetBlockUnit(posBlock, layer);
        }
        posBlock = new Vector2Int(pos.x, pos.y - 1);
        return _controller.GetBlock(posBlock, layer);
    }
    public BlockUnit GetUpperBlockUnit(Vector3Int pos, BlockLayer layer)
    {
        var posBlock = new Vector2Int(pos.x, 0);
        
        if (chunkBuilder.ChunkLevel + chunkSize == chunkBuilder.WorldHeight && pos.y == chunkSize) //Если блок под вершиной мира
        {
            return null;
            
        }

        if (pos.y == chunkSize - 1 && _chunkUpper != null )//Если блок под низом другого чанка
        {
            return _chunkUpper.GetBlockUnit(posBlock, layer);
        }
        
        posBlock = new Vector2Int(pos.x, pos.y + 1);
        return _controller.GetBlock(posBlock, layer);
    }
    #endregion

    #region HasBlock

    //Global
    public bool HasBlock(Vector3 pos, BlockLayer layer = BlockLayer.Front)
    {
        var tilemap = GetTileMapOfLayer(layer);
        return HasBlock(tilemap.WorldToCell(pos), layer);
    }

    //Local 
    public bool HasBlock(Vector3Int pos, Tilemap tilemap)
    {
        return tilemap.HasTile(pos);
    }

    public bool HasBlock(Vector3Int pos, BlockLayer layer = BlockLayer.Front)
    {
        var tilemap = GetTileMapOfLayer(layer);
        return tilemap.HasTile(pos);
    }

    #endregion

    #region Debugging

    private void DebugClick(Vector3 pos, BlockLayer layer)
    {
        var tilemap = GetTileMapOfLayer(layer);
        var toSetBlockPos = tilemap.WorldToCell(pos);

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

    public class ChunkBuilder
    {
        #region Fields

        private readonly ChunkManager _chunkManager;
        private readonly ChunkUnit _chunkUnit;
        private WorldGenerator _generator;

        private ChunkUnit _chunkUpper;//Чанк сверху
        private ChunkUnit _chunkDowner;//Чанк снизу
        private Vector2Int _chunkPos;
        private BlockData[,] _chunkFront;// = new BlockData[chunkSize, chunkSize];
        private BlockData[,] _chunkBack;// = new BlockData[chunkSize, chunkSize];

        public int WorldHeight { get; private set; }

        public int ChunkLevel { get; private set; } //Высота от начала мира до начала чанка

        #endregion

        public ChunkBuilder(ChunkUnit chunkUnit, ChunkManager chunkManager)
        {
            _chunkUnit = chunkUnit;
            _chunkManager = chunkManager;
            
            _generator = _chunkManager.generator;
            WorldHeight = _generator.worldHeight;
            _chunkPos = _chunkManager.ChunkPosInWorld(_chunkUnit);

            ChunkLevel = _chunkPos.y * chunkSize; //Высота от начала мира до начала чанка

            var chunkCoord = _chunkManager.ChunkPosInWorld(_chunkUnit);
            _chunkUpper = _chunkManager.GetUpperChunk(chunkCoord);
            _chunkDowner = _chunkManager.GetDownerChunk(chunkCoord);
        }

        #region Main

        public void GenerateChunk()
        {
            _chunkFront = new BlockData[chunkSize, chunkSize];
            _chunkBack = new BlockData[chunkSize, chunkSize];
            var dirt = _generator.dirt;
            var sand = _generator.sand;
            var stone = _generator.stone;
            var ores = _generator.ores;

            var terrainDestruct = 2;
            int surface_level; //Высота от вершины мира до поверхности земли


            //Generating Enviroment
            for (var i = 0; i < chunkSize; i++)
            {
                surface_level = WorldHeight - Random.Range(12, 18) +
                                Random.Range(-terrainDestruct - 1, terrainDestruct + 1);
                for (var j = 0; j < chunkSize; j++)
                {
                    var heightDirt = Random.Range(3, 9); //Толщина земляного покрова

                    if (ChunkLevel + j < surface_level)
                    {
                        _chunkFront[i, j] = stone;
                    }
                    else
                    {
                        if (ChunkLevel + j < surface_level + heightDirt) _chunkFront[i, j] = dirt;
                    }
                }
            }

            //Clearing
            for (var i = 1; i < chunkSize - 1; i++)
            for (var j = 1; j < chunkSize - 1; j++)
                if (_chunkFront[i, j] != null)
                    if (_chunkFront[i - 1, j] == null
                        && _chunkFront[i + 1, j] == null
                        && _chunkFront[i, j - 1] == null)
                        _chunkFront[i, j] = null;
            _chunkBack = _chunkFront;
        }
        
        public void Building()
        {
            var pos = Vector3Int.zero;
            for (var i = 0; i < chunkSize; i++)
            for (var j = 0; j < chunkSize; j++)
            {
                pos.x = i;
                pos.y = j;
                _chunkUnit.SetBlock(pos, _chunkFront[i, j], true, _chunkUnit.tilemapFrontWorld);

                _chunkUnit.SetBlock(pos, _chunkBack[i, j], true, _chunkUnit.tilemapBackWorld, BlockLayer.Back);
            }
        }

        public void Rebuild(BlockUnitChunk[,] chunkFront, BlockUnitChunk[,] chunkBack)
        {
            _chunkFront = new BlockData[chunkSize,chunkSize];
            _chunkBack  = new BlockData[chunkSize,chunkSize];
            var pos = Vector3Int.zero;
            for (var i = 0; i < chunkSize; i++)
            for (var j = 0; j < chunkSize; j++)
            {
                pos.x = i;
                pos.y = j;
                var front = chunkFront[i, j];
                var back = chunkBack[i, j];
                if (front != null)
                {
                    _chunkFront[i, j] = front.Data;
                    _chunkUnit.SetBlock(pos, front.Data, true, _chunkUnit.tilemapFrontWorld);
                    _chunkUnit.SetMemory(pos, front.Memory);
                }

                if (back != null)
                {
                    _chunkBack[i, j] = back.Data;
                    _chunkUnit.SetBlock(pos, back.Data, true, _chunkUnit.tilemapBackWorld, BlockLayer.Back);
                    _chunkUnit.SetMemory(pos, back.Memory, BlockLayer.Back);
                }
            }
        }
        
        #endregion

        #region BuildMethods

        public void GenerateBuild()
        {
            GenerateChunk();
            Building();
        }
        public void BuildFillChunk()
        {
            var generator = _chunkManager.generator;

            var dirt = generator.dirt;

            var pos = Vector3Int.zero;
            //Building
            for (var i = 0; i < chunkSize; i++)
            for (var j = 0; j < chunkSize; j++)
            {
                pos.x = i;
                pos.y = j;

                _chunkUnit.SetBlock(pos, dirt, true, _chunkUnit.tilemapBackWorld, BlockLayer.Back);

                _chunkUnit.SetBlock(pos, dirt, true, _chunkUnit.tilemapFrontWorld);
            }
        }

        #endregion
        
        #region Grass

        public void BuildingGrass()
        {
            if (ChunkLevel > WorldHeight - 30) //Если уровень чанка идет по уровню земли
            {
                var pos = Vector3Int.zero;
                //Debug.Log("Start Building Grass");
                for (var i = 0; i < chunkSize; i++)
                for (var j = 0; j < chunkSize; j++)
                {
                    pos.x = i;
                    pos.y = j;
                    
                    SetGrass(pos);
                }
            }
        }
        
        public bool CanSetGrass(Vector3Int pos, BlockData data = null)
        {
            int i = pos.x, j = pos.y;
            //Debug.Log("i: " + i + " ;j: " + j);
            //Debug.Log(_chunkFront[i, j] + ";" + _generator.dirt);
            
            
            //Debug.Log(_chunkFront);
            //if (_chunkFront == null)
            //{
            //    return false;
            //}
            if ((data ?? _chunkFront[i, j]) == _generator.dirt)
            {
                if (ChunkLevel + chunkSize == WorldHeight && j == chunkSize) //Если блок под вершиной мира
                {
                    return true;
                }

                if (j + 1 <= chunkSize - 1 && _chunkFront[i, j + 1] == null) //Если над блоком пусто
                {
                    return true;
                }

                if (j == chunkSize - 1 && 
                    _chunkUpper != null && 
                    !_chunkUpper.HasBlock(new Vector3Int(i, 0, 0))) //Если блок под низом другого чанка
                {
                    return true;
                }
            }
            
            return false;
        }

        public void RefreshDownerGrassBlock(int i, int j)
        {
            var posBlock = new Vector3Int(i, j-1, 0);
            //Debug.Log("BlockDowner: " + _chunkFront[i, j - 1]);
            var grassTile = _generator.dirt.tileVariables[0];
            if (j - 1 >= 0 && _chunkUnit.tilemapFrontWorld.GetTile(posBlock) == grassTile ) //Если над блоком пусто
            {
                //Debug.Log("Refreshed");
                _chunkUnit.tilemapFrontWorld.SetTile(posBlock, _generator.dirt.tile);
            }
            else
            {
                posBlock = new Vector3Int(i, chunkSize - 1, 0);
                //Debug.Log("BlockDowner: " + _chunkDowner.tilemapFrontWorld.GetTile(posBlock));
                if (j == 0 && _chunkDowner.tilemapFrontWorld.GetTile(posBlock) == grassTile)
                {
                    
                    //Debug.Log("Refreshed");
                    _chunkDowner.tilemapFrontWorld.SetTile(posBlock, _generator.dirt.tile);
                }
            }
        }
        
        private void SetGrass(Vector3Int pos)
        {
            if (CanSetGrass(pos)) 
            {
                //Debug.Log("Grass Seted");
                _chunkUnit.tilemapFrontWorld.SetTile(pos, _chunkFront[pos.x, pos.y].tileVariables[0]);
            }
        }

        #endregion

        public class BlockUnitChunk
        {
            public BlockData Data;

            public BaseBlockMemory.MemoryUnit Memory;
            
            public BlockUnitChunk(BlockData data, BaseBlockMemory.MemoryUnit memory)
            {
                Data = data;
                Memory = memory;
            }
        }
    }
}