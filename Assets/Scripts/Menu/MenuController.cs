using Singleton;
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

    [SerializeField] private MenuUnit currentMenu;
    private MenuUnit _previousMenu;

    private GameSceneManager _gameSceneManager;

    [SerializeField] private PauseMenuToggleController pauseMenuToggle;
    public void SetMainMenu()
    {
        _gameSceneManager.SetScene(GameScene.MainMenu);
    }
    public void SetGame()
    {
        _gameSceneManager.SetScene(GameScene.Game);
    }

    private void Awake()
    {
        _gameSceneManager = Toolbox.Instance.mGameSceneManager;
        //Debug.Log(gameSceneManager);

        SetMenu(startMenu.menuType);

        SetActiveCurrentMenu(setMenuActiveOnStart);            
    }

    private void Update()
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
            if (currentMenu != startMenu && TrySetPreviousMenu())
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

        _previousMenu = currentMenu;
        foreach (var item in this.menu)
        {
            if (item.menuType == menu)
            {
                currentMenu = item;
                currentMenu.SetActiveMenu(true);
            }
        }
    }

    public bool TrySetPreviousMenu()
    {
        if (_previousMenu != null)
        {
            var a = currentMenu;
            currentMenu.SetActiveMenu(false);
            currentMenu = _previousMenu;
            _previousMenu = a;

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