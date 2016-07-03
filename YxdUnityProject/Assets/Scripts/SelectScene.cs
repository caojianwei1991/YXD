using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SelectScene : MonoBehaviour
{
	UILabel redHeartLabel;

	void Awake ()
	{
		redHeartLabel = transform.FindChild ("RedHeart/Label").GetComponent<UILabel> ();
		transform.FindChild ("About").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => RequestAboutContent ()));
		transform.FindChild ("Back").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => 
		{
			LocalStorage.IsSwitchBG = false;
			Application.LoadLevel ("Login");
		}));
		transform.FindChild ("Zoo").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => 
		{
			LocalStorage.IsSwitchBG = true;
			LocalStorage.SceneID = "0";
			Application.LoadLevel ("Zoo");
		}));
		transform.FindChild ("Orchard").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => 
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
	}
}
