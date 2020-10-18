using UnityEngine.SceneManagement;

public class PauseButton : ButtonUnit
{
    public enum ButtonType
    {
        Continue,
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
        PauseMenu menu_ = (PauseMenu)menu;
        menu_.pauseMenuToggle.TogglePauseMenu(false);
    }

    private void ButtonClickActionSettings()
    {
        menu.controller.SetMenu(Menu.Settings);
    }

    private void ButtonClickActionMainMenu()
    {
        MoveToMainMenu();
    }

    private void MoveToMainMenu()
    {
        menu.controller.SetMainMenu();
    }

    public void ButtonClick()
    {
        ButtonClickAction();
    }
}