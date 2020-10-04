using UnityEngine;
using UnityEngine.UI;

public class UIItemPanelSlot : MonoBehaviour
{
    public ItemData.Data data;

    [SerializeField] private Image icon;

    public void SetData(ItemData data)
    {
        this.data = data.data;
        SetSprite();
    }
    private void SetSprite()
    {
        icon.enabled = true;
        icon.sprite = data.sprite;        
    }
}
