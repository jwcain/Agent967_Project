using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyorbelt : MonoBehaviour {
	public float forceMod = 1.0f;
	public GameObject startPoint;
	public GameObject endPoint;
	private Vector3 forceDir;
	public ForceMode mode;

	public void Start() {
		forceDir = endPoint.transform.position - startPoint.transform.position;
		//StartCoroutine(SpawnObjects());
	}

	private void OnCollisionStay(Collision collision) {
		//foreach (var item in collision.contacts) {
		//	collision.transform.GetComponent<Rigidbody>().AddForceAtPosition(forceDir * forceMod * Time.deltaTime, item.point, mode);
		//}
		collision.transform.GetComponent<Rigidbody>().velocity = (endPoint.transform.position-startPoint.transform.position).normalized * Time.deltaTime * forceMod;
	}

	/*
	IEnumerator SpawnObjects() {
		float timer = 0.0f;
		float rando = Random.Range(1.0f, 4.0f);

		while (true) {
			while ((timer += Time.deltaTime) < rando) yield return null;
			timer = 0.0f;
			rando = Random.Range(1.0f, 4.0f);
			SpawnPrefab(spawnable).transform.position = startPoint.transform.position;
		}
	}


	public GameObject SpawnPrefab(GameObject prefab) {
		GameObject newObj = GameObject.Instantiate(prefab);
		//newObj.transform.parent = this.transform;
		return newObj;
	}*/
}
