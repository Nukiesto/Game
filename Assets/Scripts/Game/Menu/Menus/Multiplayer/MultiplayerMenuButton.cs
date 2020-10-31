using UnityEngine;
using static MainMenuDialog;

public class MultiplayerMenuButton : ButtonUnit
{
    public enum Button
    {
        NewGame,
        LoadGame,
        Settings,
        Exit,
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
            case Button.BackToMainMenu:
                ButtonClickAction = ButtonClickActionBackToMainMenu;
                break;

            default:
                ButtonClickAction = ButtonClickActionNone;
                break;
        }
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