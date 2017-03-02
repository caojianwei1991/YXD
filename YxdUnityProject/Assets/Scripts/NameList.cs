using UnityEngine;
using System.Collections;
using SimpleJSON;

public class NameList : MonoBehaviour
{
	GameObject scrollView;
	MainGameController mgc;

	public static void Show ()
	{
		UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot> ();
		NGUITools.AddChild (uiRoot.gameObject, (GameObject)Resources.Load ("Prefabs/NameList"));
	}

	void Awake ()
	{
		transform.FindChild ("Back").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => 
		{
			Destroy (gameObject);
		}));
		scrollView = transform.FindChild ("ScrollView").gameObject;
	}

	void Start ()
	{
		mgc = transform.parent.GetComponent<MainGameController> ();
		transform.FindChild ("Sprite/Label").GetComponent<UILabel> ().text = LocalStorage.SelectClassName;
		var wf = new WWWForm ();
		wf.AddField ("GradeId", LocalStorage.SelectClassID);
		WWWProvider.Instance.StartWWWCommunication ("/student/listByGrade", wf, (x, y) =>
		{
			var jn = JSONNode.Parse (y);
			if (jn ["result"].AsInt == 1)
			{
				RefreshName (jn ["data"]);
			}
			else
			{
				Debug.LogError (string.Format ("Get /student/listByGrade Fail! GradeId:{0}", LocalStorage.SelectClassID));
			}
		});
	}

	void RefreshName (JSONNode jsonNode)
	{
		for (int i = 0; i < scrollView.transform.childCount; i++)
		{
			Destroy (scrollView.transform.GetChild (i).gameObject);
		}
		for (int i = 0; i < jsonNode.Count; i++)
		{
			int index = i;
			var tran = NGUITools.AddChild (scrollView, (GameObject)Resources.Load ("Prefabs/NameListItem")).transform;
			tran.FindChild ("Label").GetComponent<UILabel> ().text = jsonNode [index] ["studentName"].Value;
			UIButton ub = tran.GetComponent<UIButton> ();
			if (true)
			{
				ub.normalSprite = "桌子-p";
			}
			ub.onClick.Clear ();
			ub.onClick.Add (new EventDelegate (() => 
			{
				mgc.studentName.text = jsonNode [index] ["studentName"].Value;
				mgc.practiceStudentID.Add (jsonNode [index] ["studentId"].AsInt);
				mgc.finishPractice.SetActive(true);
				Destroy (gameObject);
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
