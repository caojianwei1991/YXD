using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

public class UIQuestion : MonoBehaviour
{
	UIButton mUIButton;
	UITexture mUITexture;
	UILabel mUILabel;
	MainGameController mgc;
	IEnumerator mAnimation;
	string name;

	public string Name
	{
		get{ return name;}
		set
		{
			name = value;
			mUILabel.text = name;
		}
	}

	bool isEnglish;
	
	public bool IsEnglish
	{
		get{ return isEnglish;}
		set
		{
			isEnglish = value;
		}
	}

	Texture mTexture;
	
	public Texture MTexture
	{
		get{ return mTexture;}
		set
		{
			mTexture = value;
			mUITexture.mainTexture = mTexture;
		}
	}

	string characterID;

	public string CharacterID
	{
		get{ return characterID;}
		set
		{
			characterID = value;
		}
	}

	void Awake ()
	{
		mUIButton = GetComponent<UIButton> ();
		mUITexture = GetComponent<UITexture> ();
		mUILabel = transform.FindChild ("Label").GetComponent<UILabel> ();
		mgc = transform.root.GetComponent<MainGameController> ();
	}

	public void Init ()
	{
		gameObject.SetActive (false);
		Name = "";
		MTexture = null;
		CharacterID = "";
		mUIButton.onClick.Clear ();
		if (mAnimation != null)
		{
			StopCoroutine (mAnimation);
		}
	}

	public void SetCharacterID (string Character_ID)
	{
		gameObject.SetActive (true);
		CharacterID = Character_ID;
		IsEnglish = !mgc.jsonNode ["IsEnglish"].AsBool;
		mUILabel.gameObject.SetActive (mgc.jsonNode ["DisplayText"].AsBool);
		Name = AssetData.GetNameByID (Character_ID, IsEnglish);
		mTexture = AssetData.GetImageByID (Character_ID);
		mAnimation = StartAnimation (AssetData.GetAnimationImageByID (Character_ID));
		StartCoroutine (mAnimation);
		EventDelegate.Set (mUIButton.onClick, delegate
		{
			if (mgc.GameType == GAME_TYPE.ListenPicture)
			{
				SoundPlay.Instance.Play (CharacterID, IsEnglish);
				mgc.JudgeIsMatch (CharacterID);
			}
		});
	}

	IEnumerator StartAnimation (List<Texture> textures)
	{
		while (gameObject.activeInHierarchy)
		{
			for (int i = 0; i < textures.Count; i++)
			{
				mUITexture.mainTexture = textures [i];
				yield return new WaitForSeconds (0.5f);
			}
			//yield return new WaitForSeconds (1);
		}
	}
}
