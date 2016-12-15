using UnityEngine;
using System.Collections;

public class ResetPassword : MonoBehaviour
{
	public UIInput inputUserName, inputNumber, inputCode, inputPassword, inputConfirm;
	UILabel mLabel;
	UIButton mButton;
	int remainingTime = 60;
	
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
		if (inputUserName.value.Length < 1 || inputNumber.value.Length < 1 || inputCode.value.Length < 1 || inputPassword.value.Length < 1 || inputConfirm.value.Length < 1)
		{
			Alert.Show ("信息填写不完整，请补充");
			return;
		}
		if (inputPassword.value != inputConfirm.value)
		{
			Alert.Show ("两次新密码不一致，请重新输入");
			return;
		}
		
	}
	
	void GetCode ()
	{
		if (inputNumber.value.Length != 11)
		{
			Alert.Show ("手机号位数不对，请重新输入");
			return;
		}
		mButton.isEnabled = false;
		remainingTime = 5;
		InvokeRepeating ("RefreshRemainTime", 0, 1);
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
