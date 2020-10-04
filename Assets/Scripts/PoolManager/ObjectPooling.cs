
using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Pool/ObjectPooling")]
public class ObjectPooling
{
	#region Data
	List<PoolObject> objects;
	Transform objectsParent;
	#endregion

	#region Interface
	public void Initialize(int count, PoolObject sample, Transform objects_parent)
	{
		objects = new List<PoolObject>();
		objectsParent = objects_parent;
		for (int i = 0; i < count; i++)
		{
			AddObject(sample, objects_parent);
		}
	}

	public PoolObject GetObject()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			Debug.Log(i);
			if (objects[i].gameObject.activeInHierarchy == false)
			{
				return objects[i];
			}
		}
		AddObject(objects[0], objectsParent);
		return objects[objects.Count - 1];
	}
	#endregion

	#region Methods
	void AddObject(PoolObject sample, Transform objects_parent)
	{
		GameObject temp;
		temp = Object.Instantiate(sample.gameObject);
		InitObject(temp, objects_parent);
	}
	void AddObject(PoolObject sample, Transform objects_parent, int n)
	{
		GameObject temp;
		temp = Object.Instantiate(sample.gameObject);
		temp.name = sample.name + n;
		InitObject(temp, objects_parent);
	}

	void InitObject(GameObject temp, Transform objects_parent)
	{
		temp.transform.SetParent(objects_parent);
		objects.Add(temp.GetComponent<PoolObject>());
		if (temp.GetComponent<Animator>())
			temp.GetComponent<Animator>().StartPlayback();
		temp.SetActive(false);
	}
	#endregion

}