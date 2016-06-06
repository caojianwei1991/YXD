using UnityEngine;
using System.Collections;

public class SelectScene : MonoBehaviour
{
	UILabel redHeartLabel;

	void Awake ()
	{
		redHeartLabel = transform.FindChild ("RedHeart/Label").GetComponent<UILabel> ();
		redHeartLabel.transform.parent.GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => {}));
		transform.FindChild ("About").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => {}));
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
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
