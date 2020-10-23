using UnityEngine;

public class MultiplayerMenuDialog : DialogUnit
{
    public enum DialogType
    {
        NewGame,
        LoadGame,
        BackToMainMenu
    }

    public DialogType type;

    public override dynamic GetTypeDialog()
    {
        return type;
    }

    public void ClickNewGame(string buttonType)
    {
        if (buttonType == ButtonType.Yes.ToString())
        {
            menu.controller.SetGame();
        }
        if (buttonType == ButtonType.No.ToString())
        {
            menu.controller.CloseCurrentDialog();
        }
        if (buttonType == ButtonType.Cancel.ToString())
        {
            menu.controller.CloseCurrentDialog();
        }
    }

    public void ClickLoadGame(string buttonType)
    {
        if (buttonType == ButtonType.Cancel.ToString())
        {
            menu.controller.CloseCurrentDialog();
        }
    }
}