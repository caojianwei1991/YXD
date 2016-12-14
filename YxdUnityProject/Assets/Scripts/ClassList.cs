﻿using UnityEngine;
using System.Collections;

public class ClassList : MonoBehaviour
{
	GameObject scrollView;

	public static void Show ()
	{
		UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot> ();
		NGUITools.AddChild (uiRoot.gameObject, (GameObject)Resources.Load ("Prefabs/ClassList"));
	}

	void Awake ()
	{
		transform.FindChild ("Back").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Destroy (gameObject)));
		scrollView = transform.FindChild ("ScrollView").gameObject;
	}

	void Start ()
	{
		RefreshClassName ();
	}

	void RefreshClassName ()
	{
		for (int i = 0; i < scrollView.transform.childCount; i++)
		{
			Destroy (scrollView.transform.GetChild (i).gameObject);
		}
		for (int i = 0; i < 2; i++)
		{
			var tran = NGUITools.AddChild (scrollView, (GameObject)Resources.Load ("Prefabs/ClassListItem")).transform;
			tran.FindChild ("Label").GetComponent<UILabel> ().text = "123";
			UIButton ub = tran.GetComponent<UIButton> ();
			ub.onClick.Clear ();
			ub.onClick.Add (new EventDelegate (() => 
			{
				LocalStorage.SelectClassID = 0;
				LocalStorage.SelectClassName = "123";
				NameList.Show ();
				Destroy (gameObject);
			}));
		}
		for (int i = 0; i < 30 - 2; i++)
		{
			Transform tran = NGUITools.AddChild (scrollView, (GameObject)Resources.Load ("Prefabs/ClassListItem")).transform;
			tran.GetComponent<UIWidget> ().alpha = 0;
		}
		scrollView.GetComponent<UIGrid> ().repositionNow = true;
		scrollView.GetComponent<UIScrollView> ().ResetPosition ();
	}
}
