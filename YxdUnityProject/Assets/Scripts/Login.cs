using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;

public class Login : MonoBehaviour
{
	UIInput inputSchoolID, inputUserName;
	UIToggle isSavePSW;

	void Awake ()
	{
		inputSchoolID = transform.FindChild ("InputSchoolID").GetComponent<UIInput> ();
		inputUserName = transform.FindChild ("InputUserName").GetComponent<UIInput> ();
		isSavePSW = transform.FindChild ("RememberPSW").GetComponent<UIToggle> ();
		transform.FindChild ("Login").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => StartLogin ()));
		transform.FindChild ("Exit").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Application.Quit ()));
		transform.FindChild ("Test").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Test ()));
		transform.FindChild ("RandomPlay").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => RandomPlay ()));
	}

	void Start ()
	{
		inputSchoolID.value = PlayerPrefs.GetString ("InputSchoolID", "");
		inputUserName.value = PlayerPrefs.GetString ("InputUserName", "");
		isSavePSW.value = PlayerPrefs.GetInt ("IsSavePSW", 1) == 1;
	}
	
	void StartLogin ()
	{
		if (inputSchoolID.value.Length < 1 || inputUserName.value.Length < 1)
		{
			Alert.ShowNoInput ();
			return;
		}
		var jc = new JSONClass ();
		jc.Add ("SchoolID", inputSchoolID.value);
		jc.Add ("IPAddress", "52.221.227.248");
		WWWProvider.Instance.StartWWWCommunication ("GetServerURL", jc, CheckUser);
	}

	void CheckUser (bool IsSuccess, string JsonData)
	{
		var jc = new JSONClass ();
		jc.Add ("StudentID", inputUserName.value);
		jc.Add ("SchoolID", inputSchoolID.value);
		WWWProvider.Instance.StartWWWCommunication ("CheckUser", jc, (x, y) =>
		{
			var jn = JSONNode.Parse (y);
			if (jn ["IsSuccess"].Value == "1")
			{
				bool b = isSavePSW.value;
				PlayerPrefs.SetString ("InputSchoolID", b ? inputSchoolID.value : "");
				PlayerPrefs.SetString ("InputUserName", b ? inputUserName.value : "");
				PlayerPrefs.SetInt ("IsSavePSW", b ? 1 : 0);
				LocalStorage.SchoolID = inputSchoolID.value;
				LocalStorage.StudentID = inputUserName.value;
				LocalStorage.Language = transform.FindChild ("Chinese").GetComponent<UIToggle> ().value ? "0" : "1";
				Application.LoadLevel ("Download");
			}
			else
			{

			}
		});
	}

	void Test ()
	{

	}

	void RandomPlay ()
	{
		if (Application.internetReachability != NetworkReachability.NotReachable)
		{

		}
	}
}
