using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{

	public Button BotaoBack;
	public string nomeCena = "";


	void Start ()
	{

		// =========SETAR BOTOES==========//
		BotaoBack.onClick = new Button.ButtonClickedEvent ();
		BotaoBack.onClick.AddListener (() => Back ());
	}

	//===========VOIDS NORMAIS=========//
	private void Back ()
	{
		SceneManager.LoadScene (nomeCena);
	}
}