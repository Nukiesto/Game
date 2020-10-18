using UnityEngine;
using static PauseDialog;

public class PauseMenu : MenuUnit
{
    public string sceneMainMenu;
    [SerializeField] internal PauseMenuToggleController pauseMenuToggle;

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