﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Game.Player;
using JetBrains.Annotations;
using SavingSystem;
using UnityEngine;
using static Game.UI.Console;
using Console = Game.UI.Console;

namespace Game.ItemSystem
{
    [Serializable]
    public enum InventoryType
    {
        Fast,
        Main,
        Sandbox,
        Chest
    }
    
    [SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
    public class Inventory : MonoBehaviour
    {
        #region Fields

        private bool _isOpenThirdMenu;
        private InventoryType _typeCurrentThirdMenu;

        private bool IsOpen { get; set; }

        [SerializeField] internal DataBase dataBase;

        //Fast Panel
        [SerializeField] private FastItems fastItems;

        //Main Panel
        [SerializeField] private MainItems mainItems;

        //SandBox Panel
        [SerializeField] private SandboxItems sandboxItems;

        //Chest Panel
        [SerializeField] private ChestItems chestItems;

        //Item select
        [SerializeField] internal ItemSelect itemSelect;
        [SerializeField] internal GameObject selector;

        //Dragging
        private bool _dragging;
        private ItemUnit _draggingItem;
        private Texture2D _draggingTexture;
        private Vector3 _curPos;

        //Control
        private int _rollSelect;
        private List<KeyCode> _inventoryKeyCodes;
        private bool _canInput = true;
        //Player
        public PlayerController player;
        [SerializeField] private BlockSelector blockSelector;
        private Dictionary<InventoryType, PanelItemsBase> _panels;
        
        #endregion

        #region Panels

        [Serializable]
        public abstract class PanelItemsBase
        {
            [SerializeField] internal UIItemPanel uipanel;
            internal int MaxCount;
            internal List<ItemUnit> Items; //Основная панель предметов
            public Inventory inventory;
            public bool CanAddingItems { get; private set; }
            public bool InfinityItems { get; internal set; }
            public abstract InventoryType InventoryType { get; }

            public void SetActive(bool value)
            {
                uipanel.gameObject.SetActive(value);
            }

            public void Init()
            {
                CanAddingItems = true;
                InfinityItems = false;
                MaxCount = uipanel.slots.slots.Length;
                Items = new List<ItemUnit>();
                for (var i = 0; i < MaxCount; i++)
                {
                    var unit = new ItemUnit
                    {
                        uislot = uipanel.slots.slots[i], 
                        Inventory = inventory
                    };
                    unit.uislot.inventory = inventory;
                    SetTypeItem(unit);
                    unit.uislot.type = unit.type;
                    unit.Reset();
                    Items.Add(unit);
                }

                InitOver();
            }

            public ItemData.Data GetItemData(int n)
            {
                return Items[n].data;
            }

            protected void SetInfinity()
            {
                for (var i = 0; i < MaxCount; i++)
                {
                    var item = Items[i];
                    item.SetInfinity();
                    item.uislot.SetActiveCountText(false);
                    //Debug.Log(item.uislot.type);
                }
            }

            public ItemUnit GetItemUnit(UIItemPanelSlot itemSelectUi)
            {
                for (var i = 0; i < MaxCount; i++)
                {
                    var unit = Items[i];
                    if (unit.uislot == itemSelectUi)
                    {
                        return unit;
                    }
                }

                return null;
            }

            public int AddItemCount(ItemData.Data data, int count)
            {
                if (CanAddingItems)
                {
                    for (var i = 0; i < count; i++)
                    {
                        if (!AddItem(data))
                        {
                            return i;
                        }
                    }
                }

                return 0;
            }

            public bool AddItem(ItemData.Data data)
            {
                //Debug.Log("StartFinding Had");
                if (!CanAddingItems) return false;
                
                ItemUnit unit;
                for (var i = 0; i < MaxCount; i++)
                {
                    //Debug.Log("i:" + i + " " + fastPanel[i].data?.name.translations[0].text + "; " + data.name.translations[0].text);
                    unit = Items[i];
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
                for (var i = 0; i < MaxCount; i++)
                {
                    unit = Items[i];
                    //Debug.Log("i:" + i + " "+ fastPanel[i].data);
                    if (unit.HasData()) continue;
                    
                    UpdateItemSelect(unit);
                    unit.SetData(data);
                    //Debug.Log("NULL");
                    return true;
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

            protected virtual void InitOver()
            {

            }

            public virtual void WriteItems(List<ItemData.Data> items)
            {
                var count = items.Count > MaxCount ? MaxCount : items.Count;
                for (var i = 0; i < items.Count; i++)
                {
                    var unit = Items[i];
                    UpdateItemSelect(unit);
                    unit.SetData(items[i]);
                    Debug.Log(items[i].Name);
                }
            }

            public void DisableUiSlots()
            {
                for (var i = 0; i < MaxCount; i++)
                {
                    Items[i].uislot.SetIconDisabled();
                }
            }

            public void UpdateUiSlots()
            {
                for (var i = 0; i < MaxCount; i++)
                {
                    Items[i].UpdateSprite();
                }
            }

            public virtual void ActionOnOpen()
            {
            }
        }

        [Serializable]
        public class MainItems : PanelItemsBase
        {
            public override InventoryType InventoryType => InventoryType.Main;

            protected override void SetTypeItem(ItemUnit unit)
            {
                //Debug.Log("inv: " + this + "unit: " + unit + ";" + InventoryType);
                unit.type = InventoryType;
            }
        }

        [Serializable]
        public class FastItems : PanelItemsBase
        {
            public override InventoryType InventoryType => InventoryType.Fast;

            protected override void SetTypeItem(ItemUnit unit)
            {
                //Debug.Log("inv: " + this + "unit: " + unit + ";" + InventoryType);
                unit.type = InventoryType;
            }
        }

        [Serializable]
        public class SandboxItems : PanelItemsBase
        {
            public override InventoryType InventoryType => InventoryType.Sandbox;

            protected override void SetTypeItem(ItemUnit unit)
            {
                //Debug.Log("inv: " + this + "unit: " + unit + ";" + InventoryType);
                unit.type = InventoryType;
            }

            protected override void InitOver()
            {
                InfinityItems = true;
                SetInfinity();
                inventory.dataBase.Refresh();

                var itemsList = DataBase.Instance.ItemsList; //inventory.dataBase.ItemsList;
                // var count = itemsList.Count > MaxCount ? MaxCount : itemsList.Count;
                // for (var i = 0; i < count; i++)
                // {
                //     AddItem(itemsList[i]);
                // }

                WriteItems(itemsList);
            }

            public override void WriteItems(List<ItemData.Data> items)
            {
                //Debug.Log(items.Count);
                for (var i = 0; i < items.Count; i++)
                {
                    if (!items[i].showInSandboxPanel)
                    {
                        items.RemoveAt(i);
                    }
                }

                //Debug.Log(items.Count);
                var count = items.Count > MaxCount ? MaxCount : items.Count;
                for (var i = 0; i < items.Count; i++)
                {
                    var unit = Items[i];
                    UpdateItemSelect(unit);
                    unit.SetData(items[i]);
                }
            }

            public override void ActionOnOpen()
            {
                for (var i = 0; i < MaxCount; i++)
                {
                    Items[i].uislot.SetActiveCountText(false);
                }
            }
        }

        [Serializable]
        public class ChestItems : PanelItemsBase
        {
            public override InventoryType InventoryType => InventoryType.Chest;
            public ChestMemory chestMemory;

            protected override void SetTypeItem(ItemUnit unit)
            {
                //Debug.Log("inv: " + this + "unit: " + unit + ";" + InventoryType);
                unit.type = InventoryType;
            }

            public override void ActionOnOpen()
            {
                for (var i = 0; i < MaxCount; i++)
                {
                    Items[i].uislot.SetActiveCountText(true);
                    //Items[i].uislot.SetCount(0);
                }
            }

            public void WriteChestItems(List<ChestSlotUnit> items)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    var unit = Items[i];
                    UpdateItemSelect(unit);
                    unit.SetData(items[i].Data);
                    unit.SetCount(items[i].Count);
                    unit.uislot.SetActiveCountText(true);
                }

                ToSaveBlock();
            }

            public void ToSaveBlock()
            {
                if (chestMemory != null) chestMemory.WriteItems(Items);
            }
        }

        #endregion
        
        #region Unity

        private void Start()
        {
            player = PlayerController.Instance;
            mainItems.SetActive(IsOpen);
            sandboxItems.SetActive(false);
            chestItems.SetActive(false);

            InitKeys();
            InitItemSelect();

            _panels = new Dictionary<InventoryType, PanelItemsBase>()
            {
                {InventoryType.Fast, fastItems},
                {InventoryType.Main, mainItems},
                {InventoryType.Sandbox, sandboxItems},
                {InventoryType.Chest, chestItems},
            };

            for (InventoryType i = 0; (int) i < _panels.Count; i++)
            {
                _panels[i].Init();
            }

            SetThirdMenu(InventoryType.Sandbox);

            Console.OnToggleConsoleEvent += SetCanInput;
        }

        private void Update()
        {
            if (_canInput)
                InputUpdate();
            CheckDistanceInteract();
        }

        private void OnGUI()
        {
            if (!_dragging) return;
            
            // перемещение иконки на экране
            var mousePos = Event.current.mousePosition;
            GUI.depth = 999; // поверх остальных элементов
            const float shift = 25;
            
            GUI.Label(new Rect(mousePos.x - shift, mousePos.y - shift, 50, 50), _draggingTexture);
        }

        #endregion

        #region Input

        private void KickSelectItem()
        {
            if (itemSelect?.unit?.data == null || !player.CanToCreateItem()) return;
        
            player.CreateItemKick(itemSelect.unit.data, 1);
            var unit = itemSelect.unit;

            unit.RemoveItem();
        }

        private void KickSelectItemStack()
        {
            if (itemSelect?.unit?.data == null || !player.CanToCreateItem()) return;

            var unit = itemSelect.unit;
            if (!unit.IsInfinity)
            {
                player.CreateItemKick(unit.data, unit.Count);

                unit.RemoveAllItems();
            }
            else
            {
                player.CreateItemKick(unit.data, 64);
            }
        }

        private void ToggleOpenMain(bool v)
        {
            IsOpen = v;
            mainItems.SetActive(IsOpen);

            //Debug.Log("Type: " + itemSelect.unit.type);
            if (itemSelect != null && itemSelect.unit.type != InventoryType.Fast)
            {
                selector.SetActive(false);
            }
        }

        private void ToggleOpenThirdMenu(bool v)
        {
            _isOpenThirdMenu = v;
            var menu = _panels[_typeCurrentThirdMenu];
            menu.SetActive(_isOpenThirdMenu);
            menu.UpdateUiSlots();
            if (v)
            {
                menu.ActionOnOpen();
            }

            //Debug.Log("Type: " + itemSelect.unit.type);
            if (itemSelect != null && itemSelect.unit.type == _typeCurrentThirdMenu)
            {
                selector.SetActive(false);
            }
        }

        private void InputUpdate()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.E))
            {
                CloseChest();
                ToggleOpenMain(true);
                ToggleOpenThirdMenu(!_isOpenThirdMenu);
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    CloseChest();
                    ToggleOpenMain(!IsOpen);
                    ToggleOpenThirdMenu(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                KickSelectItem();
            }

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q))
            {
                KickSelectItemStack();
            }

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0)) //Input.GetKeyDown(KeyCode.S))
            {
                if (itemSelect?.unit != null)
                {
                    var fromMenu = itemSelect.unit.type;
                    var toMenu = InventoryType.Main;
                    if (_isOpenThirdMenu)
                    {
                        toMenu = fromMenu == InventoryType.Main ? _typeCurrentThirdMenu : InventoryType.Main;
                    }
                    else
                    {
                        if (IsOpen)
                        {
                            toMenu = fromMenu == InventoryType.Main ? InventoryType.Fast : InventoryType.Main;
                        }
                    }

                    MoveItemsSpec(fromMenu, toMenu);
                }
            }

            SetControlSelect();
        }

        private void MoveItemsSpec(InventoryType from, InventoryType to)
        {
            if (to != InventoryType.Sandbox && itemSelect != null &&
                itemSelect.unit.type == from & itemSelect.unit.data != null)
            {
                MoveItems(itemSelect.unit, from, to);
            }
        }

        #region Control

        public void SetItemSelect(UIItemPanelSlot item)
        {
            InitItemSelect();

            itemSelect.itemSelectUi = item;

            itemSelect.unit = _panels[item.type].GetItemUnit(item);
            itemSelect.unit.type = item.type;

            selector.SetActive(true);
            selector.transform.position = item.transform.position;
        }

        private void SetItemSelect(ItemUnit unit)
        {
            InitItemSelect();

            itemSelect.itemSelectUi = unit.uislot;
            itemSelect.unit.type = unit.uislot.type;
            itemSelect.unit = unit;

            selector.SetActive(true);
            selector.transform.position = unit.uislot.transform.position;
        }

        private void InitKeys()
        {
            _inventoryKeyCodes = new List<KeyCode>();
            _inventoryKeyCodes.Add(KeyCode.Alpha1);
            _inventoryKeyCodes.Add(KeyCode.Alpha2);
            _inventoryKeyCodes.Add(KeyCode.Alpha3);
            _inventoryKeyCodes.Add(KeyCode.Alpha4);
            _inventoryKeyCodes.Add(KeyCode.Alpha5);
            _inventoryKeyCodes.Add(KeyCode.Alpha6);
            _inventoryKeyCodes.Add(KeyCode.Alpha7);
            _inventoryKeyCodes.Add(KeyCode.Alpha8);
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
            {
                // вправо

                if (_rollSelect < fastItems.MaxCount - 1)
                {
                    _rollSelect++;
                    SetItemSelect(fastItems.Items[_rollSelect]);
                }

            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                // влево
                if (_rollSelect > 0)
                {
                    _rollSelect--;
                    SetItemSelect(fastItems.Items[_rollSelect]);
                }
            }

        }

        private void KeyDownFast()
        {
            for (var i = 0; i < fastItems.MaxCount; i++)
            {
                if (Input.GetKeyDown(_inventoryKeyCodes[i]))
                {
                    SetItemSelect(fastItems.Items[i]);
                }
            }
        }

        #endregion

        #endregion

        #region Saving

        public List<WorldSavingSystem.PlayerData.ItemPlayerData> GetItems()
        {
            var fastItemsItems = this.fastItems.Items;
            var mainItemsItems = this.mainItems.Items;
            var list = new List<WorldSavingSystem.PlayerData.ItemPlayerData>();
        
            foreach (var item in fastItemsItems)
            {
                var itemSave = new WorldSavingSystem.PlayerData.ItemPlayerData()
                {
                    InventoryType = InventoryType.Fast,
                    Name = item.data?.Name ?? "",
                    Count = item.Count
                };
                list.Add(itemSave);
            }
            foreach (var item in mainItemsItems)
            {
                var itemSave = new WorldSavingSystem.PlayerData.ItemPlayerData()
                {
                    InventoryType = InventoryType.Main,
                    Name = item.data?.Name ?? "",
                    Count = item.Count
                };
                list.Add(itemSave);
            }

            return list;
        }

        public void WriteItems(List<WorldSavingSystem.PlayerData.ItemPlayerData> list)
        {
            var count = fastItems.Items.Count;
            
            for (var i = 0; i < count; i++)
            {
                var item = list[i];
                var itemS = fastItems.Items[i];
                var itemName = item.Name;
                
                if (itemName == "") continue;
                
                itemS.SetData(dataBase.GetItem(itemName));
                itemS.SetCount(item.Count);

            }

            var countMain = count + mainItems.Items.Count;
            for (var i = count; i < countMain; i++)
            {
                var item = list[i];
                var itemS = mainItems.Items[i - count];
                var itemName = item.Name;
                
                if (itemName == "") continue;
                
                itemS.SetData(dataBase.GetItem(itemName));
                itemS.SetCount(item.Count);
            }
        }

        #endregion
        
        private void SetThirdMenu(InventoryType type)
        {
            _typeCurrentThirdMenu = type;
            _panels[type].Init();
        }

        #region AddItem

        public void AddItem(ItemData.Data data)
        {
            for (InventoryType i = 0; (int)i < _panels.Count; i++)
            {
                if (_panels[i].AddItem(data))
                {
                    break;
                }
            }
        }
        public void AddItemCount(ItemData.Data data, int count)
        {
            var tempCount = count;
            for (InventoryType i = 0; (int)i < count; i++)
            {
                var mod = _panels[i].AddItemCount(data, tempCount);
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

        #endregion

        private void MoveItems(ItemUnit unit, InventoryType from, InventoryType to)
        {
            var a = _panels[to].AddItemCount(unit.data, unit.IsInfinity ? 64 : unit.Count);
            if (unit.IsInfinity) return;
            
            if (a > 0)
            {
                _panels[from].AddItemCount(unit.data, a);
            }
            else
            {
                unit.RemoveAllItems();
            }
        }
        public ItemUnit GetSelectedItem()
        {
            return itemSelect.unit;
        }

        public void OpenChest([NotNull]ChestMemory chestMemory)
        {
            SetThirdMenu(InventoryType.Chest);
            //Debug.Log(_typeCurrentThirdMenu);
            ToggleOpenMain(true);

            ToggleOpenThirdMenu(true);
    
            //chestItems.DisableUISlots();
            chestItems.WriteChestItems(chestMemory.items);
            //chestItems.UpdateUISlots();
            chestItems.chestMemory = chestMemory;
            //chestMemory.Debugging();
            //chestItems.DisableUISlots();
            //Debug.Log(_typeCurrentThirdMenu);
        }

        private void CloseChest()
        {
            if (_typeCurrentThirdMenu != InventoryType.Chest) return;
            
            ToggleOpenThirdMenu(false);
            SetThirdMenu(InventoryType.Sandbox);
            chestItems.ToSaveBlock();
        }

        private void CheckDistanceInteract()
        {
            if (_isOpenThirdMenu && !blockSelector.CheckInteract())
            {
                CloseThirdMenuInteract();
            }
        }

        public void CloseThirdMenuInteract()
        {
            CloseChest();
        }

        private void SetCanInput(bool value)
        {
            _canInput = value;
        }
        [Serializable]
        public class ItemSelect
        {
            public PanelItemsBase panel;
            public ItemUnit unit;
            public UIItemPanelSlot itemSelectUi;
        }
    }

    [Serializable]
    public class ItemUnit
    {
        public ItemData.Data data = null;
        public UIItemPanelSlot uislot;
        public InventoryType type;

        internal Inventory Inventory;
        public bool IsInfinity { get; internal set; }
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
            if (IsInfinity) return;
            
            Count = n;
            uislot.SetCount(n);
        }
        public void SetData(ItemData.Data itemData)
        {
            Reset();
            if (itemData == null) return;
            
            data = itemData;
            MaxCount = itemData.maxCount;
            uislot.SetSprite(itemData.sprite);
        
            Count++;
            uislot.SetCount(Count);
        }
        public bool AddItem()
        {
            if (Count >= MaxCount) return false;
            
            Count++;
            uislot.SetCount(Count);
            return true;
        }
        public bool HasData()
        {
            return data != null;
        }
        public void RemoveItem()
        {
            if (IsInfinity) return;
            
            if (Count > 0)
            {           
                Count--;
                uislot.SetCount(Count);
                if (Count > 0)
                {
                    return;
                }
            }
            
            if (Inventory.itemSelect?.unit != this) return;
            
            uislot.SetIconDisabled();
            Reset();
        }

        public void RemoveAllItems()
        {
            if (IsInfinity) return;
            
            uislot.SetIconDisabled();
            Reset();
        }
        public void Clear()
        {
            Inventory.itemSelect = null;
            uislot.SetIconDisabled();
        }

        public void SetInfinity()
        {
            IsInfinity = true;
        }

        public void UpdateSprite()
        {
            uislot.SetSprite(data?.sprite);
        }
    
    }
}