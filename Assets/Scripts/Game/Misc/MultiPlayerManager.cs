using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace Game.Misc
{
    public class MultiPlayerManager : MonoBehaviourPunCallbacks
    {
        public bool IsConnectedToMaster { get; set; }
        public bool IsOnlineGame;// { get; set; }
        public bool IsOfflineGame;

        private void Start()
        {
            PhotonPeer.RegisterType(typeof(WorldData), 200, SerializeWorld, DeserializeWorld);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("IsConnectedToMaster");
            IsConnectedToMaster = true;
        }

        public override void OnJoinedRoom()
        {
            IsOnlineGame = true;
        }

        public static byte[] SerializeWorld(object worldData)
        {
            return Serialize((WorldData)worldData);
        }
        public static object DeserializeWorld(byte[] worldBytes)
        {
            return Deserialize<WorldData>(worldBytes);
        }
        
        public static byte[] SerializeChunk(object chunkData)
        {
            return Serialize((ChunkData)chunkData);
        }
        public static object DeserializeChunk(byte[] chunkBytes)
        {
            return Deserialize<ChunkData>(chunkBytes);
        }
        
        public static byte[] SerializeBlock(object blockData)
        {
            return Serialize((BlockData)blockData);
        }
        public static object DeserializeBlock(byte[] blockBytes)
        {
            return Deserialize<BlockData>(blockBytes);
        }
        
        private static byte[] Serialize<T>(T obj) where T : class
        {
            var stream = new MemoryStream();
            var serializer = new DataContractSerializer(typeof(T));
            serializer.WriteObject(stream, obj);
            return stream.ToArray();
        }
        private static T Deserialize<T>(byte[] data) where T : class
        {
            var stream = new MemoryStream(data);
            var deserializer = new DataContractSerializer(typeof(T));
            return deserializer.ReadObject(stream) as T;
        }
    }
    [Serializable]
    public class WorldData
    {
        public int width;
        public int height;

        public ChunkData[] chunks;
    }
    [Serializable]
    public class ChunkData
    {
        public BlockData[] blocks;

        public int x;
        public int y;
    }
    [Serializable]
    public class BlockData
    {
        public string name;

        public int layer;
        
        public int x;
        public int y;
    }
}