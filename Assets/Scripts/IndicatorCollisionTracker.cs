using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorCollisionTracker : MonoBehaviour {

	public List<GameObject> objInRange = new List<GameObject>();

	public void OnTriggerEnter(Collider other) {
		//Debug.Log(other.gameObject.name);
		if (other.gameObject.tag == GameData.objectTag)
			objInRange.Add(other.gameObject);
		if (other.gameObject.GetComponent<VRPhysicalButton>() != null && other.gameObject != GameData.instance.vrplayer.handSet[0].grabbedObject && other.gameObject != GameData.instance.vrplayer.handSet[1].grabbedObject)
			other.gameObject.GetComponent<VRPhysicalButton>().action.Invoke();
	}

	public void OnTriggerExit(Collider other) {
		objInRange.Remove(other.gameObject);
	}
}
