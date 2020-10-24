using System;
using LeopotamGroup.Math;
using SavingSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private GameObject chunk;
    [SerializeField] private BlockDataBase dataBase;
    [SerializeField] private PlayerController player;
    [SerializeField] private Camera cameraMain;

    private ChunkUnit[,] chunks;
    private Vector3 posObj;
    private Vector2Int posZero;

    private Bounds bounds;

    private int chunkSize;
    public WorldGenerator generator;
    private WorldSavingSystem.WorldSaving worldSaving;

    private void Awake()
    {
        chunkSize = GameConstants.ChunkSize;

        RefreshPos();

        generator.posZeroWorld = posZero;
        generator.InitProps();

        RefreshBounds();

        BuildChunks();
        CreateWorld();
    }

    private void Start()
    {
        MovePlayerToSpawnPoint();
    }

    public void MovePlayerToSpawnPoint()
    {
        var pos = Vector3.zero;
        pos.x = posZero.x + generator.worldWidth / 2;
        pos.y = posZero.y + generator.worldHeight - 4;
        player.transform.position = pos;
        pos.z = -10;
        cameraMain.transform.position = pos;
    }

    private void RefreshPos()
    {
        posObj = transform.position;
        posZero = new Vector2Int(Mathf.FloorToInt(posObj.x), Mathf.FloorToInt(posObj.y));
    }

    private void RefreshBounds()
    {
        var a = (float) generator.worldWidth / 2;
        var b = (float) generator.worldHeight / 2;

        var center = new Vector3(posObj.x + a, posObj.y + b);
        var size = new Vector3(a * 2, b * 2);

        bounds = new Bounds(center, size);
        //Debug.Log(posObj);
        //Debug.Log(bounds + " ;Center: " + center + " ;size: " + size);
        //Debug.Log(a + ";" + b);
        //Debug.Log(generator.worldWidth);
        //Debug.Log("min: " + bounds.min + " ;max: " + bounds.max);
    }

    private void BuildChunks()
    {
        chunks = new ChunkUnit[generator.worldWidthInChunks, generator.worldHeightInChunks];
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
            chunkObj.transform.position = posZero + new Vector3(i * chunkSize, j * chunkSize);
            chunkObj.name = "Chunk(" + i + ", " + j + ")";
            //Debug.Log(new Vector3(i * chunkSize, j * chunkSize));

            chunkUnit = chunkObj.GetComponent<ChunkUnit>();
            chunkUnit.chunkManager = this;
            chunks[i, j] = chunkUnit;

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

            var pointPos = posZero - posInt;
            int i = Mathf.Abs(Mathf.FloorToInt(pointPos.x / chunkSize)),
                j = Mathf.Abs(Mathf.FloorToInt(pointPos.y / chunkSize));

            //Debug.Log("GetChunk Array Pos: " + i + ", " + j + " ;GetChunk Pos: " + posInt);

            return chunks[i, j];
        }

        return null;
    }

    public Vector2Int ChunkPosInWorld(ChunkUnit unit)
    {
        for (var i = 0; i < generator.worldWidthInChunks; i++)
        for (var j = 0; j < generator.worldHeightInChunks; j++)
            if (chunks[i, j] == unit)
                return new Vector2Int(i, j);
        return new Vector2Int(-1, -1);
    }

    public bool InBounds(Vector3 pos)
    {
        //Vector3 point = new Vector3(pos.x, pos.y);
        //Debug.Log("Point: " + point + " ;InBounds: " + bounds.Contains(point));
        //return pos.x >= posObj.x && pos.x < chunkSize * generator.worldWidth && pos.y >= posObj.y && pos.y < chunkSize * generator.worldHeight;
        return bounds.Contains(pos);
    }

    public void CreateWorld()
    {
        WorldSavingSystem.Init();

        if (WorldSavingSystem.worldsList == null)
        {
            WorldSavingSystem.worldsList = new WorldSavingSystem.WorldDataList();
        }
        
        worldSaving = new WorldSavingSystem.WorldSaving(WorldSavingSystem.worldsList);
        var name = "TestWorldSave";
        var world = new WorldSavingSystem.WorldDataUnit
        {
            name = name,
            width = generator.worldWidthInChunks,
            height = generator.worldHeightInChunks
        };
        if (!worldSaving.LoadWorldName(name)) worldSaving.CreateWorld(world);
    }

    public void ClickSaveWorld()
    {
        worldSaving.Clear();
        for (var i = 0; i < generator.worldWidthInChunks; i++)
        for (var j = 0; j < generator.worldHeightInChunks; j++)
        {
            var chunk = new WorldSavingSystem.ChunkData(i, j);
            var unit = chunks[i, j];
            //Debug.Log("ChunkData: " + chunk.x + " ;" + chunk.y + " ;ChunkUnit: " + i + " ;"+ j);
            for (var x = 0; x < chunkSize; x++)
            for (var y = 0; y < chunkSize; y++)
            {
                var blockUnitFront = unit.GetBlockUnit(new Vector2Int(x, y), BlockLayer.Front);
                var blockUnitBack = unit.GetBlockUnit(new Vector2Int(x, y), BlockLayer.Back);

                //Debug.Log("BlockUnitFront: " + blockUnitFront + ";BlockUnitBack: " + blockUnitBack);
                if (blockUnitFront != null)
                    chunk.AddChunkBlock(new WorldSavingSystem.BlockChunkData(x, y, blockUnitFront.Data.nameBlock,
                        (int) BlockLayer.Front));
                if (blockUnitBack != null)
                    chunk.AddChunkBlock(new WorldSavingSystem.BlockChunkData(x, y, blockUnitBack.Data.nameBlock,
                        (int) BlockLayer.Back));
            }

            //Debug.Log(chunk.blocks.Count);                
            worldSaving.AddChunk(chunk);
        }

        worldSaving.SaveWorld();
    }

    public void ClickLoadWorld()
    {
        worldSaving.LoadWorld();
        var count = generator.CountChunks;
        for (var i = 0; i < count; i++)
        {
            var chunk = worldSaving.GetChunkData(i);
            //Debug.Log("ChunkData: " + chunk.x + " ;" + chunk.y + " ;ChunkUnit: " + i + " ;" + j);
            if (chunk != null)
            {
                var unit = chunks[chunk.x, chunk.y];
                unit.Clear(); //Полная очистка чанка

                for (var n = 0; n < chunk.blocks.Count; n++)
                {
                    var blockData = chunk.blocks[n];
                    if (blockData.blockLayer == (int) BlockLayer.Front)
                    {
                        var blockDataMain = dataBase.GetBlock(blockData.name);

                        unit.SetBlock(new Vector3Int(blockData.x, blockData.y, 0), blockDataMain, false, BlockLayer.Back, false);
                    }

                    if (blockData.blockLayer == (int) BlockLayer.Back)
                    {
                        var blockDataMain = dataBase.GetBlock(blockData.name);

                        unit.SetBlock(new Vector3Int(blockData.x, blockData.y, 0), blockDataMain, false,
                            BlockLayer.Back);
                    }

                    unit.StartCoroutine(unit.ToBuildGrass());
                }
            }
        }
    }

    public ChunkUnit GetUpperChunk(Vector2Int pos)
    {
        //Debug.Log((pos.y + 1) + " ;" + generator.worldHeightInChunks);
        if (pos.y + 1 <= generator.worldHeightInChunks - 1) //Если чанк не на вершине
            return chunks[pos.x, pos.y + 1];
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
    private int chunkSize;

    private BlockData[,] frontWorld; //Передний мир
    private BlockData[,] backWorld; //Задний мир

    public int CountChunks => worldWidthInChunks * worldHeightInChunks;

    public void InitProps()
    {
        chunkSize = GameConstants.ChunkSize;

        worldWidth = worldWidthInChunks * chunkSize;
        worldHeight = worldHeightInChunks * chunkSize;
        endSurface = worldHeight - heightSurface;
    }

    public void Generation()
    {
        var terrainDestruct = 2;

        frontWorld = new BlockData[worldWidth, worldHeight]; //Передний мир
        backWorld = new BlockData[worldWidth, worldHeight]; //Задний мир

        int meter;
        var a = Random.Range(12, 18);
        for (var i = 0; i < worldWidth; i++)
        {
            for (meter = a; meter < worldHeight; meter++)
                if (meter < Random.Range(19, 26))
                    frontWorld[i, meter] = dirt;
                else
                    frontWorld[i, meter] = stone;
            a = Random.Range(12, 18) + Random.Range(-terrainDestruct - 1, terrainDestruct + 1);
        }

        for (var i = 1; i < worldWidth - 1; i += 1)
        for (var j = 1; j < worldHeight - 1; j += 1)
            if (frontWorld[i, j] == dirt)
                if (frontWorld[i - 1, j] == null
                    && frontWorld[i + 1, j] == null
                    && frontWorld[i, j - 1] == null)
                    frontWorld[i, j] = null;

        GenerateDungeon(20, 30, Random.Range(10, 30), frontWorld);
        GenerateOre(90, 25, Random.Range(2, 9), frontWorld, ores[0]); //coal
        GenerateOre(60, 30, Random.Range(2, 9), frontWorld, ores[1]); //iron

        MirrorWorld();
    }

    private void MirrorWorld()
    {
        var mirrored = new BlockData[worldWidth, worldHeight];
        for (var i = 1; i < worldHeight - 1; i += 1)
        for (var j = 1; j < worldWidth - 1; j += 1)
            if (worldHeight - j > 0)
                mirrored[i, j] = frontWorld[i, worldHeight - j];
        frontWorld = mirrored;
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
        var data = frontWorld[pos.x, pos.y];

        return data;
    }
}