using UnityEngine;
using System.Collections;

[AddComponentMenu("Pool/PoolSetup")]
public class PoolSetup : MonoBehaviour
{//обертка для управления статическим классом PoolManager

	#region Unity scene settings
	[SerializeField] public PoolManager.PoolPart[] pools;
	#endregion

	#region Methods

	private void OnValidate()
	{
		for (var i = 0; i < pools.Length; i++)
		{
			pools[i].name = pools[i].prefab?.name ?? "";
		}
	}

	private void Awake()
	{
		Initialize();
	}

	private void Initialize()
	{
		PoolManager.Initialize(pools);
	}
	#endregion
}