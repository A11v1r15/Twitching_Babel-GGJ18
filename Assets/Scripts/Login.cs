using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
	public InputField user;
	public InputField password;
	public InputField channel;
	public Button login;
	public string cena = "";

	void Start ()
	{
		user.text = PlayerPrefs.GetString ("User", "");
		password.text = PlayerPrefs.GetString ("Password", "");
		channel.text = PlayerPrefs.GetString ("Channel", "");
		login.onClick.AddListener (() => button());
		ChangeLanguage.instance.start (PlayerPrefs.GetString("Language", "ENG"));
	}

	private void button ()
	{	
		if (user.text != "" && password.text != "" && channel.text != "") {
			PlayerPrefs.SetString  ("User",		user.text);
			PlayerPrefs.SetString  ("Password",	password.text);
			PlayerPrefs.SetString  ("Channel",	channel.text);
			SceneManager.LoadScene (cena);
		}
	}
}
