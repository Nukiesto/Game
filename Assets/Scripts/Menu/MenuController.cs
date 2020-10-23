using UnityEngine;
using UnityEngine.EventSystems;

public enum Menu
{
    Main,
    Multiplayer,
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
    [Header("Меню")]
    [SerializeField] private MenuUnit[] menu;
    [SerializeField] private MenuUnit startMenu;

    [Header("Параметры")]
    [SerializeField] private GameScene sceneType;

    [SerializeField] private bool allowToggleMenu;
    [SerializeField] private bool setMenuActiveOnStart;

    private MenuUnit currentMenu;
    private MenuUnit previousMenu;

    private GameSceneManager gameSceneManager;

    [SerializeField] private PauseMenuToggleController pauseMenuToggle;
    public void SetMainMenu()
    {
        gameSceneManager.SetScene(GameScene.MainMenu);
    }
    public void SetGame()
    {
        gameSceneManager.SetScene(GameScene.Game);        
    }

    private void Awake()
    {
        gameSceneManager = Toolbox.instance.MGameSceneManager;
        //Debug.Log(gameSceneManager);

        SetMenu(startMenu.menuType);

        SetActiveCurrentMenu(setMenuActiveOnStart);            
    }

    private void Update()
    {
        CheckExitKey();
    }
    private void CheckExitKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Закрываем открытый диалог
            if (currentMenu != null && currentMenu.DialogIsOpen())
            {
                CloseCurrentDialog();
                //Debug.Log("1");
                return;
            }
            //Перемещаемся назад
            if (currentMenu != startMenu && TrySetPreviosMenu())
            {
                Debug.Log("2");
                return;                                       
            }
            //Открываем или закрываем меню
            if (allowToggleMenu)
            {
                bool val = !currentMenu.ActiveMenu();
                currentMenu.SetActiveMenu(val);
                pauseMenuToggle?.TogglePauseMenu(val);
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