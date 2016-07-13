using UnityEngine;
using System.Collections;
using System;

public class Alert : MonoBehaviour
{
	static Transform mTransform;

	public static void Show (string text, Action ok = null, Action cancel = null)
	{
		if (mTransform != null)
		{
			Destroy (mTransform.gameObject);
		}
		UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot> ();
		mTransform = NGUITools.AddChild (uiRoot.gameObject, (GameObject)Resources.Load ("Prefabs/Alert")).transform;
		mTransform.FindChild ("Texture/Label").GetComponent<UILabel> ().text = text;
		UIButton okUIButton = mTransform.FindChild ("Texture/Yes").GetComponent<UIButton> ();
		UIButton cancelUIButton = mTransform.FindChild ("Texture/No").GetComponent<UIButton> ();
		if (LocalStorage.Language == "1")
		{
			okUIButton.normalSprite = "yes_en";
			cancelUIButton.normalSprite = "no_en";
		}
		else
		{
			okUIButton.normalSprite = "yes_cn";
			cancelUIButton.normalSprite = "no_cn";
		}
		if (cancel == null)
		{
			okUIButton.transform.localPosition = new Vector3 (0, -250, 0);
			cancelUIButton.gameObject.SetActive (false);
		}
		EventDelegate.Set (okUIButton.onClick, delegate
		{
			if (ok != null)
			{
				ok ();
			}
			if (mTransform != null)
			{
				Destroy (mTransform.gameObject);
			}
		});
		EventDelegate.Set (cancelUIButton.onClick, delegate
		{
			if (cancel != null)
			{
				cancel ();
			}
			if (mTransform != null)
			{
				Destroy (mTransform.gameObject);
			}
		});
	}

	public static void ShowAbout (string text)
	{
		if (mTransform != null)
		{
			Destroy (mTransform.gameObject);
		}
		UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot> ();
		mTransform = NGUITools.AddChild (uiRoot.gameObject, (GameObject)Resources.Load ("Prefabs/About")).transform;
		mTransform.FindChild ("Texture/Label").GetComponent<UILabel> ().text = text;
		EventDelegate.Set (mTransform.FindChild ("Texture/Close").GetComponent<UIButton> ().onClick, delegate
		{
			if (mTransform != null)
			{
				Destroy (mTransform.gameObject);
			}
		});
	}

	public static void ShowInputInfo (Action<string, string> ok)
	{
		if (mTransform != null)
		{
			Destroy (mTransform.gameObject);
		}
		UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot> ();
		mTransform = NGUITools.AddChild (uiRoot.gameObject, (GameObject)Resources.Load ("Prefabs/InputInfo")).transform;
		UIInput inputUserName = mTransform.FindChild ("Texture/InputUserName").GetComponent<UIInput> ();
		UIInput inputEmail = mTransform.FindChild ("Texture/InputEmail").GetComponent<UIInput> ();
		EventDelegate.Set (mTransform.FindChild ("Texture/Yes").GetComponent<UIButton> ().onClick, delegate
		{
			ok.Invoke (inputUserName.value, inputEmail.value);
			if (mTransform != null)
			{
				Destroy (mTransform.gameObject);
			}
		});
		EventDelegate.Set (mTransform.FindChild ("Texture/No").GetComponent<UIButton> ().onClick, delegate
		{
			if (mTransform != null)
			{
				Destroy (mTransform.gameObject);
			}
		});
	}
}
