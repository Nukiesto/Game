using UnityEngine;

public abstract class DialogUnit : MonoBehaviour
{
    public abstract dynamic GetTypeDialog();
    
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}