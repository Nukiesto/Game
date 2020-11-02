using System;
using System.Collections;
using LeopotamGroup.Math;
using SavingSystem;
using Singleton;
using UnityEngine;

namespace Game.ChunkSystem
{
    public class ChunkManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameObject chunk;
        [SerializeField] public DataBase dataBase;
        [SerializeField] private PlayerController player;
        [SerializeField] private Camera cameraMain;

        private ChunkUnit[,] _chunks;
        private Vector3 _posObj;
        [HideInInspector]public Vector2Int posZero;

        private Bounds _bounds;

        private int _chunkSize;
        public WorldGenerator generator;
        public static ChunkManager Instance;

        #endregion
    
        private void Awake()
        {
            Instance = this;
            _chunkSize = GameConstants.ChunkSize;
        
            _posObj = transform.position;
            posZero = new Vector2Int(Mathf.FloorToInt(_posObj.x), Mathf.FloorToInt(_posObj.y));
        
            generator.InitProps();
            RefreshBounds();
        
            var saver = Toolbox.Instance.mWorldSaver;
            saver.OnLoadEvent += LoadWorld;
            saver.OnSaveEvent += AddDataToWorldSaving;
        
            BuildChunks();
        }

        private void Start()
        {
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
                        var chunkUnit = _chunks[i1, j1];
                        if (j1 == 0)
                        {
                            for (var i = 0; i < _chunkSize; i++)
                            {
                                var pos = new Vector3Int(i, 0, 0);
                                chunkUnit.DeleteBlock(pos, BlockLayer.Front, false);
                                chunkUnit.SetBlock(pos , dataLimiter, false);
                            }
                        }
                        if (j1 == h-1)
                        {
                            for (var i = 0; i < _chunkSize; i++)
                            {
                                var pos = new Vector3Int(i, _chunkSize - 1, 0);
                                chunkUnit.DeleteBlock(pos, BlockLayer.Front, false);
                                chunkUnit.SetBlock(pos , dataLimiter, false);
                            }
                        }
                        if (i1 == 0)
                        {
                            for (var i = 0; i < _chunkSize; i++)
                            {
                                var pos = new Vector3Int(0, i, 0);
                                chunkUnit.DeleteBlock(pos, BlockLayer.Front, false);
                                chunkUnit.SetBlock(pos, dataLimiter, false);
                            }
                        }

                        if (i1 != w - 1) continue;
                        {
                            //Debug.Log("Good");
                            for (var i = 0; i < _chunkSize; i++)
                            {
                                var pos = new Vector3Int(_chunkSize - 1, i, 0);
                                chunkUnit.DeleteBlock(pos, BlockLayer.Front, false);
                                chunkUnit.SetBlock(pos, dataLimiter, false);
                            }
                        }
                    }
                }
                yield break;
            }
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
            var posObj = transform.position;

            //float N = width * height;
            for (var i = 0; i < generator.worldWidthInChunks; i++)
            for (var j = 0; j < generator.worldHeightInChunks; j++)
            {
                var chunkObj = Instantiate(chunk, transform, true);
                chunkObj.transform.position = posObj + new Vector3(i * _chunkSize, j * _chunkSize);
                chunkObj.name = "Chunk(" + i + ", " + j + ")";
                //Debug.Log(new Vector3(i * chunkSize, j * chunkSize));

                var chunkUnit = chunkObj.GetComponent<ChunkUnit>();
                chunkUnit.chunkManager = this;
                chunkUnit.ToGenerate = true;
                _chunks[i, j] = chunkUnit;
                //progress.text = n / N * 100 + "%";
                //Debug.Log("World Generated: " + n / N * 100 + "%; " + n);
            }
        }
    
        public Vector2Int ChunkPosInWorld(ChunkUnit unit)
        {
            for (var i = 0; i < generator.worldWidthInChunks; i++)
            for (var j = 0; j < generator.worldHeightInChunks; j++)
                if (_chunks[i, j] == unit)
                    return new Vector2Int(i, j);
            return new Vector2Int(-1, -1);
        }

        private bool InBounds(Vector3 pos)
        {
            //Vector3 point = new Vector3(pos.x, pos.y);
            //Debug.Log("Point: " + point + " ;InBounds: " + bounds.Contains(point));
            //return pos.x >= posObj.x && pos.x < chunkSize * generator.worldWidth && pos.y >= posObj.y && pos.y < chunkSize * generator.worldHeight;
            return _bounds.Contains(pos);
        }

        private void AddDataToWorldSaving(WorldSavingSystem.WorldSaving worldSaving)
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

        private void LoadWorld(WorldSavingSystem.WorldSaving worldSaving)
        {
            var count = generator.CountChunks;
            var toolbox = Toolbox.Instance;
            var entityManager = toolbox.mEntityManager;
            var itemManager = toolbox.mItemManager;
            for (var i = 0; i < count; i++)
            {
                var chunkLoaded = worldSaving.GetChunkData(i);
                //Debug.Log("ChunkData: " + chunk.x + " ;" + chunk.y + " ;ChunkUnit: " + i + " ;" + j);
                if (chunkLoaded == null) continue;
            
                var entities = chunkLoaded.entities;
                //Debug.Log("EntitiesCount: " + entities.Count);
                foreach (var entity in entities)
                {
                    entityManager.Create(new Vector3(entity.X, entity.Y, 0), entity.EntityType);
                }
                var items = chunkLoaded.items;
                //Debug.Log("ItemsCount: " + items.Count);
                foreach (var item in items)
                {
                    itemManager.CreateItem(new Vector3(item.X, item.Y, 0), dataBase.GetItem(item.Name));
                }
                //Debug.Log("ItemsCount: " + items.Count);
                //Debug.Log("x" + chunkLoaded.x + " ;y" + chunkLoaded.y);
                var unit = _chunks[chunkLoaded.x, chunkLoaded.y];
                //unit.Clear(); //Полная очистка чанка
                unit.ToGenerate = false;
                var chunkFront = new ChunkUnit.ChunkBuilder.BlockUnitChunk[_chunkSize,_chunkSize];
                var chunkBack  = new ChunkUnit.ChunkBuilder.BlockUnitChunk[_chunkSize,_chunkSize];
                foreach (var blockData in chunkLoaded.blocks)
                {
                    switch (blockData.blockLayer)
                    {
                        case (int) BlockLayer.Front:
                        {
                            var blockDataMain = dataBase.GetBlock(blockData.name);
                            if (blockData.memory != null)
                            {
                                var mem = blockData.memory as ChestMemory;
                            }
                            chunkFront[blockData.x, blockData.y] = new ChunkUnit.ChunkBuilder.BlockUnitChunk(blockDataMain, blockData.memory, blockData.memStr);
                            break;
                        }
                        case (int) BlockLayer.Back:
                        {
                            var blockDataMain = dataBase.GetBlock(blockData.name);
                            chunkBack[blockData.x, blockData.y] = new ChunkUnit.ChunkBuilder.BlockUnitChunk(blockDataMain, blockData.memory, blockData.memStr);
                            break;
                        }
                    }
                }
                unit.chunkBuilder = new ChunkUnit.ChunkBuilder(unit, this);
                unit.chunkBuilder.Rebuild(chunkFront, chunkBack);
            }
        }

        #region GetChunk
        //Global
        public ChunkUnit GetChunk(Vector3 pos)
        {
            if (!InBounds(pos)) return null;
        
            var posInt = new Vector2Int(MathFast.FloorToInt(pos.x), MathFast.FloorToInt(pos.y));

            var pointPos = posZero - posInt;
            int i = Mathf.Abs(Mathf.FloorToInt(pointPos.x / _chunkSize)),
                j = Mathf.Abs(Mathf.FloorToInt(pointPos.y / _chunkSize));

            //Debug.Log("GetChunk Array Pos: " + i + ", " + j + " ;GetChunk Pos: " + posInt);

            return _chunks[i, j];

        }
        //Local
        public ChunkUnit GetUpperChunk(Vector2Int pos)
        {
            //Debug.Log((pos.y + 1) + " ;" + generator.worldHeightInChunks);
            return pos.y + 1 <= generator.worldHeightInChunks - 1 ? _chunks[pos.x, pos.y + 1] : null;
        }
        public ChunkUnit GetDownerChunk(Vector2Int pos)
        {
            //Debug.Log((pos.y + 1) + " ;" + generator.worldHeightInChunks);
            return pos.y - 1 > 0 ? _chunks[pos.x, pos.y - 1] : null;
        }

        #endregion
        #region Debugging
        public void CreateTestEntity()
        {
            var pos = player.transform.position;
            Toolbox.Instance.mEntityManager.Create(pos, EntityType.TestBot);
        }
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
        public int worldWidthInChunks;
        public int worldHeightInChunks;

        public BlockData dirt;
        public BlockData sand;
        public BlockData stone;
        public BlockData[] ores;
    
        [HideInInspector] public int worldWidth;
        [HideInInspector] public int worldHeight;
    
        public int CountChunks => worldWidthInChunks * worldHeightInChunks;

        public void InitProps()
        {
            var chunkSize = GameConstants.ChunkSize;

            worldWidth = worldWidthInChunks * chunkSize;
            worldHeight = worldHeightInChunks * chunkSize;
        }
    }
}