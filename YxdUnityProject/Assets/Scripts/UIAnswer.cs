using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

public class UIAnswer : MonoBehaviour
{
	Vector3 defaultPos;
	UIDragObject mUIDragObject;
	UIButton mUIButton;
	UILabel mUILabel;
	MainGameController mgc;
	string name;

	public string Name
	{
		get{ return name;}
		set
		{
			name = value;
			if (mUILabel != null)
			{
				mUILabel.text = name;
			}
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
		defaultPos = transform.localPosition;
		mUIDragObject = GetComponent<UIDragObject> ();
		mUIButton = GetComponent<UIButton> ();
		mUILabel = transform.FindChild ("Label").GetComponent<UILabel> ();
		mgc = transform.root.GetComponent<MainGameController> ();
	}

	public void Init ()
	{
		gameObject.SetActive (false);
		Name = "";
		CharacterID = "";
		mUIButton.onClick.Clear ();
		mUIDragObject.enabled = false;
		transform.localPosition = defaultPos;
	}

	public void SetDrag()
	{
		mUIDragObject.enabled = true;
	}

	public void SetCharacterID (string Character_ID)
	{
		gameObject.SetActive (true);
		CharacterID = Character_ID;
		IsEnglish = mgc.jsonNode ["IsEnglish"].AsBool;
		Name = AssetData.GetNameByID (CharacterID, IsEnglish);
		EventDelegate.Set (mUIButton.onClick, delegate
		{
			SoundPlay.Instance.Play (CharacterID, IsEnglish);
			if (mgc.GameType == GAME_TYPE.ReadPicture)
			{
				mgc.JudgeIsMatch (CharacterID);
			}
		});
	}
}
