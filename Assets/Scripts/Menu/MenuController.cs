
using UnityEngine;

public enum Menu
{
    Main,
    Settings
}

public class MenuController : MonoBehaviour
{       
    public enum ButtonDialog
    {
        Yes,
        No,
        Cancel
    }
    
    public Menu startMenu;

    public MenuUnit[] menu;

    private MenuUnit currentMenu;
    private MenuUnit previousMenu;

    void Start()
    {
        SetMenu(startMenu);
    }

    void Update()
    {

    }    
    
    public void SetMenu(Menu menu)
    {
        currentMenu?.SetActiveMenu(false);

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
    public void SetPreviosMenu()
    {
        if (previousMenu != null)
        {
            var a = currentMenu;
            currentMenu.SetActiveMenu(false);
            currentMenu = previousMenu;
            previousMenu = a;

            currentMenu.SetActiveMenu(true);
        }        
    }
    public void CloseDialog()
    {
        currentMenu.SetActiveDialogs(false);
        currentMenu.SetActiveButtons(true);
    }
}
