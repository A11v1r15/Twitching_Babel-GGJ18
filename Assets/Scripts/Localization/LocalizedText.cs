using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{

	public string key;

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (GO ());
	}

	private IEnumerator GO ()
	{
		while (!LocalizationManager.instance.GetIsReady ()) {
			yield return null;
		}

		Text text = GetComponent<Text> ();
		text.text = LocalizationManager.instance.GetLocalizedValue (key);
	}
}