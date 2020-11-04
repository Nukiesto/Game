using UnityEngine;
using static MainMenuDialog;

public class MainMenuButton : ButtonUnit
{
    public enum Button
    {
        SoloGame,
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
            case Button.SoloGame:
                ButtonClickAction = ButtonClickActionSoloGame;
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

    private void ButtonClickActionSoloGame()
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
        ButtonClickAction?.Invoke();
    }
}