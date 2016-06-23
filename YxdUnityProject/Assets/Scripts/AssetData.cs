using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public enum ASSET_TYPE : int
{
	ChineseVoice = 0,
	EnglishVoice = 1,
	Image = 2,
	AnimationImages = 3,
	Name = 4
}


public class AssetData
{
	public AudioClip ChineseVoice{ get; private set; }

	public AudioClip EnglishVoice{ get; private set; }

	public string ChineseName{ get; private set; }
	
	public string EnglishName{ get; private set; }

	public Texture Image{ get; private set; }

	public List<Texture> AnimationImages{ get; private set; }

	public static Dictionary<string, AssetData> AssetDataDic{ get; private set; }

	public string SceneID{ get; private set; }

	public AssetData ()
	{
		ChineseVoice = null;
		EnglishVoice = null;
		ChineseName = null;
		EnglishName = null;
		Image = null;
		SceneID = null;
		AnimationImages = new List<Texture> ();
	}

	public static void Add (string ID, ASSET_TYPE AssetType, WWW www = null, JSONNode JsonNode = null)
	{
		if (AssetDataDic == null)
		{
			AssetDataDic = new Dictionary<string, AssetData> ();
		}

		bool isContainsKey = AssetDataDic.ContainsKey (ID);
		AssetData ad = isContainsKey ? AssetDataDic [ID] : new AssetData ();

		switch (AssetType)
		{
			case ASSET_TYPE.ChineseVoice:
				ad.ChineseVoice = www.audioClip;
				break;
			case ASSET_TYPE.EnglishVoice:
				ad.EnglishVoice = www.audioClip;
				break;
			case ASSET_TYPE.Image:
				ad.Image = www.texture;
				break;
			case ASSET_TYPE.AnimationImages:
				ad.AnimationImages.Add (www.texture);
				break;
			case ASSET_TYPE.Name:
				ad.ChineseName = JsonNode ["ChineseName"].Value;
				ad.EnglishName = JsonNode ["EnglishName"].Value;
				ad.SceneID = JsonNode ["SceneID"].Value;
				break;
			default:
				break;
		}
		if (!isContainsKey)
		{
			AssetDataDic.Add (ID, ad);
		}
	}

	public static string GetNameByID (string CharacterID, bool IsEnglish)
	{
		if (AssetDataDic != null && AssetDataDic.ContainsKey (CharacterID))
		{
			return IsEnglish ? AssetDataDic [CharacterID].EnglishName : AssetDataDic [CharacterID].ChineseName;
		}
		else
		{
			Debug.LogError (string.Format ("GetNameByID AssetDataDic is null or not key! CharacterID:{0}", CharacterID));
			return null;
		}
	}

	public static AudioClip GetVoiceByID (string CharacterID, bool IsEnglish)
	{
		if (AssetDataDic != null && AssetDataDic.ContainsKey (CharacterID))
		{
			return IsEnglish ? AssetDataDic [CharacterID].EnglishVoice : AssetDataDic [CharacterID].ChineseVoice;
		}
		else
		{
			Debug.LogError (string.Format ("GetVoiceByID AssetDataDic is null or not key! CharacterID:{0}", CharacterID));
			return null;
		}
	}

	public static Texture GetImageByID (string CharacterID)
	{
		if (AssetDataDic != null && AssetDataDic.ContainsKey (CharacterID))
		{
			return AssetDataDic [CharacterID].Image;
		}
		else
		{
			Debug.LogError (string.Format ("GetImageByID AssetDataDic is null or not key! CharacterID:{0}", CharacterID));
			return null;
		}
	}

	public static List<Texture> GetAnimationImageByID (string CharacterID)
	{
		if (AssetDataDic != null && AssetDataDic.ContainsKey (CharacterID))
		{
			return AssetDataDic [CharacterID].AnimationImages;
		}
		else
		{
			Debug.LogError (string.Format ("GetAnimationImageByID AssetDataDic is null or not key! CharacterID:{0}", CharacterID));
			return new List<Texture> ();
		}
	}
}
