using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;

public class SpeechRecognizer : MonoBehaviour
{
	MainGameController mgc;
	bool isStop;

	void Awake ()
	{
		mgc = transform.root.GetComponent<MainGameController> ();
	}

	public void OnPress (bool pressed)
	{
		if (pressed)
		{
			AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
			if (isStop)
			{
				jo.Call ("StopIse");
			}
			else
			{
				jo.Call ("StartIse", mgc.answer.IsEnglish ? "en_us" : "zh_cn", "[word]" + mgc.answer.Name);
			}
			isStop = !isStop;
		}
		else
		{
			//CallSpeechRecognizer ("StopSpeechRecognizer");
		}
	}
}
