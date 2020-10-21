using UnityEngine;

[AddComponentMenu("PoolLocal/PoolManagerLocal")]
public class PoolManagerLocal : MonoBehaviour
{
	private static PoolPart[] pools;

	[System.Serializable]
	public struct PoolPart
	{
		public string name;
		public PoolObject prefab;
		public int count;
		public ObjectPooling ferula;
	}
	public void Initialize(PoolPart[] newPools)
	{
		pools = newPools;
        for (int i = 0; i < pools.Length; i++)
		{
			//Debug.Log("a" + i);
			if (pools[i].prefab != null)
			{
				//Debug.Log(i);
				pools[i].ferula = new ObjectPooling();
				pools[i].ferula.Initialize(pools[i].count, pools[i].prefab, transform);
			}
		}
	}
	public GameObject GetObject(string name, Vector3 position, Quaternion rotation)
	{
		GameObject result = null;
		if (pools != null)
		{
			for (int i = 0; i < pools.Length; i++)
			{
				if (string.Compare(pools[i].name, name) == 0)
				{
					result = pools[i].ferula.GetObject().gameObject;
					result.transform.position = position;
					result.transform.rotation = rotation;
					result.SetActive(true);
					return result;
				}
			}
		}
		return result;
	}
}
