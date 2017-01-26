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
	UITexture animUITexture;
	IEnumerator mAnimation;
	Texture[] characterTexture = new Texture[2];

	public Transform mTransform{ set; private get; }

	/// <summary>
	/// The local position.
	/// </summary>
	Vector3 localPosition;
	
	public Vector3 LocalPosition
	{
		get{ return localPosition;}
		set
		{
			localPosition = value;
			if (mTransform != null)
			{
				mTransform.localPosition = localPosition;
			}
		}
	}

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
		mTransform = transform;
		defaultPos = mTransform.localPosition;
		mUIDragObject = GetComponent<UIDragObject> ();
		mUIButton = GetComponent<UIButton> ();
		mUILabel = mTransform.FindChild ("Label").GetComponent<UILabel> ();
		animUITexture = mTransform.FindChild ("Texture").GetComponent<UITexture> ();
		mgc = mTransform.root.GetComponent<MainGameController> ();
		for (int i = 0; i < characterTexture.Length; i++)
		{
			characterTexture [i] = (Texture)Resources.Load ("Texture/Character" + i);
		}
	}

	public void Init ()
	{
		gameObject.SetActive (false);
		Name = "";
		CharacterID = "";
		mUIButton.onClick.Clear ();
		mUIDragObject.enabled = false;
		LocalPosition = defaultPos;
		animUITexture.mainTexture = null;
		if (mAnimation != null)
		{
			StopCoroutine (mAnimation);
		}
		mUIButton.isEnabled = true;
	}

	public void SetButtonIsEnabled (bool IsEnabled)
	{
		mUIButton.isEnabled = IsEnabled;
	}

	public void SetDrag ()
	{
		mUIDragObject.enabled = true;
	}

	void OnPress (bool pressed)
	{
		if (mgc.GameType == GAME_TYPE.LinkPicture)
		{
			if (!pressed)
			{
				Ray ray = UICamera.currentCamera.ScreenPointToRay (UICamera.currentTouch.pos);
				RaycastHit[] hits = Physics.RaycastAll (ray, 20);
				bool isMatch = false;
				for (int i = 0; hits != null && i < hits.Length; i++)
				{
					UIQuestion uq = hits [i].collider.GetComponent<UIQuestion> ();
					if (uq != null)
					{
						if (uq.CharacterID == CharacterID)
						{
							isMatch = true;
							uq.ActiveNameUI ();
							gameObject.SetActive (false);
						}
						else
						{
							SoundPlay.Instance.PlayLocal (Random.Range (21, 23), IsEnglish);
						}
						mgc.JudgeIsMatch ();
						break;
					}
				}
				if (!isMatch)
				{
					iTween.MoveTo (gameObject, iTween.Hash ("position", defaultPos, "islocal", true, "time", 1, "easetype", iTween.EaseType.easeOutBack));
				}
			}
			else
			{
				mgc.uiFinger.Init ();
				iTween.Stop (gameObject);
			}
		}
	}

	public void SetCharacterID (string Character_ID)
	{
		gameObject.SetActive (true);
		CharacterID = Character_ID;
		IsEnglish = mgc.GetCurrentQuestion.isEnglish;
		Name = AssetData.GetNameByID (CharacterID, IsEnglish);
		EventDelegate.Set (mUIButton.onClick, delegate
		{
			SoundPlay.Instance.Play (CharacterID, IsEnglish);
			mgc.uiFinger.Init ();
			if (mgc.GameType == GAME_TYPE.ReadPicture)
			{
				mgc.SetAllAnswersBtnIsEnable (false);
				mgc.uiCharacter.MoveTo (animUITexture.transform.position, () =>
				{
					mgc.uiCharacter.ItweenMoveTo (mgc.answer.mTransform.position + animUITexture.transform.localPosition * mgc.answer.mTransform.lossyScale.x);
					iTween.MoveTo (gameObject, iTween.Hash ("position", mgc.answer.mTransform.localPosition, "islocal", true, "time", 1, "easetype", iTween.EaseType.easeOutBack, "oncomplete", "OnComplete"));
				});
			}
		});
	}

	void OnComplete ()
	{
		if (!mgc.JudgeIsMatch (CharacterID))
		{
			mgc.uiCharacter.Init ();
			iTween.MoveTo (gameObject, iTween.Hash ("position", defaultPos, "islocal", true, "time", 1, "easetype", iTween.EaseType.easeOutBack, "oncomplete", "OnComplete1"));
		}
	}

	void OnComplete1 ()
	{
		mgc.SetAllAnswersBtnIsEnable (true);
	}

	IEnumerator StartAnimation (Texture[] textures)
	{
		while (gameObject.activeInHierarchy)
		{
			for (int i = 0; i < textures.Length; i++)
			{
				animUITexture.mainTexture = textures [i];
				yield return new WaitForSeconds (0.5f);
			}
			//yield return new WaitForSeconds (1);
		}
	}
}
