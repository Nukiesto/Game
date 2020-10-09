using UnityEngine;

public class ComponentToggleVisible : MonoBehaviour
{
    [SerializeField] private Renderer component;

    private void OnBecameInvisible()
    {
        //Debug.Log("IsDisabled");
        component.enabled = false;
        
    }
    private void OnBecameVisible()
    {
        //Debug.Log("IsEnabled");
        component.enabled = true;        
    } 
}
