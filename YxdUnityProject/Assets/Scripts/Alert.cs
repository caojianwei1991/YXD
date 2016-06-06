using UnityEngine;
using System.Collections;

public class Alert
{
	public static void ShowNoInput()
	{
		UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot>();
		NGUITools.AddChild(uiRoot.gameObject, (GameObject)Resources.Load("Prefabs/NoInput"));

	}
}
