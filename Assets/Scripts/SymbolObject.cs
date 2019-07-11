using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSymbol", menuName = "Symbol", order = 3)]
public class SymbolObject : ScriptableObject {
	public int UID;
	public Sprite sprite;
}
