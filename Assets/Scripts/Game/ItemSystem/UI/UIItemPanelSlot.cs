using Game.ItemSystem;
using UnityEngine;
using UnityEngine.UI;

public class UIItemPanelSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] public Text count;
    public InventoryType type;
    public Inventory inventory;
    public void SetSprite(Sprite sprite)
    {
        if (sprite != null)
        {
            icon.enabled = true;
            icon.sprite = sprite; 
        }
        else
        {
            icon.enabled = false;
        }
        
               
    }
    public void SetIconDisabled()
    {
        icon.enabled = false;
    }
    public void SetCount(int countSet)
    {
        //Debug.Log("Set Count:" + count);
        count.text = countSet != 0 ? countSet.ToString() : "";        
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

    public void SetActiveCountText(bool value)
    {
        count.enabled = value;
    }
}
