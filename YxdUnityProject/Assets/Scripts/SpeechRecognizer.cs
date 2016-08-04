using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;

public class SpeechRecognizer : MonoBehaviour
{
	MainGameController mgc;
	bool isStop;
	Texture[] voiceTexture = new Texture[3];
	UITexture voiceUITexture;
	IEnumerator mAnimation;

	void Awake ()
	{
		mgc = transform.root.GetComponent<MainGameController> ();
		for (int i = 1; i <= voiceTexture.Length; i++)
		{
			voiceTexture [i - 1] = (Texture)Resources.Load ("Texture/Voice" + i);
		}
		voiceUITexture = GetComponent<UITexture> ();
		mAnimation = StartAnimation ();

	}

	public void Init ()
	{
		gameObject.SetActive (false);
		voiceUITexture.mainTexture = voiceTexture [0];
		StopCoroutine (mAnimation);
		if (isStop)
		{
			AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
			jo.Call ("CancelIse");
		}
		isStop = false;
	}

	public void StopAnimation()
	{
		isStop = false;
		StopCoroutine (mAnimation);
	}

	public void OnPress (bool pressed)
	{
		if (pressed)
		{
			AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
//			if (isStop)
//			{
//				StopCoroutine (mAnimation);
//				jo.Call ("StopIse");
//			}
//			else
			{
				StartCoroutine (mAnimation);
				jo.Call ("StartIse", mgc.answer.IsEnglish ? "en_us" : "zh_cn", "[word]" + mgc.answer.Name);
			}
			isStop = true;
		}
		else
		{
			//CallSpeechRecognizer ("StopSpeechRecognizer");
		}
	}

	IEnumerator StartAnimation ()
	{
		while (gameObject.activeInHierarchy)
		{
			for (int i = 0; i < voiceTexture.Length; i++)
			{
				voiceUITexture.mainTexture = voiceTexture [i];
				yield return new WaitForSeconds (0.5f);
			}
			//yield return new WaitForSeconds (1);
		}
	}

	void OnDestroy ()
	{
		Init ();
	}
}
