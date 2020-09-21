
public class SettingsDialog : DialogUnit
{
    public enum DialogType
    {
        Graphics,
        Sound,
        GamePlay,
        Controls
    }

    public DialogType type;

    public override dynamic GetTypeDialog()
    {
        return type;
    }
}