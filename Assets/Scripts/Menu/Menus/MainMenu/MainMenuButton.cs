using UnityEngine;
using static MainMenuDialog;

public class MainMenuButton : ButtonUnit
{
    public enum Button
    {
        NewGame,
        LoadGame,
        Multiplayer,
        Settings,
        Exit
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
            case Button.NewGame:
                ButtonClickAction = ButtonClickActionNewGame;
                break;

            case Button.LoadGame:
                ButtonClickAction = ButtonClickActionLoadGame;
                break;
            case Button.Multiplayer:
                ButtonClickAction = ButtonClickActionMultiplayer;
                break;
            case Button.Settings:
                ButtonClickAction = ButtonClickActionSettings;
                break;
            case Button.Exit:
                ButtonClickAction = ButtonClickActionExit;
                break;
            default:
                ButtonClickAction = ButtonClickActionNone;
                break;
        }
    }

    private void ButtonClickActionNewGame()
    {
        menu.controller.SetMenu(Menu.WorldList);
    }

    private void ButtonClickActionLoadGame()
    {
        menu.StartDialog(DialogType.LoadGame);
    }
    private void ButtonClickActionMultiplayer()
    {
        menu.controller.SetMenu(Menu.Multiplayer);
    }
    private void ButtonClickActionSettings()
    {
        menu.controller.SetMenu(Menu.Settings);
    }

    private void ButtonClickActionExit()
    {
        //Debug.Log("YES QUIT!");
        Application.Quit();
    }

    public void ButtonClick()
    {
        ButtonClickAction();
    }
}