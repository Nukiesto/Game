using Game.UI.Menu.Menus.Multiplayer;
using UnityEngine;
using static MainMenuDialog;

public class MultiplayerMenuButton : ButtonUnit
{
    public enum Button
    {
        JoinRandomRoom,
        BackToMainMenu
    }

    public Button buttonType;

    private void Start()
    {
        InitDelegButtonClick();
    }

    private void InitDelegButtonClick()
    {
        switch (buttonType)
        {
            case Button.JoinRandomRoom:
                ButtonClickAction = ButtonClickActionJoinRandomRoom;
                break;
            case Button.BackToMainMenu:
                ButtonClickAction = ButtonClickActionBackToMainMenu;
                break;

            default:
                ButtonClickAction = ButtonClickActionNone;
                break;
        }
    }
    private void ButtonClickActionJoinRandomRoom()
    {
        var menu0 = (MultiplayerMenu)menu;
        menu0.RandomJoinRoom();
    }
    private void ButtonClickActionBackToMainMenu()
    {
        menu.controller.SetMenu(Menu.Main);
    }

    public void ButtonClick()
    {
        ButtonClickAction();
    }
}