using LeopotamGroup.Collections;
using LeopotamGroup.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TilemapCollider2D))]
[RequireComponent(typeof(TilemapRenderer))]
public class ChunkUnit : MonoBehaviourBase
{
    //Inspector
    [SerializeField] private BlockUnit blockInit;
    [SerializeField, HideInInspector] private float meshWidth = 16f;
    [SerializeField, HideInInspector] private float meshHeight = 16f;

    //Data
    [SerializeField, HideInInspector] private BlockUnit[,] blocks;

    //Tilemap
    void Start()
    {
        InitComponents();
        InitBlocks();

    }

    #region Mesh
    
    #endregion
    #region ContextMenu
    [ContextMenu("SetEnableAllBlocks")]
    public void SetEnableAllBlocks()
    {
        SetActiveAllBlocks(true);
    }
    [ContextMenu("SetDisableAllBlocks")]
    public void SetDisableAllBlocks()
    {
        SetActiveAllBlocks(false);
    }
    [ContextMenu("ToggleSetDrawAllBlocks")]
    public void ToggleSetDrawAllBlocks()
    {
        //meshRenderer.enabled = !meshRenderer.enabled;
    }
    [ContextMenu("SetDisableRandomBlock")]
    public void SetDisableRandomBlock()
    {
        SetActiveBlock(Random.Range(0, 8), Random.Range(0, 8), false);
    }
    #endregion

    private void InitComponents()
    {
        //meshFilter = GetComponent<MeshFilter>();
        //meshCollider = GetComponent<MeshCollider>();
        //meshRenderer = GetComponent<MeshRenderer>();
    }

    private bool ToBlockDraw(int x, int y)
    {
        return blocks[x, y] != null;
    }
    private void InitBlocks()
    {
        blocks = new BlockUnit[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                int a = Random.Range(0, 2);
                blocks[i, j] = (a == 1) ? blockInit : null;
                //Debug.Log(a + ";" + blocks[i, j]);
            }
        }
    }
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
    public void SetActiveAllBlocks(bool value)
    {
        //for (int i = 0; i < blockInit.Length; i++)
        //{
        //    blockInit[i].SetActive(value);
        //}      
    }
    public void SetActiveBlock(int i, int j, bool value)
    {
        //blocks[i, j]?.SetActive(value);
    }
    public void SetBlock(int i, int j, BlockData data)
    {
        SetActiveBlock(i, j, true);
        blocks[i, j].SetData(data);
    }
#if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;
        int size = GameConstants.chunkSize;

        Gizmos.color = Color.red;
        
        Gizmos.DrawLine(pos, pos + new Vector3(size, 0));
        Gizmos.DrawLine(pos + new Vector3(0, size), pos + new Vector3(size, size));
        Gizmos.DrawLine(pos, pos + new Vector3(0, size));
        Gizmos.DrawLine(pos + new Vector3(size, 0), pos + new Vector3(size, size));
    }
#endif
}
