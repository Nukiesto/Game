using LeopotamGroup.Common;
using LeopotamGroup.Math;
using UnityEngine;

public class BlockSelector : MonoBehaviourBase
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

        pos.x = MathFast.Floor(pos.x) + 0.5f;
        pos.y = MathFast.Floor(pos.y) + 0.5f;
        pos.z = 0;

        transform.position = pos;
    }
}
