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
}
