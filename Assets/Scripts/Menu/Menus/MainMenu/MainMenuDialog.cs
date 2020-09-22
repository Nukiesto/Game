
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuDialog : DialogUnit
{
    public enum DialogType
    {
        NewGame,
        LoadGame,
        NoSaveGame
    }

    public DialogType type;
    
    public override dynamic GetTypeDialog()
    {
        return type;
    }

    public void ClickNewGame(string buttonType)
    {
        if (buttonType == ButtonType.Yes.ToString())
        {
            MainMenu a = (MainMenu)menu;

            SceneManager.LoadScene(a.gameSceneName);
        }
        if (buttonType == ButtonType.No.ToString())
        {
            menu.controller.CloseCurrentDialog();
        }
        if (buttonType == ButtonType.Cancel.ToString())
        {
            menu.controller.CloseCurrentDialog();
        }
    }
    public void ClickLoadGame(string buttonType)
    {
        if (buttonType == ButtonType.Yes.ToString())
        {
            if (PlayerPrefs.HasKey("SavedLevel"))
            {//Если есть сохранения игры то загружаем их
             //Debug.Log("I WANT TO LOAD THE SAVED GAME");
             //levelToLoad = PlayerPrefs.GetString("SavedLevel");
             //SceneManager.LoadScene(levelToLoad);
            }
            else
            {//Если их нет выводим диалог об их отсувствии                
                MainMenu a = (MainMenu)menu;
                a.StartDialog(DialogType.NoSaveGame);
            }
        }
        if (buttonType == ButtonType.No.ToString())
        {
            menu.controller.CloseCurrentDialog();
        }
        if (buttonType == ButtonType.Cancel.ToString())
        {
            menu.controller.CloseCurrentDialog();
        }
    }
}


