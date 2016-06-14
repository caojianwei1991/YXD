using UnityEngine;
using System.Collections;
using System;

public class SoundPlay : MonoBehaviour
{
	static SoundPlay instance;
	static AudioSource audioSource;
	
	public static SoundPlay Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = new GameObject ("SoundPlay");
				instance = go.AddComponent<SoundPlay> ();
				audioSource = go.AddComponent<AudioSource> ();
				audioSource.playOnAwake = false;
				audioSource.loop = false;
			}
			return instance;
		}
	}

	public void Play (string CharacterID, bool IsEnglish, Action CallBack = null)
	{
		StartCoroutine (StartPlay (CharacterID, IsEnglish, CallBack));
	}

	IEnumerator StartPlay (string CharacterID, bool IsEnglish, Action CallBack)
	{
		audioSource.clip = AssetData.GetVoiceByID (CharacterID, IsEnglish);
		audioSource.Play ();
		yield return new WaitForSeconds (audioSource.clip.length);
		if (CallBack != null)
		{
			CallBack ();
		}
	}
}
