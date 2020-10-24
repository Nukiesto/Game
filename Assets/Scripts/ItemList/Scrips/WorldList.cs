using UnityEngine;
using System.Collections;
using SavingSystem;
using UnityEngine.Serialization;

public class WorldList : MonoBehaviour {

	public ItemList itemList;
	private List<ItemListUnit[]> itemListUnits;

	private int _currentWorldId;
	private bool isSelected;
	
	[SerializeField] private GameObject selectedWorldMenu;
	[SerializeField] private GameObject createWorldMenu;
	
	private void Start()
	{
		WorldSavingSystem.Init();
		var count = WorldSavingSystem.WorldsList.Worlds.Count;
		Debug.Log(count);
		itemListUnits = new ItemListUnit[count];
		itemList.WorldList = this;
		for (var i = 0; i < count; i++)
		{
			itemListUnits[i] = new ItemListUnit(WorldSavingSystem.WorldsList.Worlds[i], i);
			itemList.AddToList(i, WorldSavingSystem.WorldsList.Worlds[i], true);
		}
		
		// worldSaving = new WorldSavingSystem.WorldSaving(WorldSavingSystem.WorldsList);
		// var name = "TestWorldSave";
		// var world = new WorldSavingSystem.WorldDataUnit
		// {
		// 	name = name,
		// 	width = generator.worldWidthInChunks,
		// 	height = generator.worldHeightInChunks
		// };
		// if (!worldSaving.LoadWorldName(name)) worldSaving.CreateWorld(world);
	}

	public void SetSelect(int id)
	{
		_currentWorldId = id;
		isSelected = true;
		createWorldMenu.SetActive(false);
		selectedWorldMenu.SetActive(true);
		Debug.Log(this + " Выбран элемент списка -> '" + itemListUnits[id] + "'. Идентификатор объекта: " + id);
	}
	
	public void DeleteWorld()
	{
		
	}
	public void CreateWorld()
	{
		//var unit = new ItemListUnit("", itemListUnits.Length);
		//itemListUnits.Add();
		//int i = Random.Range(0, itemTitle.Length);
		//itemList.AddToList(itemID[i], itemTitle[i], true);
	}
	
	public void Clear()
	{
		itemList.ClearItemList();
	}

	#region SelectedMenu

	public void UnSelect()
	{
		isSelected = false;
		createWorldMenu.SetActive(true);
		selectedWorldMenu.SetActive(false);
	}
	public void LoadWorld()
	{
		
	}
	public void DestroyWorld()
	{
		itemListUnits
		itemList.UpdateList(_currentWorldId);
	}

	#endregion
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
