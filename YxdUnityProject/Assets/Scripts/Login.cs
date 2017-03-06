using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;

public class Login : MonoBehaviour
{
	UIInput inputUserName, inputPassword;
	UIToggle isSavePSW, cnUIToggle, enUIToggle;
	WWWProvider wwwProvider;
	UIButton btnExit, btnLogin, btnRandomPlay;

	void Awake ()
	{
		wwwProvider = WWWProvider.Instance;
		inputUserName = transform.FindChild ("InputUserName").GetComponent<UIInput> ();
		inputPassword = transform.FindChild ("InputPassword").GetComponent<UIInput> ();
		isSavePSW = transform.FindChild ("RememberPSW").GetComponent<UIToggle> ();
		cnUIToggle = transform.FindChild ("Chinese").GetComponent<UIToggle> ();
		enUIToggle = transform.FindChild ("English").GetComponent<UIToggle> ();
		btnLogin = transform.FindChild ("Login").GetComponent<UIButton> ();
		btnLogin.onClick.Add (new EventDelegate (() => StartLogin ()));
		btnExit = transform.FindChild ("Exit").GetComponent<UIButton> ();
		btnExit.onClick.Add (new EventDelegate (() => wwwProvider.Exit ()));
		transform.FindChild ("Test").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Test ()));
		btnRandomPlay = transform.FindChild ("RandomPlay").GetComponent<UIButton> ();
		btnRandomPlay.onClick.Add (new EventDelegate (() => RandomPlay ()));
		cnUIToggle.onChange.Add (new EventDelegate (() => ChangeToggle ()));
		transform.FindChild ("Instruction").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => TeacherLogin.Show ()));
		inputUserName.onChange.Add (new EventDelegate (() =>
		{
			string str = PlayerPrefs.GetString ("InputUserName", "");
			string str1 = inputUserName.value.Trim ();
			if (str != "" && str1 != "")
			{
				if (str1.Length > str.Length || str.Substring (0, str1.Length) != str1)
				{
					inputPassword.value = "";
					inputUserName.onChange.Clear ();
				}
			}
		}));
		LocalStorage.SchoolID = "";
		//LocalStorage.StudentID = "";
		LocalStorage.Score = 0;
		LocalStorage.IsTest = false;
		LocalStorage.SelectClassID = -1;
	}

	void Start ()
	{
		inputUserName.value = PlayerPrefs.GetString ("InputUserName", "");
		inputPassword.value = PlayerPrefs.GetString ("InputPassword", "");
		isSavePSW.value = PlayerPrefs.GetInt ("IsSavePSW", 1) == 1;
		LocalStorage.Language = "0";//PlayerPrefs.GetString ("Language", "0");
		if (LocalStorage.Language == "0")
		{
			cnUIToggle.value = true;
			enUIToggle.value = false;
		}
		else
		{
			cnUIToggle.value = false;
			enUIToggle.value = true;
		}
		SoundPlay.Instance.PlayLocal (25, enUIToggle.value, () => SoundPlay.Instance.PlayBG ());
	}

	void ChangeToggle ()
	{
		LocalStorage.Language = cnUIToggle.value ? "0" : "1";
		if (cnUIToggle.value)
		{
			btnExit.normalSprite = "exit_cn";
			btnLogin.normalSprite = "login_cn";
			btnRandomPlay.GetComponent<UITexture> ().mainTexture = (Texture)Resources.Load ("Texture/casual_cn");
		}
		else
		{
			btnExit.normalSprite = "exit_en";
			btnLogin.normalSprite = "login_en";
			btnRandomPlay.GetComponent<UITexture> ().mainTexture = (Texture)Resources.Load ("Texture/casual_en");
		}
	}
	
	void StartLogin ()
	{
//		if (inputUserName.value.Trim ().Length < 1 || inputPassword.value.Trim ().Length < 1)
//		{
//			SignUp.Show ();
//			return;
//		}
		var wf = new WWWForm ();
		wf.AddField ("StudentId", inputUserName.value.Trim ());
		wf.AddField ("Password", WWWProvider.GetMD5 (inputPassword.value.Trim ()));
		WWWProvider.Instance.StartWWWCommunication ("/student/login", wf, (x, y) =>
		{
			var jn = JSONNode.Parse (y);
			if (jn ["result"].AsInt == 1)
			{
				bool b = isSavePSW.value;
				PlayerPrefs.SetString ("InputUserName", b ? inputUserName.value.Trim () : "");
				PlayerPrefs.SetString ("InputPassword", b ? inputPassword.value.Trim () : "");
				PlayerPrefs.SetInt ("IsSavePSW", b ? 1 : 0);
				LocalStorage.accountType = AccountType.Student;
				LocalStorage.StudentID = jn ["data"] ["id"].AsInt;
				Application.LoadLevel ("SelectScene");
			}
			else
			{
				inputUserName.value = "";
				inputPassword.value = "";
				Alert.Show ("用户名或秘密错误，是否忘记密码？", () => ResetPassword.Show (), () => {});
			}
		});
	}

	public void StartSignUp()
	{
		SignUp.Show ();
	}

	void EnterQuizPlay ()
	{
		bool b = isSavePSW.value;
		PlayerPrefs.SetString ("InputUserName", b ? inputUserName.value.Trim () : "");
		PlayerPrefs.SetString ("InputPassword", b ? inputPassword.value.Trim () : "");
		PlayerPrefs.SetInt ("IsSavePSW", b ? 1 : 0);
		LocalStorage.SchoolID = inputUserName.value;
		//LocalStorage.StudentID = inputPassword.value;
		LocalStorage.Language = cnUIToggle.value ? "0" : "1";
		PlayerPrefs.SetString ("Language", LocalStorage.Language);
		Application.LoadLevel ("Download");
	}

	void Test ()
	{
		LocalStorage.IsTest = true;
		StartLogin ();
	}

	void RandomPlay ()
	{
		LocalStorage.accountType = AccountType.RandomPlay;
		Application.LoadLevel ("Zoo");
		return;
		if (inputUserName.value.Length < 1 || inputPassword.value.Length < 1)
		{
			Alert.ShowInputInfo ((UserName, Email) =>
			{
				if (UserName.Length > 0 && Email.Length > 0)
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
				jc.Add ("SchoolID", inputUserName.value);
				jc.Add ("IPAddress", "52.221.227.248");
				//WWWProvider.Instance.StartWWWCommunication ("GetServerURL", jc, RandomPlayCheckUser);
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
		jc.Add ("SchoolID", inputPassword.value);
//		WWWProvider.Instance.StartWWWCommunication ("CheckUser", jc, (x, y) =>
//		{
//			var jn = JSONNode.Parse (y);
//			if (jn ["IsSuccess"].Value == "1")
//			{
//				LocalStorage.IsRandomPlay = true;
//				EnterQuizPlay ();
//			}
//			else
//			{
//				Alert.Show ("用户名或秘密错误，请重新输入！");
//			}
//		});
	}

	void UpdateUserID (string UserName, string Email)
	{
		var jc = new JSONClass ();
		jc.Add ("UserName", UserName);
		jc.Add ("Email", Email);
		jc.Add ("loginMachnie", SystemInfo.deviceUniqueIdentifier);
		jc.Add ("currentDateTime", DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss"));
		if (Application.internetReachability != NetworkReachability.NotReachable)
		{
//			WWWProvider.Instance.StartWWWCommunication ("UpdateUserID", jc, (x, y) =>
//			{
//				var jn = JSONNode.Parse (y);
//				if (jn ["IsSuccess"].Value == "1")
//				{
//					//LocalStorage.StudentID = UserName;
//					LocalStorage.Email = Email;
//					EnterRandomPlay ();
//				}
//			});
		}
		else
		{
			EnterRandomPlay ();
		}
	}

	void EnterRandomPlay ()
	{
		LocalStorage.SchoolID = "kudospark";
		LocalStorage.IsRandomPlay = true;
		LocalStorage.Language = cnUIToggle.value ? "0" : "1";
		PlayerPrefs.SetString ("Language", LocalStorage.Language);
		Application.LoadLevel ("Download");
	}
}
