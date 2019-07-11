using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePit : Disposal {

	private void OnTriggerEnter(Collider other) {
		if (other.tag == GameData.objectTag) {
			StartCoroutine(FireyDeath(other.GetComponent<Evidence>()));
		}
	}


	IEnumerator FireyDeath(Evidence other) {
		//[TODO] add fire to this object

		float counter = 0.0f;
		while ((counter += Time.deltaTime) <= 0.75f)
			yield return null;

		ReturnEvidence(other);
	}
}
