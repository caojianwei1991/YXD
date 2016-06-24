using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class SoundPlay : MonoBehaviour
{
	static SoundPlay instance;
	static AudioSource gameAudioSource;
	static AudioSource bGAudioSource;
	
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
				bGAudioSource.volume = 0.3f;
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
		"Alexandre Desplat - Moving In",
		"Alexandre Desplat - Whack-Bat Majorette Ensemble",
		"Brian Tyler - Now You See Me",
		"Danny Adler - The Cleaner",
		"Deyan Pavlov - La Marche Des Petits Renards",
		"Disney - The Ballad Of Davy Crockett",
		"Hans Zimmer - Mal Mart",
		"Mr. Scruff - Donkey Ride",
		"The Chemical Brothers - The Devil is in the details",
		"Theodore Shapiro - Evil With a Dog Face",
		"Theodore Shapiro - Two Year Montage",
		"Various Artists - Visit Of The Giant House",
		"中山真斗 - TEBASAKI"
	};

	public void Play (string CharacterID, bool IsEnglish, Action CallBack = null)
	{
		StartCoroutine (StartPlay (CharacterID, IsEnglish, CallBack));
	}

	IEnumerator StartPlay (string CharacterID, bool IsEnglish, Action CallBack)
	{
		gameAudioSource.clip = AssetData.GetVoiceByID (CharacterID, IsEnglish);
		gameAudioSource.Play ();
		yield return new WaitForSeconds (gameAudioSource.clip.length);
		if (CallBack != null)
		{
			CallBack ();
		}
	}

	public void PlayLocal (int GameSoundID, bool IsEnglish, Action CallBack = null)
	{
		StartCoroutine (StartPlayLocal (GameSoundID, IsEnglish, CallBack));
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
}
