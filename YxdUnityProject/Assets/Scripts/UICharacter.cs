using UnityEngine;
using System.Collections;
using System;

public class UICharacter : MonoBehaviour
{
	Vector3 defaultPos;
	UIButton mUIButton;
	UITexture mUITexture;
	MainGameController mgc;
	IEnumerator mAnimation;
	Texture[] mTextures = new Texture[2];
	SpringPosition mSpringPosition;
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

	bool isEnglish;
	
	public bool IsEnglish
	{
		get{ return isEnglish;}
		set
		{
			isEnglish = value;
		}
	}

	void Awake ()
	{
		defaultPos = transform.localPosition;
		mUIButton = GetComponent<UIButton> ();
		mUITexture = GetComponent<UITexture> ();
		mgc = transform.root.GetComponent<MainGameController> ();
		for (int i = 0; i < mTextures.Length; i++)
		{
			mTextures [i] = (Texture)Resources.Load ("Texture/Character" + i);
		}
		mSpringPosition = GetComponent<SpringPosition> ();
	}

	public void Init ()
	{
		//gameObject.SetActive (false);
		//transform.localPosition = defaultPos;
		MoveTo (defaultPos, null, true);
		MTexture = null;
		mUIButton.onClick.Clear ();
		if (mAnimation != null)
		{
			StopCoroutine (mAnimation);
		}
		mUIButton.isEnabled = true;
	}

	public void Show (bool IsLiaison, Action CallBack = null)
	{
		gameObject.SetActive (true);
		EventDelegate.Set (mUIButton.onClick, () => SetBtnEvent (IsLiaison, CallBack));
		SetBtnEvent (IsLiaison, CallBack);
	}

	void SetBtnEvent (bool IsLiaison, Action CallBack)
	{
		mUIButton.isEnabled = false;
		if (mAnimation != null)
		{
			StopCoroutine (mAnimation);
		}
		mAnimation = StartAnimation ();
		StartCoroutine (mAnimation);
		IsEnglish = mgc.jsonNode ["IsEnglish"].AsBool;
		SoundPlay.Instance.PlayLocal (mgc.characterSoundID, IsEnglish, () =>
		{
			if (IsLiaison)
			{
				SoundPlay.Instance.Play (mgc.answer.CharacterID, IsEnglish, () =>
				{
					mUIButton.isEnabled = true;
					if (mAnimation != null)
					{
						StopCoroutine (mAnimation);
					}
					if (CallBack != null)
					{
						CallBack ();
					}
				});
			}
			else
			{
				mUIButton.isEnabled = true;
				if (mAnimation != null)
				{
					StopCoroutine (mAnimation);
				}
				if (CallBack != null)
				{
					CallBack ();
				}
			}
		});
	}

	public void PlayResultSound (int CharacterSoundID, bool IsLiaison, Action CallBack = null)
	{
		mUIButton.isEnabled = false;
		if (mAnimation != null)
		{
			StopCoroutine (mAnimation);
		}
		mAnimation = StartAnimation ();
		StartCoroutine (mAnimation);
		SoundPlay.Instance.PlayLocal (CharacterSoundID, IsEnglish, () =>
		{
			if (IsLiaison)
			{
				SoundPlay.Instance.Play (mgc.answer.CharacterID, IsEnglish, () =>
				{
					mUIButton.isEnabled = true;
					if (mAnimation != null)
					{
						StopCoroutine (mAnimation);
					}
					if (CallBack != null)
					{
						CallBack ();
					}
				});
			}
			else
			{
				mUIButton.isEnabled = true;
				if (mAnimation != null)
				{
					StopCoroutine (mAnimation);
				}
				if (CallBack != null)
				{
					CallBack ();
				}
			}
		});
	}

	public void MoveTo (Vector3 Position, Action CallBack = null, bool isLocal = false)
	{
		mUIButton.isEnabled = false;
		if (mAnimation != null)
		{
			StopCoroutine (mAnimation);
		}
		mAnimation = StartAnimation ();
		StartCoroutine (mAnimation);

		mSpringPosition.enabled = true;
		mSpringPosition.strength = 5;
		mSpringPosition.target = isLocal ? Position : transform.InverseTransformVector (Position);
		if (CallBack == null)
		{
			mSpringPosition.onFinished = null;
		}
		else
		{
			mSpringPosition.onFinished = delegate
			{
				CallBack ();
			};
		}
	}

	public void ItweenMoveTo (Vector3 Position)
	{
		iTween.MoveTo (gameObject, iTween.Hash ("position", transform.InverseTransformVector (Position), "islocal", true, "time", 1, "easetype", iTween.EaseType.easeOutBack));
	}
	
	IEnumerator StartAnimation ()
	{
		while (gameObject.activeInHierarchy)
		{
			for (int i = 0; i < mTextures.Length; i++)
			{
				mUITexture.mainTexture = mTextures [i];
				yield return new WaitForSeconds (0.5f);
			}
			//yield return new WaitForSeconds (1);
		}
	}
}
