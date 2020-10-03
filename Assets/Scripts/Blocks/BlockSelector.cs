using LeopotamGroup.Common;
using LeopotamGroup.Math;
using UnityEngine;

public class BlockSelector : MonoBehaviourBase
{
    [Header("Компоненты")]
    [SerializeField] private Camera cameraMain;
    [SerializeField] private ChunkManager chunkManager;

    [Header("Другое")]
    [SerializeField] private BlockData blockToSet;
    [SerializeField, HideInInspector] private Vector3 onWorldPos;

    void Update()
    {
        onWorldPos = GetPoint();
        AlignPositionGrid(onWorldPos);

        ClickBlock();
    }
    private void ClickDeleteBlock()
    {
        ChunkUnit chunk = chunkManager.GetChunk(onWorldPos);
        //Debug.Log(chunk);
        chunk.DeleteBlock(onWorldPos, true);
    }
    private void ClickPlaceBlock()
    {
        ChunkUnit chunk = chunkManager.GetChunk(onWorldPos);
        //Debug.Log(chunk);
        chunk.SetBlock(onWorldPos, blockToSet, true);
    }

    private void ClickBlock()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickDeleteBlock();
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            ClickPlaceBlock();
            return;
        }       
    }
    private void AlignPositionGrid(Vector3 pos)
    {
        pos.x = MathFast.Floor(pos.x) + 0.5f;
        pos.y = MathFast.Floor(pos.y) + 0.5f;
        pos.z = 0;

        transform.position = pos;
    }

    private Vector3 GetPoint()
    {
        Vector3 pos = cameraMain.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);

        pos.z = 0;

        return pos;
    }
}
