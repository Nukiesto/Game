using LeopotamGroup.EditorHelpers;
using UnityEngine;

public class FunctionalKeys : MonoBehaviour
{
    
    private int counter;

    [Header("F1-Toggle UI")]
    private bool enableUI = true;
    [SerializeField] private Canvas[] canvases;
    [SerializeField] private FpsCounter fpsCounter;
    [SerializeField] private SpriteRenderer blockSelector;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            enableUI = !enableUI;
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].enabled = enableUI;

            }
            fpsCounter.enabled = enableUI;
            blockSelector.enabled = enableUI;
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            TakeThumbnail.Generate(Application.persistentDataPath + "/img " + counter + ".png", null, 0.5f);
            counter++;
            return;
        }
    }
}
