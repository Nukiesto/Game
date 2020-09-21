using UnityEngine;

public class ButtonUnit : MonoBehaviour
{
    [HideInInspector] public MenuController menuController;
    [HideInInspector] public MenuUnit menu;

    protected delegate void delegButtonClickAction();
    protected delegButtonClickAction ButtonClickAction;

    protected void ButtonClickActionNone()
    {

    }
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
