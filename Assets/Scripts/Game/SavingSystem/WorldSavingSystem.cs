using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
    
namespace SavingSystem
{
	public static class WorldSavingSystem
	{
		private static readonly string worldDirName = "Worlds";
        public static WorldDataList WorldsList;
        public static string CurrentWorld;
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

                    SaveList();
                }
            }

            public void RemoveWorld(string name)
            {
                if (name != null)
                {
                    Directory.Delete(DirnameWorlds + "/" + name, true);
                    LoadWorldList();
                    Worlds.Remove(name);
                    SaveList();
                }
            }

            private void SaveList()
            {
                // Конвертируем в json
                var jsonString = JsonHelper.ToJson(Worlds.ToArray(), true);

                using (var fs = new FileStream(FnameWorldList, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.SetLength(0);//Очистка файла
                    using (var writer = new StreamWriter(fs))
                    {
                        writer.Write(jsonString);
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
            private WorldDataList _worldList;
            public WorldDataUnit WorldDataUnit;
            public PlayerData PlayerData;

            public string FnameInfo => WorldDataUnit.Dirname + "/" + WorldDataUnit.name + ".info";
            public string FnamePlayerData => WorldDataUnit.Dirname + "/" + "PlayerData" + ".info";
            
            public WorldSaving( WorldDataList worldList)
            {
                this._worldList = worldList;
            }
            public bool CreateWorld(WorldDataUnit data)
            {
                if (!_worldList.WorldIsExists(data.name))
                {
                    SetDataUnit(data);

                    _worldList.AddWorldToList(data.name);//Добавляем мир в список миров

                    CreateWorldDir(WorldDataUnit.Dirname);//Создаем папку с миром	

                    //CreateFileInfoWorld();//Создаем файл с информацией о мире
                    SaveInfo();//Сохранение инфо и мире
                    return true;
                }
                return false;
            }
            private void SetDataUnit(WorldDataUnit data)
            {
                WorldDataUnit = data;
            }
            public void SaveWorld()
            {
                if (WorldDataUnit != null)
                {
                    SaveInfo();
                    for (var i = 0; i < WorldDataUnit.chunks.Count; i++)
                    {
                        var chunk = WorldDataUnit.GetChunk(i);
                        if (chunk != null)
                        {
                            var chunkSaving = new ChunkSaving(chunk, WorldDataUnit);
                            chunkSaving.ChunkSave();
                        }
                    }
                }
            }
            public void LoadWorld()
            {
                if (WorldDataUnit != null)
                {
                    WorldDataUnit.Clear();
                    for (var i = 0; i < WorldDataUnit.width; i++)
                    {
                        for (var j = 0; j < WorldDataUnit.height; j++)
                        {
                            var chunkSaving = new ChunkSaving(new ChunkData(i, j), WorldDataUnit);
                            chunkSaving.ChunkLoad();

                            //Debug.Log("ChunkData: " + chunkSaving.chunkData.x + " ;" + chunkSaving.chunkData.y);
                            WorldDataUnit.AddChunk(chunkSaving.chunkData);                                   
                        }
                    }
                }
            }

            public bool LoadWorldName(string name)
            {
                if (_worldList.WorldIsExists(name))
                {
                    //Debug.Log("WorldLoading");
                    var dirname = DirnameWorlds + "/" + name;
                    if (!Directory.Exists(dirname))
                    {
                        CreateWorldDir(dirname);
                    }

                    if (WorldDataUnit == null)
                    {
                        WorldDataUnit = new WorldDataUnit()
                        {
                            name = name
                        };

                    }
                    else
                    {
                        WorldDataUnit.name = name;
                    }

                    LoadInfo();
                    LoadWorld();
                    
                    return true;
                }
                return false;
            }
            public void SaveInfo()
            {             
                using (var fs = new FileStream(FnameInfo, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        var data = new string[3];

                        data[0] = WorldDataUnit.width.ToString();
                        data[1] = WorldDataUnit.height.ToString();
                        data[2] = WorldDataUnit.toGenerateWorld.ToString();
                        
                        //Debug.Log(fnameInfo);
                        // Конвертируем в json
                        var jsonString = JsonHelper.ToJson(data, true);
                        //Writing
                        writer.Write(jsonString);
                    }
                }
            }
            public bool LoadInfo()
            {
                using (var fs = new FileStream(FnameInfo, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                {
                    using (var writer = new StreamReader(fs))
                    {
                        var text = writer.ReadToEnd();

                        if (text != "")
                        {
                            var a = JsonHelper.FromJson<string>(text);

                            var w = int.Parse(a[0]);
                            var h = int.Parse(a[1]);
                            var toGenerateWorld = bool.Parse(a[2]);
                            
                            if (w > 0 &&  h > 0) {
                                WorldDataUnit.width = w;
                                WorldDataUnit.height = h;
                                WorldDataUnit.toGenerateWorld = toGenerateWorld;
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
                using (var fs = new FileStream(FnamePlayerData, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        var jsonString = JsonConvert.SerializeObject(PlayerData);

                        writer.Write(jsonString);
                    }
                }
            }
            public void LoadPlayerData()
            {
                using (var fs = new FileStream(FnamePlayerData, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                {
                    using (var writer = new StreamReader(fs))
                    {
                        var jsonString = writer.ReadToEnd();

                        if (jsonString != "")
                        {
                            //PlayerData = JsonConvert.DeserializeObject<PlayerData>(jsonString);
                            //Debug.Log(PlayerData);
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
                return WorldDataUnit?.GetChunk(n);
            }
            public void AddChunk(ChunkData data)
            {
                WorldDataUnit?.AddChunk(data);
            }
            public void SetChunk(ChunkData data)
            {
                WorldDataUnit?.SetChunk(data);
            }
            public void Clear()
            {
                WorldDataUnit?.Clear();
            }
        }
        [Serializable]
        public class WorldDataUnit
        {
            public int width;//chunks
            public int height;
            public bool toGenerateWorld;
            
            public string name;
            public int CountChunks => width * height;
            public string Dirname => DirnameWorlds + "/" + name;
            public List<ChunkData> chunks;
            public WorldDataUnit()
            {
                chunks = new List<ChunkData>();

                for (var i = 0; i < width; i++)
                {
                    for (var j = 0; j < height; j++)
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
                for (var i = 0; i < chunks.Count; i++)
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
                var fname = dataUnit.Dirname + "/" + "Chunk " + chunkData.x + ";" + chunkData.y + ".chunkData";

                using (var fs = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    fs.SetLength(0);//Очистка файла
                    using (var writer = new StreamWriter(fs))
                    {                       
                        var data = new string[2 + chunkData.blocks.Count + chunkData.items.Count];

                        data[0] = chunkData.blocks.Count.ToString();
                        data[1] = chunkData.items.Count.ToString();

                        var start = 2;var end = 2 + chunkData.blocks.Count;
                        //Debug.Log("Count: " + chunkData.blocks.Count);
                        //Debug.Log("Start: " + start);
                        //Debug.Log("End: " + end);
                        for (var i = start; i < end; i++)
                        {
                            var item = chunkData.blocks[i - start];
                            //Debug.Log("i: " + (i - start));
                            if (item.memory != null)
                            {
                                //item.memory.ItemConverterType = item.memory.GetType();
                                
                                
                                /*var m = item.memory as ChestMemory;
                                if (m != null && m.itemsToSave != null)
                                {
                                    //item.memory.ItemConverterParameters = new object[] {"Items"};
                                    //Debug.Log("ChestMemSavedParameters: " + item.memory.ItemConverterParameters[0]);
                                    //item.memory.ItemConverterType = typeof(ChestMemory.ChestMemoryUnit);
                                    Debug.Log("ChestMemSavedItemsCount: " + m.itemsToSave?.Count);
                                }
                                else
                                {
                                    Debug.Log("ChestMemSavedItemsCount: -1");
                                }*/
                            }
                            //item.memory.ItemConverterType = 
                            data[i] = JsonConvert.SerializeObject(item);
                            if (item.memory != null)
                            {
                                //Debug.Log("Save: " + data[i]);
                            }
                        }
                        start = end + 1; end = start + chunkData.items.Count;
                        if (chunkData.items.Count > 0)
                        {
                            for (var i = start; i < end; i++)
                            {
                                
                                data[i] = JsonConvert.SerializeObject(chunkData.items[i - start]);
                            }
                        }
                        
                        // Конвертируем в json
                        var jsonString = JsonHelper.ToJson(data, true);

                        writer.Write(jsonString);
                    }
                }
            }
            public void ChunkLoad()
            {
                var fname = dataUnit.Dirname + "/" + "Chunk " + chunkData.x + ";" + chunkData.y + ".chunkData";

                using (var fs = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new StreamReader(fs))
                    {
                        var text = reader.ReadToEnd();

                        if (text != "")
                        {
                            var data = JsonHelper.FromJson<string>(text);
                            chunkData.blocks.Clear();

                            var start = 2; var end = 2 + int.Parse(data[0]);
                            for (var i = start; i < end; i++)
                            {
                                //Debug.Log("i: " + i + " ;data: " + data[i - start]);
                                var blockData = JsonConvert.DeserializeObject<BlockChunkData>(data[i]);
                                //DeserializeObject<BlockChunkData>(data[i]);
                                var l = "'".ToCharArray()[0];
                                var str = JsonConvert.ToString(data[i], l, StringEscapeHandling.Default);
                                
                                var rg = new Regex(@"memory.:(.*?)}'");
                                var result = rg.Match(str).Groups[1].Value;
                                
                                blockData.memStr = result;
                                
                                //if (blockData.memory != null)
                                //{
                                    //Debug.Log(data[i]);
                                    //Debug.Log("BlockLoadTypeConvert: " + blockData.memory.ItemConverterType);
                                    //Debug.Log("BlockLoadTypeConvertParameters: " + blockData.memory.ItemConverterParameters[0]);
                                    //Debug.Log(data[i]);
                                    //Debug.Log(blockData.memory.Description);
                                    //var m = blockData.memory as ChestMemory;
                                    //Debug.Log(m?.itemsToSave?.Count);
                                    //var t = m.Items.GetType();
                                    //var t = (List<ChestSlotUnitSave>)blockData.memory.ItemConverterParameters[0];
                                    //Debug.Log(t);
                                    //m.Items = t;
                                    //blockData.memory = m;
                                //}
                                
                                chunkData.AddChunkBlock(blockData);
                            }
                            start = end + 1; end = start + int.Parse(data[1]);
                            for (var i = start; i < end; i++)
                            {
                                var entityData = JsonConvert.DeserializeObject<ItemChunkData>(data[i - start]);
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
                 /*if (data.memory != null)
                 {
                     var m = data.memory as ChestMemory;
                     if (m != null && m.itemsToSave != null)
                     {
                         Debug.Log("AddChestMemLoadedItemsCount: " + m.itemsToSave.Count );
                     }
                     else
                     {
                         Debug.Log("AddChestMemLoadedItemsCount: -1");
                     }
                 }*/
                blocks.Add(data);
            }
            public void AddChunkItem(ItemChunkData data)
            {
                items.Add(data);
            }
        }
        [Serializable]
        public class BlockChunkData
        {
            public int x;
            public int y;
            
            public string name;
            public int blockLayer;

            //[JsonPropertyAttribute("Memory", ItemConverterType = typeof(BaseBlockMemory))]
            public BaseBlockMemory memory;
            [JsonIgnore]
            public string memStr;
            
            public BlockChunkData(int x, int y, string name, int blockLayer, BaseBlockMemory memory)
            {
                this.x = x;
                this.y = y;
                this.name = name;
                this.blockLayer = blockLayer;
                this.memory = memory;
                /*if (this.memory != null)
                {
                    var m = this.memory as ChestMemory.ChestMemoryUnit;
                    if (m != null && m.Items != null)
                    {
                        Debug.Log("ChestMemAdd1ItemsCount: " + m.Items?.Count );
                    }
                    else
                    {
                        Debug.Log("ChestMemAdd1ItemsCount: -1");
                    }
                }*/
            }
        }
        public class EntityChunkData
        {
            public int X;
            public int Y;
        }
        public class ItemChunkData
        {
            public EntityChunkData Entity;
        }
        [JsonObject(IsReference = true)]
        [Serializable]
        public class PlayerData
        {           
            public float x;
            public float y;           
        }  

        #endregion     
    }
}