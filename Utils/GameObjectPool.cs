using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameObjectPool:MonoBehaviour {

	public GameObject prefabObject;
	public int startPool = 10;
	public int incrementBy = 5;

	public bool spawnAsChildren = false;
	public bool normalizeLocalScale = false;

	public bool uniqueName = false;

	List<GameObject> objectPool = new List<GameObject>();

	void Start()
	{
		AddToPool(startPool);
	}

	void AddToPool(int count)
	{
		for (int i = 0; i < count; i++)
		{
			GameObject poolObject = Instantiate(prefabObject) as GameObject;
			poolObject.SetActive(false);

			if (spawnAsChildren)
				poolObject.transform.parent = transform;
			if (normalizeLocalScale)
				poolObject.transform.localScale = Vector3.one;

			objectPool.Add(poolObject);

			if (uniqueName)
				poolObject.name = prefabObject + "(" + objectPool.Count + ")";
		}
	}

	public GameObject[] ActiveObjects
	{
		get
		{
			return objectPool.Where(target => target.activeSelf).ToArray();
		}
	}

	public GameObject FreeObject
	{
		get
		{
			var freeObject = (from item in objectPool
							  where item.activeSelf == false
							  select item).FirstOrDefault();

			if (freeObject == null)
			{
				AddToPool(incrementBy);
				freeObject = FreeObject;
			}
			freeObject.SetActive(true);
			return freeObject;
		}
	}
}
