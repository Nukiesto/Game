using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SavingSystem
{
	public static class WorldSavingSystem
	{
		private static readonly string worldDirName = "Worlds";
        public static WorldDataList WorldsList;
		internal static string DirnameWorlds => Application.persistentDataPath + "/" + worldDirName;

        public static void Init()
		{
			InitDirectoryWorlds();

			WorldsList = new WorldDataList();
        }
		public static void InitDirectoryWorlds()
		{
			if (!Directory.Exists(DirnameWorlds))
			{
				Directory.CreateDirectory(DirnameWorlds);
				Debug.Log(DirnameWorlds);
			}
		}

        #region WorldList
        public class WorldDataList
        {
            internal List<string> Worlds;
            private readonly string fileNameListWorld = "WorldList.worldlist";

            private string FnameWorldList => DirnameWorlds + "/" + fileNameListWorld;

            public WorldDataList()
            {
                Worlds = new List<string>();

                LoadWorldList();
            }

            //Methods
            public void LoadWorldList()
            {
                var pathFile = FnameWorldList;
                using (var fs = new FileStream(pathFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new StreamReader(fs))
                    {
                        var text = reader.ReadToEnd();

                        if (text != "")
                        {
                            Worlds = JsonHelper.FromJson<string>(text).ToList();
                        }
                    }
                }
            }
            public void AddWorldToList(string name)
            {
                if (name != null)
                {
                    LoadWorldList();
                    Worlds.Add(name);

                    // Конвертируем в json
                    string jsonString = JsonHelper.ToJson(Worlds.ToArray(), true);

                    using (var fs = new FileStream(FnameWorldList, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                    {
                        using (var writer = new StreamWriter(fs))
                        {
                            writer.Write(jsonString);
                        }
                    }
                }
            }
            public bool WorldIsExists(string name)
            {
                return Worlds.Contains(name);
            }
        } 
        #endregion
        #region WorldSaving
        public class WorldSaving
        {
            private WorldDataList worldList;
            private WorldDataUnit worldDataUnit;
            public PlayerData playerData;

            public string fnameInfo => worldDataUnit.Dirname + "/" + worldDataUnit.name + ".info";

            public WorldSaving( WorldDataList worldList)
            {
                this.worldList = worldList;
            }
            public bool CreateWorld(WorldDataUnit data)
            {
                if (!worldList.WorldIsExists(data.name))
                {
                    SetDataUnit(data);

                    worldList.AddWorldToList(data.name);//Добавляем мир в список миров

                    CreateWorldDir(worldDataUnit.Dirname);//Создаем папку с миром	

                    //CreateFileInfoWorld();//Создаем файл с информацией о мире
                    SaveInfo();//Сохранение инфо и мире
                    return true;
                }
                return false;
            }
            private void SetDataUnit(WorldDataUnit data)
            {
                worldDataUnit = data;
            }
            public void SaveWorld()
            {
                if (worldDataUnit != null)
                {
                    if (!Directory.Exists(worldDataUnit.Dirname))
                    {
                        if (!LoadInfo())
                        {
                            SaveInfo();//Сохранение инфо и мире
                        }
                    }

                    for (int i = 0; i < worldDataUnit.chunks.Count; i++)
                    {
                        ChunkData chunk = worldDataUnit.GetChunk(i);
                        if (chunk != null)
                        {
                            ChunkSaving chunkSaving = new ChunkSaving(chunk, worldDataUnit);
                            chunkSaving.ChunkSave();
                        }
                    }
                }
            }
            public void LoadWorld()
            {
                if (worldDataUnit != null)
                {
                    worldDataUnit.Clear();
                    for (int i = 0; i < worldDataUnit.width; i++)
                    {
                        for (int j = 0; j < worldDataUnit.height; j++)
                        {
                            ChunkSaving chunkSaving = new ChunkSaving(new ChunkData(i, j), worldDataUnit);
                            chunkSaving.ChunkLoad();

                            //Debug.Log("ChunkData: " + chunkSaving.chunkData.x + " ;" + chunkSaving.chunkData.y);
                            worldDataUnit.AddChunk(chunkSaving.chunkData);                                   
                        }
                    }
                }
            }

            public bool LoadWorldName(string name)
            {
                if (worldList.WorldIsExists(name))
                {
                    //Debug.Log("WorldLoading");
                    string dirname = DirnameWorlds + "/" + name;
                    if (!Directory.Exists(dirname))
                    {
                        CreateWorldDir(dirname);
                    }

                    if (worldDataUnit == null)
                    {
                        worldDataUnit = new WorldDataUnit()
                        {
                            name = name
                        };

                    }
                    else
                    {
                        worldDataUnit.name = name;
                    }        
                        
                    if (!LoadInfo())
                    {
                        SaveInfo();//Сохранение инфо и мире
                    }                   
                    LoadWorld();
                    return true;
                }
                return false;
            }
            public void SaveInfo()
            {             
                using (var fs = new FileStream(fnameInfo, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        string[] data = new string[2];

                        data[0] = worldDataUnit.width.ToString();
                        data[1] = worldDataUnit.height.ToString();

                        //Debug.Log(fnameInfo);
                        // Конвертируем в json
                        string jsonString = JsonHelper.ToJson(data, true);
                        //Записываем
                        writer.Write(jsonString);
                    }
                }
            }
            public bool LoadInfo()
            {
                using (var fs = new FileStream(fnameInfo, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                {
                    using (var writer = new StreamReader(fs))
                    {
                        string text = writer.ReadToEnd();

                        if (text != "")
                        {
                            string[] a = JsonHelper.FromJson<string>(text);

                            int w = int.Parse(a[0]);
                            int h = int.Parse(a[1]);

                            if (w > 0 &&  h > 0) {
                                worldDataUnit.width = w;
                                worldDataUnit.height = h;
                                return true;
                            }
                            //Debug.Log("x: " + worldDataUnit.width + " ;y: " + worldDataUnit.height);       
                        }
                    }
                }
                return false;
            }
            public void SavePlayerData()
            {
                using (var fs = new FileStream(fnameInfo, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        string jsonString = JsonConvert.SerializeObject(playerData);

                        writer.Write(jsonString);
                    }
                }
            }
            public void LoadPlayerData()
            {
                string pathFile = fnameInfo;

                using (var fs = new FileStream(fnameInfo, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                {
                    using (var writer = new StreamReader(fs))
                    {
                        string jsonString = writer.ReadToEnd();

                        if (jsonString != "")
                        {
                            playerData = JsonConvert.DeserializeObject<PlayerData>(jsonString);
                        }
                    }
                }
            }

            private void CreateWorldDir(string dirname)
            {
                if (!Directory.Exists(dirname))
                {
                    Directory.CreateDirectory(dirname);
                }
            }
            public ChunkData GetChunkData(int n)
            {
                return worldDataUnit?.GetChunk(n);
            }
            public void AddChunk(ChunkData data)
            {
                worldDataUnit?.AddChunk(data);
            }
            public void SetChunk(ChunkData data)
            {
                worldDataUnit?.SetChunk(data);
            }
            public void Clear()
            {
                worldDataUnit?.Clear();
            }
        }
        [Serializable]
        public class WorldDataUnit
        {
            public int width;//chunks
            public int height;

            public string name;
            public int CountChunks => width * height;
            public string Dirname => DirnameWorlds + "/" + name;
            public List<ChunkData> chunks;
            public WorldDataUnit()
            {
                chunks = new List<ChunkData>();

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        AddChunk(new ChunkData(i, j));
                    }
                }
            }
            public void AddChunk(ChunkData data)
            {
                if (chunks.Count < CountChunks)
                {
                    chunks.Add(data);
                }
            }
            public void SetChunk(ChunkData data)
            {
                for (int i = 0; i < chunks.Count; i++)
                {
                    if (chunks[i].x == data.x && chunks[i].y == data.y)
                    {
                        chunks[i] = data;
                        break;
                    }
                }
            }
            public ChunkData GetChunk(int i)
            {
                if (i <= chunks.Count)
                {
                    return chunks[i];
                }
                return null;
            }
            public void Clear()
            {
                chunks.Clear();
            }
        }
        #endregion

        #region Chunk
        public class ChunkSaving
        {
            public ChunkData chunkData;
            public WorldDataUnit dataUnit;

            public ChunkSaving(ChunkData chunkData, WorldDataUnit dataUnit)
            {
                this.chunkData = chunkData;
                this.dataUnit = dataUnit;
            }
            public void ChunkSave()
            {
                string fname = dataUnit.Dirname + "/" + "Chunk " + chunkData.x + ";" + chunkData.y + ".chunkData";

                using (var fs = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    fs.SetLength(0);//Очистка файла
                    using (var writer = new StreamWriter(fs))
                    {                       
                        string[] data = new string[2 + chunkData.blocks.Count + chunkData.items.Count];

                        data[0] = chunkData.blocks.Count.ToString();
                        data[1] = chunkData.items.Count.ToString();

                        int start = 2; int end = 2 + chunkData.blocks.Count;
                        //Debug.Log("Count: " + chunkData.blocks.Count);
                        //Debug.Log("Start: " + start);
                        //Debug.Log("End: " + end);
                        for (int i = start; i < end; i++)
                        {
                            //Debug.Log("i: " + (i - start));
                            data[i] = JsonConvert.SerializeObject(chunkData.blocks[i - start]);
                        }
                        start = end + 1; end = start + chunkData.items.Count;
                        if (chunkData.items.Count > 0)
                        {
                            for (int i = start; i < end; i++)
                            {
                                data[i] = JsonConvert.SerializeObject(chunkData.items[i - start]);
                            }
                        }
                        
                        // Конвертируем в json
                        string jsonString = JsonHelper.ToJson(data, true);

                        writer.Write(jsonString);
                    }
                }
            }
            public void ChunkLoad()
            {
                string fname = dataUnit.Dirname + "/" + "Chunk " + chunkData.x + ";" + chunkData.y + ".chunkData";

                using (var fs = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new StreamReader(fs))
                    {
                        string text = reader.ReadToEnd();

                        if (text != "")
                        {
                            string[] data = JsonHelper.FromJson<string>(text);
                            chunkData.blocks.Clear();

                            int start = 2; int end = 2 + int.Parse(data[0]);
                            for (int i = start; i < end; i++)
                            {
                                //Debug.Log("i: " + i + " ;data: " + data[i - start]);
                                BlockChunkData blockData = JsonConvert.DeserializeObject<BlockChunkData>(data[i]);
                                chunkData.AddChunkBlock(blockData);
                            }
                            start = end + 1; end = start + int.Parse(data[1]);
                            for (int i = start; i < end; i++)
                            {
                                ItemChunkData entityData = JsonConvert.DeserializeObject<ItemChunkData>(data[i - start]);
                                chunkData.AddChunkItem(entityData);
                            }
                         }
                    }
                }
            }
        }
        #endregion

        #region Data

        [Serializable]
        public class ChunkData
        {
            public int x;
            public int y;

            public List<BlockChunkData> blocks = new List<BlockChunkData>();
            public List<ItemChunkData> items = new List<ItemChunkData>();
            public int chunkSize = GameConstants.ChunkSize;
            public ChunkData(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
            public void AddChunkBlock(BlockChunkData data)
            {
                blocks.Add(data);
            }
            public void AddChunkItem(ItemChunkData data)
            {
                items.Add(data);
            }
        }
        public class BlockChunkData
        {
            public int x;
            public int y;
            
            public string name;
            public int blockLayer;

            public BlockChunkData(int x, int y, string name, int blockLayer)
            {
                this.x = x;
                this.y = y;
                this.name = name;
                this.blockLayer = blockLayer;
            }
        }
        public class EntityChunkData
        {
            public int x;
            public int y;
        }
        public class ItemChunkData
        {
            public EntityChunkData entity;
        }
        [JsonObject(IsReference = true)]
        [Serializable]
        public class PlayerData
        {           
            public int x;
            public int y;           
        }  

        #endregion     
    }
}