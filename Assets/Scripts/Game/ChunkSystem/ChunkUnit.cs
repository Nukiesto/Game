﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum BlockLayer
{
    Front = 0,
    Back = 1
}

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
    private ChunkBuilder _chunkBuilder;

    #endregion

    #region Other

    private static readonly int chunkSize = GameConstants.ChunkSize;
    [HideInInspector] public ChunkManager chunkManager;
    private Vector3 _posObj;

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
        _chunkBuilder = new ChunkBuilder(this, chunkManager);
        _chunkBuilder.GenerateBuild();
        StartCoroutine(ToBuildGrass());
    }

    #endregion

    public IEnumerator ToBuildGrass()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            _chunkBuilder.BuildingGrass();
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
        //posObjGlobal = new Vector3Int(Mathf.FloorToInt(posObj.x), Mathf.FloorToInt(posObj.y), 0);
    }

    #region SetBlock

    //Local
    public bool SetBlock(Vector3Int pos, BlockData data, bool checkCollisions, Tilemap tilemap,
        BlockLayer layer = BlockLayer.Front)
    {
        var hasBlock = checkCollisions && HasBlock(pos, tilemap);
        if (InBounds(pos) && !hasBlock)
            if (data != null)
            {
                tilemap.SetTile(pos, data.tile);
                tilemapFrontWorldLightObstacle.SetTile(pos, data.tile);
                _controller.AddUnit(data, pos, layer);
                return true;
            }

        return false;
    }

    //Global
    public bool SetBlock(Vector3Int pos, BlockData data, bool checkCollisions, BlockLayer layer = BlockLayer.Front)
    {
        var tilemap = GetTileMapOfLayer(layer);
        return SetBlock(pos, data, checkCollisions, tilemap, layer);
    }

    public bool SetBlock(Vector3 pos, BlockData data, bool checkCollisions, BlockLayer layer = BlockLayer.Front)
    {
        var tilemap = GetTileMapOfLayer(layer);
        var posInt = tilemap.WorldToCell(pos);

        return SetBlock(posInt, data, checkCollisions, tilemap);
    }

    #endregion

    #region DeleteBlock

    //Local
    public void DeleteBlock(Vector3Int pos, Tilemap tilemap, BlockLayer layer)
    {
        if (InBounds(pos) && tilemap.GetTile(pos) != null)
        {
            var blockUnit = _controller.GetBlock(pos.x, pos.y, layer);
            if (blockUnit.data.isBreackable)
            {
                //Очистка
                _controller.DeleteBlock(blockUnit);
                tilemap.SetTile(pos, null);
                tilemapFrontWorldLightObstacle.SetTile(pos, null);

                #region CreateItem

                if (blockUnit.data.toCreateItem)
                {
                    var posCreateItem = new Vector3 //Создание предмета в центре блока
                    {
                        x = _posObj.x + Mathf.Floor(pos.x) + 0.5f,
                        y = _posObj.y + Mathf.Floor(pos.y) + 0.5f
                    };
                    //Debug.Log("ItemCreated: " + posCreateItem);
                    ItemManager.CreateItem(posCreateItem, blockUnit.GetItem());
                }
            }

            #endregion
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

    internal class ChunkBuilder
    {
        #region Fields

        private readonly ChunkManager _chunkManager;
        private readonly ChunkUnit _chunkUnit;
        private WorldGenerator _generator;

        private ChunkUnit _chunkUpper;
        private Vector2Int _chunkPos;
        private BlockData[,] _chunkFront = new BlockData[chunkSize, chunkSize];
        private BlockData[,] _chunkBack = new BlockData[chunkSize, chunkSize];

        private int _worldHeight;

        private int _chunkLevel; //Высота от начала мира до начала чанка

        #endregion

        public ChunkBuilder(ChunkUnit chunkUnit, ChunkManager chunkManager)
        {
            _chunkUnit = chunkUnit;
            _chunkManager = chunkManager;
            
            _generator = _chunkManager.generator;
            _worldHeight = _generator.worldHeight;
            _chunkPos = _chunkManager.ChunkPosInWorld(_chunkUnit);

            _chunkLevel = _chunkPos.y * chunkSize; //Высота от начала мира до начала чанка

            var chunkCoord = _chunkManager.ChunkPosInWorld(_chunkUnit);
            _chunkUpper = _chunkManager.GetUpperChunk(chunkCoord);
        }

        #region Main

        public void GenerateChunk()
        {
            var dirt = _generator.dirt;
            var sand = _generator.sand;
            var stone = _generator.stone;
            var ores = _generator.ores;

            var terrainDestruct = 2;
            int surface_level; //Высота от вершины мира до поверхности земли


            //Generating Enviroment
            for (var i = 0; i < chunkSize; i++)
            {
                surface_level = _worldHeight - Random.Range(12, 18) +
                                Random.Range(-terrainDestruct - 1, terrainDestruct + 1);
                for (var j = 0; j < chunkSize; j++)
                {
                    var heightDirt = Random.Range(3, 9); //Толщина земляного покрова

                    if (_chunkLevel + j < surface_level)
                    {
                        _chunkFront[i, j] = stone;
                    }
                    else
                    {
                        if (_chunkLevel + j < surface_level + heightDirt) _chunkFront[i, j] = dirt;
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
        
        private void Rebuild(BlockData[,] chunkFront, BlockData[,] chunkBack)
        {
            _chunkFront = chunkFront;
            _chunkBack = chunkBack;
            Building();
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
            if (chunk_level > worldHeight - 30) //Если уровень чанка идет по уровню земли
            {
                var pos = Vector3Int.zero;
                for (var i = 0; i < chunkSize; i++)
                for (var j = 0; j < chunkSize; j++)
                {
                    pos.x = i;
                    pos.y = j;

                    SetGrass(pos);
                }
            }
        }

        private void SetGrass(Vector3Int pos)
        {
            int i = pos.x, j = pos.y;
            if (_chunkFront[i, j] == _generator.dirt)
            {
                if (_chunkLevel + chunkSize == _worldHeight && j == chunkSize) //Если блок под вершиной мира
                {
                    _chunkUnit.tilemapFrontWorld.SetTile(pos, _chunkFront[i, j].tileVariables[0]);
                    return;
                }

                if (j + 1 <= chunkSize - 1 && _chunkFront[i, j + 1] == null) //Если над блоком пусто
                {
                    _chunkUnit.tilemapFrontWorld.SetTile(pos, _chunkFront[i, j].tileVariables[0]);
                    return;
                }

                if (j == chunkSize - 1 && _chunkUpper != null) //Если блок под низом другого чанка
                    if (!_chunkUpper.HasBlock(new Vector3Int(i, 0, 0)))
                    {
                        _chunkUnit.tilemapFrontWorld.SetTile(pos, _chunkFront[i, j].tileVariables[0]);
                    }
            }
        }

        #endregion
    }
}