using UnityEngine;

public class ChunkUnit : MonoBehaviour
{
    //Inspector
    [SerializeField] private BlockUnit[] blockInit;

    //Data
    private BlockUnit[,] blocks;

    void Start()
    {
        InitBlocks();
    }

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
        for (int i = 0; i < blockInit.Length; i++)
        {
            BlockUnit block = blockInit[i];
            block.SetDrawing(!block.IsDrawing());
        }
    }
    [ContextMenu("SetDisableRandomBlock")]
    public void SetDisableRandomBlock()
    {
        SetActiveBlock(Random.Range(0, 8), Random.Range(0, 8), false);
    }
    #endregion
    private void InitBlocks()
    {
        blocks = new BlockUnit[8, 8];
        int n = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                blocks[i, j] = blockInit[n];
                n++;
            }
        }
    }
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
    public void SetActiveAllBlocks(bool value)
    {
        for (int i = 0; i < blockInit.Length; i++)
        {
            blockInit[i].SetActive(value);
        }      
    }
    public void SetActiveBlock(int i, int j, bool value)
    {
        blocks[i, j].SetActive(value);
    }
    public void SetBlock(int i, int j, BlockData data)
    {
        SetActiveBlock(i, j, true);
        blocks[i, j].SetData(data);
    }
}
