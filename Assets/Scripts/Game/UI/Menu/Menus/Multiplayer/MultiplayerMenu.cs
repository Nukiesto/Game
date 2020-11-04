using Singleton;
using UnityEngine;
using static MainMenuDialog;

namespace Game.UI.Menu.Menus.Multiplayer
{
    public class MultiplayerMenu : MenuUnit
    {
        [SerializeField] private LobbyManager lobbyManager;
        public void StartDialogLocal(DialogType type)
        {
            foreach (var dialog in dialogs)
            {
                if (dialog.GetTypeDialog() == type)
                {
                    dialog.SetActive(true);
                    AddDialog(dialog);
                }
            }
        }

        public override void StartDialog(dynamic type)
        {
            base.StartDialog();
            StartDialogLocal((DialogType)type);
        }

        public void RandomJoinRoom()
        {
            if (Toolbox.Instance.mMultiPlayerManager.IsConnectedToMaster)
                lobbyManager.JoinRandomRoom();
        }

        public void CreateRoom()
        {
            lobbyManager.CreateRoom();
        }
    }
}