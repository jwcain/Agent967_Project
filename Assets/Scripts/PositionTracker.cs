using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTracker : MiscUtil.Buffer<Vector3> {

	public PositionTracker(int capacity) { MaxCapacity = capacity; }


	public Vector3 GetVelocity() {
		Vector3[] positions = ToArray();
		float weight = 0.05f;
		Vector3 result = positions[1]-positions[0];/*Vector3.zero;
		for (int i = 0; i < positions.Length; i++) {
			result = (weight * (positions[i] - positions[0])) + ((1.0f - weight) * result);
		}*/
		result *= 20;
	//	Debug.Log(Count + " | "+result);
		return result;
	}

}
