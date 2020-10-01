﻿using UnityEngine;

public enum Menu
{
    Main,
    Pause,
    Settings
}

public enum ButtonDialog
{
    Yes,
    No,
    Cancel
}

public class MenuController : MonoBehaviour
{
    [Header("Основные параметры")]
    public Menu startMenuEnum;
    public bool loadMenuInStart;
    public bool allowDisableMenu;
    public MenuUnit[] menu;

    [Header("Меню")]
    private MenuUnit currentMenu;
    private MenuUnit previousMenu;
    private MenuUnit startMenu;

    private void Start()
    {
        if (loadMenuInStart)
        {
            SetMenu(startMenuEnum);
            startMenu = currentMenu;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentMenu != null && currentMenu.DialogIsOpen())//Если открыт диалог
            {
                CloseCurrentDialog();//Закрываем его
            }
            else
            {
                if (currentMenu != startMenu)//Если текущее меню не явлеется начальным
                {
                    TrySetPreviosMenu();//Пытаемся переместиться на предыдущее меню
                }
                else//Иначе установить его :\
                {
                    if (startMenu == null)
                    {
                        SetMenu(startMenuEnum);
                        startMenu = currentMenu;
                    }
                    else
                    {
                        if (allowDisableMenu)
                        {
                            currentMenu.SetActiveMenu(!currentMenu.ActiveMenu());
                        }
                    }
                }
            }
        }
    }

    public void SetMenu(Menu menu)
    {
        SetActiveCurrentMenu(false);

        previousMenu = currentMenu;
        foreach (var item in this.menu)
        {
            if (item.menuType == menu)
            {
                currentMenu = item;
                currentMenu.SetActiveMenu(true);
            }
        }
    }

    public bool TrySetPreviosMenu()
    {
        if (previousMenu != null)
        {
            var a = currentMenu;
            currentMenu.SetActiveMenu(false);
            currentMenu = previousMenu;
            previousMenu = a;

            currentMenu.SetActiveMenu(true);
            return true;
        }
        return false;
    }

    public void CloseCurrentDialog()
    {
        currentMenu.CloseFirstDialog();//Выключаем диалог
        if (!currentMenu.DialogIsOpen())//Если не открыт диалог
        {
            currentMenu.SetActiveButtons(true);//Включаем кнопки
        }
    }

    public void SetActiveCurrentMenu(bool value)
    {
        currentMenu?.SetActiveMenu(value);
    }

    public bool IsCurrentMenuActive()
    {
        return currentMenu.gameObject.activeSelf;
    }
}