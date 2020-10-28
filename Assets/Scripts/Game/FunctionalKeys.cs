using Singleton;
using UnityEngine;

public class FunctionalKeys : MonoBehaviour
{
    
    private int _counter;

    [Header("F1-Toggle UI")]
    private bool _enableUi = true;
    [SerializeField] private Canvas[] canvases;
    [SerializeField] private SpriteRenderer[] spritesToToggle;
    [SerializeField] private GameObject[] objectsToToggle;

    [Header("F3-Toggle UI")]
    private bool _enableUiFunc = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _enableUi = !_enableUi;
            for (var i = 0; i < canvases.Length; i++)
            {
                canvases[i].enabled = _enableUi;
            }            
            for (var i = 0; i < spritesToToggle.Length; i++)
            {
                spritesToToggle[i].enabled = _enableUi;
            }
            for (var i = 0; i < objectsToToggle.Length; i++)
            {
                objectsToToggle[i].SetActive(_enableUi);
            }
            return;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            _enableUiFunc = !_enableUiFunc;
            Toolbox.Instance.mFpscounter.enabled = _enableUiFunc;
            return;
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            TakeThumbnail.Generate(Application.persistentDataPath + "/img " + _counter + ".png", null, 0.5f);
            _counter++;
        }
    }
}
