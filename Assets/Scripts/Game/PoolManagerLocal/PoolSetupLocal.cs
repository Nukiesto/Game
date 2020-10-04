using UnityEngine;

[AddComponentMenu("PoolLocal/PoolSetupLocal")]
public class PoolSetupLocal : MonoBehaviour
{//обертка для управления статическим классом PoolManager

	#region Unity scene settings
	[SerializeField] private PoolManagerLocal.PoolPart[] pools;
	[SerializeField] private PoolManagerLocal pool;
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
		pool.Initialize(pools);
	}
	#endregion
}
