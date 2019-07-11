using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesDisplay : MonoBehaviour {

	public Sprite Equal;
	public Sprite NotEqual;

	public RulesLine[] rulesLines = new RulesLine[5];

	public static RulesDisplay instance;

	private void Start() {
		ResetDisplay();
		instance = this;
	}

	public void ResetDisplay() {
		for (int i = 0; i < rulesLines.Length; i++) {
			rulesLines[i].gameObject.SetActive(false);
		}
	}


	public void SetRule(Sprite leftIcon, Sprite centerIcon, Sprite rightIcon, int pos) {
		rulesLines[pos].leftImage.sprite = leftIcon;
		rulesLines[pos].centerImage.sprite = centerIcon;
		rulesLines[pos].rightImage.sprite = rightIcon;
		rulesLines[pos].gameObject.SetActive(true);

	}
}
