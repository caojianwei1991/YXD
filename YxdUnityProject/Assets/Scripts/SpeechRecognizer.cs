using UnityEngine;
using System.Collections;

public class SpeechRecognizer : MonoBehaviour
{
	string[] recognitionLanguage = {"zh_cn","en_us"};
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void CallSpeechRecognizer (string methodName)
	{
		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
		jo.Call (methodName, recognitionLanguage[capKeyIndex]);
	}
	int capKeyIndex;
	void OnPress (bool pressed)
	{
		if (pressed)
		{
			speechStr = "";
			CallSpeechRecognizer ("StartSpeechRecognizer");
		}
		else
		{
			if (capKeyIndex == 0)
			{
				capKeyIndex = 1;
			}
			else
			{
				capKeyIndex = 0;
			}
			CallSpeechRecognizer ("StopSpeechRecognizer");
		}
	}

	string speechStr;

	void ReceiveSpeechRecognizer (string str)
	{
		speechStr += str;
		transform.FindChild ("Label").GetComponent<UILabel> ().text = speechStr;
	}
}
