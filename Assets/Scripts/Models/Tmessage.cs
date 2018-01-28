using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tmessage
{

	public string user, displayMessage, originalMessage, command, arg;

	public Tmessage (string u, string m)
	{
		user = u;
		originalMessage = m;
		displayMessage = m;
		setCommand ();
	}

	public string getOriginal ()
	{
		return originalMessage;
	}

	public override string ToString ()
	{
		if (command != "" && command != local ("command_msg")) {
			string mColor;
			string uColor;
			if (command == local ("command_war")) {
				mColor = "red";
			} else if (command == local ("command_famine")) {
				mColor = "yellow";
			} else if (command == local ("command_pestilence")) {
				mColor = "green";
			} else if (command == local ("command_death")) {
				mColor = "black";
			} else if (command == local ("command_ddos")) {
				mColor = "blue";
			} else if (command == local ("command_voice")) {
				mColor = "aqua";
			} else if (command == local ("command_text")) {
				mColor = "olive";
			} else if (command == local ("command_eletricity")) {
				mColor = "orange";
			} else if (command == local ("command_radio")) {
				mColor = "purple";
			} else if (command == local ("command_data")) {
				mColor = "lime";
			} else {
				mColor = "grey";
			}
			if (command == local ("command_war") || command == local ("command_famine") || command == local ("command_pestilence") || command == local ("command_death") || command == local ("command_ddos")) {
				uColor = "navy";
			} else if (command == local ("command_voice") || command == local ("command_text") || command == local ("command_eletricity") || command == local ("command_radio") || command == local ("command_data")) {
				uColor = "brown";
			} else {
				uColor = "grey";
			}
			return System.String.Format ("<color={2}>{0}</color>: <color={3}>{1}</color>", user, displayMessage, uColor, mColor);
		} else if (command == local ("command_msg")) {
			return System.String.Format ("<color=magenta>“{0}”\n\t-{1}</color>", displayMessage.Substring (displayMessage.IndexOf (' ') + 1), user);
		} else {
			return System.String.Format ("{0}: {1}", user, displayMessage);
		}
	}

	//Comandos:	war famine pestilence death DDOS
	//			voice text eletricity radio data
	private void setCommand ()
	{
		conditionalCommand ("command_war", true);
		conditionalCommand ("command_famine", true);
		conditionalCommand ("command_pestilence", true);
		conditionalCommand ("command_death", true);
		conditionalCommand ("command_ddos", true);
		conditionalCommand ("command_voice", false);
		conditionalCommand ("command_text", false);
		conditionalCommand ("command_eletricity", false);
		conditionalCommand ("command_radio", false);
		conditionalCommand ("command_data", false);
		conditionalCommand ("command_msg", false);
		if (originalMessage.ToLower ().StartsWith ("kappa "))
			command = "kappa";
	}

	private void conditionalCommand (string com, bool args)
	{
		if (originalMessage.ToLower ().StartsWith (local (com) + " ")) {
			command = local (com);
			displayMessage = originalMessage.Substring (local (com).Length + 1);
			if (args && displayMessage.Contains (" ")) {
				arg = displayMessage.Substring (0, displayMessage.IndexOf (' '));
			} else if (args) {
				arg = displayMessage;
			}
			displayMessage = originalMessage;
		}
	}

	private string local (string s)
	{
		return LocalizationManager.instance.GetLocalizedValue (s);
	}
}