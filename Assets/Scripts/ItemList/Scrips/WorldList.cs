using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SavingSystem;
using UnityEngine.Serialization;

public class WorldList : MonoBehaviour {

	public ItemList itemList;
	private List<ItemListUnit> _itemListUnits;

	private int _currentWorldId;
	private bool isSelected;
	
	[SerializeField] private GameObject selectedWorldMenu;
	[SerializeField] private GameObject createWorldMenu;

	[SerializeField] private Text title;
	[SerializeField] private InputField inputFieldNameWorld;
	[SerializeField] private MenuController menuController;
	
	private string _createWorldName;
	private bool _toLoadWorld;
	private void Start()
	{
		WorldSavingSystem.Init();
		var count = WorldSavingSystem.WorldsList.Worlds.Count;
		
		_itemListUnits = new List<ItemListUnit>();
		
		itemList.WorldList = this;
		
		for (var i = 0; i < count; i++)
		{
			_itemListUnits.Add(item: new ItemListUnit(title: WorldSavingSystem.WorldsList.Worlds[index: i], id: i));
			itemList.AddToList(id: i, text: WorldSavingSystem.WorldsList.Worlds[index: i], resetScrollbar: true);
		}

		EndFieldName();
	}

	public void SetSelect(int id)
	{
		_currentWorldId = id;
		isSelected = true;
		createWorldMenu.SetActive(value: false);
		selectedWorldMenu.SetActive(value: true);
		SetTitle();
		//Debug.Log(this + " Выбран элемент списка -> '" + _itemListUnits[id] + "'. Идентификатор объекта: " + id);
	}
	public void CreateWorld()
	{
		if (_createWorldName != "")
		{
			var count = WorldSavingSystem.WorldsList.Worlds.Count;

			var worldSaving = new WorldSavingSystem.WorldSaving(WorldSavingSystem.WorldsList);
			
			//for (var j = 0; j < count; j++)
			//{
			//	if (_createWorldName == _itemListUnits[index: j].Title)
			//	{
			//		return;
			//	}
			//}

			//count++;
			var world = new WorldSavingSystem.WorldDataUnit
			{
				name = _createWorldName,
				width = 16,
			 	height = 8,
			    toGenerateWorld = true
			};
			if (worldSaving.CreateWorld(world))
			{
				var unit = new ItemListUnit(title: _createWorldName, id: count++);
				_itemListUnits.Add(item: unit);
				itemList.AddToList(id: unit.Id, text: _createWorldName, resetScrollbar: true);
				//WorldSavingSystem.WorldsList.AddWorldToList(name: _createWorldName);
				SetSelect(id: unit.Id);
			}
		}
	}
	public void Clear()
	{
		itemList.ClearItemList();
	}

	#region SelectedMenu

	public void UnSelect()
	{
		isSelected = false;
		createWorldMenu.SetActive(value: true);
		selectedWorldMenu.SetActive(value: false);
	} 
	public void LoadWorld()
	{
		WorldSavingSystem.CurrentWorld = _itemListUnits[index: _currentWorldId].Title;
		menuController.SetGame();
	}
	public void DestroyWorld()
	{
		WorldSavingSystem.WorldsList.RemoveWorldFromList(name: _itemListUnits[index: _currentWorldId].Title);
		_itemListUnits.RemoveAt(index: _currentWorldId);
		itemList.UpdateList(id: _currentWorldId);
		
		UnSelect();
	}
	private void SetTitle()
	{
		title.text = _itemListUnits[index: _currentWorldId].Title;
	}
	
	#endregion

	public void EndFieldName()
	{
		_createWorldName = inputFieldNameWorld.text;
		//Debug.Log("Name World: " + _createWorldName);
	}
	internal class ItemListUnit
	{
		public string Title;
		public int Id;

		public ItemListUnit(string title, int id)
		{
			Title = title;
			Id = id;
		}
	}
}
