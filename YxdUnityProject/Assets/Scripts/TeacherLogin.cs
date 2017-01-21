using UnityEngine;
using System.Collections;
using SimpleJSON;

public class TeacherLogin : MonoBehaviour
{
	UIInput inputUserName;
	UIInput inputPassword;

	public static void Show ()
	{
		UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot> ();
		NGUITools.AddChild (uiRoot.gameObject, (GameObject)Resources.Load ("Prefabs/TeacherLogin"));
	}

	void Awake ()
	{
		inputUserName = transform.FindChild ("Texture/InputUserName").GetComponent<UIInput> ();
		inputPassword = transform.FindChild ("Texture/InputPassword").GetComponent<UIInput> ();
		transform.FindChild ("Texture/No").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Destroy (gameObject)));
		transform.FindChild ("Texture/Yes").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Login ()));
		inputUserName.value = PlayerPrefs.GetString ("InputUserName", "");
		inputPassword.value = PlayerPrefs.GetString ("InputPassword", "");
	}
	
	void Login ()
	{
		if (inputUserName.value.Trim ().Length > 0 && inputPassword.value.Trim ().Length > 0)
		{
			var wf = new WWWForm ();
			wf.AddField ("TeacherId", inputUserName.value.Trim ());
			wf.AddField ("Password", WWWProvider.GetMD5 (inputPassword.value.Trim ()));
			WWWProvider.Instance.StartWWWCommunication ("/teacher/login", wf, (x, y) =>
			{
				Destroy (gameObject);
				var jn = JSONNode.Parse (y);
				if (jn ["result"].AsInt == 1)
				{
					LocalStorage.accountType = AccountType.Teacher;
					LocalStorage.TeacherID = jn ["data"] ["id"].AsInt;
					ClassList.Show ();
				}
				else
				{
					Alert.Show ("用户名或秘密错误，是否忘记密码？", () => ResetPassword.Show (), () => {});
				}
			});
		}
		else
		{
			Alert.Show ("没有输入教师用户名或密码，请重新输入！");
		}
	}

}
