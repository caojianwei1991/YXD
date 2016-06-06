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

	public AssetData ()
	{
		ChineseVoice = null;
		EnglishVoice = null;
		ChineseName = null;
		EnglishName = null;
		Image = null;
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
				break;
			default:
				break;
		}
		if (!isContainsKey)
		{
			AssetDataDic.Add (ID, ad);
		}
	}
}
