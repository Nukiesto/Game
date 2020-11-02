using System.Collections.Generic;
using UnityEngine;

public abstract class MenuUnit : MonoBehaviour
{
    public ButtonUnit[] buttons;
    public DialogUnit[] dialogs;

    public Menu menuType;

    [HideInInspector] public MenuController controller;
    [HideInInspector] public Stack<DialogUnit> dialogsOpened = new Stack<DialogUnit>();

    private void Start()
    {
        controller = FindObjectOfType<MenuController>();
        foreach (var button in buttons)
        {
            button.menu = this;
        }
        foreach (var dialog in dialogs)
        {
            dialog.menu = this;
        }
    }

    public virtual void StartDialog(dynamic type = null)
    {
        SetActiveButtons(false);
    }

    public virtual void SetActiveMenu(bool value)
    {
        gameObject.SetActive(value);
        //foreach (var dialog in dialogs)
        //{
        //    dialog.SetActive(false);
        //}
    }

    public bool ActiveMenu()
    {
        return gameObject.activeSelf;
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

    public void AddDialog(DialogUnit dialog)
    {
        dialogsOpened.Push(dialog);
    }

    public void CloseFirstDialog()
    {
        dialogsOpened.Pop().SetActive(false);
    }

    public bool DialogIsOpen()
    {
        return dialogsOpened.Count > 0;
    }
}