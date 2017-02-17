using UnityEngine;
using System.Collections;
using SimpleJSON;

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
		transform.FindChild ("Back").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => 
		{
			LocalStorage.IsSwitchBG = false;
			Application.LoadLevel ("Login");
		}));
		scrollView = transform.FindChild ("ScrollView").gameObject;
	}

	void Start ()
	{
		var wf = new WWWForm ();
		wf.AddField ("TeacherId", LocalStorage.TeacherID);
		WWWProvider.Instance.StartWWWCommunication ("/grade/listByTeacher", wf, (x, y) =>
		{
			var jn = JSONNode.Parse (y);
			if (jn ["result"].AsInt == 1)
			{
				RefreshClassName (jn ["data"]);
			}
			else
			{
				Debug.LogError (string.Format ("Get /grade/listByTeacher Fail! TeacherID:{0}", LocalStorage.TeacherID));
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
			var tran = NGUITools.AddChild (scrollView, (GameObject)Resources.Load ("Prefabs/ClassListItem")).transform;
			tran.FindChild ("Label").GetComponent<UILabel> ().text = jsonNode [index] ["name"].Value;
			UIButton ub = tran.GetComponent<UIButton> ();
			ub.onClick.Clear ();
			ub.onClick.Add (new EventDelegate (() => 
			{
				LocalStorage.SelectClassID = jsonNode [index] ["id"].AsInt;
				LocalStorage.SelectClassName = jsonNode [index] ["name"].Value;
				PaperList.Show ();
				Destroy (gameObject);
			}));
		}
		for (int i = 0; i < 30 - jsonNode.Count; i++)
		{
			Transform tran = NGUITools.AddChild (scrollView, (GameObject)Resources.Load ("Prefabs/ClassListItem")).transform;
			tran.GetComponent<UIWidget> ().alpha = 0;
		}
		scrollView.GetComponent<UIGrid> ().repositionNow = true;
		scrollView.GetComponent<UIScrollView> ().ResetPosition ();
	}
}
