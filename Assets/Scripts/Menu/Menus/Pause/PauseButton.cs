
using UnityEngine.SceneManagement;
using static PauseDialog;

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
        PauseMenu a = (PauseMenu)menu;

        SceneManager.LoadScene(a.sceneMainMenu);
    }
    public void ButtonClick()
    {
        ButtonClickAction();
    }
}
