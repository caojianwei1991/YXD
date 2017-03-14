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
		transform.FindChild ("Close").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => 
		{
			Destroy (gameObject);
			if (LocalStorage.accountType == AccountType.Teacher)
			{
				ClassList.Show ();
			}
		}));
		scrollView = transform.FindChild ("ScrollView").gameObject;
	}

	void Start ()
	{
		transform.FindChild ("Label").GetComponent<UILabel> ().text = LocalStorage.accountType == AccountType.Teacher ? LocalStorage.SelectClassName : string.Format("第{0}周", LocalStorage.SelectWeek);
		var wf = new WWWForm ();
		int TestType = LocalStorage.accountType == AccountType.Teacher ? 1 : 0;
		wf.AddField ("TestType", TestType);
		wf.AddField ("StudentId", LocalStorage.StudentID);
		wf.AddField ("GradeId", LocalStorage.SelectClassID);
		wf.AddField ("Category", LocalStorage.SelectWeek);
		WWWProvider.Instance.StartWWWCommunication ("/test/listTestPaper", wf, (x, y) =>
		{
			var jn = JSONNode.Parse (y);
			if (jn ["result"].AsInt == 1)
			{
				RefreshClassName (jn ["data"]);
			}
			else
			{
				Debug.LogError (string.Format ("Get /test/listTestPaper Fail! TestType:{0},StudentId:{1},GradeId:{2},", TestType, LocalStorage.StudentID, LocalStorage.SelectClassID));
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
			var tran = NGUITools.AddChild (scrollView, (GameObject)Resources.Load ("Prefabs/PaperListItem")).transform;
			tran.FindChild ("Label").GetComponent<UILabel> ().text = jsonNode [index] ["paperName"].Value;
			UIButton ub = tran.GetComponent<UIButton> ();
			ub.onClick.Clear ();
			ub.onClick.Add (new EventDelegate (() => 
			{
				LocalStorage.PaperID = jsonNode [index] ["id"].AsInt;
				Application.LoadLevel ("Zoo");
			}));
		}
		for (int i = 0; i < 3 - jsonNode.Count; i++)
		{
			Transform tran = NGUITools.AddChild (scrollView, (GameObject)Resources.Load ("Prefabs/PaperListItem")).transform;
			tran.GetComponent<UIWidget> ().alpha = 0;
		}
		scrollView.GetComponent<UIGrid> ().repositionNow = true;
		scrollView.GetComponent<UIScrollView> ().ResetPosition ();
	}
}
