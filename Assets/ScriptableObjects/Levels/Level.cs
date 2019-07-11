using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewLevel", menuName = "Level", order = 1)]
public class Level : ScriptableObject {
	public int scoreThreshold;
	public Level nextLevel;

	public GameManager.Rule[] rules;

	public DisposalObject[] disposals { get { return new DisposalObject[] { d1, d2, d3, d4, d5, d6, d7, d8 }; } }
	public bool[] inputs { get { return new bool[] { i1, i2, i3, i4, i5, i6, i7, i8 }; } }

	public DisposalObject d1;
	public DisposalObject d2;
	public DisposalObject d3;
	public DisposalObject d4;
	public DisposalObject d5;
	public DisposalObject d6;
	public DisposalObject d7;
	public DisposalObject d8;

	public bool i1;
	public bool i2;
	public bool i3;
	public bool i4;
	public bool i5;
	public bool i6;
	public bool i7;
	public bool i8;
	public bool i9;
}
