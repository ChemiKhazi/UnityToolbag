using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GenericPool : MonoBehaviour {
	
	public int incrementBy = 5;
	public bool spawnAsChildren = true;
	public bool uniqueNames = false;

	Dictionary<GameObject, List<GameObject>> poolIndex = new Dictionary<GameObject, List<GameObject>>();

	public void StartPooling(GameObject instance)
	{
		AddToPool(instance, incrementBy);
	}

	void AddToPool(GameObject prefabObject, int count)
	{
		if (poolIndex.ContainsKey(prefabObject) == false)
			poolIndex.Add(prefabObject, new List<GameObject>());

		for (int i = 0; i < count; i++)
		{
			GameObject poolObject = Instantiate(prefabObject) as GameObject;
			if (spawnAsChildren)
				poolObject.transform.parent = this.transform;
			poolObject.SetActive(false);
			poolIndex[prefabObject].Add(poolObject);

			if (uniqueNames)
			{
				poolObject.name = prefabObject.name + " (" + poolIndex[prefabObject].Count + ")";
			}
		}
	}

	public GameObject GetFreeObject(GameObject prefab)
	{
		if (poolIndex.ContainsKey(prefab) == false)
			AddToPool(prefab, incrementBy);

		List<GameObject> poolList = poolIndex[prefab];
		var freeObject = (from item in poolList
						  where item.activeSelf == false
						  select item).FirstOrDefault();

		if (freeObject == null)
		{
			AddToPool(prefab, incrementBy);
			freeObject = GetFreeObject(prefab);
		}

		freeObject.SetActive(true);
		return freeObject;
	}
}
