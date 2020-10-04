using LeopotamGroup.Math;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField, HideInInspector] private ChunkUnit[,] chunks;
    [SerializeField, HideInInspector] private Vector3 posObj;
    [SerializeField, HideInInspector] private Vector2Int posZero;

    [SerializeField] private GameObject chunk;

    [SerializeField] private int width;
    [SerializeField] private int height;

    //[SerializeField] private Text progress;

    [SerializeField, HideInInspector] private int chunkSize;
    private void Awake()
    {
        chunkSize = GameConstants.chunkSize;
        
        BuildChunks();       

        RefreshPos();
    }
    private void RefreshPos()
    {
        posObj = transform.position;
        posZero = new Vector2Int(Mathf.FloorToInt(posObj.x), Mathf.FloorToInt(posObj.y));
    }
    private void BuildChunks()
    {
        chunks = new ChunkUnit[width, height];
        Vector3 posZero = transform.position;
        GameObject chunkObj;
        ChunkUnit chunkUnit;

        //float N = width * height;
        int n = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {               
                chunkObj = Instantiate(chunk);
                chunkObj.transform.parent = transform;
                chunkObj.transform.position = posZero + new Vector3(i * chunkSize, j* chunkSize);
                chunkObj.name = "Chunk(" + i + ", " + j + ")";
                //Debug.Log(new Vector3(i * chunkSize, j * chunkSize));

                chunkUnit = chunkObj.GetComponent<ChunkUnit>();
                chunkUnit.chunkManager = this;
                chunks[i, j] = chunkUnit;

                n++;
                //progress.text = n / N * 100 + "%";
                //Debug.Log("World Generated: " + n / N * 100 + "%; " + n);
            }
        }
    }
    public ChunkUnit GetChunk(Vector3 pos)
    {       
        if (InBounds(pos))
        {
            Vector2Int posInt = new Vector2Int(MathFast.FloorToInt(pos.x), MathFast.FloorToInt(pos.y));

            Vector2Int pointPos = posZero - posInt;
            int i = Mathf.Abs(Mathf.FloorToInt(pointPos.x / chunkSize)),
                j = Mathf.Abs(Mathf.FloorToInt(pointPos.y / chunkSize));

            //Debug.Log("GetChunk Array Pos: " + i + ", " + j + " ;GetChunk Pos: " + posInt);

            return chunks[i, j];
        }

        return null;       
    }
    public bool InBounds(Vector3 pos)
    {
        return pos.x >= posObj.x && pos.x < chunkSize * width && pos.y >= posObj.y && pos.y < chunkSize * height;
    }
    //private void Moving(int v, int h)
    //{
    //    Vector3 pos = transform.position;

    //    pos.x += v * chunkSize;
    //    pos.y += h * chunkSize;

    //    transform.position = pos;
    //}
    //public void BecameInvisible(ChunkUnit chunk)
    //{        
    //    chunksUnloaded.Add(chunk);
    //    Debug.Log("Reported" + chunksUnloaded.Count);
    //    if (chunksUnloaded.Count == 4)
    //    {
    //        Moving(1, 0);
    //        for (int i = 0; i < 4; i++)
    //        {
    //            chunksUnloaded[i].SetActive(true);
    //            chunksUnloaded.RemoveAt(i);
    //        }
            
    //    }      
    //}
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        int sizeX = chunkSize * width;
        int sizeY = chunkSize * height;

        Gizmos.color = Color.green;

        Gizmos.DrawLine(pos, pos + new Vector3(sizeX, 0));
        Gizmos.DrawLine(pos + new Vector3(0, sizeY), pos + new Vector3(sizeX, sizeY));
        Gizmos.DrawLine(pos, pos + new Vector3(0, sizeY));
        Gizmos.DrawLine(pos + new Vector3(sizeX, 0), pos + new Vector3(sizeX, sizeY));
    }
#endif
}
