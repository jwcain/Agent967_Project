using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirTube : Disposal {


	List<Collider> objs = new List<Collider>();
	bool animLockout = false;


	public void OnTriggerEnter(Collider other) {
		if (objs.Contains(other) == false && other.gameObject.tag == GameData.objectTag)
			objs.Add(other);
	}

	public void OnTriggerExit(Collider other) {
		if (objs.Contains(other) == true)
			objs.Remove(other);
	}


	public void OnTubeButtonPress() {
		if (animLockout == false) {
			FMODUnity.RuntimeManager.PlayOneShot("event:/Button", this.transform.position);
			StartCoroutine(TubeAnim());
		}
	}


	IEnumerator TubeAnim() {
		animLockout = true;
		FMODUnity.RuntimeManager.PlayOneShot("event:/AirTube", this.transform.position);
		//Take the objects we are moving
		Collider[] objs2 = objs.ToArray();
		foreach (Collider c in objs2)
			objs.Remove(c);

		float time = 3.0f;
		while (time > 0) {
			foreach(Collider collision in objs2)
				collision.transform.GetComponent<Rigidbody>().velocity = Vector3.up * Time.fixedDeltaTime * 200.0f;
			time -= Time.deltaTime;
			yield return null;
		}
		//Remove the objects
		for (int i = 0; i < objs2.Length; i++) {
			Collider collision = objs2[i];
			ReturnEvidence(collision.gameObject.GetComponent<Evidence>());
		}
		animLockout = false;
	}
}
