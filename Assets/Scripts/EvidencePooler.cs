using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidencePooler : MonoBehaviour {

	public Dictionary<int, Queue<GameObject>> pools = new Dictionary<int, Queue<GameObject>>();

	public static EvidencePooler instance;

	private void Start() {
		instance = this;
	}

	/// <summary>
	/// Returns the pool of objects for the ID
	/// </summary>
	/// <param name="objectID"></param>
	/// <returns></returns>
	private static Queue<GameObject> GetPool(int objectID) {
		if (instance.pools.ContainsKey(objectID) == false) {
			instance.pools.Add(objectID, new Queue<GameObject>());
		}

		return instance.pools[objectID];
	}

	/// <summary>
	/// Creates a new object of the specified ID
	/// </summary>
	/// <param name="objectID"></param>
	/// <returns></returns>
	public static GameObject CreateNewEvidence(EvidenceObject evidenceObject) {
		GameObject newObj = GameObject.Instantiate(evidenceObject.spawnedObject);
		newObj.SetActive(false);
		return newObj;
	}

	/// <summary>
	/// Returns an object of the given ID. If there is none in the pool, it gets created. The object is returned inactive.
	/// </summary>
	/// <param name="objectID"></param>
	/// <returns></returns>
	public static GameObject GetObject(EvidenceObject evidenceObject) {
		Queue<GameObject> pool = GetPool(evidenceObject.UID);

		if (pool.Count == 0) {
			return CreateNewEvidence(evidenceObject);
		}
		else {
			return pool.Dequeue();
		}
	}

	/// <summary>
	/// Adds an object to the pool for the specified ID
	/// </summary>
	/// <param name="objectID"></param>
	/// <param name="obj"></param>
	public static void ReturnObject(int objectID, GameObject obj) {
		obj.SetActive(false);
		GetPool(objectID).Enqueue(obj);
	}

	/// <summary>
	/// Emptys the pool and destroys all stored objects. Optionally can destroy all generated pools.
	/// </summary>
	/// <param name="fullReset"></param>
	public void ClearPool(bool fullReset = false) {
		Debug.Log("Clearing Pool");
		//Destroy every game object
		foreach (Queue<GameObject> q in pools.Values) {
			while (q.Count > 0) Destroy(q.Dequeue());
		}
		if (fullReset)
			//Create a new pool
			pools = new Dictionary<int, Queue<GameObject>>();
	}
}
