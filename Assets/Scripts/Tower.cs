using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{

	public Sprite TowerLayer;
	public Sprite TowerBase;
	public Sprite[] TowerSpecialLayer;
	public Sprite[] MessageSprites;
	public Font TowerFont;
	public GameObject currentLayer;
	public GameObject messageIcon;
	public Color textColor;
	public float baseSpeed;
	public float speedMultiplier = 1.0f;
	public string currentMedia;
	public string currentState;
	public string nextState;
	public string nextMedia;
	public int integrity;
	public float baseDeterioration = 0.05f;
	public float stateDeterioration = 0.00f;
	public List<GameObject> tower = new List<GameObject> ();
	public float voiceSpeed, textSpeed, eletricitySpeed, radioSpeed, dataSpeed;

	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < 100; i++) {
			GameObject temp = new GameObject ();
			GameObject tempText = new GameObject ();
			GameObject tempNumber = new GameObject ();
			temp.AddComponent<Image> ();
			if (i == 0) {
				temp.GetComponent<Image> ().sprite = TowerBase;
			} else if (Random.value < 0.3f) {
				temp.GetComponent<Image> ().sprite = TowerSpecialLayer [(int)(Random.value * TowerSpecialLayer.Length)];
			} else {
				temp.GetComponent<Image> ().sprite = TowerLayer;
			}
			temp.transform.position = new Vector2 (Screen.width / 2f + ((i == 0) ? 0 : 25), 115f + temp.GetComponent<RectTransform> ().rect.height * 1.5f * i / 6 * 5);
			if (i == 0)
				temp.GetComponent<RectTransform> ().localScale = new Vector3 (6.5f, 2.2f, 1.0f);
			else
				temp.GetComponent<RectTransform> ().localScale = new Vector3 (4.5f - 0.03f * i, 1.5f, 1.0f);
			temp.name = "Layer " + (i + 1);
			tempText.transform.SetParent (temp.transform);
			tempText.transform.position = temp.transform.position;
			tempText.AddComponent<Text> ();
			tempText.GetComponent<Text> ().font = TowerFont;
			tempText.GetComponent<Text> ().color = textColor;
			tempText.name = "Text " + (i + 1);
			tempNumber.transform.SetParent (temp.transform);
			tempNumber.transform.position = new Vector2 (temp.transform.position.x * 1.5f, temp.transform.position.y);
			tempNumber.AddComponent<Text> ();
			tempNumber.GetComponent<Text> ().font = TowerFont;
			tempNumber.GetComponent<Text> ().color = Color.black;
			tempNumber.GetComponent<Text> ().text = (i + 1).ToString ();
			tempNumber.name = "Number " + (i + 1);
			tower.Add (temp);
		}
		tower.Reverse ();
		foreach (var item in tower) {
			item.transform.SetParent (GameObject.Find ("Tower").transform);
		}
		tower.Reverse ();
		voiceSpeed = 0.2f;
		textSpeed = 0.4f;
		eletricitySpeed	= 0.6f;
		radioSpeed = 0.8f;
		dataSpeed = 1.0f;
		string[] types = { "voice", "text", "eletricity", "radio", "data" };
		int rand = Mathf.FloorToInt (Random.value * types.Length);
		currentMedia = types [rand];
		nextMedia = types [rand];
		messageIcon.GetComponent<Image> ().sprite = MessageSprites [rand];
	}
	
	// Update is called once per frame
	void Update ()
	{
		GameObject bg = GameObject.Find ("BackGround");

		float finalSpeed = baseSpeed * Mathf.Max (Mathf.Min ((speedMultiplier), 0.2f), 1.0f);

		bg.transform.position = new Vector2 (bg.transform.position.x, bg.transform.position.y - finalSpeed * Time.deltaTime / 2f);

		foreach (var item in tower) {
			item.transform.position = new Vector2 (item.transform.position.x, item.transform.position.y - finalSpeed * Time.deltaTime);
		}

		foreach (var item in GameObject.FindGameObjectsWithTag("kappa")) {
			item.transform.position = new Vector2 (item.transform.position.x, item.transform.position.y + 200 * Time.deltaTime);
		}

		foreach (var item in tower) {
			Vector3[] v = new Vector3[4];
			item.GetComponent<RectTransform> ().GetWorldCorners (v);

			float minY = Mathf.Min (v [0].y, v [1].y, v [2].y, v [3].y);

			if (minY > Screen.height - item.GetComponent<RectTransform> ().rect.height * 2) {
				if (item != currentLayer)
					changeLayer (item);
				break;
			}
		}
	}

	private void changeLayer (GameObject nLayer)
	{
		GetComponent<TwitchChat> ().SendMessage ("closePool");
		currentLayer = nLayer;
		changeMedia ();
		updateState ();
		messageIcon.transform.SetParent (currentLayer.transform);
		messageIcon.transform.localPosition = Vector2.zero;
	}

	private void changeMedia ()
	{
		if (currentMedia != nextMedia) {
			baseDeterioration += 0.02f;
		} else {
			baseDeterioration -= 0.02f;
		}
		currentMedia = nextMedia;
		if (Random.value < (baseDeterioration + stateDeterioration)) {
			deteriorate ();
		}
		switch (currentMedia) {
		case "voice":
			messageIcon.GetComponent<Image> ().sprite = MessageSprites [0];
			break;
		case "text":
			messageIcon.GetComponent<Image> ().sprite = MessageSprites [1];
			break;
		case "eletricity":
			messageIcon.GetComponent<Image> ().sprite = MessageSprites [2];
			break;
		case "radio":
			messageIcon.GetComponent<Image> ().sprite = MessageSprites [3];
			break;
		case "data":
			messageIcon.GetComponent<Image> ().sprite = MessageSprites [4];
			break;
		}
	}

	private void deteriorate ()
	{
		integrity--;
	}

	private void updateState ()
	{	
		if (currentState == "") {
			currentState = nextState;
			nextState = "";
			foreach (int k in GetComponent<TwitchChat>().statuses.Keys) {
				if (currentLayer.name == "Layer " + k) {
					currentState = GetComponent<TwitchChat> ().statuses [k];
				}
			}
		} else {
			currentState = "";
		}
		switch (currentState) {
		case "war":
			switch (currentMedia) {
			case "voice":
				stateDeterioration = 0.05f;
				break;
			case "text":
				stateDeterioration = 0.10f;
				break;
			case "eletricity":
				eletricitySpeed += 0.40f;
				stateDeterioration = -0.05f;
				break;
			case "radio":
				radioSpeed += 0.20f;
				stateDeterioration = 0.00f;
				break;
			case "data":
				dataSpeed -= 0.20f;
				stateDeterioration = 0.00f;
				break;
			}
			break;
		case "famine":
			switch (currentMedia) {
			case "voice":
				stateDeterioration = 0.10f;
				break;
			case "text":
				speedMultiplier -= 0.20f;
				stateDeterioration = 0.00f;
				break;
			case "eletricity":
				eletricitySpeed -= 0.20f;
				stateDeterioration = 0.00f;
				break;
			case "radio":
				radioSpeed -= 0.20f;
				stateDeterioration = -0.05f;
				break;
			case "data":
				dataSpeed -= 0.20f;
				stateDeterioration = 0.00f;
				break;
			}
			break;
		case "pestilence":
			switch (currentMedia) {
			case "voice":
				stateDeterioration = 0.05f;
				break;
			case "text":
				speedMultiplier -= 0.20f;
				stateDeterioration = -0.10f;
				break;
			case "eletricity":
				eletricitySpeed -= 0.40f;
				stateDeterioration = 0.10f;
				break;
			case "radio":
				radioSpeed += 0.40f;
				stateDeterioration = 0.05f;
				break;
			case "data":
				dataSpeed -= 0.20f;
				stateDeterioration = 0.00f;
				break;
			}
			break;
		case "death":
			switch (currentMedia) {
			case "voice":
				stateDeterioration = 0.05f;
				break;
			case "text":
				speedMultiplier -= 0.20f;
				stateDeterioration = 0.05f;
				break;
			case "eletricity":
				eletricitySpeed -= 0.20f;
				stateDeterioration = 0.05f;
				break;
			case "radio":
				radioSpeed -= 0.20f;
				stateDeterioration = 0.05f;
				break;
			case "data":
				dataSpeed -= 0.20f;
				stateDeterioration = 0.05f;
				break;
			}
			break;
		case "DDOS":
			switch (currentMedia) {
			case "voice":
				voiceSpeed += 0.60f;
				stateDeterioration = -0.05f;
				break;
			case "text":
				speedMultiplier += 0.20f;
				stateDeterioration = -0.05f;
				break;
			case "eletricity":
				eletricitySpeed += 0.20f;
				stateDeterioration = -0.05f;
				break;
			case "radio":
				stateDeterioration = -0.05f;
				break;
			case "data":
				dataSpeed -= 0.60f;
				stateDeterioration = 0.30f;
				break;
			}
			break;
		default:
			stateDeterioration = 0.00f;
			break;
		}
		switch (currentMedia) {
		case "voice":
			speedMultiplier = voiceSpeed;
			break;
		case "text":
			speedMultiplier = textSpeed;
			break;
		case "eletricity":
			speedMultiplier = eletricitySpeed;
			break;
		case "radio":
			speedMultiplier = radioSpeed;
			break;
		case "data":
			speedMultiplier = dataSpeed;
			break;
		}
	}

	public void GameOver(){
		Application.Quit();
	}
}
