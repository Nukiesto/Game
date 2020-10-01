using static MainMenuDialog;

public class MainMenu : MenuUnit
{
    //public override void SetActiveMenu(bool value)
    //{
    //    base.SetActiveMenu(value);
    //}
    public string gameSceneName;

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