using System;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryType
{
    fast,
    main
}

public class Inventory : MonoBehaviour
{
    private bool isOpenMain = false;

    //Fast Panel
    [SerializeField] private FastItems fastItems;

    //Main Panel
    [SerializeField] private MainItems mainItems;

    //Item select
    internal ItemSelect itemSelect;
    [SerializeField] internal GameObject selector;

    //Dragging
    private bool dragging;
    private ItemUnit dragging_item;
    private Texture2D dragging_texture;
    private Vector3 curPos;

    private Dictionary<InventoryType, PanelItemsBase> panels;
    public class PanelItemsBase
    {
        [SerializeField] private UIItemPanel uipanel;
        private int maxCount;
        private List<ItemUnit> items;//Основная панель предметов
        public Inventory inventory;

        public void SetActive(bool value)
        {
            uipanel.gameObject.SetActive(value);
        }
        public void Init()
        {
            maxCount = uipanel.slots.slots.Length;
            items = new List<ItemUnit>();
            for (int i = 0; i < maxCount; i++)
            {
                ItemUnit unit = new ItemUnit() { uislot = uipanel.slots.slots[i] };
                unit.inventory = inventory;
                unit.uislot.type = InventoryType.fast;
                unit.uislot.inventory = inventory;
                unit.Reset();
                items.Add(unit);
            }
        }
        public ItemData.Data GetItemData(int n)
        {
            return items[n].data;
        }

        public ItemUnit GetItemUnit(UIItemPanelSlot itemSelectUI)
        {
            for (int i = 0; i < maxCount; i++)
            {
                ItemUnit unit = items[i];
                if (unit.uislot == itemSelectUI)
                {
                    return unit;
                }
            }
            return null;
        }
        public int AddItemCount(ItemData.Data data, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (!AddItem(data))
                {
                    return i;
                }
            }
            return 0;
        }
        public bool AddItem(ItemData.Data data)
        {
            //Debug.Log("StartFinding Had");
            for (int i = 0; i < maxCount; i++)
            {
                //Debug.Log("i:" + i + " " + fastPanel[i].data?.name.translations[0].text + "; " + data.name.translations[0].text);
                if (items[i].data == data)
                {
                    items[i].AddItem();
                    Debug.Log("Is Had");
                    return true;
                }
            }
            //Debug.Log("StartFinding Null");
            for (int i = 0; i < maxCount; i++)
            {
                //Debug.Log("i:" + i + " "+ fastPanel[i].data);
                if (!items[i].HasData())
                {
                    items[i].SetData(data);
                    Debug.Log("NULL");
                    return true;
                }
            }
            return false;
            //return ItemAddMain(data);
        }
    }
    [Serializable]
    public class MainItems : PanelItemsBase
    {

    }
    [Serializable]
    public class FastItems : PanelItemsBase
    {

    }
    public class ItemSelect 
    {
        public PanelItemsBase panel;
        public InventoryType panelType;

        public ItemUnit unit;
        public UIItemPanelSlot itemSelectUI;
    }

    void Start()
    {
        mainItems.SetActive(isOpenMain);
        panels = new Dictionary<InventoryType, PanelItemsBase>() {
            { InventoryType.fast, fastItems },
            { InventoryType.main, mainItems },
        };
        for (InventoryType i = 0; (int)i < panels.Count; i++)
        {
            panels[i].Init();
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleOpenMain();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
        {
            if (itemSelect.panelType == InventoryType.fast & itemSelect.unit.data != null)
            {
                MoveItems(itemSelect.unit, InventoryType.fast, InventoryType.main);
            }
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.W))
        {
            if (itemSelect.panelType == InventoryType.main & itemSelect.unit.data != null)
            {
                MoveItems(itemSelect.unit, InventoryType.main, InventoryType.fast);
            }
        }
    }

    private void ToggleOpenMain()
    {
        isOpenMain = !isOpenMain;
        mainItems.SetActive(isOpenMain);

        if (itemSelect.itemSelectUI != null && itemSelect.panelType == InventoryType.main)
        {
            selector.SetActive(false);
        }
    }

    public void SetItemSelect(UIItemPanelSlot item)
    {
        if (itemSelect == null)
            itemSelect = new ItemSelect();

        itemSelect.itemSelectUI = item;
        itemSelect.unit = panels[itemSelect.panelType].GetItemUnit(item);
        itemSelect.panelType = item.type;

        selector.SetActive(true);
        selector.transform.position = item.transform.position;
    }
    public void AddItem(ItemData.Data data)
    {
        for (InventoryType i = 0; (int)i < panels.Count; i++)
        {
            if (panels[i].AddItem(data))
            {
                break;
            }
        }
    }
    public void AddItemCount(ItemData.Data data, int count)
    {
        int tempCount = count;
        for (InventoryType i = 0; (int)i < count; i++)
        {
            int mod = panels[i].AddItemCount(data, tempCount);
            if (mod == 0)
            {
                break;
            }
            else
            {
                tempCount = mod;
            }
        }
    }
    public void MoveItems(ItemUnit unit, InventoryType from,InventoryType to)
    {
        int a = panels[to].AddItemCount(unit.data, unit.Count);
        if (a > 0)
        {
            panels[from].AddItemCount(unit.data, a);
        }

        unit.Clear();
        unit.Reset();
    }
    public ItemUnit GetSelectedItem()
    {
        return itemSelect.unit;
    }
    void OnGUI()
    {
        if (dragging)
        {
            // перемещение иконки на экране
            Vector2 mousePos = Event.current.mousePosition;
            GUI.depth = 999; // поверх остальных элементов
            float shift = 50 / 2;
            GUI.Label(new Rect(mousePos.x - shift, mousePos.y - shift, 50, 50), dragging_texture);
        }
    }
}

[Serializable]
public class ItemUnit
{
    public ItemData.Data data = null;
    public UIItemPanelSlot uislot;
    internal Inventory inventory;

    public int Count { get; private set; }
    public int MaxCount { get; private set; }
    public void Reset()
    {
        data = null;
        Count = 0;
        uislot.SetCount(0);
    }
    public void SetCount(int n)
    {
        Count = n;
        uislot.SetCount(0);
    }
    public void SetData(ItemData.Data data)
    {
        Reset();

        this.data = data;
        MaxCount = data.maxCount;
        uislot.SetSprite(data.sprite);

        Count++;
        uislot.SetCount(Count);
    }
    public void AddItem()
    {
        if (Count < MaxCount)
        {
            Count++;
            uislot.SetCount(Count);
        }
    }
    public bool HasData()
    {
        return data != null;
    }
    public void RemoveItem()
    {
        if (Count > 0)
        {           
            Count--;
            uislot.SetCount(Count);
            if (Count > 0)
            {
                return;
            }
        }
        if (inventory.itemSelect.unit == this)
        {
            uislot.SetIconDisabled();
            Reset();
        }
    }    
    public void Clear()
    {
        inventory.itemSelect = null;
        uislot.SetIconDisabled();
    }
}

