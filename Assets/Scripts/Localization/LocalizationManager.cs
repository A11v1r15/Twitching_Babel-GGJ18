using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour
{

	public static LocalizationManager instance;

	private Dictionary<string, string> localizedText;
	private bool isReady = false;
	private string missingTextString = "Localized text not found";

	// Use this for initialization
	void Awake ()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
	}

	void Start ()
	{
		LoadLocalizedText (PlayerPrefs.GetString("Language", "ENG") + ".json");
	}

	public void LoadLocalizedText (string fileName)
	{
		string filePath;

		localizedText = new Dictionary<string, string> ();

		if (Application.platform == RuntimePlatform.Android) {
			// Android
			string oriPath = System.IO.Path.Combine (Application.streamingAssetsPath, fileName);

			// Android only use WWW to read file
			WWW reader = new WWW (oriPath);
			while (!reader.isDone) {
			}

			filePath = Application.persistentDataPath + "/loc";
			System.IO.File.WriteAllBytes (filePath, reader.bytes);

		} else {
			//Original
			filePath = Path.Combine (Application.streamingAssetsPath, fileName);
			// iOS
			//dbPath = System.IO.Path.Combine(Application.streamingAssetsPath, "db.bytes");
		}
		//string filePath = Path.Combine (Application.streamingAssetsPath, fileName);

		if (File.Exists (filePath)) {
			string dataAsJson = File.ReadAllText (filePath);
			LocalizationData loadedData = JsonUtility.FromJson<LocalizationData> (dataAsJson);

			for (int i = 0; i < loadedData.items.Length; i++) {
				localizedText.Add (loadedData.items [i].key, loadedData.items [i].value);   
			}

			Debug.Log ("Data loaded, dictionary contains: " + localizedText.Count + " entries");
		} else {
			Debug.LogError ("Cannot find file!");
		}

		isReady = true;
	}

	public string GetLocalizedValue (string key)
	{
		string result = missingTextString;
		if (localizedText.ContainsKey (key)) {
			result = localizedText [key];
		}

		return result;

	}

	public bool GetIsReady ()
	{
		return isReady;
	}

}