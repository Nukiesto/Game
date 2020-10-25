using UnityEngine;
using UnityEngine.UI;

public class UIItemPanelSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text count;
    public InventoryType type;
    public Inventory inventory;
    public void SetSprite(Sprite sprite)
    {
        icon.enabled = true;
        icon.sprite = sprite;        
    }
    public void SetIconDisabled()
    {
        icon.enabled = false;
    }
    public void SetCount(int count)
    {
        //Debug.Log("Set Count:" + count);
        this.count.text = count != 0 ? count.ToString() : "";        
    }
    public void SlotClicked()
    {
        //Debug.Log("Clicked");
        inventory.SetItemSelect(this);
    }

    private void OnMouseDrag()
    {
        Debug.Log("Dragging");
    }
    private void OnMouseDown()
    {
        Debug.Log("Down");
    }
    private void OnMouseUp()
    {
        Debug.Log("Up");
    }

    public void SetCountTextDisabled()
    {
        count.enabled = false;
    }
}
