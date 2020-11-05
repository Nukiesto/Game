using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Game.ChunkSystem;
using Game.ItemSystem;
using Game.Player;
using Photon.Pun;
using Photon.Realtime;
using Singleton;
using UnityEngine;
using Console = Game.UI.Console;

namespace Game.Misc
{
    public class WorldMultiplayerManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        private enum Event
        {
            UpdateWorld,
            SetBlock,
            DestroyBlock,
            CreateItem,
            CreateEntity
        }
        
        [SerializeField] private Console console;
        
        [Header("Player")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Bar[] bars;
        [SerializeField] private Inventory inventory;
        [SerializeField] private ChunkManager chunkManager;

        private List<PlayerController> _players = new List<PlayerController>();
        
        private bool _playerCreated;

        private PlayerController minePlayer;
        
        private void AddPlayer(PlayerController playerController)
        {
            _players.Add(playerController);
        }

        private void RemovePlayer(PlayerController playerController)
        {
            _players.Remove(playerController);
        }
        
        
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            console.WriteString("Player " + otherPlayer.NickName + " left the room");
        }

        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            StartCoroutine(WaitForDisconnect());
        }
        
        public override void OnJoinedRoom()
        {
            if (!_playerCreated)
            {
                Debug.Log("CreatePlayer in online");
                StartCoroutine(TryCreatePlayer());
                _playerCreated = true;

                if (PhotonNetwork.IsMasterClient)
                {
                    var worldSaver = Toolbox.Instance.mWorldSaver;
                    worldSaver.InitWorld();
                }
            }
        }

        private void Awake()
        {
            var worldManager = Toolbox.Instance.mWorldManager;
            worldManager.OnStartSceneEvent += StartScene;
        }

        private void StartScene()
        {
            var manager = Toolbox.Instance.mMultiPlayerManager;
            if (manager.IsOfflineGame)
            {
                if (!_playerCreated)
                {
                    var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                    Debug.Log("CreatePlayer in offline");
                    CreatePlayer(player);
                    _playerCreated = true;
                }
            }
        }
        private void CreatePlayer(GameObject player)
        {
            player.GetComponent<BarManager>().SetBars(bars);
            var playerController = player.GetComponent<PlayerController>();
            playerController.SetInventory(inventory);
            inventory.player = playerController;
            CameraManager.SetTarget(player);
            AddPlayer(playerController);
        } 
        private IEnumerator TryCreatePlayer()
        {
            yield return new WaitWhile(() => PhotonNetwork.CurrentRoom == null);
            
            //while (PhotonNetwork.CurrentRoom == null)
            //    yield return 0;
            var player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
            CreatePlayer(player);
        }
        private IEnumerator WaitForDisconnect()
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            Toolbox.Instance.mMultiPlayerManager.IsConnectedToMaster = false;
                
            while (PhotonNetwork.IsConnected)
                yield return 0;
            Toolbox.Instance.mFpscounter.enabled = false;
            Toolbox.Instance.mSceneManager.SetScene(GameScene.MainMenu);
        }
        
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            //newPlayer.
            console.WriteString("Player " + newPlayer.NickName + " entered the room");
            
            Debug.Log("IsMaster: " + PhotonNetwork.IsMasterClient);

            if (PhotonNetwork.IsMasterClient)
            {
                //Разослать подключенному игроку данные о мире и заставить его сгенерировать этот мир

                var options = new RaiseEventOptions()
                {
                    TargetActors = new[]
                    {
                        newPlayer.ActorNumber
                    }
                };
                
                var sendOptions = new SendOptions()
                {
                    Reliability = true
                };

                var chunks = chunkManager.Chunks;
                var w = chunks.GetLength(0);
                var h = chunks.GetLength(1);
                var chunksData = new ChunkData[w * h];
                var n = 0;
                var _chunkSize = GameConstants.ChunkSize;
                for (var i = 0; i < w; i++)
                {
                    for (var j = 0; j < h; j++)
                    {
                        var blocks = chunks[i, j].GetBlocks();
                        var blocksData = new BlockData[blocks.Count];
                        for (var index = 0; index < blocks.Count; index++)
                        {
                            var block = blocks[index];
                            if (block != null)
                            {
                                blocksData[index] = new BlockData()
                                {
                                    name = block.Data.nameBlock,
                                    layer = (int) block.Layer,
                                    x = block.PosChunk.x,
                                    y = block.PosChunk.y
                                };
                            }
                        }

                        chunksData[n] = new ChunkData()
                        {
                            blocks = blocksData,
                            x = i,
                            y = j
                        };
                        n++;
                    }
                }

                var generator = chunkManager.generator;
                var content = new WorldData
                {
                    chunks = chunksData,
                    width = generator.worldWidthInChunks,
                    height = generator.worldHeightInChunks
                };
                Debug.Log("Sended");
                PhotonNetwork.RaiseEvent(125, content, options, sendOptions);
            }
        }
        public void OnEvent(EventData photonEvent)
        {
            //Debug.Log(photonEvent.Code);
            switch (photonEvent.Code)
            {
                case 125:
                    Debug.Log("Received");
                    var data = (WorldData)photonEvent.CustomData;
                    var _chunkSize = GameConstants.ChunkSize;
                    var dataBase = DataBase.Instance;
                    
                    chunkManager.BuildChunks(false);
                    for (var i = 0; i < data.width * data.height; i++)
                    {
                        var chunkData = data.chunks[i];
                            
                        //Debug.Log("ChunkData: " + chunk.x + " ;" + chunk.y + " ;ChunkUnit: " + i + " ;" + j);
                        if (chunkData == null) continue;
                        
                        var unit = chunkManager.Chunks[chunkData.x, chunkData.y];
                        //unit.Clear(); //Полная очистка чанка
                        unit.ToGenerate = false;
                        var chunkFront = new ChunkUnit.ChunkBuilder.BlockUnitChunk[_chunkSize,_chunkSize];
                        var chunkBack  = new ChunkUnit.ChunkBuilder.BlockUnitChunk[_chunkSize,_chunkSize];
                        Debug.Log("blocksCount: " + chunkData.blocks.Length);
                        foreach (var blockData in chunkData.blocks)
                        {
                            if (blockData != null)
                            {
                                Debug.Log("layer: " + blockData.layer + " ;name: " + blockData.name);
                                switch (blockData.layer)
                                {
                                    case (int) BlockLayer.Front:
                                    {
                                        var blockDataMain = dataBase.GetBlock(blockData.name);
                                        chunkFront[blockData.x, blockData.y] =
                                            new ChunkUnit.ChunkBuilder.BlockUnitChunk(blockDataMain, null, "");//blockData.memory, blockData.memStr);
                                        break;
                                    }
                                    case (int) BlockLayer.Back:
                                    {
                                        var blockDataMain = dataBase.GetBlock(blockData.name);
                                        chunkBack[blockData.x, blockData.y] = 
                                            new ChunkUnit.ChunkBuilder.BlockUnitChunk(blockDataMain, null, "");//blockData.memory, blockData.memStr);
                                        break;
                                    }
                                }
                            }
                        }
                        unit.chunkBuilder = new ChunkUnit.ChunkBuilder(unit, chunkManager);
                        unit.chunkBuilder.Rebuild(chunkFront, chunkBack);
                    }
                    break;
            }
        }
    }
}