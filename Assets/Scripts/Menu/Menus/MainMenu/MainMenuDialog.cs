
public class MainMenuDialog : DialogUnit
{
    public enum DialogType
    {
        NewGame,
        LoadGame,
        NoSaveGame
    }

    public DialogType type;

    public override dynamic GetTypeDialog()
    {
        return type;
    }
}
