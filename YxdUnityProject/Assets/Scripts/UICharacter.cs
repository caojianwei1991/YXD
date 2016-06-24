using UnityEngine;
using System.Collections;

public class UICharacter : MonoBehaviour
{
	Vector3 defaultPos;
	UIButton mUIButton;
	UITexture mUITexture;
	MainGameController mgc;
	IEnumerator mAnimation;
	Texture[] mTextures = new Texture[2];

	public bool IsLiaison{ get; private set; }

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
	}

	public void Init ()
	{
		gameObject.SetActive (false);
		transform.localPosition = defaultPos;
		MTexture = null;
		mUIButton.onClick.Clear ();
		if (mAnimation != null)
		{
			StopCoroutine (mAnimation);
		}
	}

	public void Show (bool Is_Liaison)
	{
		IsLiaison = Is_Liaison;
		gameObject.SetActive (true);
		EventDelegate.Set (mUIButton.onClick, () => SetBtnEvent ());
		SetBtnEvent ();
	}

	void SetBtnEvent ()
	{
		mUIButton.isEnabled = false;
		mAnimation = StartAnimation ();
		StartCoroutine (mAnimation);
		bool IsEnglish = mgc.jsonNode ["IsEnglish"].AsBool;
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
				});
			}
			else
			{
				mUIButton.isEnabled = true;
				if (mAnimation != null)
				{
					StopCoroutine (mAnimation);
				}
			}
		});
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
