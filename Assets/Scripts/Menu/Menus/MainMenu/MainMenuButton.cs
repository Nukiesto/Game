
using UnityEngine;
using static MainMenuDialog;

public class MainMenuButton : ButtonUnit
{
    public enum Button
    {
        NewGame,       
        LoadGame,
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
        menu.StartDialog(DialogType.NewGame);
    }
    private void ButtonClickActionLoadGame()
    {
        menu.StartDialog(DialogType.LoadGame);
    }
    private void ButtonClickActionSettings()
    {
        menuController.SetMenu(Menu.Settings);
    }
    private void ButtonClickActionExit()
    {
        Debug.Log("YES QUIT!");
        Application.Quit();
    }
    
    public void ButtonClick()
    {
        ButtonClickAction();
    }
}
