using System;
using System.Collections;
using System.Collections.Generic;
using Game.Bot;
using Game.Lighting;
using Singleton;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using UsefulScripts;

public enum BlockLayer
{
    Front = 0,
    Back = 1
}

public enum ChunkTilemap
{
    Back,
    BackLightSource,
    Front,
    FrontUnSolid,
    FrontLightObstacle,
    FrontLightSource
}

[RequireComponent(typeof(ChunkUnit))]
[RequireComponent(typeof(ChunkBlockController))]
public class ChunkUnit : MonoBehaviour
{
    #region Fields

    #region Tilemaps

    [Header("Tilemaps")] [SerializeField] internal Tilemap tilemapBackWorld;
    [SerializeField] internal Tilemap tilemapBackWorldLightSources;
    [SerializeField] internal Tilemap tilemapBackBackWorld;
    [SerializeField] internal Tilemap tilemapFrontWorld;
    [SerializeField] internal Tilemap tilemapFrontWorldUnSolid;
    [SerializeField] internal Tilemap tilemapFrontWorldLightObstacle;
    [SerializeField] internal Tilemap tilemapFrontWorldLightSources;

    private Dictionary<ChunkTilemap, Tilemap> _tilemaps;
    private Dictionary<BlockLayer, Tilemap> _dicTile;

    #endregion

    #region Components

    private ChunkBlockController _controller;
    [HideInInspector] public ChunkBuilder chunkBuilder;

    #endregion

    #region Other

    private static readonly int chunkSize = GameConstants.ChunkSize;
    [HideInInspector] public ChunkManager chunkManager;
    private Vector3 _posObj;
    private ChunkUnit _chunkDowner;
    private ChunkUnit _chunkUpper;
    private Dictionary<Vector3Int, LightBlock> _lightBlocks;
    public Vector2Int posChunk;
    
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
        
        posChunk = chunkManager.ChunkPosInWorld(this);
        _chunkDowner = chunkManager.GetDownerChunk(posChunk);
        _chunkUpper = chunkManager.GetUpperChunk(posChunk);

        StartCoroutine(ToBuildGrass());
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.K))
    //     {
    //         chunkBuilder.BuildingGrass();
    //     }
    // }

    #endregion

    private IEnumerator ToBuildGrass()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            chunkBuilder.BuildingGrass();
            yield break;
        }
    }

    private IEnumerator ToBuildTwo()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            chunkBuilder.BuildTwo();
            yield break;
        }
    }

    private void Init()
    {
        _lightBlocks = new Dictionary<Vector3Int, LightBlock>();
        //Controller
        _controller = GetComponent<ChunkBlockController>();
        _controller.SetChunk(this);
        //TilemapDic block layer
        _dicTile = new Dictionary<BlockLayer, Tilemap>
        {
            {BlockLayer.Front, tilemapFrontWorld},
            {BlockLayer.Back, tilemapBackWorld}
        };
        //Tilemaps
        _tilemaps = new Dictionary<ChunkTilemap, Tilemap>
        {
            {ChunkTilemap.Back, tilemapBackWorld},
            {ChunkTilemap.BackLightSource, tilemapBackWorldLightSources},
            {ChunkTilemap.Front, tilemapFrontWorld},
            {ChunkTilemap.FrontUnSolid, tilemapFrontWorldUnSolid},
            {ChunkTilemap.FrontLightObstacle, tilemapFrontWorldLightObstacle},
            {ChunkTilemap.FrontLightSource, tilemapFrontWorldLightSources},
        };
        //Pos
        _posObj = transform.position;
    }

    #region LightBlock

    private void CreateLightBlock(Vector3Int pos, Color color, float size = 10)
    {
        if (!_lightBlocks.ContainsKey(pos))
        {
            var pos3 = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0);
            var obj = PoolManager.GetObject("LightBlock", transform.position + pos3, Quaternion.identity);
            if (obj != null)
            {
                var light = obj.GetComponent<LightBlock>();
                light.SetSize(size);
                light.SetColor(color);
                _lightBlocks.Add(pos, light);
            }
        }
    }

    private void DestroyLightBlock(Vector3Int pos)
    {
        if (_lightBlocks.ContainsKey(pos))
        {
            var lightBlock = _lightBlocks[pos];

            if (lightBlock != null)
            {
                _lightBlocks[pos]?.DestroyLight();
                _lightBlocks.Remove(pos);
            }
        }
    }

    #endregion

    #region SetBlock

    //Local
    public bool SetBlock(Vector3Int pos, BlockData data, bool checkCollisions, Tilemap tilemap,
        BlockLayer layer = BlockLayer.Front, bool toStartCor = false)
    {
        var hasBlock = checkCollisions && HasBlock(pos, layer);
        if (layer == BlockLayer.Back && !(data?.toPlaceBack ?? false))
        {
            return false;
        }

        if (InBounds(pos) && !hasBlock && data != null)
        {
            if (data.isSolid)
            {
                tilemap.SetTile(pos, data.tile);
            }
            else
            {
                GetTileMap(ChunkTilemap.FrontUnSolid).SetTile(pos, data.tile);
            }

            if (!data.isLightSource)
            {
                if (layer == BlockLayer.Front && data.isLightObstacle)
                {
                    GetTileMap(ChunkTilemap.FrontLightObstacle).SetTile(pos, data.tile);
                }
            }
            else
            {
                if (layer == BlockLayer.Front)
                {
                    GetTileMap(ChunkTilemap.FrontLightSource).SetTile(pos, data.tileLightSource);
                    CreateLightBlock(pos, new Color(1f, 0.75f, 0.13f, 150f / 255f));
                }
                else
                {
                    GetTileMap(ChunkTilemap.BackLightSource).SetTile(pos, data.tileLightSource);
                    CreateLightBlock(pos, new Color(1f, 0.75f, 0.13f, 150f / 255f));
                }
            }

            _controller.AddUnit(data, pos, layer, tilemap, toStartCor);
            return true;
        }
        //Debug.Log("Inbounds: " + InBounds(pos) + " ;HasBlock: " + !hasBlock);
        return false;
    }

    //Global
    public bool SetBlock(Vector3Int pos, BlockData data, bool checkCollisions, BlockLayer layer = BlockLayer.Front,
        bool toStartCor = false)
    {
        var tilemap = GetTileMapOfLayer(layer);
        return SetBlock(pos, data, checkCollisions, tilemap, layer, toStartCor);
    }

    public bool SetBlock(Vector3 pos, BlockData data, bool checkCollisions, BlockLayer layer = BlockLayer.Front,
        bool toStartCor = false)
    {
        var tilemap = GetTileMapOfLayer(layer);
        var posInt = tilemap.WorldToCell(pos);

        return SetBlock(posInt, data, checkCollisions, tilemap, layer, toStartCor);
    }

    #endregion

    #region DeleteBlock

    public bool CanBreakBlock(Vector3 pos, BlockLayer layer = BlockLayer.Front)
    {
        var blockPos = tilemapFrontWorld.WorldToCell(pos);
        if (layer == BlockLayer.Back && HasBlock(pos, BlockLayer.Front))
        {
            return false;
        }

        if (InBounds(blockPos) && HasBlock(blockPos, layer))
        {
            var blockUnit = _controller.GetBlock(blockPos.x, blockPos.y, layer);
            var blockUpper = GetUpperBlockUnit(blockPos, layer);
            
            if (blockUnit.Data.isBreackable && (blockUpper != null ? !blockUpper.Data.mustHaveDownerBlock : true))
            {
                //Debug.Log(blockUnit.Data.nameBlock + ":" + blockUnit.Data.isBreackable);
                return true;
            }
        }

        return false;
    }

    //Local
    public void DeleteBlock(Vector3Int pos, BlockLayer layer, bool createItem = true)
    {
        if (layer == BlockLayer.Back && HasBlock(pos, BlockLayer.Front))
        {
            return;
        }

        if (InBounds(pos) && HasBlock(pos, layer))
        {
            var blockUnit = _controller.GetBlock(pos.x, pos.y, layer);
            var blockUpper = GetUpperBlockUnit(pos, layer);

            if (blockUnit.Data.isBreackable && blockUpper != null ? !blockUpper.Data.mustHaveDownerBlock : true)
            {
                //Clear
                _controller.DeleteUnit(blockUnit);
                DestroyLightBlock(pos);
                if (layer == BlockLayer.Front)
                {
                    tilemapFrontWorld.SetTile(pos, null);
                    tilemapFrontWorldUnSolid.SetTile(pos, null);
                    tilemapFrontWorldLightSources.SetTile(pos, null);
                    tilemapFrontWorldLightObstacle.SetTile(pos, null);
                }
                else
                {
                    tilemapBackWorld.SetTile(pos, null);
                    tilemapBackWorldLightSources.SetTile(pos, null);
                }

                #region CreateItem

                if (createItem && blockUnit.Data.toCreateItem)
                {
                    var posCreateItem = new Vector3 //Создание предмета в центре блока
                    {
                        x = _posObj.x + Mathf.Floor(pos.x) + 0.5f,
                        y = _posObj.y + Mathf.Floor(pos.y) + 0.5f
                    };
                    //Debug.Log("ItemCreated: " + posCreateItem);
                    Toolbox.Instance.mItemManager.CreateItem(posCreateItem, blockUnit.GetItem());
                }

                #endregion
            }
        }
    }

    //Global
    public void DeleteBlock(Vector3 pos, BlockLayer layer = BlockLayer.Front, bool createItem = true)
    {
        var blockPos = tilemapFrontWorld.WorldToCell(pos); //Получение расположения

        DeleteBlock(blockPos, layer, createItem);
    }

    #endregion

    #region Other

    private void SetMemory(Vector3Int pos, string memory, BlockLayer layer = BlockLayer.Front)
    {
        var block = _controller.GetBlock(pos.x, pos.y, layer);
        if (block != null && memory != null)
        {
            //var mem = (BaseBlockMemory)ScriptableObject.CreateInstance(typeof(BaseBlockMemory));
            //var m = mem as ChestMemory;
            //Debug.Log("MemCreated: " + mem);
            //Debug.Log(block.Memory);
            block.Memory?.SetMemoryUnit(memory);

            //if (mem != null)
            //{
            //    mem.SetMemoryUnit(memory, this);
            //    block.Memory = mem;
            //}
        }
    }

    private Tilemap GetTileMap(ChunkTilemap chunkTilemapType)
    {
        return _tilemaps[chunkTilemapType];
    }

    private Tilemap GetTileMapOfLayer(BlockLayer layer)
    {
        return _dicTile[layer];
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
        tilemapBackWorldLightSources.ClearAllTiles();
        tilemapFrontWorld.ClearAllTiles();
        tilemapFrontWorldUnSolid.ClearAllTiles();
        tilemapFrontWorldLightObstacle.ClearAllTiles();
        tilemapFrontWorldLightSources.ClearAllTiles();
    }

    internal void BuildBackBack(BlockData block)
    {
        var pos = Vector3Int.zero;
        var tile = block.tile;
        for (var i = 0; i < chunkSize; i++)
        for (var j = 0; j < chunkSize; j++)
        {
            pos.x = i;
            pos.y = j;
                    
            tilemapBackBackWorld.SetTile(pos, tile);
        }
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

    public bool HasBlockUnit(Vector3 pos, BlockLayer layer = BlockLayer.Front)
    {
        return _controller.GetBlock(new Vector2(pos.x, pos.y), layer) != null;
    }
    public bool HasBlockUnit(Vector3Int pos, BlockLayer layer = BlockLayer.Front)
    {
        return _controller.GetBlock(new Vector2Int(pos.x, pos.y), layer) != null;
    }
    //Global
    public bool HasBlock(Vector3 pos, BlockLayer layer = BlockLayer.Front)
    {
        return HasBlockUnit(pos, layer);
    }

    //Local 
    public bool HasBlock(Vector3Int pos, BlockLayer layer = BlockLayer.Front)
    {
        return HasBlockUnit(pos, layer);
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
            _chunkUnit.posChunk = _chunkManager.ChunkPosInWorld(_chunkUnit);

            ChunkLevel = _chunkUnit.posChunk.y * chunkSize; //Высота от начала мира до начала чанка

            var chunkCoord = _chunkManager.ChunkPosInWorld(_chunkUnit);
            _chunkUpper = _chunkManager.GetUpperChunk(chunkCoord);
            _chunkDowner = _chunkManager.GetDownerChunk(chunkCoord);

            BuildingBackBack();
        }

        #region Main

        private void GenerateChunk()
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

        private void Building()
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
                var back  = chunkBack [i, j];
                if (front != null)
                {
                    _chunkFront[i, j] = front.Data;
                    //if (front.Memory != null)
                    //{
                    //    var m = front.Memory as ChestMemory;
                        //Debug.Log("frontMemChest: " + front.memStr);  
                    //}
                    
                    _chunkUnit.SetBlock(pos, front.Data, true, _chunkUnit.tilemapFrontWorld);
                    _chunkUnit.SetMemory(pos, front.memStr);
                }

                if (back != null)
                {
                    _chunkBack[i, j] = back.Data;
                    _chunkUnit.SetBlock(pos, back.Data, true, _chunkUnit.tilemapBackWorld, BlockLayer.Back);
                    //if (back.Memory != null)
                    //{
                        
                    //}
                    _chunkUnit.SetMemory(pos, back.memStr, BlockLayer.Back);
                }
            }
        }
        
        #endregion

        #region BuildMethods

        public void GenerateBuild()
        {
            GenerateChunk();
            Building();
            _chunkUnit.StartCoroutine(_chunkUnit.ToBuildTwo());
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
        public void BuildingBackBack()
        {
            if (ChunkLevel < WorldHeight - 30) //Если уровень чанка ниже по уровня земли
            {
                _chunkUnit.BuildBackBack(_generator.stone);
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

        public void BuildTwo()
        {
            GenerateDungeon(
                RandomScripts.Choose<int>(new RandomScripts.ChanceItem[]
                {
                    new RandomScripts.ChanceItem(1, 2),
                    new RandomScripts.ChanceItem(2, 4),
                    new RandomScripts.ChanceItem(3, 7),
                    new RandomScripts.ChanceItem(4, 5),
                }), 
                RandomScripts.Choose<int>(new RandomScripts.ChanceItem[]
                {
                    new RandomScripts.ChanceItem(35, 2),
                    new RandomScripts.ChanceItem(15, 7),
                    new RandomScripts.ChanceItem(20, 5),
                    new RandomScripts.ChanceItem(25, 4),
                }), 
                RandomScripts.Choose<int>(new RandomScripts.ChanceItem[]
                {
                    new RandomScripts.ChanceItem(10, 2),
                    new RandomScripts.ChanceItem(20, 4),
                    new RandomScripts.ChanceItem(45, 5),
                    new RandomScripts.ChanceItem(50, 7),
                }));
            GenerateOre(
                RandomScripts.Choose<int>(new RandomScripts.ChanceItem[]
                {
                    new RandomScripts.ChanceItem(2, 2),
                    new RandomScripts.ChanceItem(3, 4),
                }), 
                RandomScripts.Choose<int>(new RandomScripts.ChanceItem[]
                {
                    new RandomScripts.ChanceItem(35, 2),
                    new RandomScripts.ChanceItem(15, 7),
                    new RandomScripts.ChanceItem(20, 5),
                    new RandomScripts.ChanceItem(25, 4),
                }), 
                RandomScripts.Choose<int>(new RandomScripts.ChanceItem[]
                {
                    new RandomScripts.ChanceItem(3, 2),
                    new RandomScripts.ChanceItem(5, 4),
                    new RandomScripts.ChanceItem(6, 8),
                }), _generator.ores[0]); //coal
            GenerateOre(RandomScripts.Choose<int>(new RandomScripts.ChanceItem[]
                {
                    new RandomScripts.ChanceItem(2, 2),
                    new RandomScripts.ChanceItem(3, 4),
                }), 
                RandomScripts.Choose<int>(new RandomScripts.ChanceItem[]
                {
                    new RandomScripts.ChanceItem(35, 2),
                    new RandomScripts.ChanceItem(15, 7),
                    new RandomScripts.ChanceItem(20, 5),
                    new RandomScripts.ChanceItem(25, 4),
                }), 
                RandomScripts.Choose<int>(new RandomScripts.ChanceItem[]
                {
                    new RandomScripts.ChanceItem(3, 2),
                    new RandomScripts.ChanceItem(5, 4),
                    new RandomScripts.ChanceItem(6, 8),
                }), _generator.ores[1]); //iron
        }
        private void GenerateDungeon(int n, int startHeight, int size)
        {
            if (ChunkLevel < startHeight)
            {
                for (var i = 0; i < n; i += 1) //Количество пещер
                {
                    var xx = Random.Range(0, chunkSize);
                    var yy = Random.Range(0, chunkSize); //Глубина, ниже которой начнут генерироваться пещеры
                    for (var j = 0; j < size; j += 1) //Размер одной пещеры
                    {
                        var rr = Random.Range(0, 4);
                        if (rr == 0) xx = Mathf.Min(xx + 1, chunkSize);
                        if (rr == 1) xx = Mathf.Max(xx - 1, 0);
                        if (rr == 2) yy = Mathf.Min(yy + 1, chunkSize);
                        if (rr == 3) yy = Mathf.Max(yy - 1, 0);
                        if ((xx < 0 || xx > chunkSize) && (yy < 0 || yy > startHeight))
                        {
                            xx = Random.Range(0, chunkSize);
                            yy = Random.Range(startHeight, chunkSize);
                        }

                        //Debug.Log("x: " + xx + " ;y:" + yy);
                        if (xx < chunkSize && yy < chunkSize)
                            _chunkUnit.DeleteBlock(new Vector3Int(xx, yy, 0), BlockLayer.Front, false);
                    }
                }
            }
        }

        private void GenerateOre(int n, int startHeight, int size, BlockData block)
        {
            if (ChunkLevel < startHeight)
            {
                for (var i = 0; i < n; i += 1) //Количество залежей руды
                {
                    var xx = Random.Range(0, chunkSize);
                    var yy = Random.Range(0, chunkSize); //Глубина, ниже которой начнут генерироваться руда
                    for (var j = 0; j < size; j += 1) //Кол-во блоков в одной жиле
                    {
                        var rr = Random.Range(0, 4);
                        if (rr == 0) xx = Mathf.Min(xx + 1, chunkSize);
                        if (rr == 1) xx = Mathf.Max(xx - 1, 0);
                        if (rr == 2) yy = Mathf.Min(yy + 1, startHeight);
                        if (rr == 3) yy = Mathf.Max(yy - 1, 0);
                        if ((xx < 0 || xx > chunkSize) && (yy < 0 || yy > chunkSize))
                        {
                            xx = Random.Range(0, chunkSize);
                            yy = Random.Range(startHeight, chunkSize);
                        }

                        if (xx < chunkSize && yy < chunkSize)
                            _chunkUnit.SetBlock(new Vector3Int(xx, yy, 0), block, false, BlockLayer.Front);
                    }
                }
            }
        }
        
        public class BlockUnitChunk
        {
            public BlockData Data;

            public BaseBlockMemory Memory;
            public string memStr;
            
            public BlockUnitChunk(BlockData data, BaseBlockMemory memory, string memStr)
            {
                Data = data;
                Memory = memory;
                this.memStr = memStr;
            }
        }
    }
}