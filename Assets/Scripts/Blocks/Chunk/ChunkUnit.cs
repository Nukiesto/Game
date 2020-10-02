using LeopotamGroup.Collections;
using LeopotamGroup.Common;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class ChunkUnit : MonoBehaviourBase
{
    //Inspector
    [SerializeField] private BlockUnit blockInit;
    [SerializeField, HideInInspector] private float meshWidth = 16f;
    [SerializeField, HideInInspector] private float meshHeight = 16f;

    //Data
    [SerializeField, HideInInspector] private BlockUnit[,] blocks;

    //Mesh
    [SerializeField, HideInInspector] private MeshFilter meshFilter;
    [SerializeField, HideInInspector] private MeshCollider meshCollider;
    [SerializeField, HideInInspector] private MeshRenderer meshRenderer;
    //[SerializeField, HideInInspector] private Mesh mesh;
    void Start()
    {
        InitComponents();
        InitBlocks();
        RebuildMesh();
    }

    #region Mesh
    public void RebuildMesh()
    {
        meshFilter.mesh = new Mesh();
        Mesh mesh = meshFilter.mesh;

        var vertices = new FastList<Vector3>();
        var triangles = new FastList<int>();
        var uvs = new FastList<Vector2>();

        int size = GameConstants.chunkSize;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (ToBlockDraw(x, y))
                {
                    AddQuad(x, y, vertices, triangles, uvs);
                    //SetTile(x, y, x, y, uvs);
                }               
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.SetUVs(0, uvs.ToList());
        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshCollider.sharedMesh = mesh;
    }
    private void AddQuad(float x, float y, FastList<Vector3> vertices, FastList<int> triangles, FastList<Vector2> uvs)
    {
        int vertNum = vertices.Count;
        //vertices.Add(new Vector3(x, y, 0)); uvs.Add(new Vector2(0, 0));
        //vertices.Add(new Vector3(x + 1, y, 0)); uvs.Add(new Vector2(1, 0));
        //vertices.Add(new Vector3(x + 1, y + 1, 0)); uvs.Add(new Vector2(1, 1));
        //vertices.Add(new Vector3(x, y + 1, 0)); uvs.Add(new Vector2(0, 1));

        vertices.Add(new Vector3(x, y, 0)); uvs.Add(new Vector2(x, y));
        vertices.Add(new Vector3(x + 1, y, 0)); uvs.Add(new Vector2(x + 1, y));
        vertices.Add(new Vector3(x + 1, y + 1, 0)); uvs.Add(new Vector2(x + 1, y +1));
        vertices.Add(new Vector3(x, y + 1, 0)); uvs.Add(new Vector2(x, y + 1));

        triangles.AddRange(new int[]{
            vertNum+0, vertNum+3, vertNum+1, // Triangle 1
            vertNum+1, vertNum+3, vertNum+2 // Triangle 2
        });
    }
    /// <summary>
    /// Устанавливаем UV-координаты четырехугольника в данной строке / столбце, чтобы
    /// что он показывает заданную часть текстуры тайловой карты
    /// </summary>
    /// <param name="row">строка (y) квадрата для установки </param>
    /// <param name="col">столбец (x) квадрата для установки </param>
    /// <param name="tileMapRow">строка (y) тайла карты текстуры для использования </param>
    /// <param name="tileMapCol">столбец (x) тайла карты текстуры для использования </param>
    public void SetTile(int row, int col, int tileMapRow, int tileMapCol, FastList<Vector2> uvs)
    {
        int size = GameConstants.chunkSize;
        int quadNum = row * size + col;
        int vertNum = quadNum * 4;
        Debug.Log("Setting " + row + "," + col + " at quad " + quadNum + ", vert " + vertNum);
        float du = 1f / size;
        float dv = 1f / size;
        float u = tileMapCol * du;
        float v = 1 - tileMapRow * dv;
        uvs[vertNum] = new Vector2(u, v);
        uvs[vertNum + 1] = new Vector2(u + du, v);
        uvs[vertNum + 2] = new Vector2(u + du, v - dv);
        uvs[vertNum + 3] = new Vector2(u, v - dv);
    }
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
        meshRenderer.enabled = !meshRenderer.enabled;
    }
    [ContextMenu("SetDisableRandomBlock")]
    public void SetDisableRandomBlock()
    {
        SetActiveBlock(Random.Range(0, 8), Random.Range(0, 8), false);
    }
    #endregion

    private void InitComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
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
                Debug.Log(a + ";" + blocks[i, j]);
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
