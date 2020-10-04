using LeopotamGroup.Math;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private GameObject chunk;
    
    private ChunkUnit[,] chunks;
    private Vector3 posObj;
    private Vector2Int posZero;

    private Bounds bounds;

    private int chunkSize;
    public WorldGenerator generator;

    private void Awake()
    {
        chunkSize = GameConstants.chunkSize;

        RefreshPos();
        

        generator.posZeroWorld = posZero;
        generator.InitProps();

        RefreshBounds();

        generator.Generation();
        BuildChunks();                    
    }
    private void RefreshPos()
    {
        posObj = transform.position;
        posZero = new Vector2Int(Mathf.FloorToInt(posObj.x), Mathf.FloorToInt(posObj.y));
    }
    private void RefreshBounds()
    {
        float a = (float)generator.worldWidth / 2;
        float b = (float)generator.worldHeight / 2;

        Vector3 center = new Vector3(posObj.x + a, posObj.y + b);
        Vector3 size = new Vector3(a*2, b*2);

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
        Vector3 posZero = transform.position;
        GameObject chunkObj;
        ChunkUnit chunkUnit;

        //float N = width * height;
        int n = 0;
        for (int i = 0; i < generator.worldWidthInChunks; i++)
        {
            for (int j = 0; j < generator.worldHeightInChunks; j++)
            {               
                chunkObj = Instantiate(chunk);
                chunkObj.transform.parent = transform;
                chunkObj.transform.position = posZero + new Vector3(i * chunkSize, j* chunkSize);
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
    }
    public ChunkUnit GetChunk(Vector3 pos)
    {       
        if (InBounds(pos))
        {
            Vector2Int posInt = new Vector2Int(MathFast.FloorToInt(pos.x), MathFast.FloorToInt(pos.y));

            Vector2Int pointPos = posZero - posInt;
            int i = Mathf.Abs(Mathf.FloorToInt(pointPos.x / chunkSize)),
                j = Mathf.Abs(Mathf.FloorToInt(pointPos.y / chunkSize));

            //Debug.Log("GetChunk Array Pos: " + i + ", " + j + " ;GetChunk Pos: " + posInt);

            return chunks[i, j];
        }

        return null;       
    }
    public bool InBounds(Vector3 pos)
    {
        //Vector3 point = new Vector3(pos.x, pos.y);
        //Debug.Log("Point: " + point + " ;InBounds: " + bounds.Contains(point));
        //return pos.x >= posObj.x && pos.x < chunkSize * generator.worldWidth && pos.y >= posObj.y && pos.y < chunkSize * generator.worldHeight;
        return bounds.Contains(pos);
    }
    //private void Moving(int v, int h)
    //{
    //    Vector3 pos = transform.position;

    //    pos.x += v * chunkSize;
    //    pos.y += h * chunkSize;

    //    transform.position = pos;
    //}
    //public void BecameInvisible(ChunkUnit chunk)
    //{        
    //    chunksUnloaded.Add(chunk);
    //    Debug.Log("Reported" + chunksUnloaded.Count);
    //    if (chunksUnloaded.Count == 4)
    //    {
    //        Moving(1, 0);
    //        for (int i = 0; i < 4; i++)
    //        {
    //            chunksUnloaded[i].SetActive(true);
    //            chunksUnloaded.RemoveAt(i);
    //        }
            
    //    }      
    //}
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

    private BlockData[,] frontWorld;//Передний мир
    private BlockData[,] backWorld;//Задний мир

    public void InitProps()
    {
        chunkSize = GameConstants.chunkSize;

        worldWidth = worldWidthInChunks * chunkSize;
        worldHeight = worldHeightInChunks * chunkSize;
        endSurface = worldHeight - heightSurface;     
    }    

    public void Generation()
    {
        int terrainDestruct = 2;

        frontWorld = new BlockData[worldWidth, worldHeight];//Передний мир
        backWorld  = new BlockData[worldWidth, worldHeight];//Задний мир

        int meter;
        int a = Random.Range(12, 18);
        for (int i = 0; i < worldWidth; i++)
        {           
            for (meter = a; meter < worldHeight; meter++)
            {
                if (meter < Random.Range(19, 26))
                {
                    frontWorld[i, meter] = dirt;
                }
                else
                {
                    frontWorld[i, meter] = stone;                                     
                }
            }
            a = Random.Range(12, 18) + Random.Range(-terrainDestruct - 1, terrainDestruct + 1);
        }
        for (int i = 1; i < worldWidth - 1; i += 1)
        {
            for (int j = 1; j < worldHeight - 1; j += 1)
            {
                if (frontWorld[i, j] == dirt)
                {
                    if ((frontWorld[i - 1, j] == null)
                    && (frontWorld[i + 1, j] == null)
                    && (frontWorld[i, j - 1] == null))
                    {
                        frontWorld[i, j] = null;
                    }
                }
            }
        }       

        GenerateDungeon(20, 30, Random.Range(10, 30), frontWorld);
        GenerateOre(90, 25, Random.Range(2, 9), frontWorld, ores[0]);//coal
        GenerateOre(60, 30, Random.Range(2, 9), frontWorld, ores[1]);//iron

        MirrorWorld();
    }
    private void MirrorWorld()
    {
        BlockData[,] mirrored = new BlockData[worldWidth, worldHeight];
        for (int i = 1; i < worldHeight - 1; i += 1)
        {
            for (int j = 1; j < worldWidth - 1; j += 1)
            {
                if (worldHeight - j > 0)
                {
                    mirrored[i, j] = frontWorld[i, worldHeight - j];
                }
            }
        }
        frontWorld = mirrored;
    }
    private void GenerateDungeon(int n, int startHeight, int size, BlockData[,] world)
    {
        for (int i = 0; i < n; i += 1) //Количество пещер
        {
            int xx = Random.Range(0, worldWidth);
            int yy = Random.Range(startHeight, worldHeight); //Глубина, ниже которой начнут генерироваться пещеры
            for (int j = 0; j < size; j += 1) //Размер одной пещеры
            {
                int rr = Random.Range(0, 4);
                if (rr == 0) {xx = Mathf.Min(xx + 1, worldWidth);}
                if (rr == 1) { xx = Mathf.Max(xx - 1, 0); }
                if (rr == 2) { yy = Mathf.Min(yy + 1, worldHeight); }
                if (rr == 3) { yy = Mathf.Max(yy - 1, 0); }
                if (((xx < 0) || (xx > worldWidth))
                && (yy < 0) || (yy > startHeight))
                {
                    xx = Random.Range(0, worldWidth);
                    yy = Random.Range(startHeight, worldHeight);
                }
                Debug.Log("x: " + xx + " ;y:" + yy);
                if (xx < worldWidth && yy < worldHeight) 
                    world[xx, yy] = null;
            }
        }
    }   
    private void GenerateOre(int n, int startHeight, int size, BlockData[,] world, BlockData block)
    {
        for (var i = 0; i < n; i += 1) //Количество залежей руды
        {
            int xx = Random.Range(0, worldWidth);
            int yy = Random.Range(startHeight, worldHeight); //Глубина, ниже которой начнут генерироваться руда
            for (var j = 0; j < size; j += 1) //Кол-во блоков в одной жиле
            {
                int rr = Random.Range(0, 4);
                if (rr == 0){ xx = Mathf.Min(xx + 1, worldWidth); }
                if (rr == 1) { xx = Mathf.Max(xx - 1, 0); }
                if (rr == 2) { yy = Mathf.Min(yy + 1, startHeight); }
                if (rr == 3) { yy = Mathf.Max(yy - 1, 0); }
                if (((xx < 0) || (xx > worldWidth)) && (yy < 0) || (yy > worldHeight))       
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
        BlockData data = frontWorld[pos.x, pos.y]; 

        return data;
    }
}