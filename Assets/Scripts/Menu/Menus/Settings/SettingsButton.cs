using static SettingsDialog;

public class SettingsButton : ButtonUnit
{
    public enum ButtonType
    {
        Graphics,
        Sound,
        GamePlay,
        Controls,
        Back
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
            case ButtonType.Graphics:
                ButtonClickAction = ButtonClickActionGraphics;
                break;

            case ButtonType.Sound:
                ButtonClickAction = ButtonClickActionSound;
                break;

            case ButtonType.GamePlay:
                ButtonClickAction = ButtonClickActionGamePlay;
                break;

            case ButtonType.Controls:
                ButtonClickAction = ButtonClickActionControls;
                break;

            case ButtonType.Back:
                ButtonClickAction = ButtonClickActionBack;
                break;

            default:
                ButtonClickAction = ButtonClickActionNone;
                break;
        }
    }

    private void ButtonClickActionGraphics()
    {
        menu.StartDialog(DialogType.Graphics);
    }

    private void ButtonClickActionSound()
    {
        menu.StartDialog(DialogType.Sound);
    }

    private void ButtonClickActionGamePlay()
    {
        menu.StartDialog(DialogType.GamePlay);
    }

    private void ButtonClickActionControls()
    {
        menu.StartDialog(DialogType.Controls);
    }

    private void ButtonClickActionBack()
    {
        menu.controller.TrySetPreviosMenu();
    }

    public void ButtonClick()
    {
        ButtonClickAction();
    }
}