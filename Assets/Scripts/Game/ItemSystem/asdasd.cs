//using UnityEngine;
//using UnityEngine.UI;
//using System;
//using System.Collections;

//public class Inventoryasd : MonoBehaviour
//{

//	public Transform[] allItems;
//	public Sprite[] itemIcon;
//	public int iconSize = 50;
//	public Sprite background;
//	private GameObject[] cell;
//	private Texture2D tmpTexture;
//	private int iconCoumt;
//	private int itemCoumt;
//	private GameObject tmpObject;
//	private GameObject cell_tmp;
//	private Vector3 curPos;
//	public static int grid_id;

//	void Awake()
//	{
//		grid_id = -1;
//		cell = new GameObject[transform.childCount];
//		for (int j = 0; j < cell.Length; j++)
//		{
//			cell[j] = transform.GetChild(j).gameObject;
//			cell[j].GetComponentInChildren<Grid>().ID = j;
//		}
//	}

//	void Reset()
//	{
//		if (tmpTexture)
//		{
//			tmpTexture = null;
//			if (grid_id == -1 && !tmpObject)
//			{
//				// создание объекта с добавлением высоты по Y
//				GameObject clone = Instantiate(allItems[itemCoumt].gameObject, curPos + new Vector3(0, 100, 0), Quaternion.identity) as GameObject;
//				RaycastHit hit;
//				if (Physics.Linecast(curPos, clone.transform.position, out hit))
//				{
//					// корректировка высоты на поверхности, с учетом размера коллайдера
//					clone.transform.position -= new Vector3(0, hit.distance, 0);
//				}
//			}
//		}
//		if (tmpObject)
//		{
//			tmpObject.SetActive(true);
//			tmpObject = null;
//		}
//		cell_tmp = null;
//	}

//	void Update()
//	{
//		RaycastHit hit;
//		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//		if (Physics.Raycast(ray, out hit))
//		{
//			curPos = hit.point;
//			if (Input.GetMouseButton(0))
//			{
//				if (hit.transform.GetComponent<AddToInventory>() && dragging)
//				{
//					tmpObject = hit.transform.gameObject;
//					tmpObject.SetActive(false);
//					int i = hit.transform.GetComponent<AddToInventory>().itemID;
//					for (int j = 0; j < itemIcon.Length; j++)
//					{
//						// поиск id массива иконок, относительно номера объекта
//						string n = itemIcon[j].name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[0];
//						if (n == i.ToString())
//						{
//							tmpTexture = itemIcon[j].texture;
//							iconCoumt = j;
//							for (int f = 0; f < allItems.Length; f++)
//							{
//								// поиск id массива предметов, относительно номера объекта
//								n = allItems[f].name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[0];
//								if (n == i.ToString()) itemCoumt = f;
//							}
//						}
//					}
//				}
//			}
//			else
//			{
//				if (!Input.GetMouseButton(0) && grid_id == -1) Reset();
//			}
//		}
//		else
//		{
//			if (!Input.GetMouseButton(0) && grid_id == -1) Reset();
//		}

//		if (Input.GetMouseButtonUp(0) && grid_id > -1 && tmpTexture)
//		{
//			foreach (GameObject obj in cell)
//			{
//				GameObject tmp = obj.transform.GetChild(0).gameObject;

//				// меняем местами иконки, внутри рюкзака
//				if (tmp.GetComponent<Grid>().ID == grid_id && tmp.GetComponent<Grid>().active && cell_tmp)
//				{
//					cell_tmp.GetComponent<Grid>().icon_id = tmp.GetComponent<Grid>().icon_id;
//					cell_tmp.GetComponent<Grid>().item_id = tmp.GetComponent<Grid>().item_id;
//					cell_tmp.GetComponent<Grid>().active = true;
//					cell_tmp.GetComponent<Image>().sprite = itemIcon[tmp.GetComponent<Grid>().icon_id];

//					tmp.GetComponent<Grid>().item_id = itemCoumt;
//					tmp.GetComponent<Grid>().icon_id = iconCoumt;
//					tmp.GetComponent<Grid>().active = true;
//					tmp.GetComponent<Image>().sprite = itemIcon[iconCoumt];

//					Reset();
//				}

//				// добавление новой иконки в инвентарь
//				if (tmp.GetComponent<Grid>().ID == grid_id && !tmp.GetComponent<Grid>().active)
//				{
//					tmp.GetComponent<Grid>().item_id = itemCoumt;
//					tmp.GetComponent<Grid>().icon_id = iconCoumt;
//					tmp.GetComponent<Grid>().active = true;
//					tmp.GetComponent<Image>().sprite = itemIcon[iconCoumt];
//					Destroy(tmpObject);
//				}
//			}
//			Reset();
//		}

//		if (Input.GetMouseButtonDown(0) && grid_id > -1 && !tmpTexture)
//		{
//			foreach (GameObject obj in cell)
//			{
//				GameObject tmp = obj.transform.GetChild(0).gameObject;

//				// выбор иконки из рюкзака
//				if (tmp.GetComponent<Grid>().ID == grid_id && tmp.GetComponent<Grid>().active)
//				{
//					cell_tmp = tmp;
//					iconCoumt = tmp.GetComponent<Grid>().icon_id;
//					itemCoumt = tmp.GetComponent<Grid>().item_id;
//					tmp.GetComponent<Grid>().active = false;
//					tmpTexture = tmp.GetComponent<Image>().sprite.texture;
//					tmp.GetComponent<Image>().sprite = background;
//				}
//			}
//		}
//	}

//	void OnGUI()
//	{
//		if (tmpTexture)
//		{
//			// перемещение иконки на экране
//			Vector2 mousePos = Event.current.mousePosition;
//			GUI.depth = 999; // поверх остальных элементов
//			float shift = iconSize / 2;
//			GUI.Label(new Rect(mousePos.x - shift, mousePos.y - shift, iconSize, iconSize), tmpTexture);
//		}
//	}
//}