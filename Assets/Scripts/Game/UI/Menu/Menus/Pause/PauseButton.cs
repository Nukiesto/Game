using Singleton;
using UnityEngine.SceneManagement;

public class PauseButton : ButtonUnit
{
    public enum ButtonType
    {
        Continue,
        SaveGame,
        Settings,
        MainMenu
    }

    public ButtonType buttonType;

    private void Start()
    {
        InitDelegButtonClick();
    }

    private void InitDelegButtonClick()
    {
        switch (buttonType)
        {
            case ButtonType.Continue:
                ButtonClickAction = ButtonClickActionContinue;
                break;
            
            case ButtonType.SaveGame:
                ButtonClickAction = ButtonClickActionSaveGame;
                break;
            
            case ButtonType.Settings:
                ButtonClickAction = ButtonClickActionSettings;
                break;
            
            
            case ButtonType.MainMenu:
                ButtonClickAction = ButtonClickActionMainMenu;
                break;

            default:
                ButtonClickAction = ButtonClickActionNone;
                break;
        }
    }
    
    
    private void ButtonClickActionContinue()
    {
        menu.controller.SetActiveCurrentMenu(false);
        var pauseMenu = (PauseMenu)menu;
        pauseMenu.pauseMenuToggle.TogglePauseMenu(false);
    }
    private void ButtonClickActionSaveGame()
    {
        Toolbox.Instance.mWorldSaver.SaveWorld();
        
        menu.controller.SetActiveCurrentMenu(false);
        var pauseMenu = (PauseMenu)menu;
        pauseMenu.pauseMenuToggle.TogglePauseMenu(false);
    }

    private void ButtonClickActionSettings()
    {
        menu.controller.SetMenu(Menu.Settings);
    }

    private void ButtonClickActionMainMenu()
    {
        var toolbox = Toolbox.Instance;
        toolbox.mWorldSaver.SaveWorld();
        toolbox.InitGame = null;
        
        MoveToMainMenu();
    }

    private void MoveToMainMenu()
    {
        Toolbox.Instance.mFpscounter.enabled = false;
        menu.controller.SetMainMenu();
    }

    public void ButtonClick()
    {
        ButtonClickAction();
    }
}