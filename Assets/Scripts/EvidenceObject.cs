using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewEvidence", menuName = "Evidence", order = 1)]
public class EvidenceObject : ScriptableObject {
	public int UID;
	public Sprite sprite;
	public GameObject spawnedObject;

	public Vector3 eulerOffsetForRegularPlayerWhileHolding = Vector3.zero;
}
