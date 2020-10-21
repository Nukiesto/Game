using UnityEngine;

public class FunctionalKeys : MonoBehaviour
{
    
    private int counter;

    [Header("F1-Toggle UI")]
    private bool enableUI = true;
    [SerializeField] private Canvas[] canvases;
    [SerializeField] private SpriteRenderer[] spritesToToggle;
    [SerializeField] private GameObject[] objectsToToggle;

    [Header("F3-Toggle UI")]
    private bool enableUIFunc = false;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            enableUI = !enableUI;
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].enabled = enableUI;
            }            
            for (int i = 0; i < spritesToToggle.Length; i++)
            {
                spritesToToggle[i].enabled = enableUI;
            }
            for (int i = 0; i < objectsToToggle.Length; i++)
            {
                objectsToToggle[i].SetActive(enableUI);
            }
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            enableUIFunc = !enableUIFunc;
            Toolbox.instance.MFpscounter.enabled = enableUIFunc;
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            TakeThumbnail.Generate(Application.persistentDataPath + "/img " + counter + ".png", null, 0.5f);
            counter++;
            return;
        }
    }
}
