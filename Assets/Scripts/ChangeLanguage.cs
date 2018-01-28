using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLanguage : MonoBehaviour
{
	public static ChangeLanguage instance;

	void Awake ()
	{
		if (instance == null) {
			instance = this;
		}
	}

	public void button (string loc)
	{
		PlayerPrefs.SetString ("Language", loc);
		LocalizationManager.instance.LoadLocalizedText (PlayerPrefs.GetString("Language", "ENG") + ".json");
		StartCoroutine (GO ());
	}

	public void start (string loc)
	{
		PlayerPrefs.SetString ("Language", loc);
		LocalizationManager.instance.LoadLocalizedText (PlayerPrefs.GetString("Language", "ENG") + ".json");
	}

	private IEnumerator GO ()
	{
		while (!LocalizationManager.instance.GetIsReady ()) {
			yield return null;
		}

		SceneManager.LoadScene ("Login");
	}
}