using UnityEngine;

public class UIItemPanel : MonoBehaviour
{
    public Slots slots;
    [SerializeField] private ItemData itemTest;

    [System.Serializable]
    public class Slots
    {
        public UIItemPanelSlot[] slots;
    }
}
