using UnityEngine;
using System.Collections;

[AddComponentMenu("Pool/PoolSetup")]
public class PoolSetup : MonoBehaviour
{//обертка для управления статическим классом PoolManager

	#region Unity scene settings
	[SerializeField] private PoolManager.PoolPart[] pools;
	#endregion

	#region Methods
	void OnValidate()
	{
		for (int i = 0; i < pools.Length; i++)
		{
			pools[i].name = pools[i].prefab?.name ?? "";
		}
	}

	void Awake()
	{
		Initialize();
	}

	void Initialize()
	{
		PoolManager.Initialize(pools);
	}
	#endregion
}