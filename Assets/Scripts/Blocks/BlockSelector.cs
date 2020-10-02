using UnityEngine;

public class BlockSelector : MonoBehaviour
{
    [SerializeField] private Camera cameraMain;

    void Start()
    {

    }

    void Update()
    {
        transform.position = cameraMain.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);

        AlignPositionGrid();
    }

    private void AlignPositionGrid()
    {
        var pos = transform.position;

        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f;
        pos.z = 0;

        transform.position = pos;
    }
}
