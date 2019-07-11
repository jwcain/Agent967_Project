using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disposal : MonoBehaviour {
	public DisposalObject scriptableObject;

	public void ReturnEvidence(Evidence evidence) {
		//if (evidence.gameObject == GameData.instance.vrplayer.handSet[0].grabbedObject)
		//	GameData.instance.vrplayer.handSet[0].grabbedObject = null;
		//if (evidence.gameObject == GameData.instance.vrplayer.handSet[1].grabbedObject)
		//	GameData.instance.vrplayer.handSet[0].grabbedObject = null;
		int evidenceID = evidence.scriptableObject.UID;
		int disposalID = scriptableObject.UID;
		int? symbolID = (evidence.optionalSymbol != null) ? (int?)evidence.optionalSymbol.UID : null;
		GameManager.instance.ReportEvidenceDistruction(evidenceID, disposalID, symbolID);
		EvidencePooler.ReturnObject(evidence.scriptableObject.UID, evidence.gameObject);
	}
}
