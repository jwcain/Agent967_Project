using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidDisposal : Disposal {
	private void OnTriggerEnter(Collider other) {
		if (other.tag == GameData.objectTag) {
			StartCoroutine(AcideyDeath(other.GetComponent<Evidence>()));
		}
	}


	IEnumerator AcideyDeath(Evidence other) {
		//[TODO] Disolve this object?

		float counter = 0.0f;
		while ((counter += Time.deltaTime) <= 0.75f)
			yield return null;

		ReturnEvidence(other);
	}
}
