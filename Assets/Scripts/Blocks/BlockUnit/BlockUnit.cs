using LeopotamGroup.Common;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(BlockUnitUpdate))]
[RequireComponent(typeof(BlockUnitMemory))]
public class BlockUnit : MonoBehaviourBase
{
    //Inspector
    [SerializeField] private string blockName;

    //Components
    private SpriteRenderer sprite;
    private BoxCollider2D boxCollider2D;
    private BlockUnitUpdate updater;
    private BlockUnitMemory memory;

    //Data
    public BlockData data;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        memory = GetComponent<BlockUnitMemory>();
        memory.SetBlockUnit(this);

        updater = GetComponent<BlockUnitUpdate>();
        updater.SetMemory(memory);
        SetBlockProperties();
    }

    #region Data
    public void ClearData()
    {
        data = null;
        updater.Remove();
        memory.Remove();
        sprite.sprite = null;
    }
    public void SetData(BlockData data)
    {
        this.data = data;
        SetBlockProperties();
        updater.SetScript(data);
        memory.SetMemory(data);
    }
    #endregion

    private void SetBlockProperties()
    {
        sprite.sprite = data.sprite;
        SetActiveUpdate(data.HasScript());//Переклчючает updater от наличия скрипта update`а
        InitSolid();
    }
    private void InitSolid()
    {
        SetSolid(data.isSolid);
    }
    private void SetSolid(bool value)
    {
        boxCollider2D.enabled = value;
    }
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
    public void SetDrawing(bool value)
    {
        sprite.enabled = value;
    }
    public bool IsDrawing() => sprite.enabled;

    public void SetActiveUpdate(bool value)
    {
        updater.enabled = value;
    }
}
