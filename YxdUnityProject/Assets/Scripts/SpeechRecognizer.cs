using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;

public class SpeechRecognizer : MonoBehaviour
{
	string language;
	string answer;

	/// <summary>
	/// 设置识别语言和内容
	/// </summary>
	/// <param name="IsEnglish">"zh_cn"是中文，"en_us"是英文.</param>
	/// <param name="Answer">Answer.</param>
	public void SetLanguageAndAnswer (bool IsEnglish, string Answer)
	{
		language = IsEnglish ? "en_us" : "zh_cn";
		answer = "[word]" + Answer;
	}

	void OnPress (bool pressed)
	{
		if (pressed)
		{
			AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
			jo.Call ("StartIse", language, answer);
		}
		else
		{
			//CallSpeechRecognizer ("StopSpeechRecognizer");
		}
	}
}
