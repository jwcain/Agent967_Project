using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDisposal", menuName = "Disposal", order = 2)]
public class DisposalObject : ScriptableObject {
	public int UID;
	public Sprite sprite;
	public GameObject spawnedObject;
}
