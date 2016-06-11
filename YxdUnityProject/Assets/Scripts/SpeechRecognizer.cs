using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;

public class SpeechRecognizer : MonoBehaviour
{
	bool isEnglish;
	string answer;

	/// <summary>
	/// 设置识别语言和内容
	/// </summary>
	/// <param name="IsEnglish">"zh_cn"是中文，"en_us"是英文.</param>
	/// <param name="Answer">Answer.</param>
	public void SetLanguageAndAnswer (bool IsEnglish, string Answer)
	{
		isEnglish = IsEnglish;
		answer = Answer;
	}

	void OnPress (bool pressed)
	{
		if (pressed)
		{
			AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
			jo.Call ("StartIse", isEnglish ? "en_us" : "zh_cn", "[word]" + answer);
		}
		else
		{
			//CallSpeechRecognizer ("StopSpeechRecognizer");
		}
	}
}
