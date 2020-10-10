using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SavingSystem
{
	public class WorldSavingSystem
	{
		private readonly string worldDirName = "Worlds";
		private readonly int ChunkSize = GameConstants.chunkSize;

		private WorldDataList worldsList;
		private WorldSaving worldSaving;
		internal string dirnameWorlds { get => Application.dataPath + "/" + worldDirName; }

		public void Init()
		{
			InitDirectoryWorlds();

			worldsList = new WorldDataList(this);	

			worldSaving = new WorldSaving(this, worldsList);
			worldSaving.CreateWorld(new WorldDataUnit(this) { name = "TestWorld2", width = 32, height = 8});
			
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 8; j++)
                {
					ChunkData chunk = new ChunkData(i,j);

					worldSaving.AddChunk(chunk);
				}
            }

			worldSaving.SaveWorld();
			//worldSaving.LoadWorldName("TestWorld");

			//Debug.Log(worldsList.worlds.Count);
		}
		public void InitDirectoryWorlds()
		{
			if (!Directory.Exists(dirnameWorlds))
			{
				Directory.CreateDirectory(dirnameWorlds);
				Debug.Log(dirnameWorlds);
			}
		}

        #region WorldList
        public class WorldDataList
        {
            internal List<string> worlds;
            private readonly string fileNameListWorld = "WorldList.worldlist";

            private WorldSavingSystem system;
            private string FnameWorldList { get => system.dirnameWorlds + "/" + fileNameListWorld; }
            public WorldDataList(WorldSavingSystem system)
            {
                worlds = new List<string>();
                this.system = system;

                ////Создаем файл лист с мирами если его нет
                //string fname = FnameWorldList;
                //if (!File.Exists(fname))
                //{
                //	File.Create(fname);
                //}

                LoadWorldList();
            }

            //Methods
            public void LoadWorldList()
            {
                string pathFile = FnameWorldList;
                using (var fs = new FileStream(pathFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new StreamReader(fs))
                    {
                        string text = reader.ReadToEnd();

                        if (text != "")
                        {
                            worlds = JsonHelper.FromJson<string>(text).ToList();
                        }
                    }
                }
            }
            public void AddWorldToList(string name)
            {
                if (name != null)
                {
                    LoadWorldList();
                    worlds.Add(name);

                    // Конвертируем в json
                    string jsonString = JsonHelper.ToJson(worlds.ToArray(), true);

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
                return worlds.Contains(name);
            }
        } 
        #endregion
        #region WorldSaving
        public class WorldSaving
        {
            private WorldSavingSystem system;
            private WorldDataList worldList;
            private WorldDataUnit worldDataUnit;

            public string fnameInfo { get => worldDataUnit.Dirname + "/" + worldDataUnit.name + ".info"; }
            public WorldSaving(WorldSavingSystem system, WorldDataList worldList)
            {
                this.system = system;
                this.worldList = worldList;
            }
            public void CreateWorld(WorldDataUnit data)
            {
                if (!worldList.WorldIsExists(data.name))
                {
                    SetDataUnit(data);

                    worldList.AddWorldToList(data.name);//Добавляем мир в список миро

                    CreateWorldDir();//Создаем папку с миром	

                    CreateFileInfoWorld();//Создаем файл с информацией о мире
                    SaveInfo();//Сохранение инфо и мире
                }
            }
            private void SetDataUnit(WorldDataUnit data)
            {
                worldDataUnit = data;
            }
            private void CreateFileInfoWorld()
            {
                string fname = fnameInfo;
            }
            public void SaveWorld()
            {
                if (worldDataUnit != null)
                {
                    for (int i = 0; i < worldDataUnit.chunks.Count; i++)
                    {
                        ChunkData chunk = worldDataUnit.GetChunk(i);
                        if (chunk != null)
                        {
                            ChunkSaving chunkSaving = new ChunkSaving(chunk, system, worldDataUnit);
                            chunkSaving.ChunkSave();
                        }
                    }
                }
            }
            public void LoadWorld()
            {
                //if (worldDataUnit != null)
                //{				
                //	for (int i = 0; i < worldDataUnit.chunks.Count; i++)
                //	{
                //		ChunkSaving chunkSaving = new ChunkSaving(new ChunkData(), system, worldDataUnit);
                //		chunkSaving.Chunkload();					
                //	}
                //}
            }

            public void LoadWorldName(string name)
            {
                if (worldList.WorldIsExists(name))
                {
                    string dirname = system.dirnameWorlds + "/" + name;
                    if (Directory.Exists(dirname))
                    {
                        worldDataUnit = new WorldDataUnit(system);
                        worldDataUnit.name = name;

                        LoadInfo();
                    }
                }
            }
            public void SaveInfo()
            {
                List<string> data = new List<string>();

                data.Add(worldDataUnit.width.ToString());
                data.Add(worldDataUnit.height.ToString());

                //Debug.Log(fnameInfo);
                // Конвертируем в json
                string jsonString = JsonHelper.ToJson(data.ToArray(), true);
                //Записываем
                using (var fs = new FileStream(fnameInfo, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        writer.Write(jsonString);
                    }
                }
            }
            public void LoadInfo()
            {
                string pathFile = fnameInfo;

                using (var fs = new FileStream(fnameInfo, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                {
                    using (var writer = new StreamReader(fs))
                    {
                        string text = writer.ReadToEnd();

                        if (text != "")
                        {
                            string[] a = JsonHelper.FromJson<string>(text);

                            int.TryParse(a[0], out worldDataUnit.width);
                            int.TryParse(a[1], out worldDataUnit.height);
                            Debug.Log("x: " + worldDataUnit.width + " ;y: " + worldDataUnit.height);
                        }
                    }
                }
            }
            private void CreateWorldDir()
            {
                string dirname = worldDataUnit.Dirname;
                if (!Directory.Exists(dirname))
                {
                    Directory.CreateDirectory(dirname);
                }
            }

            public void AddChunk(ChunkData data)
            {
                worldDataUnit?.AddChunk(data);
            }
        }
        [Serializable]
        public class WorldDataUnit
        {
            private WorldSavingSystem system;

            public int width;//chunks
            public int height;

            public string name;
            public int CountChunks { get => width * height; }
            public string Dirname { get => system.dirnameWorlds + "/" + name; }
            public List<ChunkData> chunks;
            public WorldDataUnit(WorldSavingSystem system)
            {
                chunks = new List<ChunkData>();
                this.system = system;
            }
            public void AddChunk(ChunkData data)
            {
                if (chunks.Count < CountChunks)
                {
                    chunks.Add(data);
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
        }
        #endregion

        #region Chunk
        public class ChunkSaving
        {
            public ChunkData chunkData;
            public WorldSavingSystem system;
            public WorldDataUnit dataUnit;
            private int chunkSize;
            public ChunkSaving(ChunkData chunkData, WorldSavingSystem system, WorldDataUnit dataUnit)
            {
                this.chunkData = chunkData;
                this.system = system;
                this.dataUnit = dataUnit;

                chunkSize = system.ChunkSize;
            }
            public void ChunkSave()
            {
                string fname = dataUnit.Dirname + "/" + "Chunk " + chunkData.x + ";" + chunkData.y + ".chunkData";

                using (var fs = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        List<string> data = new List<string>();

                        for (int i = 0; i < chunkData.blocks.Count; i++)
                        {
                            data.Add(JsonConvert.SerializeObject(chunkData.blocks[i]));
                        }

                        // Конвертируем в json
                        string jsonString = JsonHelper.ToJson(data.ToArray(), true);

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
                            string[] a = JsonHelper.FromJson<string>(text);
                            chunkData.blocks.Clear();
                            for (int i = 0; i < a.Length; i++)
                            {
                                BlockChunkData data = JsonConvert.DeserializeObject<BlockChunkData>(a[i]);
                                chunkData.AddChunkBlock(data);
                            }
                        }
                    }
                }
            }
        }
        [Serializable]
        public class ChunkData
        {
            public int x;
            public int y;

            public List<BlockChunkData> blocks = new List<BlockChunkData>();
            public int chunkSize = GameConstants.chunkSize;
            public ChunkData(int x, int y)
            {
                this.x = x;
                this.y = y;


            }
            public void AddBlock(int x, int y, BlockData data)
            {
                blocks.Add(new BlockChunkData(x, y, data));
            }
            public void AddChunkBlock(BlockChunkData data)
            {
                blocks.Add(data);
            }
        }
        public class BlockChunkData
        {
            public int x;
            public int y;

            public BlockData data;

            public BlockChunkData(int x, int y, BlockData data)
            {
                this.x = x;
                this.y = y;
                this.data = data;
            }
        }
        #endregion
    }
}