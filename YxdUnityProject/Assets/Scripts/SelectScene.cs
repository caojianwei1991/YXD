using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SelectScene : MonoBehaviour
{
	UILabel redHeartLabel;

	void Awake ()
	{
		redHeartLabel = transform.FindChild ("RedHeart/Label").GetComponent<UILabel> ();
		redHeartLabel.transform.parent.GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => {}));
		transform.FindChild ("About").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => RequestAboutContent ()));
		transform.FindChild ("Back").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Application.LoadLevel ("Login")));
		transform.FindChild ("Zoo").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => 
		{
			LocalStorage.SceneID = "0";
			Application.LoadLevel ("Zoo");
		}));
		transform.FindChild ("Orchard").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => 
		{
			LocalStorage.SceneID = "1";
			Application.LoadLevel ("Orchard");
		}));
	}

	void RequestAboutContent ()
	{
		var jc = new JSONClass ();
		jc.Add ("SchoolID", LocalStorage.SchoolID);
		jc.Add ("APPStatus", "0");
		WWWProvider.Instance.StartWWWCommunication ("GetAboutText", jc, (x, y) =>
		{
			var jn = JSONNode.Parse (y);
			Alert.ShowAbout (jn ["aboutText"].Value);
		});
	}
	
	void Start ()
	{
		redHeartLabel.text = LocalStorage.Score.ToString();
	}
}
