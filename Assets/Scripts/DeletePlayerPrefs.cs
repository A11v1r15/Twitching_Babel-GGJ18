#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class DeletePlayerPrefs : EditorWindow
{
	[MenuItem ("Window/Clear PlayerPrefs")]
	static void Init ()
	{
		PlayerPrefs.DeleteAll ();
	}
}
#endif