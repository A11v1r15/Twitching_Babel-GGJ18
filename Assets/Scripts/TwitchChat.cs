using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class TwitchChat : MonoBehaviour
{
	public Sprite Kappa;
	public List<Tmessage> messages;

	private TcpClient twitchClient;
	private StreamReader reader;
	private StreamWriter writer;

	public List<string> pool;
	public List<int> warPool;
	public List<int> faminePool;
	public List<int> pestilencePool;
	public List<int> deathPool;
	public List<int> ddosPool;
	public Queue<Tmessage> queue;

	public Dictionary<int, string> statuses;

	private string username, password, channelName;
	//Get the password from https://twitchapps.com/tmi

	public Text chatBox;

	void Start ()
	{	
		GUIStyle style = new GUIStyle ();
		style.richText = true;
		username = PlayerPrefs.GetString ("User");
		password = PlayerPrefs.GetString ("Password");
		channelName = PlayerPrefs.GetString ("Channel");
		queue 			= new Queue<Tmessage> ();
		messages 		= new List<Tmessage> ();
		statuses 		= new Dictionary<int, string> ();
		warPool			= new List<int> ();
		faminePool 		= new List<int> ();
		pestilencePool 	= new List<int> ();
		deathPool 		= new List<int> ();
		ddosPool		= new List<int> ();
		Connect ();
	}

	void Update ()
	{
		if (!twitchClient.Connected) {
			Connect ();
		}

		ReadChat ();
	}

	private void Connect ()
	{
		twitchClient = new TcpClient ("irc.chat.twitch.tv", 6667);
		reader = new StreamReader (twitchClient.GetStream ());
		writer = new StreamWriter (twitchClient.GetStream ());

		writer.WriteLine ("PASS " + password);
		writer.WriteLine ("NICK " + username);
		writer.WriteLine ("USER " + username + " 8 * :" + username);
		writer.WriteLine ("JOIN #" + channelName);
		writer.Flush ();
	}

	private void ReadChat ()
	{
		if (twitchClient.Available > 0) {
			var message = reader.ReadLine ();

			if (message.Contains ("PRIVMSG")) {
				//get users name
				var splitPoint = message.IndexOf ("!", 1);
				var chatName = message.Substring (0, splitPoint);
				chatName = chatName.Substring (1);

				//get users message
				splitPoint = message.IndexOf (":", 1);
				message = message.Substring (splitPoint + 1);
				print (String.Format ("{0}: {1}", chatName, message));
				Tmessage current = new Tmessage (chatName, message);
				queue.Enqueue (current);
				pool.Add (current.command);
				int tempPool;
				if (current.command == (local ("command_war"))) {
					if (Int32.TryParse (current.arg, out tempPool))
						warPool.Add (tempPool);
				}
				if (current.command ==  (local ("command_famine"))) {
					if (Int32.TryParse (current.arg, out tempPool))
						faminePool.Add (tempPool);
				}
				if (current.command ==  (local ("command_pestilence"))) {
					if (Int32.TryParse (current.arg, out tempPool))
						pestilencePool.Add (tempPool);
				}
				if (current.command ==  (local ("command_death"))) {
					if (Int32.TryParse (current.arg, out tempPool))
						deathPool.Add (tempPool);
				}
				if (current.command ==  (local ("command_ddos"))) {
					if (Int32.TryParse (current.arg, out tempPool))
						ddosPool.Add (tempPool);
				}

				//display
				if (queue.Count > 10)
					queue.Dequeue ();
				printQueue ();

				//kappa
				if (current.command == "kappa") {
					GameObject kappa = new GameObject ();
					kappa.AddComponent<Image> ();
					kappa.GetComponent<Image> ().sprite = Kappa;
					kappa.transform.position =	new Vector2 (UnityEngine.Random.value * Screen.width - Screen.width / 2, -100f);
					kappa.tag = "kappa";
					kappa.GetComponent<RectTransform> ().localScale = new Vector3 (1f + UnityEngine.Random.value * 2f, 1f + UnityEngine.Random.value * 2f, 1f);
					kappa.transform.SetParent (FindObjectOfType<Canvas> ().transform);
				}

				//put in the layer
				if (current.command == local ("command_msg")) {
					GetComponent<Tower> ().currentLayer.GetComponentInChildren<Text> ().text += String.Format ("“{1}”\n\t-{0}", current.user, current.displayMessage.Substring (current.displayMessage.IndexOf (' ') + 1)) + "\n";
					messages.Add (current);
				}
			}
		}
	}

	public void closePool ()
	{
		pool.Sort ();
		Dictionary<string, int> temp = new Dictionary<string, int> ();
		if (pool.Contains (local ("command_voice")))
			temp.Add ("voice", pool.LastIndexOf (local ("command_voice")) - pool.IndexOf (local ("command_voice")));
		if (pool.Contains (local ("command_text")))
			temp.Add ("text", pool.LastIndexOf (local ("command_text")) - pool.IndexOf (local ("command_text")));
		if (pool.Contains (local ("command_eletricity")))
			temp.Add ("eletricity", pool.LastIndexOf (local ("command_eletricity"))	- pool.IndexOf (local ("command_eletricity")));
		if (pool.Contains (local ("command_radio")))
			temp.Add ("radio", pool.LastIndexOf (local ("command_radio")) - pool.IndexOf (local ("command_radio")));
		if (pool.Contains (local ("command_data")))
			temp.Add ("data", pool.LastIndexOf (local ("command_data")) - pool.IndexOf (local ("command_data")));
		if (pool.Contains (local ("command_war"))) {
			temp.Add ("war", pool.LastIndexOf (local ("command_war")) - pool.IndexOf (local ("command_war")));
		}
		if (pool.Contains (local ("command_famine"))) {
			temp.Add ("famine", pool.LastIndexOf (local ("command_famine")) - pool.IndexOf (local ("command_famine")));
		}
		if (pool.Contains (local ("command_pestilence"))) {
			temp.Add ("pestilence", pool.LastIndexOf (local ("command_pestilence"))	- pool.IndexOf (local ("command_pestilence")));
		}
		if (pool.Contains (local ("command_death"))) {
			temp.Add ("death", pool.LastIndexOf (local ("command_death")) - pool.IndexOf (local ("command_death")));
		}
		if (pool.Contains (local ("command_ddos"))) {
			temp.Add ("ddos", pool.LastIndexOf (local ("command_ddos")) - pool.IndexOf (local ("command_ddos")));
		}
		if (temp.Count > 0) {
			string fracz = temp.OrderByDescending (pair => pair.Value).First ().Key;
			if (fracz == "voice" || fracz == "text" || fracz == "eletricity" || fracz == "radio" || fracz == "data") {
				GetComponent<Tower> ().nextMedia = fracz;
			} else if (fracz == "war") {
				if (frequentier (warPool) > 0)
					statuses.Add (frequentier (warPool), "war");
					else
				GetComponent<Tower> ().nextState = "war";
			} else if (fracz == "famine") {
				if (frequentier (faminePool) > 0)
					statuses.Add (frequentier (faminePool), "famine");
				else
					GetComponent<Tower> ().nextState = "famine";
			} else if (fracz == "pestilence") {
				if (frequentier (pestilencePool) > 0)
					statuses.Add (frequentier (pestilencePool), "pestilence");
				else
				GetComponent<Tower> ().nextState = "pestilence";
			} else if (fracz == "death") {
				if (frequentier (deathPool) > 0)
					statuses.Add (frequentier (deathPool), "death");
				else
				GetComponent<Tower> ().nextState = "death";
			} else if (fracz == "ddos") {
				if (frequentier (ddosPool) > 0)
					statuses.Add (frequentier (ddosPool), "ddos");
				else
				GetComponent<Tower> ().nextState = "ddos";
			}
		}
		pool.Clear ();
	}

	void printQueue ()
	{
		chatBox.text = "";
		foreach (Tmessage m in queue) {
			chatBox.text = chatBox.text + "\n" + m;
		}
	}

	public int frequentier (List<int> test){
		Dictionary<int, int> dick = new Dictionary<int, int> ();
		test.Sort ();
		foreach (int t in test) {
			dick.Add (t, (test.LastIndexOf (t) - test.IndexOf (t)));
		}
		return (dick[dick.OrderByDescending (pair => pair.Value).First ().Key] > 1) ? dick.OrderByDescending (pair => pair.Value).First ().Key : 0;
	}

	private string local (string s)
	{
		return LocalizationManager.instance.GetLocalizedValue (s);
	}
}
