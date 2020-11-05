using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Menu.Menus.Multiplayer
{
    public class RoomListEntry : MonoBehaviour
        {
            public Text roomNameText;
            public Text roomPlayersText;
            public Button joinRoomButton;
    
            private string _roomName;
    
            public void Start()
            {
                joinRoomButton.onClick.AddListener(() =>
                {
                    if (PhotonNetwork.InLobby)
                    {
                        PhotonNetwork.LeaveLobby();
                    }
    
                    PhotonNetwork.JoinRoom(_roomName);
                });
            }
    
            public void Initialize(string nameRoom, byte currentPlayers, byte maxPlayers)
            {
                _roomName = nameRoom;
    
                roomNameText.text = nameRoom;
                roomPlayersText.text = currentPlayers + " / " + maxPlayers;
            }
        }
}