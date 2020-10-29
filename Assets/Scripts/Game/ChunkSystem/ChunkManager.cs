using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Game.Bot;
using JetBrains.Annotations;
using LeopotamGroup.Math;
using SavingSystem;
using Singleton;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private GameObject chunk;
    [SerializeField] public DataBase dataBase;
    [SerializeField] private PlayerController player;
    [SerializeField] private Camera cameraMain;

    private ChunkUnit[,] _chunks;
    private Vector3 _posObj;
    private Vector2Int _posZero;

    private Bounds _bounds;

    private int _chunkSize;
    public WorldGenerator generator;
    //private WorldSavingSystem.WorldSaving _worldSaving;
    public static ChunkManager Instance;
    
    private void Awake()
    {
        Instance = this;
        _chunkSize = GameConstants.ChunkSize;
        
        RefreshPos();

        generator.posZeroWorld = _posZero;
        generator.InitProps();

        RefreshBounds();
        
        BuildChunks();
    }

    private void Start()
    {
        MovePlayerToSpawnPoint();
        StartCoroutine(BuildLimiters());
    }

    private IEnumerator BuildLimiters()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            //Debug.Log("Started");
            var dataLimiter = dataBase.GetBlock("Limiter");
            var w = generator.worldWidthInChunks;
            var h = generator.worldHeightInChunks;
            for (var i1 = 0; i1 < w; i1++)
            {
                for (var j1 = 0; j1 < h; j1++)
                {
                    var chunk = _chunks[i1, j1];
                    if (j1 == 0)
                    {
                        for (var i = 0; i < _chunkSize; i++)
                        {
                            var pos = new Vector3Int(i, 0, 0);
                            chunk.DeleteBlock(pos, BlockLayer.Front, false);
                            chunk.SetBlock(pos , dataLimiter, false);
                        }
                    }
                    if (j1 == h-1)
                    {
                        for (var i = 0; i < _chunkSize; i++)
                        {
                            var pos = new Vector3Int(i, _chunkSize - 1, 0);
                            chunk.DeleteBlock(pos, BlockLayer.Front, false);
                            chunk.SetBlock(pos , dataLimiter, false);
                        }
                    }
                    if (i1 == 0)
                    {
                        for (var i = 0; i < _chunkSize; i++)
                        {
                            var pos = new Vector3Int(0, i, 0);
                            chunk.DeleteBlock(pos, BlockLayer.Front, false);
                            chunk.SetBlock(pos, dataLimiter, false);
                        }
                    }
                    if (i1 == w-1)
                    {
                        //Debug.Log("Good");
                        for (var i = 0; i < _chunkSize; i++)
                        {
                            var pos = new Vector3Int(_chunkSize - 1, i, 0);
                            chunk.DeleteBlock(pos, BlockLayer.Front, false);
                            chunk.SetBlock(pos, dataLimiter, false);
                        }
                    }
                }
            }
            yield break;
        }
    }
    public void CreateTestEntity()
    {
        var pos = player.transform.position;
        Toolbox.Instance.mEntityManager.Create(pos, EntityType.TestBot);
    }
    public void MovePlayerToSpawnPoint()
    {
        var pos = Vector3.zero;
        pos.x = _posZero.x + generator.worldWidth / 2;
        pos.y = _posZero.y + generator.worldHeight - 4;
        player.transform.position = pos;
        pos.z = -10;
        cameraMain.transform.position = pos;
    }

    private void RefreshPos()
    {
        _posObj = transform.position;
        _posZero = new Vector2Int(Mathf.FloorToInt(_posObj.x), Mathf.FloorToInt(_posObj.y));
    }

    private void RefreshBounds()
    {
        var a = (float) generator.worldWidth / 2;
        var b = (float) generator.worldHeight / 2;

        var center = new Vector3(_posObj.x + a, _posObj.y + b);
        var size = new Vector3(a * 2, b * 2);

        _bounds = new Bounds(center, size);
        //Debug.Log(posObj);
        //Debug.Log(bounds + " ;Center: " + center + " ;size: " + size);
        //Debug.Log(a + ";" + b);
        //Debug.Log(generator.worldWidth);
        //Debug.Log("min: " + bounds.min + " ;max: " + bounds.max);
    }

    private void BuildChunks()
    {
        _chunks = new ChunkUnit[generator.worldWidthInChunks, generator.worldHeightInChunks];
        var posZero = transform.position;
        GameObject chunkObj;
        ChunkUnit chunkUnit;

        //float N = width * height;
        var n = 0;
        for (var i = 0; i < generator.worldWidthInChunks; i++)
        for (var j = 0; j < generator.worldHeightInChunks; j++)
        {
            chunkObj = Instantiate(chunk);
            chunkObj.transform.parent = transform;
            chunkObj.transform.position = posZero + new Vector3(i * _chunkSize, j * _chunkSize);
            chunkObj.name = "Chunk(" + i + ", " + j + ")";
            //Debug.Log(new Vector3(i * chunkSize, j * chunkSize));

            chunkUnit = chunkObj.GetComponent<ChunkUnit>();
            chunkUnit.chunkManager = this;
            chunkUnit.ToGenerate = true;
            _chunks[i, j] = chunkUnit;
            n++;
            //progress.text = n / N * 100 + "%";
            //Debug.Log("World Generated: " + n / N * 100 + "%; " + n);
        }
    }

    public ChunkUnit GetChunk(Vector3 pos)
    {
        if (InBounds(pos))
        {
            var posInt = new Vector2Int(MathFast.FloorToInt(pos.x), MathFast.FloorToInt(pos.y));

            var pointPos = _posZero - posInt;
            int i = Mathf.Abs(Mathf.FloorToInt(pointPos.x / _chunkSize)),
                j = Mathf.Abs(Mathf.FloorToInt(pointPos.y / _chunkSize));

            //Debug.Log("GetChunk Array Pos: " + i + ", " + j + " ;GetChunk Pos: " + posInt);

            return _chunks[i, j];
        }

        return null;
    }

    public Vector2Int ChunkPosInWorld(ChunkUnit unit)
    {
        for (var i = 0; i < generator.worldWidthInChunks; i++)
        for (var j = 0; j < generator.worldHeightInChunks; j++)
            if (_chunks[i, j] == unit)
                return new Vector2Int(i, j);
        return new Vector2Int(-1, -1);
    }

    public bool InBounds(Vector3 pos)
    {
        //Vector3 point = new Vector3(pos.x, pos.y);
        //Debug.Log("Point: " + point + " ;InBounds: " + bounds.Contains(point));
        //return pos.x >= posObj.x && pos.x < chunkSize * generator.worldWidth && pos.y >= posObj.y && pos.y < chunkSize * generator.worldHeight;
        return _bounds.Contains(pos);
    }

    public void AddDataToWorldSaving(WorldSavingSystem.WorldSaving worldSaving)
    {
        worldSaving.Clear();
        worldSaving.WorldDataUnit.toGenerateWorld = false;

        for (var i = 0; i < generator.worldWidthInChunks; i++)
        for (var j = 0; j < generator.worldHeightInChunks; j++)
        {
            var chunk = new WorldSavingSystem.ChunkData(i, j);
            var unit = _chunks[i, j];
            //Debug.Log("ChunkData: " + chunk.x + " ;" + chunk.y + " ;ChunkUnit: " + i + " ;"+ j);
            for (var x = 0; x < _chunkSize; x++)
            for (var y = 0; y < _chunkSize; y++)
            {
                var blockUnitFront = unit.GetBlockUnit(new Vector2Int(x, y), BlockLayer.Front);
                var blockUnitBack  = unit.GetBlockUnit(new Vector2Int(x, y), BlockLayer.Back);

                //Debug.Log("BlockUnitFront: " + blockUnitFront + ";BlockUnitBack: " + blockUnitBack);
                if (blockUnitFront != null)
                {
                    BaseBlockMemory memUnit = null;
                    if (blockUnitFront.Memory != null)
                    {
                        memUnit = blockUnitFront.Memory.GetMemoryUnit();
                    }

                    //if (memUnit != null)
                    //{
                        //Debug.Log("MemUnitSave: " + memUnit);
                    //}
                    chunk.AddChunkBlock(new WorldSavingSystem.BlockChunkData(x, y, blockUnitFront.Data.nameBlock,
                        (int) BlockLayer.Front, memUnit));
                }

                if (blockUnitBack != null)
                {
                    BaseBlockMemory memUnit1 = null;
                    if (blockUnitBack.Memory != null)
                    {
                        memUnit1 = blockUnitBack.Memory.GetMemoryUnit();
                    }
                    chunk.AddChunkBlock(new WorldSavingSystem.BlockChunkData(x, y, blockUnitBack.Data.nameBlock, 
                        (int) BlockLayer.Back, memUnit1));
                }
            }

            //Debug.Log(chunk.blocks.Count);                
            worldSaving.AddChunk(chunk);
        }

       
    }
    public void LoadWorld(WorldSavingSystem.WorldSaving worldSaving)
    {
        var count = generator.CountChunks;
        var toolbox = Toolbox.Instance;
        var entityManager = toolbox.mEntityManager;
        var itemManager = toolbox.mItemManager;
        for (var i = 0; i < count; i++)
        {
            var chunkLoaded = worldSaving.GetChunkData(i);
            //Debug.Log("ChunkData: " + chunk.x + " ;" + chunk.y + " ;ChunkUnit: " + i + " ;" + j);
            if (chunkLoaded != null)
            {
                var entities = chunkLoaded.entities;
                //Debug.Log("EntitiesCount: " + entities.Count);
                for (var j = 0; j < entities.Count; j++)
                {
                    var entity = entities[j];
                    entityManager.Create(new Vector3(entity.X, entity.Y, 0), entity.EntityType);
                }
                var items = chunkLoaded.items;
                //Debug.Log("ItemsCount: " + items.Count);
                for (var j = 0; j < items.Count; j++)
                {
                    var item = items[j];
                    
                    itemManager.CreateItem(new Vector3(item.X, item.Y, 0), dataBase.GetItem(item.Name));
                }
                //Debug.Log("ItemsCount: " + items.Count);
                //Debug.Log("x" + chunkLoaded.x + " ;y" + chunkLoaded.y);
                var unit = _chunks[chunkLoaded.x, chunkLoaded.y];
                //unit.Clear(); //Полная очистка чанка
                unit.ToGenerate = false;
                var chunkFront = new ChunkUnit.ChunkBuilder.BlockUnitChunk[_chunkSize,_chunkSize];
                var chunkBack  = new ChunkUnit.ChunkBuilder.BlockUnitChunk[_chunkSize,_chunkSize];
                for (var n = 0; n < chunkLoaded.blocks.Count; n++)
                {
                    var blockData = chunkLoaded.blocks[n];
                    if (blockData.blockLayer == (int) BlockLayer.Front)
                    {
                        var blockDataMain = dataBase.GetBlock(blockData.name);
                        if (blockData.memory != null)
                        {
                            var mem = blockData.memory as ChestMemory;
                        }
                        chunkFront[blockData.x, blockData.y] = new ChunkUnit.ChunkBuilder.BlockUnitChunk(blockDataMain, blockData.memory, blockData.memStr);
                    }

                    if (blockData.blockLayer == (int) BlockLayer.Back)
                    {
                        var blockDataMain = dataBase.GetBlock(blockData.name);
                        chunkBack[blockData.x, blockData.y] = new ChunkUnit.ChunkBuilder.BlockUnitChunk(blockDataMain, blockData.memory, blockData.memStr);
                    }
                }
                unit.chunkBuilder = new ChunkUnit.ChunkBuilder(unit, this);
                unit.chunkBuilder.Rebuild(chunkFront, chunkBack);
            }
        }
    }

    public ChunkUnit GetUpperChunk(Vector2Int pos)
    {
        //Debug.Log((pos.y + 1) + " ;" + generator.worldHeightInChunks);
        if (pos.y + 1 <= generator.worldHeightInChunks - 1) //Если чанк не на вершине
            return _chunks[pos.x, pos.y + 1];
        return null;
    }
    public ChunkUnit GetDownerChunk(Vector2Int pos)
    {
        //Debug.Log((pos.y + 1) + " ;" + generator.worldHeightInChunks);
        if (pos.y - 1 > 0) //Если чанк не самый низкий
            return _chunks[pos.x, pos.y - 1];
        return null;
    }
    #region Debugging

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        int sizeX = generator.worldWidth;
        int sizeY = generator.worldHeight;

        Gizmos.color = Color.green;

        Gizmos.DrawLine(pos, pos + new Vector3(sizeX, 0));
        Gizmos.DrawLine(pos + new Vector3(0, sizeY), pos + new Vector3(sizeX, sizeY));
        Gizmos.DrawLine(pos, pos + new Vector3(0, sizeY));
        Gizmos.DrawLine(pos + new Vector3(sizeX, 0), pos + new Vector3(sizeX, sizeY));
    }
#endif

    #endregion
}

[Serializable]
public class WorldGenerator
{
    public int heightSurface;

    public int dirtLevelHeightMin;
    public int dirtLevelHeightMax;

    public int worldWidthInChunks;
    public int worldHeightInChunks;

    public BlockData dirt;
    public BlockData sand;
    public BlockData stone;
    public BlockData[] ores;

    [HideInInspector] public int endSurface;
    [HideInInspector] public int worldWidth;
    [HideInInspector] public int worldHeight;

    [HideInInspector] public Vector2Int posZeroWorld;
    private int _chunkSize;

    private BlockData[,] _frontWorld; //Передний мир
    private BlockData[,] _backWorld; //Задний мир

    public int CountChunks => worldWidthInChunks * worldHeightInChunks;

    public void InitProps()
    {
        _chunkSize = GameConstants.ChunkSize;

        worldWidth = worldWidthInChunks * _chunkSize;
        worldHeight = worldHeightInChunks * _chunkSize;
        endSurface = worldHeight - heightSurface;
    }

    public void Generation()
    {
        var terrainDestruct = 2;

        _frontWorld = new BlockData[worldWidth, worldHeight]; //Передний мир
        _backWorld = new BlockData[worldWidth, worldHeight]; //Задний мир

        int meter;
        var a = Random.Range(12, 18);
        for (var i = 0; i < worldWidth; i++)
        {
            for (meter = a; meter < worldHeight; meter++)
                if (meter < Random.Range(19, 26))
                    _frontWorld[i, meter] = dirt;
                else
                    _frontWorld[i, meter] = stone;
            a = Random.Range(12, 18) + Random.Range(-terrainDestruct - 1, terrainDestruct + 1);
        }

        for (var i = 1; i < worldWidth - 1; i += 1)
        for (var j = 1; j < worldHeight - 1; j += 1)
            if (_frontWorld[i, j] == dirt)
                if (_frontWorld[i - 1, j] == null
                    && _frontWorld[i + 1, j] == null
                    && _frontWorld[i, j - 1] == null)
                    _frontWorld[i, j] = null;

        GenerateDungeon(20, 30, Random.Range(10, 30), _frontWorld);
        GenerateOre(90, 25, Random.Range(2, 9), _frontWorld, ores[0]); //coal
        GenerateOre(60, 30, Random.Range(2, 9), _frontWorld, ores[1]); //iron

        MirrorWorld();
    }

    private void MirrorWorld()
    {
        var mirrored = new BlockData[worldWidth, worldHeight];
        for (var i = 1; i < worldHeight - 1; i += 1)
        for (var j = 1; j < worldWidth - 1; j += 1)
            if (worldHeight - j > 0)
                mirrored[i, j] = _frontWorld[i, worldHeight - j];
        _frontWorld = mirrored;
    }

    private void GenerateDungeon(int n, int startHeight, int size, BlockData[,] world)
    {
        for (var i = 0; i < n; i += 1) //Количество пещер
        {
            var xx = Random.Range(0, worldWidth);
            var yy = Random.Range(startHeight, worldHeight); //Глубина, ниже которой начнут генерироваться пещеры
            for (var j = 0; j < size; j += 1) //Размер одной пещеры
            {
                var rr = Random.Range(0, 4);
                if (rr == 0) xx = Mathf.Min(xx + 1, worldWidth);
                if (rr == 1) xx = Mathf.Max(xx - 1, 0);
                if (rr == 2) yy = Mathf.Min(yy + 1, worldHeight);
                if (rr == 3) yy = Mathf.Max(yy - 1, 0);
                if ((xx < 0 || xx > worldWidth)
                    && yy < 0 || yy > startHeight)
                {
                    xx = Random.Range(0, worldWidth);
                    yy = Random.Range(startHeight, worldHeight);
                }

                //Debug.Log("x: " + xx + " ;y:" + yy);
                if (xx < worldWidth && yy < worldHeight)
                    world[xx, yy] = null;
            }
        }
    }

    private void GenerateOre(int n, int startHeight, int size, BlockData[,] world, BlockData block)
    {
        for (var i = 0; i < n; i += 1) //Количество залежей руды
        {
            var xx = Random.Range(0, worldWidth);
            var yy = Random.Range(startHeight, worldHeight); //Глубина, ниже которой начнут генерироваться руда
            for (var j = 0; j < size; j += 1) //Кол-во блоков в одной жиле
            {
                var rr = Random.Range(0, 4);
                if (rr == 0) xx = Mathf.Min(xx + 1, worldWidth);
                if (rr == 1) xx = Mathf.Max(xx - 1, 0);
                if (rr == 2) yy = Mathf.Min(yy + 1, startHeight);
                if (rr == 3) yy = Mathf.Max(yy - 1, 0);
                if ((xx < 0 || xx > worldWidth) && yy < 0 || yy > worldHeight)
                {
                    xx = Random.Range(0, worldWidth);
                    yy = Random.Range(startHeight, worldHeight);
                }

                if (xx < worldWidth && yy < worldHeight)
                    world[xx, yy] = block;
            }
        }
    }

    public BlockData GetBlock(Vector2Int pos)
    {
        pos -= posZeroWorld;
        //Debug.Log(pos);
        var data = _frontWorld[pos.x, pos.y];

        return data;
    }
}