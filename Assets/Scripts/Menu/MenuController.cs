using UnityEngine;
using UnityEngine.SceneManagement;

public enum Menu
{
    Main,
    Pause,
    Settings
}
public enum CurrentScene
{
    Game,
    MainMenu
}
public enum ButtonDialog
{
    Yes,
    No,
    Cancel
}

public class MenuController : MonoBehaviour
{
    //Основные параметры
    private bool allowDisableMenu;

    [SerializeField] private MenuUnit[] menu;

    [SerializeField] private CurrentScene currentScene;

    [Header("Меню")]
    private MenuUnit currentMenu;
    private MenuUnit previousMenu;
    private MenuUnit startMenu;

    public void SetMainMenu()
    {
        allowDisableMenu = false;

        SceneManager.LoadScene("MainMenu");

        SetMenu(Menu.Main);
        startMenu = currentMenu;
        SetActiveCurrentMenu(true);

        if (currentScene == CurrentScene.Game)
        {
            SceneManager.UnloadSceneAsync("Game");
        }       

        currentScene = CurrentScene.MainMenu;      
    }
    public void SetGame()
    {
        allowDisableMenu = true;        

        SceneManager.LoadSceneAsync("Game");

        if (currentScene == CurrentScene.MainMenu)
        {
            CloseCurrentDialog();
            SceneManager.UnloadSceneAsync("MainMenu");
        }

        SetMenu(Menu.Pause);
        startMenu = currentMenu;

        SetActiveCurrentMenu(false);      

        currentScene = CurrentScene.Game;        
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        switch (currentScene)
        {
            case CurrentScene.Game:
                SetGame();
                break;
            case CurrentScene.MainMenu:
                SetMainMenu();
                break;
            default:
                SetMainMenu();
                break;
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
                        Debug.LogWarning("StartMenu null");
                        //SetMenu();
                        //startMenu = currentMenu;
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