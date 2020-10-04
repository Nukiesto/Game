using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BlockUnitUpdate))]
[RequireComponent(typeof(BlockUnitMemory))]
public class BlockUnit : MonoBehaviour
{
    //Inspector
    [SerializeField] private string blockName;

    //Components
    private TileBase tile;
    private BlockUnitUpdate updater;
    private BlockUnitMemory memory;

    //Data
    [SerializeField] public BlockData data;

    private void Awake()
    {
        InitComponents();
    }

    private void InitComponents()
    {
        memory = GetComponent<BlockUnitMemory>();
        memory.SetBlockUnit(this);

        updater = GetComponent<BlockUnitUpdate>();
        updater.SetMemory(memory);
    }
    #region Data
    public void ClearData()
    {
        data = null;
        updater.Remove();
        memory.Remove();
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
        SetActiveUpdate(data.HasScript());//Переклчючает updater от наличия скрипта update`а
    }
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetActiveUpdate(bool value)
    {
        updater.enabled = value;
    }
}
