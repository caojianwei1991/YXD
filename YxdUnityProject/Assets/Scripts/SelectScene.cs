using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SelectScene : MonoBehaviour
{
	UILabel redHeartLabel;
	UIButton btnZoo, btnOrchard;
	GameObject content;

	void Awake ()
	{
		redHeartLabel = transform.FindChild ("RedHeart/Label").GetComponent<UILabel> ();
		//transform.FindChild ("RedHeart").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Share.Show ()));
		transform.FindChild ("About").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => RequestAboutContent ()));
		transform.FindChild ("Back").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => 
		{
			LocalStorage.IsSwitchBG = false;
			Application.LoadLevel ("Login");
		}));
		content = transform.FindChild ("SceneList/UIWrap Content").gameObject;
//		btnZoo = transform.FindChild ("SceneList/Zoo").GetComponent<UIButton> ();
//		btnZoo.onClick.Add (new EventDelegate (() => 
//		{
//			LocalStorage.IsSwitchBG = true;
//			LocalStorage.SceneID = "0";
//			Application.LoadLevel ("Zoo");
//		}));
//		btnOrchard = transform.FindChild ("SceneList/Orchard").GetComponent<UIButton> ();
//		btnOrchard.onClick.Add (new EventDelegate (() => 
//		{
//			LocalStorage.IsSwitchBG = true;
//			LocalStorage.SceneID = "1";
//			Application.LoadLevel ("Orchard");
//		}));

	}

	void RequestAboutContent ()
	{
		var jc = new JSONClass ();
		jc.Add ("SchoolID", LocalStorage.SchoolID);
		jc.Add ("APPStatus", LocalStorage.SchoolID == "" ? "1" : "0");
//		WWWProvider.Instance.StartWWWCommunication ("GetAboutText", jc, (x, y) =>
//		{
//			var jn = JSONNode.Parse (y);
//			Alert.ShowAbout (jn ["aboutText"].Value);
//		});
	}
	
	void Start ()
	{
		redHeartLabel.text = LocalStorage.Score.ToString ();
		SoundPlay.Instance.PlayBG ();
		if (LocalStorage.accountType == AccountType.Teacher)
		{
			if(LocalStorage.SelectClassID == -1)
			{
				ClassList.Show ();
			}
			else
			{
				PaperList.Show ();
			}
		}
		else
		{
			StartCoroutine (RefreshWeeks ());
		}
//		if (LocalStorage.Language == "1")
//		{
//			btnZoo.GetComponent<UITexture> ().mainTexture = (Texture)Resources.Load ("Texture/zoo_en");
//			btnOrchard.GetComponent<UITexture> ().mainTexture = (Texture)Resources.Load ("Texture/orchard_en");
//		}
//		else
//		{
//			btnZoo.GetComponent<UITexture> ().mainTexture = (Texture)Resources.Load ("Texture/zoo_cn");
//			btnOrchard.GetComponent<UITexture> ().mainTexture = (Texture)Resources.Load ("Texture/orchard_cn");
//		}
	}

	IEnumerator RefreshWeeks ()
	{
		for (int i = 0; i < content.transform.childCount; i++)
		{
			Destroy (content.transform.GetChild (i).gameObject);
		}
		int width = -3150;
		Transform centerTransform = null;
		for (int i = 1; i < 21; i++)
		{
			int index = i;
			var tran = NGUITools.AddChild (content, (GameObject)Resources.Load ("Prefabs/SceneListItem")).transform;
			if (index == LocalStorage.SelectWeek)
			{
				centerTransform = tran;
			}
			tran.localPosition = new Vector3 (width, 0, 0);
			width += 450;
			tran.FindChild ("Label").GetComponent<UILabel> ().text = string.Format ("{0}", index);
			UICenterOnClick uc = tran.GetComponent<UICenterOnClick> ();
			uc.weekIndex = index;
		}
		content.GetComponent<UIWrapContent> ().SortBasedOnScrollMovement ();
		yield return new WaitForEndOfFrame ();
		content.GetComponent<UICenterOnChild> ().CenterOn (centerTransform);
		//content.transform.parent.GetComponent<UIScrollView> ().ResetPosition ();
	}
}
