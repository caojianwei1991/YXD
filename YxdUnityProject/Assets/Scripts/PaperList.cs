using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PaperList : MonoBehaviour
{
	GameObject scrollView;

	public static void Show ()
	{
		UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot> ();
		NGUITools.AddChild (uiRoot.gameObject, (GameObject)Resources.Load ("Prefabs/PaperList"));
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
		var wf = new WWWForm ();
		wf.AddField ("TestType", 1);
		wf.AddField ("StudentId", LocalStorage.StudentID);
		wf.AddField ("GradeId", LocalStorage.SelectClassID);
		WWWProvider.Instance.StartWWWCommunication ("/paper/list", wf, (x, y) =>
		{
			var jn = JSONNode.Parse (y);
			if (jn ["result"].AsInt == 1)
			{
				RefreshClassName (jn ["data"]);
			}
			else
			{
				Debug.LogError ("Get PaperList Fail!");
			}
		});
	}

	void RefreshClassName (JSONNode jsonNode)
	{
		for (int i = 0; i < scrollView.transform.childCount; i++)
		{
			Destroy (scrollView.transform.GetChild (i).gameObject);
		}
		for (int i = 0; i < jsonNode.Count; i++)
		{
			int index = i;
			var tran = NGUITools.AddChild (scrollView, (GameObject)Resources.Load ("Prefabs/NameListItem")).transform;
			tran.FindChild ("Label").GetComponent<UILabel> ().text = jsonNode [index] ["name"].Value;
			UIButton ub = tran.GetComponent<UIButton> ();
			ub.onClick.Clear ();
			ub.onClick.Add (new EventDelegate (() => 
			{

			}));
		}
		for (int i = 0; i < 30 - jsonNode.Count; i++)
		{
			Transform tran = NGUITools.AddChild (scrollView, (GameObject)Resources.Load ("Prefabs/NameListItem")).transform;
			tran.GetComponent<UIWidget> ().alpha = 0;
		}
		scrollView.GetComponent<UIGrid> ().repositionNow = true;
		scrollView.GetComponent<UIScrollView> ().ResetPosition ();
	}
}
