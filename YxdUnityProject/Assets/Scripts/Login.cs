using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;

public class Login : MonoBehaviour
{
	UIInput inputSchoolID, inputUserName;
	UIToggle isSavePSW, cnUIToggle, enUIToggle;

	void Awake ()
	{
		inputSchoolID = transform.FindChild ("InputSchoolID").GetComponent<UIInput> ();
		inputUserName = transform.FindChild ("InputUserName").GetComponent<UIInput> ();
		isSavePSW = transform.FindChild ("RememberPSW").GetComponent<UIToggle> ();
		cnUIToggle = transform.FindChild ("Chinese").GetComponent<UIToggle> ();
		enUIToggle = transform.FindChild ("English").GetComponent<UIToggle> ();
		transform.FindChild ("Login").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => StartLogin ()));
		transform.FindChild ("Exit").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Application.Quit ()));
		transform.FindChild ("Test").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Test ()));
		transform.FindChild ("RandomPlay").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => RandomPlay ()));
		LocalStorage.StudentID = "";
		LocalStorage.Score = 0;
	}

	void Start ()
	{
		var a = WWWProvider.Instance;
		inputSchoolID.value = PlayerPrefs.GetString ("InputSchoolID", "");
		inputUserName.value = PlayerPrefs.GetString ("InputUserName", "");
		isSavePSW.value = PlayerPrefs.GetInt ("IsSavePSW", 1) == 1;
		if (PlayerPrefs.GetString ("Language", "0") == "0")
		{
			cnUIToggle.value = true;
			enUIToggle.value = false;
		}
		else
		{
			cnUIToggle.value = false;
			enUIToggle.value = true;
		}
	}
	
	void StartLogin ()
	{
		if (inputSchoolID.value.Length < 1 || inputUserName.value.Length < 1)
		{
			Alert.Show ("你没有输入学校或用户\n名信息，是否返回输入");
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
				LocalStorage.IsRandomPlay = false;
				EnterQuizPlay ();
			}
			else
			{
				Alert.Show ("用户名或秘密错误，请重新输入！");
			}
		});
	}

	void EnterQuizPlay ()
	{
		bool b = isSavePSW.value;
		PlayerPrefs.SetString ("InputSchoolID", b ? inputSchoolID.value : "");
		PlayerPrefs.SetString ("InputUserName", b ? inputUserName.value : "");
		PlayerPrefs.SetInt ("IsSavePSW", b ? 1 : 0);
		LocalStorage.SchoolID = inputSchoolID.value;
		LocalStorage.StudentID = inputUserName.value;
		LocalStorage.Language = cnUIToggle.value ? "0" : "1";
		PlayerPrefs.SetString ("Language", LocalStorage.Language);
		Application.LoadLevel ("Download");
	}

	void Test ()
	{
		StartLogin ();
	}

	void RandomPlay ()
	{
		if (inputSchoolID.value.Length < 1 || inputUserName.value.Length < 1)
		{
			Alert.ShowInputInfo ((UserName, Email) =>
			{
				if (UserName.Length > 0)
				{
					UpdateUserID (UserName, Email);
				}
				else
				{
					EnterRandomPlay ();
				}
			});
		}
		else
		{
			if (Application.internetReachability != NetworkReachability.NotReachable)
			{
				var jc = new JSONClass ();
				jc.Add ("SchoolID", inputSchoolID.value);
				jc.Add ("IPAddress", "52.221.227.248");
				WWWProvider.Instance.StartWWWCommunication ("GetServerURL", jc, RandomPlayCheckUser);
			}
			else
			{
				EnterRandomPlay ();
			}
		}
	}

	void RandomPlayCheckUser (bool IsSuccess, string JsonData)
	{
		var jc = new JSONClass ();
		jc.Add ("StudentID", inputUserName.value);
		jc.Add ("SchoolID", inputSchoolID.value);
		WWWProvider.Instance.StartWWWCommunication ("CheckUser", jc, (x, y) =>
		{
			var jn = JSONNode.Parse (y);
			if (jn ["IsSuccess"].Value == "1")
			{
				LocalStorage.IsRandomPlay = true;
				EnterQuizPlay ();
			}
			else
			{
				Alert.Show ("用户名或秘密错误，请重新输入！");
			}
		});
	}

	void UpdateUserID (string UserName, string Email)
	{
		var jc = new JSONClass ();
		jc.Add ("UserName", LocalStorage.StudentID);
		jc.Add ("Email", LocalStorage.Email);
		jc.Add ("loginMachnie", SystemInfo.deviceUniqueIdentifier);
		jc.Add ("currentDateTime", DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss"));
		if (Application.internetReachability != NetworkReachability.NotReachable)
		{
			WWWProvider.Instance.StartWWWCommunication ("UpdateUserID", jc, (x, y) =>
			{
				var jn = JSONNode.Parse (y);
				if (jn ["IsSuccess"].Value == "1")
				{
					LocalStorage.StudentID = UserName;
					LocalStorage.Email = Email;
					EnterRandomPlay ();
				}
			});
		}
		else
		{
			EnterRandomPlay ();
		}
	}

	void EnterRandomPlay ()
	{
		LocalStorage.IsRandomPlay = true;
		LocalStorage.Language = cnUIToggle.value ? "0" : "1";
		PlayerPrefs.SetString ("Language", LocalStorage.Language);
		Application.LoadLevel ("Download");
	}
}
