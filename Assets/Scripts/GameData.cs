using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {
	public static readonly string objectTag = "GameSpawnable";
	public static readonly string vrHandTag = "Indicator";
	public const float circleDegrees = 360.0f;
	public int sides = 8;

	public float degreePerSide { get { return circleDegrees / sides; } }

	public float innerSize = 3.0f;
	public float outerSize = 10.0f;

	public static GameData instance;

	public VRPlayerController vrplayer;

	private void Start() {
		instance = this;
	}

}
