using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Singleton;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.UI.Menu.Menus.Multiplayer
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Text logText;

        [Header("RoomList")] 
        [SerializeField] private GameObject roomlistContent;
        [SerializeField] private GameObject roomlistPanel;
        [SerializeField] private GameObject roomlistPrefabItem;
        
        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListEntries;
        
        private void Start()
        {
            PhotonNetwork.NickName = "Player" + Random.Range(1000, 9999);
            Log("Player`s name is set to: " + PhotonNetwork.NickName);
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.ConnectUsingSettings();
        }

        public void CreateRoom()
        {
            PhotonNetwork.CreateRoom(null, new RoomOptions() {MaxPlayers = 2});
        }

        public void JoinRandomRoom()
        {
            var manager = Toolbox.Instance.mMultiPlayerManager;
            var cond = manager.IsConnectedToMaster;
            if (cond)
            {
                Log("Tryed Join Random Room");
                Debug.Log("Tryed Join Random Room");
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnJoinedRoom()
        {
            Log("On Joined Room");
            Debug.Log("On Joined Room");
            var manager = Toolbox.Instance.mMultiPlayerManager;
            manager.IsOnlineGame = true;
            
            var gameSceneManager = Toolbox.Instance.mSceneManager;
            gameSceneManager.SetSceneAlt(GameScene.Game);
            PhotonNetwork.LoadLevel("Game");
        }

        private IEnumerator LoadLevel()
        {
            while (true)
            {
                Log("On Joined Room");
                Debug.Log("On Joined Room");
                var manager = Toolbox.Instance.mMultiPlayerManager;
                manager.IsOnlineGame = true;
            
                var gameSceneManager = Toolbox.Instance.mSceneManager;
                gameSceneManager.SetSceneAlt(GameScene.Game);
                yield return null;
                PhotonNetwork.LoadLevel("Game");
            }
        }
        public override void OnConnectedToMaster()
        {
            Log("Connected to Master");
        }

        private void Log(string message)
        {
            logText.text += "\n" + message;
        }
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }

        private void ClearRoomListView()
        {
            foreach (var entry in roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListEntries.Clear();
        }

        private void UpdateRoomListView()
        {
            foreach (var info in cachedRoomList.Values)
            {
                var entry = Instantiate(roomlistPrefabItem, roomlistContent.transform, true);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                roomListEntries.Add(info.Name, entry);
            }
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (var info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }
    }
}