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
            var cond = Toolbox.Instance.mMultiPlayerManager.IsConnectedToMaster;
            if (cond)
            {
                Log("Tryed Join Random Room");
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnJoinedRoom()
        {
            Log("On Joined Room");
            
            var gameSceneManager = Toolbox.Instance.mSceneManager;
            gameSceneManager.SetSceneAlt(GameScene.Game);

            PhotonNetwork.LoadLevel("Game");
        }

        public override void OnConnectedToMaster()
        {
            Log("Connected to Master");
        }

        private void Log(string message)
        {
            logText.text += "\n" + message;
        }
    }
}