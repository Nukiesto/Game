using UnityEngine;

public enum Menu
{
    Main,
    Pause,
    Settings,
    WorldList
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

    [Header("Меню")]
    [SerializeField] private MenuUnit[] menu;

    [Header("Параметры")]
    [SerializeField] private GameScene menuType;

    private MenuUnit currentMenu;
    private MenuUnit previousMenu;
    private MenuUnit startMenu;

    private GameSceneManager gameSceneManager;
    
    public void SetMainMenu()
    {
        allowDisableMenu = false;

        gameSceneManager.SetScene(GameScene.MainMenu);

        SetMenu(Menu.Main);
        startMenu = currentMenu;
        SetActiveCurrentMenu(true);

        gameSceneManager.SetSceneCurrent();
    }
    public void SetGame()
    {
        allowDisableMenu = true;        

        gameSceneManager.SetScene(GameScene.Game);
        if (gameSceneManager.CurrentScene == GameScene.MainMenu)
        {
            CloseCurrentDialog();
            gameSceneManager.SetSceneCurrent();
        }

        SetMenu(Menu.Pause);
        startMenu = currentMenu;

        SetActiveCurrentMenu(false);           
    }

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        gameSceneManager = Toolbox.instance.GetGameSceneManager();
        switch (menuType)
        {
            case GameScene.Game:
                SetGame();
                break;
            case GameScene.MainMenu:
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