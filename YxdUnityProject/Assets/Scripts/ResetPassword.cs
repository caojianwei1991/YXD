﻿using UnityEngine;
using System.Collections;
using SimpleJSON;

public class ResetPassword : MonoBehaviour
{
	public UIInput inputUserName, inputNumber, inputCode, inputPassword, inputConfirm;
	UILabel mLabel;
	UIButton mButton;
	int remainingTime = 60;
	string validationCode;
	
	public static void Show ()
	{
		UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot> ();
		NGUITools.AddChild (uiRoot.gameObject, (GameObject)Resources.Load ("Prefabs/ResetPassword"));
	}
	
	void Awake ()
	{
		transform.FindChild ("Texture/No").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Destroy (gameObject)));
		transform.FindChild ("Texture/Yes").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => StartResetPassword ()));
		mLabel = transform.FindChild ("Texture/InputNumber/BtnGetCode").GetComponent<UILabel> ();
		mButton = mLabel.gameObject.GetComponent<UIButton> ();
		mButton.onClick.Add (new EventDelegate (() => GetCode ()));
	}
	
	void StartResetPassword ()
	{
		if (/*inputUserName.value.Trim ().Length < 1 || */inputNumber.value.Trim ().Length < 1 || inputCode.value.Trim ().Length < 1 || inputPassword.value.Trim ().Length < 1 || inputConfirm.value.Trim ().Length < 1)
		{
			Alert.Show ("信息填写不完整，请补充！");
			return;
		}
		if (inputPassword.value != inputConfirm.value)
		{
			Alert.Show ("两次新密码不一致，请重新输入！");
			return;
		}
		var wf = new WWWForm ();
		wf.AddField ("StudentId", inputNumber.value.Trim ());
		wf.AddField ("Password", WWWProvider.GetMD5 (inputPassword.value.Trim ()));
		wf.AddField ("VerificationCode", inputCode.value.Trim ());
		WWWProvider.Instance.StartWWWCommunication ("/student/resetPassword", wf, (x, y) =>
		{
			var jn = JSONNode.Parse (y);
			if (jn ["result"].AsInt == 1)
			{
				Alert.Show ("密码重置成功，请重新登录！", () => Destroy (gameObject));
			}
			else
			{
				Alert.Show ("密码重置失败，请重新重置！");
			}
		});
	}
	
	void GetCode ()
	{
		if (inputNumber.value.Trim ().Length != 11)
		{
			Alert.Show ("手机号位数不对，请重新输入！");
			return;
		}
		validationCode = "";
		mButton.isEnabled = false;
		remainingTime = 60;
		InvokeRepeating ("RefreshRemainTime", 0, 1);
		var wf = new WWWForm ();
		wf.AddField ("PhoneNumber", inputNumber.value.Trim ());
		WWWProvider.Instance.StartWWWCommunication ("/sms/validationCode", wf, (x, y) =>
		{
			var jn = JSONNode.Parse (y);
			if (jn ["result"].AsInt == 1)
			{
				validationCode = jn ["data"] ["content"].Value.Trim ();
			}
			else
			{
				Alert.Show ("获取验证码失败，请重新获取！");
				mLabel.text = "获取验证码";
				mButton.isEnabled = true;
				CancelInvoke ("RefreshRemainTime");
			}
		});
	}
	
	void RefreshRemainTime ()
	{
		if (--remainingTime <= 0)
		{
			mLabel.text = "获取验证码";
			mButton.isEnabled = true;
			CancelInvoke ("RefreshRemainTime");
		}
		else
		{
			mLabel.text = string.Format ("{0}秒后重发", remainingTime);
		}
	}
}
