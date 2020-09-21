using UnityEngine;

public abstract class MenuUnit : MonoBehaviour
{
    public ButtonUnit[] buttons;
    public DialogUnit[] dialogs;

    public Menu menuType;

    private MenuController controller;

    private void Start()
    {
        controller = FindObjectOfType<MenuController>();
        foreach (var button in buttons)
        {
            button.menuController = controller;
        }
        foreach (var button in buttons)
        {
            button.menu = this;
        }
    }
    public virtual void StartDialog(dynamic type = null)
    {
        SetActiveButtons(false);
    }
    public virtual void SetActiveMenu(bool value)
    {
        gameObject.SetActive(value);
        foreach (var dialog in dialogs)
        {
            dialog.SetActive(false);
        }
    }

    public void SetActiveButtons(bool value)
    {
        foreach (var button in buttons)
        {
            button.SetActive(value);
        }
    }
    public void SetActiveDialogs(bool value)
    {
        foreach (var dialog in dialogs)
        {
            dialog.SetActive(value);
        }
    }
}
