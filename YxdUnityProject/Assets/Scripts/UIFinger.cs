using UnityEngine;
using System.Collections;

public class UIFinger : MonoBehaviour
{
	Vector3 defaultPos;
	UITexture mUITexture;
	MainGameController mgc;
	IEnumerator mAnimation;
	Texture[] mTextures = new Texture[2];
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
		mUITexture = GetComponent<UITexture> ();
		mgc = transform.root.GetComponent<MainGameController> ();
		for (int i = 0; i < mTextures.Length; i++)
		{
			mTextures [i] = (Texture)Resources.Load ("Texture/Finger" + i);
		}
	}

	public void Init ()
	{
		gameObject.SetActive (false);
		transform.localPosition = defaultPos;
		MTexture = null;
		if (mAnimation != null)
		{
			StopCoroutine (mAnimation);
		}
	}

	public void Show ()
	{
		gameObject.SetActive (true);
		mAnimation = StartAnimation ();
		StartCoroutine (mAnimation);
	}

	public void SetPos (Vector3 Pos)
	{
		transform.localPosition = Pos;
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
