using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;

public class SoundPlay : MonoBehaviour
{
	static SoundPlay instance;
	static AudioSource gameAudioSource;
	static AudioSource bGAudioSource;
	List<IEnumerator> iEnumeratorList = new List<IEnumerator> ();
	
	public static SoundPlay Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = new GameObject ("SoundPlay");
				instance = go.AddComponent<SoundPlay> ();
				gameAudioSource = go.AddComponent<AudioSource> ();
				gameAudioSource.playOnAwake = false;
				gameAudioSource.loop = false;
				bGAudioSource = go.AddComponent<AudioSource> ();
				bGAudioSource.playOnAwake = false;
				bGAudioSource.loop = true;
				bGAudioSource.volume = 0.1f;
				DontDestroyOnLoad (go);
			}
			return instance;
		}
	}

	string[] gameSoundPaths = 
	{
		"",
		"1-欢迎来到易学岛",
		"2-图中的动物是什么",
		"3-图中的水果是什么",
		"4-你真棒",
		"5-你真聪明",
		"6-你好厉害呀",
		"7-啊哦，这次没对哦",
		"8-很遗憾，答错啦",
		"9-真遗憾，再来一次吧",
		"10-你能告诉我这个是什么吗",
		"11-篮子里的是",
		"12-请跟我读",
		"13-你能写出来这个是什么水果吗",
		"14-应该这样写",
		"15-再写一次吧",
		"16-哪个图片是",
		"17-这个是",
		"18-你再找找看",
		"19-给动物放上正确的标记吧",
		"20-给水果放上正确的标记吧",
		"21-哎呀，再来一次吧",
		"22-错啦，我不是这个",
		"23-欢迎下次再来光临易学岛",
		"24-你玩太久啦，先休息一下吧"
	};
	string[] bGSoundPaths = 
	{
		"Alexandre Desplat - Whack-Bat Majorette Ensemble",
		"Deyan Pavlov - La Marche Des Petits Renards",
		"Lullatone - Here Comes the Sweater Weather (纯音乐)",
		"Sing, R. Sing! - 幼女幻奏",
		"The Chemical Brothers - The Devil is in the details",
		"中山真斗 - 水谷家の食卓",
		"仲西匡 - 秋と落ち葉と",
		"増田俊郎 - 祝!恋ばな成就!",
		"金子隆博 - 二人でお酒"
	};

	public void Play (string CharacterID, bool IsEnglish, Action CallBack = null)
	{
		var ie = StartPlay (CharacterID, IsEnglish, CallBack);
		iEnumeratorList.Add (ie);
		StartCoroutine (ie);
	}

	IEnumerator StartPlay (string CharacterID, bool IsEnglish, Action CallBack)
	{
		var clip = AssetData.GetVoiceByID (CharacterID, IsEnglish);
		if (clip == null)
		{
			AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
			isReceiveTts = false;
			jo.Call ("StartTts", AssetData.GetNameByID (CharacterID, IsEnglish));
			yield return StartCoroutine (WaitTtsResult ());
		}
		else
		{
			gameAudioSource.clip = clip;
			gameAudioSource.Play ();
			yield return new WaitForSeconds (gameAudioSource.clip.length);

		}
		if (CallBack != null)
		{
			CallBack ();
		}
	}

	bool isReceiveTts;

	void ReceiveTts (string s)
	{
		isReceiveTts = true;
	}

	IEnumerator WaitTtsResult ()
	{
		while (!isReceiveTts)
		{
			yield return new WaitForSeconds (0.5f);
		}
	}

	public void PlayLocal (int GameSoundID, bool IsEnglish, Action CallBack = null)
	{
		var ie = StartPlayLocal (GameSoundID, IsEnglish, CallBack);
		iEnumeratorList.Add (ie);
		StartCoroutine (ie);
	}

	IEnumerator StartPlayLocal (int GameSoundID, bool IsEnglish, Action CallBack)
	{
		StringBuilder path = new StringBuilder ();
		path.Append ("Sound/");
		path.Append (gameSoundPaths [GameSoundID]);
		path.Append (IsEnglish ? "/E" : "/C");
		gameAudioSource.clip = (AudioClip)Resources.Load (path.ToString ());
		gameAudioSource.Play ();
		yield return new WaitForSeconds (gameAudioSource.clip.length);
		if (CallBack != null)
		{
			CallBack ();
		}
	}

	int lastBGID = -1;

	public void PlayBG ()
	{
		if (!LocalStorage.IsSwitchBG)
		{
			return;
		}
		int randomID = UnityEngine.Random.Range (0, bGSoundPaths.Length);
		while (randomID == lastBGID)
		{
			randomID = UnityEngine.Random.Range (0, bGSoundPaths.Length);
		}
		lastBGID = randomID;
		StringBuilder path = new StringBuilder ();
		path.Append ("Sound/");
		path.Append ("BackgroundMusic/");
		path.Append (bGSoundPaths [randomID]);
		bGAudioSource.clip = (AudioClip)Resources.Load (path.ToString ());
		bGAudioSource.Play ();
	}

	public static void PauseBG (bool isPause)
	{
		if (isPause)
		{
			if (bGAudioSource.isPlaying)
			{
				bGAudioSource.Pause ();
			}
		}
		else
		{
			if (!bGAudioSource.isPlaying)
			{
				bGAudioSource.Play ();
			}
		}
	}

	public void DestroyiEnumeratorList ()
	{
		for (int i = 0; i < iEnumeratorList.Count; i++)
		{
			if (iEnumeratorList [i] != null)
			{
				StopCoroutine (iEnumeratorList [i]);
			}
		}
	}
}
