using UnityEngine;

public abstract class DialogUnit : MonoBehaviour
{
    public enum ButtonType
    {
        Yes,
        No,
        Cancel
    }

    [HideInInspector] public MenuUnit menu;

    public abstract dynamic GetTypeDialog();

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}