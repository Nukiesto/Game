
using static SettingsDialog;

public class SettingsMenu : MenuUnit
{
    public void StartDialogLocal(DialogType type)
    {
        foreach (var dialog in dialogs)
        {
            if (dialog.GetTypeDialog() == type)
            {
                dialog.SetActive(true);
                AddDialog(dialog);
            }
        }
    }

    public override void StartDialog(dynamic type)
    {
        base.StartDialog();
        StartDialogLocal((DialogType)type);
    }
}
