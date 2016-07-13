using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SelectScene : MonoBehaviour
{
	UILabel redHeartLabel;
	UIButton btnZoo, btnOrchard;

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
		btnZoo = transform.FindChild ("Zoo").GetComponent<UIButton> ();
		btnZoo.onClick.Add (new EventDelegate (() => 
		{
			LocalStorage.IsSwitchBG = true;
			LocalStorage.SceneID = "0";
			Application.LoadLevel ("Zoo");
		}));
		btnOrchard = transform.FindChild ("Orchard").GetComponent<UIButton> ();
		btnOrchard.onClick.Add (new EventDelegate (() => 
		{
			LocalStorage.IsSwitchBG = true;
			LocalStorage.SceneID = "1";
			Application.LoadLevel ("Orchard");
		}));
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
		if (LocalStorage.Language == "1")
		{
			btnZoo.GetComponent<UITexture> ().mainTexture = (Texture)Resources.Load ("Texture/zoo_en");
			btnOrchard.GetComponent<UITexture> ().mainTexture = (Texture)Resources.Load ("Texture/orchard_en");
		}
		else
		{
			btnZoo.GetComponent<UITexture> ().mainTexture = (Texture)Resources.Load ("Texture/zoo_cn");
			btnOrchard.GetComponent<UITexture> ().mainTexture = (Texture)Resources.Load ("Texture/orchard_cn");
		}
	}
}
