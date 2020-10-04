using UnityEngine;

public class UIItemPanel : MonoBehaviour
{
    [SerializeField] private Slots slots;
    [SerializeField] private ItemData itemTest;

    private void Awake()
    {
        for (int i = 0; i < slots.slots.Length; i++)
        {
            slots.slots[i].SetData(itemTest);
        }
    }
    [System.Serializable]
    public class Slots
    {
        public UIItemPanelSlot[] slots;
    }
}
