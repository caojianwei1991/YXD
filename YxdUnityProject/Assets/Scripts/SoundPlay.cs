using UnityEngine;
using System.Collections;

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

	public void Play (string CharacterID, bool IsEnglish)
	{
		if (AssetData.AssetDataDic != null && AssetData.AssetDataDic.ContainsKey (CharacterID))
		{
			audioSource.clip = IsEnglish ? AssetData.AssetDataDic [CharacterID].EnglishVoice : AssetData.AssetDataDic [CharacterID].ChineseVoice;
			audioSource.Play ();
		}
		else
		{
			Debug.LogError (string.Format ("AssetData.AssetDataDic is null or not contains key! CharacterID:{0}", CharacterID));
		}

	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
