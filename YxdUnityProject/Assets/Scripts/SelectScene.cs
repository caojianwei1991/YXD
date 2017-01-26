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
		transform.FindChild ("RedHeart").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Share.Show ()));
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
		WWWProvider.Instance.StartWWWCommunication ("GetAboutText", jc, (x, y) =>
		{
			var jn = JSONNode.Parse (y);
			Alert.ShowAbout (jn ["aboutText"].Value);
		});
	}
	
	void Start ()
	{
		redHeartLabel.text = LocalStorage.Score.ToString ();
		SoundPlay.Instance.PlayBG ();
		if (LocalStorage.accountType == AccountType.Teacher)
		{
			ClassList.Show ();
		}
		else
		{
			PaperList.Show ();
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

	void RefreshWeeks (JSONNode jsonNode)
	{
		for (int i = 0; i < content.transform.childCount; i++)
		{
			Destroy (content.transform.GetChild (i).gameObject);
		}
		for (int i = 0; i < jsonNode.Count; i++)
		{
			int index = i;
			var tran = NGUITools.AddChild (content, (GameObject)Resources.Load ("Prefabs/SceneListItem")).transform;
			tran.FindChild ("Label").GetComponent<UILabel> ().text = jsonNode [index] ["paperName"].Value;
			UIButton ub = tran.GetComponent<UIButton> ();
			ub.onClick.Clear ();
			ub.onClick.Add (new EventDelegate (() => 
			{
				
			}));
		}
		content.GetComponent<UIWrapContent> ().WrapContent ();
		content.transform.parent.GetComponent<UIScrollView> ().ResetPosition ();
	}
}
