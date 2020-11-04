using System.Collections;
using Game.ItemSystem;
using Game.Player;
using Game.UI;
using Photon.Pun;
using Singleton;
using UnityEngine;

namespace Game.Misc
{
    public class WorldMultiplayerManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Console console;
        
        [Header("Player")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Bar[] bars;
        [SerializeField] private Inventory inventory;
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            console.WriteString("Player " + newPlayer.NickName + " entered the room: ");
        }
        
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            console.WriteString("Player " + otherPlayer.NickName + " left the room: ");
        }

        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            StartCoroutine(WaitForDisconnect());
        }
        
        private void Start()
        {
            //if (PhotonNetwork.IsMasterClient)
            //{
                StartCoroutine(TryCreatePlayer());
            //}
            //else
            //{
            //   CreatePlayer();
            //}
        }

        private void CreatePlayer()
        {
            var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            player.GetComponent<BarManager>().SetBars(bars);
            var playerController = player.GetComponent<PlayerController>();
            playerController.SetInventory(inventory);
            inventory.player = playerController;
            CameraManager.FindTarget();
        } 
        private IEnumerator TryCreatePlayer()
        {
            while (PhotonNetwork.CurrentRoom == null)
                yield return 0;
            CreatePlayer();
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
    }
}