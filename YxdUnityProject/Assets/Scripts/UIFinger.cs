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
		mUITexture = transform.FindChild ("Finger").GetComponent<UITexture> ();
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
		if (mAnimation != null)
		{
			StopCoroutine (mAnimation);
		}
		mAnimation = StartAnimation ();
		StartCoroutine (mAnimation);
	}

	public void SetPos (Vector3 Pos)
	{
		transform.localPosition = Pos;
	}
	
	public IEnumerator SetPos (Transform t)
	{
		yield return new WaitForSeconds(0.1f);
		Vector3 v3 = t.position;
		UITexture ut = t.GetComponent<UITexture> ();
		v3.x = v3.x + ut.width * 0.5f * t.lossyScale.x;
		v3.y = v3.y + ut.height * 0.5f * t.lossyScale.y;
		transform.position = v3;
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
