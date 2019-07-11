using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public TMPro.TextMeshProUGUI scoreText;
	private long score = 0;

	public Level level;

	public GameObject[] inputMachines = new GameObject[0];
	public SymbolObject[] symbols;
	public EvidenceObject[] evidence;
	public DisposalObject[] disposalMachines;

	private GameObject[] spawnPoints;

	private Rule[] ruleSet;

	List<GameObject> spawnedObjects = new List<GameObject>();

	List<EvidenceObject> viableEvidence;

	public UnityEngine.UI.Image blackoutImage;

	Coroutine spawnObjs;

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
	}

	private void Start() {
		if (GameManager.instance != null)
			Destroy(this.gameObject);
		else {
			instance = this;
			DontDestroyOnLoad(this.gameObject);
			SetupLevel();
			StartCoroutine(FadeIn());
		}
	}

	private void SetupLevel() {
		if (level == null) {
			RandomLevel();
			//Reload the level after 5 minutes
			StartCoroutine(ReloadRandomLevelAfterTime(5*60));
		}
		else {
			ruleSet = (Rule[])level.rules.Clone();
			DisposalObject[] dos = level.disposals;
			bool[] ins = level.inputs;
			for (int i = 0; i < GameData.instance.sides; i++) {
				if (ins[i]) {
					PlaceAtSlot(i, SpawnPrefab(inputMachines[0]));
				}
				else if (dos[i] != null) {
					PlaceAtSlot(i, SpawnPrefab(disposalMachines[dos[i].UID].spawnedObject));
				}
			}
		}

		DisplayRules();
		//Gather spawnpoints
		spawnPoints = GameObject.FindGameObjectsWithTag("EvidenceSpawnpoint");
		viableEvidence = new List<EvidenceObject>();
		foreach (Rule r in ruleSet)
			viableEvidence.Add(evidence[r.input]);

		spawnObjs = StartCoroutine(SpawnObjects());
	}

	private void RandomLevel() {
		Debug.Log("Generating random level");
		//Create rules
		ruleSet = GenerateRules();



		//Dynamically load gameplay modules.
		int slotsRemaining = GameData.instance.sides;

		//Figure out what of each type of disposal is being used
		List<int> requiredDisposals = new List<int>();
		foreach (Rule r in ruleSet)
			if (requiredDisposals.Contains(r.disposal) == false)
				requiredDisposals.Add(r.disposal);

		//Minimum of 1 ouptut, otherwise a minimum equal to the amount of rules
		int outputSides = (requiredDisposals.Count > 0) ? requiredDisposals.Count : 1;// Random.Range((requiredDisposals.Count > 0) ? requiredDisposals.Count : 1, slotsRemaining);
		slotsRemaining -= outputSides;


		//Minumum of 1 Input
		int inputSides = 1;// Random.Range(1, slotsRemaining - 1);

		slotsRemaining -= inputSides;


		int[] slots = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
		Shuffle<int>(slots);
		//Create a new randomized array to represent what is an output(false) and what is an input(true)
		bool[] obs = new bool[GameData.instance.sides - slotsRemaining];

		//Debug.Log(inputSides + " | " + outputSides + " | " + slotsRemaining + " | " + obs.Length);
		//Init the array
		{
			int i = 0;
			for (; i < inputSides; i++) obs[i] = true;
			for (; i - inputSides < outputSides; i++) obs[i] = false;
		}
		Shuffle<bool>(obs);

		for (int i = 0; i < obs.Length; i++) {
			int rngMachine = Random.Range(0, ((obs[i]) ? inputMachines.Length : disposalMachines.Length));
			if (obs[i]) {
				//Make an input machine
				PlaceAtSlot(slots[i], SpawnPrefab(inputMachines[rngMachine]));
			}
			else {
				//If we have not spawned everything we have to, spawn it
				if (requiredDisposals.Count > 0) {
					PlaceAtSlot(slots[i], SpawnPrefab(disposalMachines[requiredDisposals[0]].spawnedObject));
					requiredDisposals.RemoveAt(0);

				}
				else {
					//Otherwise spawn a random object
					PlaceAtSlot(slots[i], SpawnPrefab(disposalMachines[rngMachine].spawnedObject));

				}
			}
		}
	}

	IEnumerator ReloadRandomLevelAfterTime(float timeInSeconds) {
		while (timeInSeconds > 0) { timeInSeconds -= Time.deltaTime; yield return null; }
		StartCoroutine(ReloadLevel());
	}

	IEnumerator SpawnObjects() {
		float timer = 0.0f;
		float rando = Random.Range(1.5f, 3.25f);

		while (true) {
			while ((timer += Time.deltaTime) < rando) yield return null;
			timer = 0.0f;
			rando = Random.Range(1.0f, 4.0f);
			GameObject spawnPoint = GetSpawnpoint();
			if (spawnPoint != null) {
				GameObject newEvidence = EvidencePooler.GetObject(viableEvidence[Random.Range(0, viableEvidence.Count)]);
				newEvidence.transform.position = spawnPoint.transform.position;
				newEvidence.SetActive(true);
			}
		}
	}

	private GameObject GetSpawnpoint() {
		return spawnPoints[Random.Range(0, spawnPoints.Length)];
	}


	public void PlaceAtSlot(int slot, GameObject obj) {
		Vector3 basePos = Vector3.zero + Vector3.up;
		Vector3 offSet = PolarToCartesian(slot * GameData.instance.degreePerSide, GameData.instance.innerSize);
		obj.transform.position = basePos + offSet;
		//Rotate the object.
		obj.transform.rotation = Quaternion.Euler(new Vector3(0.0f, -(slot * GameData.instance.degreePerSide) , 0.0f));
	}



	public GameObject SpawnPrefab(GameObject prefab) {
		GameObject newObj = GameObject.Instantiate(prefab);
		spawnedObjects.Add(newObj);
		//newObj.transform.parent = this.transform;
		return newObj;
	}

	
	private Vector3 PolarToCartesian(float eulerAngle, float radius) {
		return new Vector3(radius * Mathf.Cos(eulerAngle * Mathf.Deg2Rad), 0.0f, GameData.instance.innerSize * Mathf.Sin(eulerAngle * Mathf.Deg2Rad));
	}

	private static void Shuffle<T>(T[] list) {
		int n = list.Length;
		while (n > 1) {
			n--;
			int k = Random.Range(0, n-1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public void ReportEvidenceDistruction(int evidence, int disposal, int? symbol = null) {
		bool correctPlacement = false;
		foreach (Rule rule in ruleSet) {
			if (rule.isSymbolBased) {
				if (symbol == null) {
					//We ignore this rule since this one is symbol based and we cannot disprove it wihtout a symbol.
					continue;
				}
				else {
					int convertedSymbol = (int)symbol;
					if (convertedSymbol == rule.input) {
						if (disposal == rule.disposal) {
							//This is correct, so we do not modify the true statement.
							correctPlacement = true;
							break;
						}
						else {
							//Rule states this should go elsewhere, so this is wrong
							correctPlacement = false;
							break;
						}
					}
					else {
						//No match, therefore we cannot disprove anything.
						continue;
					}
				}
			}
			else {
				if (evidence == rule.input) {
					if (disposal == rule.disposal) {
						//Debug.Log("");
						//This is valid
						correctPlacement = true;
						break;
					}
					else {
						//Rule states this should go elsewhere, so this is wrong
						correctPlacement = false;
						break;
					}
				}
				else {
					//No evidence match, so we cannot disprove anything.
					continue;
				}
			}
		}
		if (correctPlacement) score++; else score--;
		scoreText.text = "Evidence Score:\n" + score;

		if (level != null && score >= level.scoreThreshold) {
			StartCoroutine(ReloadLevel());
		}
	}

	bool alreadyReloading = false;
	IEnumerator ReloadLevel() {
		if (alreadyReloading)
			yield break;
		yield return FadeOut();
		yield return new WaitForSeconds(1.0f);

		alreadyReloading = true;
		Debug.Log("Reloading level");
		StopCoroutine(spawnObjs);
		level = (level == null) ? null : level.nextLevel;
		EvidencePooler.instance.ClearPool();
		GameObject[] spawnedEvidence = GameObject.FindGameObjectsWithTag(GameData.objectTag);
		//Destroy all evidence remaining in the scene
		for (int i = 0; i < spawnedEvidence.Length; i++) {
			Destroy(spawnedEvidence[i]);
		}
		while (spawnedObjects.Count > 0) {
			GameObject temp = spawnedObjects[0];
			spawnedObjects.RemoveAt(0);
			Destroy(temp);
		}
		GameData.instance.vrplayer.handSet[0].grabbedObject = null;
		GameData.instance.vrplayer.handSet[1].grabbedObject = null;


		SetupLevel();
		yield return new WaitForSeconds(1.0f);
		yield return FadeIn();
		alreadyReloading = false;
	}

	private void DisplayRules() {
		for (int i = 0; i < ruleSet.Length; i++) {
			Rule r = ruleSet[i];
			Sprite leftSprite = (r.isSymbolBased) ? symbols[r.input].sprite : evidence[r.input].sprite;
			Sprite centerSprite = (r.equal) ? RulesDisplay.instance.Equal : RulesDisplay.instance.NotEqual;
			//Debug.Log(r.disposal);
			Sprite rightSprite = disposalMachines[r.disposal].sprite;

			RulesDisplay.instance.SetRule(leftSprite, centerSprite, rightSprite, i);	
		}
	}

	[System.Serializable]
	public struct Rule {
		/// <summary>
		/// The UID of either the symbol/evidence that is the input of the rule
		/// </summary>
		public int input;
		/// <summary>
		/// bool to track if this is based on a symbol or evidence
		/// </summary>
		public bool isSymbolBased;
		/// <summary>
		/// Tracks if this is an equal or not equal
		/// </summary>
		public bool equal;
		/// <summary>
		/// Trackes the UID of the disposal unit
		/// </summary>
		public int disposal;

		public Rule(int input, bool equal, int disposal, bool isSymbolBased=false) {
			this.input = input;
			this.equal = equal;
			this.disposal = disposal;
			this.isSymbolBased = isSymbolBased;
		}
	}

	/// <summary>
	/// Generates random rules for the game
	/// </summary>
	/// <param name="keySet"> the set of rules displayed on the in game screen, to the player</param>
	private Rule[] GenerateRules() {
		List<Rule> keySet = new List<Rule>();

		//[TODO] Make this rule generation interesting.

		//For each evidence, choose a recipticle for it to go into.
		for (int i = 0; i < evidence.Length; i++) {
			keySet.Add(new Rule(evidence[i].UID, true, disposalMachines[Random.Range(0, disposalMachines.Length)].UID));
		}

		return keySet.ToArray();
	}


	Color32 black = new Color32(0, 0, 0, 255);
	Color32 transparent = new Color32(0, 0, 0, 0);

	IEnumerator FadeOut() {
		float maxtime = 3.0f;
		float time = 3.0f;
		while (time > 0.0f) {
			time -= Time.deltaTime;
			blackoutImage.color = Color32.Lerp(black, transparent, time/maxtime);

			yield return null;
		}
		blackoutImage.color = black;
	}

	IEnumerator FadeIn() {
		float maxtime = 3.0f;
		float time = 3.0f;
		while (time > 0.0f) {
			time -= Time.deltaTime;
			blackoutImage.color = Color32.Lerp(transparent, black, time / maxtime);
			yield return null;
		}
		blackoutImage.color = transparent;
	}
}
