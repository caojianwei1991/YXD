using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;

public class SpeechRecognizer : MonoBehaviour
{
	MainGameController mgc;

	void Awake ()
	{
		mgc = transform.root.GetComponent<MainGameController> ();
	}

	void OnPress (bool pressed)
	{
		if (pressed)
		{
			AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
			jo.Call ("StartIse", mgc.answer.IsEnglish ? "en_us" : "zh_cn", mgc.answer.Name);
		}
		else
		{
			//CallSpeechRecognizer ("StopSpeechRecognizer");
		}
	}
}
