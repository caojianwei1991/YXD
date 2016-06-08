using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;

public class SpeechRecognizer : MonoBehaviour
{
	string[] recognitionLanguage = {"zh_cn","en_us"};
	public int capKeyIndex;
	public string answer;

	void OnPress (bool pressed)
	{
		if (pressed)
		{
			AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
			jo.Call ("StartIse", recognitionLanguage [capKeyIndex], "[word]" + answer);
		}
		else
		{
			//CallSpeechRecognizer ("StopSpeechRecognizer");
		}
	}
}
