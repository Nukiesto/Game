using System;
using Game.ChunkSystem;
using Game.Game;
using Game.HpSystem;
using Game.ItemSystem;
using Photon.Pun;
using SavingSystem;
using Singleton;
using UnityEditor;
using UnityEngine;
using Console = Game.UI.Console;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D itemMagnet;
        [SerializeField] private float itemPickRadius;
        [SerializeField] private GameObject itemCreatePos;
        private Inventory inventory;
        [SerializeField] private ChunkManager chunkManager;
        [SerializeField] private GameObject flashLight;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private HpPlayer hpPlayer;
        [SerializeField] private PhotonView photonView;
        
        [HideInInspector] public string nickName = "Player";
        
        private bool _canInput = true;
        private bool _flashLightActive;
        public static PlayerController Instance;

        private Toolbox _toolbox;

        private void Awake()
        {
            _toolbox = Toolbox.Instance;
            if (NotMine()) return;
            
            var worldSaver = _toolbox.mWorldSaver;
            worldSaver.OnSaveEvent += OnSave;
            worldSaver.OnLoadEvent += OnLoad;
        }

        private void Start()
        {
            if (NotMine())
            {
                var worldManager1 = _toolbox.mWorldManager;
                Transform transform0;
                (transform0 = transform).position = worldManager1.SpawnPoint;
                worldManager1.MoveCameraToPoint(transform0.position);
            }
            
            Debug.Log("Started");
            Instance = this;
            itemMagnet.radius = itemPickRadius;

            var worldManager = _toolbox.mWorldManager;
            Transform transform1;
            (transform1 = transform).position = worldManager.TryGetLoadedPoint(out var loadedPoint) ? loadedPoint : worldManager.SpawnPoint;
            worldManager.MoveCameraToPoint(transform1.position);

            //Events
            hpPlayer.OnDeathEvent += OnDeath;
            
            Console.OnToggleConsoleEvent += SetCanInput;
            
            var gameManager = _toolbox.mGameManager;
            gameManager.OnChangeGameModeEvent += OnChangeGameMode;
        }

        private bool NotMine()
        {
            return PhotonNetwork.InRoom && !photonView.IsMine;
        }
        public void SetInventory(Inventory inventoryToSet)
        {
            inventory = inventoryToSet;
        }
        private void OnLoad(WorldSavingSystem.WorldSaving worldSaving)
        {
            var playerData = worldSaving.PlayerData;
            hpPlayer.SetHp(playerData.hp);
            _toolbox.mGameManager.SetGameMode(playerData.gameMode);
            nickName = playerData.nickname;
            
            //Debug.Log(playerData.hp);
        }
        private void OnSave(WorldSavingSystem.WorldSaving worldSaving)
        {
            var savePos = transform.position;
            var playerData = new WorldSavingSystem.PlayerData();
            var worldManager = _toolbox.mWorldManager;
            playerData.x = savePos.x;
            playerData.y = savePos.y;
            playerData.spawnX = worldManager.SpawnPoint.x;
            playerData.spawnY = worldManager.SpawnPoint.y;
            playerData.spawnPointInited = true;
            playerData.gameMode = _toolbox.mGameManager.GetGameMode();
            playerData.items = inventory.GetItems();
            playerData.hp = hpPlayer.GetHp();
            playerData.nickname = nickName;
            
            worldSaving.PlayerData = playerData;
            worldSaving.AddPlayerData();
        
            var worldSaver = _toolbox.mWorldSaver;
            worldSaver.OnSaveEvent -= OnSave;
        }
        private void OnDeath()
        {
            if (NotMine()) return;
            
            hpPlayer.FillHp();
            MoveToSpawn();
        }
        public void MoveToSpawn()
        {
            var worldManager = Toolbox.Instance.mWorldManager;
            var transform1 = transform;
            transform1.position = worldManager.SpawnPoint;
            worldManager.MoveCameraToPoint(transform1.position);
        }
        public void MoveToPos(Vector3 pos)
        {
            if (NotMine()) return;
            
            var worldManager = Toolbox.Instance.mWorldManager;
            var transform1 = transform;
            transform1.position = pos;
            worldManager.MoveCameraToPoint(pos);
        }
        private void Update()
        {
            if (NotMine()) return;
            
            if (!_canInput) return;
        
            if (Input.GetKeyDown(KeyCode.F))
            {
                _flashLightActive = !_flashLightActive;
                flashLight.SetActive(_flashLightActive);
            }
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (NotMine()) return;
            
            //Debug.Log(col.gameObject.name);
            if (!col.gameObject.CompareTag("Item")) return;
        
            var obj = col.gameObject;
            //Debug.Log(obj.GetComponent<Item>().data);
            inventory.AddItem(obj.GetComponent<Item>().data);
            obj.SetActive(false);
        }
        public bool CanToCreateItem()
        {
            var pos = itemCreatePos.transform.position;
            return !chunkManager.GetChunk(pos).HasBlock(pos);
        }
        public void CreateItemKick(ItemData.Data data, int count)
        {
            if (NotMine()) return;
            
            var pos = itemCreatePos.transform.position;

            var itemManager = Toolbox.Instance.mItemManager;

            for (var i = 0; i < count; i++)
            {
                itemManager.CreateItem(pos, data);
            }       
        }

        private void SetCanInput(bool value)
        {
            _canInput = value;
            playerMovement.SetCanMove(value);
        }

        private void OnChangeGameMode(GameMode gameMode)
        {
            hpPlayer.SetGodMode(gameMode == GameMode.Sandbox);
        }
    }
}
