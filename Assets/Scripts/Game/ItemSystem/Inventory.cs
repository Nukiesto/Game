using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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
    [SerializeField] internal ItemSelect itemSelect;
    [SerializeField] internal GameObject selector;

    //Dragging
    private bool dragging;
    private ItemUnit dragging_item;
    private Texture2D dragging_texture;
    private Vector3 curPos;

    //Control
    private int rollSelect;
    private List<KeyCode> inventoryKeyCodes;

    //Player
    [SerializeField] private PlayerController player;
    private Dictionary<InventoryType, PanelItemsBase> panels;
    [Serializable]
    public abstract class PanelItemsBase
    {
        [SerializeField] private UIItemPanel uipanel;
        internal int maxCount;
        internal List<ItemUnit> items;//Основная панель предметов
        public Inventory inventory;
        public abstract InventoryType InventoryType { get; }

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
                unit.uislot.inventory = inventory;
                SetTypeItem(unit);
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
            ItemUnit unit;
            for (int i = 0; i < maxCount; i++)
            {
                //Debug.Log("i:" + i + " " + fastPanel[i].data?.name.translations[0].text + "; " + data.name.translations[0].text);
                unit = items[i];
                if (unit.data == data)
                {
                    if (unit.AddItem())
                    {
                        UpdateItemSelect(unit);
                        //Debug.Log("Is Had");
                        return true;
                    }                    
                }
            }
            //Debug.Log("StartFinding Null");
            for (int i = 0; i < maxCount; i++)
            {
                unit = items[i];
                //Debug.Log("i:" + i + " "+ fastPanel[i].data);
                if (!unit.HasData())
                {
                    UpdateItemSelect(unit);
                    unit.SetData(data);
                    //Debug.Log("NULL");
                    return true;
                }
            }
            return false;
            //return ItemAddMain(data);
        }
        public void UpdateItemSelect(ItemUnit unit)
        {
            //if (inventory.itemSelect.unit == unit)
            //{
            //    inventory.itemSelect.unit = unit;
            //    inventory.itemSelect.unit.data = unit.data;
            //    inventory.itemSelect.unit.MaxCount = unit.data.maxCount;

            //    inventory.itemSelect.unit.Count++;
            //}
        }
        protected abstract void SetTypeItem(ItemUnit unit);
    }
    [Serializable]
    public class MainItems : PanelItemsBase
    {
        public override InventoryType InventoryType => InventoryType.main;

        protected override void SetTypeItem(ItemUnit unit)
        {
            //Debug.Log("inv: " + this + "unit: " + unit + ";" + InventoryType);
            unit.type = InventoryType;
        }
    }
    [Serializable]
    public class FastItems : PanelItemsBase
    {
        public override InventoryType InventoryType => InventoryType.fast;

        protected override void SetTypeItem(ItemUnit unit)
        {
            //Debug.Log("inv: " + this + "unit: " + unit + ";" + InventoryType);
            unit.type = InventoryType;
        }
    }
    [Serializable]
    public class ItemSelect 
    {
        public PanelItemsBase panel;

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
            //Debug.Log("InvType: " + panels[i].InventoryType);
            panels[i].Init();
        }

        InitKeys();
        InitItemSelect();       
    }
    void Update()
    {
        InputUpdate();
    }
    #region Input
    private void KickSelectItem()
    {
        if (itemSelect != null && itemSelect.unit != null && itemSelect.unit.data != null)
        {
            if (player.CanToCreateItem())
            {
                player.CreateItemKick(itemSelect.unit.data, 1);
                ItemUnit unit = itemSelect.unit;

                unit.RemoveItem();
            }
        }
    }
    private void KickSelectItemStack()
    {
        if (itemSelect != null && itemSelect.unit != null && itemSelect.unit.data != null)
        {
            if (player.CanToCreateItem())
            {
                player.CreateItemKick(itemSelect.unit.data, itemSelect.unit.Count);
                ItemUnit unit = itemSelect.unit;

                unit.Clear();
                unit.Reset();
                return;
            }                     
        }        
    }
    private void ToggleOpenMain()
    {
        isOpenMain = !isOpenMain;
        mainItems.SetActive(isOpenMain);
        //Debug.Log("Type: " + itemSelect.unit.type);
        if (itemSelect != null && itemSelect.itemSelectUI != null && itemSelect.unit.type == InventoryType.main)
        {
            selector.SetActive(false);
        }
    }   
    private void InputUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleOpenMain();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            KickSelectItem();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q))
        {
            KickSelectItemStack();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
        {
            if (itemSelect != null && itemSelect.unit.type == InventoryType.fast & itemSelect.unit.data != null)
            {
                MoveItems(itemSelect.unit, InventoryType.fast, InventoryType.main);
            }
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.W))
        {
            if (itemSelect != null && itemSelect.unit.type == InventoryType.main & itemSelect.unit.data != null)
            {
                MoveItems(itemSelect.unit, InventoryType.main, InventoryType.fast);
            }
        }

        SetControlSelect();
    }

    #region Control
    public void SetItemSelect(UIItemPanelSlot item)
    {
        InitItemSelect();

        itemSelect.itemSelectUI = item;

        itemSelect.unit = panels[item.type].GetItemUnit(item);
        itemSelect.unit.type = item.type;          

        selector.SetActive(true);
        selector.transform.position = item.transform.position;
    }
    public void SetItemSelect(ItemUnit unit)
    {
        InitItemSelect();

        itemSelect.itemSelectUI = unit.uislot;
        itemSelect.unit.type = unit.uislot.type;
        itemSelect.unit = unit;

        selector.SetActive(true);
        selector.transform.position = unit.uislot.transform.position;
    }

    private void InitKeys()
    {
        inventoryKeyCodes = new List<KeyCode>();
        inventoryKeyCodes.Add(KeyCode.Alpha0);
        inventoryKeyCodes.Add(KeyCode.Alpha1);
        inventoryKeyCodes.Add(KeyCode.Alpha2);
        inventoryKeyCodes.Add(KeyCode.Alpha3);
        inventoryKeyCodes.Add(KeyCode.Alpha4);
        inventoryKeyCodes.Add(KeyCode.Alpha5);
        inventoryKeyCodes.Add(KeyCode.Alpha6);
        inventoryKeyCodes.Add(KeyCode.Alpha7);
    }
    private void InitItemSelect()
    {
        if (itemSelect == null)
            itemSelect = new ItemSelect();
    }
    private void SetControlSelect()
    {
        RollSet();
        KeyDownFast();
    }
    private void RollSet()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        { // вправо

            if (rollSelect < fastItems.maxCount - 1)
            {
                rollSelect++;
                SetItemSelect(fastItems.items[rollSelect]);
            }

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        { // влево
            if (rollSelect > 0)
            {
                rollSelect--;
                SetItemSelect(fastItems.items[rollSelect]);
            }
        }

    }
    private void KeyDownFast()
    {
        for (int i = 0; i < fastItems.maxCount; i++)
        {
            if (Input.GetKeyDown(inventoryKeyCodes[i]))
            {
                SetItemSelect(fastItems.items[i]);
            }
        }
    }
    #endregion
    #endregion
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
    public InventoryType type;

    internal Inventory inventory;

    public int Count { get; internal set; }
    public int MaxCount { get; internal set; }
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
    public bool AddItem()
    {
        if (Count < MaxCount)
        {
            Count++;
            uislot.SetCount(Count);
            return true;
        }
        return false;
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

