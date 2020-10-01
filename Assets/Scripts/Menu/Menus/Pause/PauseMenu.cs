using static PauseDialog;

public class PauseMenu : MenuUnit
{
    public string sceneMainMenu;

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