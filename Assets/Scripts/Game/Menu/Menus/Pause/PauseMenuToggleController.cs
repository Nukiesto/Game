using Krivodeling.UI.Effects;
using UnityEngine;

public class PauseMenuToggleController : MonoBehaviour
{
    [SerializeField] private UIBlur system;

    [Range(0, 1)]
    [SerializeField] private float beginBlurTime;
    [Range(0, 1)]
    [SerializeField] private float endBlurTime;

    public void TogglePauseMenu(bool value)
    {
        if (value)
        {
            system.BeginBlur(beginBlurTime);
        }
        else
        {
            system.EndBlur(endBlurTime);
        }
    }
}
