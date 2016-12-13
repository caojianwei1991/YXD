using UnityEngine;
using System.Collections;

public class ClassList : MonoBehaviour
{
	public static void Show ()
	{
		UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot> ();
		NGUITools.AddChild (uiRoot.gameObject, (GameObject)Resources.Load ("Prefabs/ClassList"));
	}

	void Awake ()
	{
		transform.FindChild ("Back").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Destroy (gameObject)));
	}

	void Start ()
	{

	}


}
