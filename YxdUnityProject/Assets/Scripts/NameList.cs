using UnityEngine;
using System.Collections;

public class NameList : MonoBehaviour
{
	GameObject scrollView;

	public static void Show ()
	{
		UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot> ();
		NGUITools.AddChild (uiRoot.gameObject, (GameObject)Resources.Load ("Prefabs/NameList"));
	}

	void Awake ()
	{
		transform.FindChild ("Back").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => 
		{
			ClassList.Show ();
			Destroy (gameObject);
		}));
		scrollView = transform.FindChild ("ScrollView").gameObject;
	}

	void Start ()
	{
		transform.FindChild ("Sprite/Label").GetComponent<UILabel> ().text = LocalStorage.SelectClassName;
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
			var tran = NGUITools.AddChild (scrollView, (GameObject)Resources.Load ("Prefabs/NameListItem")).transform;
			tran.FindChild ("Label").GetComponent<UILabel> ().text = "123";
			UIButton ub = tran.GetComponent<UIButton> ();
			if (true)
			{
				ub.normalSprite = "桌子-p";
			}
			ub.onClick.Clear ();
			ub.onClick.Add (new EventDelegate (() => 
			{

			}));
		}
		for (int i = 0; i < 30 - 2; i++)
		{
			Transform tran = NGUITools.AddChild (scrollView, (GameObject)Resources.Load ("Prefabs/NameListItem")).transform;
			tran.GetComponent<UIWidget> ().alpha = 0;
		}
		scrollView.GetComponent<UIGrid> ().repositionNow = true;
		scrollView.GetComponent<UIScrollView> ().ResetPosition ();
	}
}
