using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class HWR : MonoBehaviour
{
	int screenH;
	Texture[] characterTexture = new Texture[2];
	UITexture character;
	UIButton OK;
	Bounds bounds;
	MainGameController mgc;

	void Awake ()
	{
		mgc = transform.root.GetComponent<MainGameController> ();
		screenH = Screen.height;
		OK = transform.FindChild ("OK").GetComponent<UIButton> ();
		OK.onClick.Add (new EventDelegate (() => StartHWR ()));
		OK.isEnabled = false;
		transform.FindChild ("Clear").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => ClearLine ()));

		character = transform.FindChild ("Character").GetComponent<UITexture> ();
		for (int i = 0; i < characterTexture.Length; i++)
		{
			characterTexture [i] = (Texture)Resources.Load ("Texture/Character" + i);
		}

		bounds = gameObject.collider.bounds;
	}

	void OnEnable ()
	{
		StartCoroutine (StartCharacterAnim ());
	}

	IEnumerator StartCharacterAnim ()
	{
		while (character.gameObject.activeInHierarchy)
		{
			for (int i = 0; i < characterTexture.Length; i++)
			{
				character.mainTexture = characterTexture [i];
				yield return new WaitForSeconds (0.5f);
			}
			//yield return new WaitForSeconds (1);
		}
	}

	public void StartHWR ()
	{
		Debug.Log ("traceList.Count=" + traceList.Count);
		ListToString ();
		Debug.Log ("traceDataStr=" + traceDataStr.ToString ());
		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
		string language = mgc.answer.IsEnglish ? "hwr.cloud.freewrite.english" : "hwr.cloud.freewrite";
		jo.Call ("StartHWR", traceDataStr.ToString (), language);
		gameObject.SetActive (false);
		OK.isEnabled = false;
	}

	void ClearLine ()
	{
		lineList.Clear ();
		traceDataStr.Remove (0, traceDataStr.Length);
		traceList.Clear ();
		for (int i = 0; i < lineRendererList.Count; i++)
		{
			Destroy (lineRendererList [i].gameObject);
		}
		lineRendererList.Clear ();
		OK.isEnabled = false;
	}

	List<Vector2> traceList = new List<Vector2> ();
	List<Vector3> lineList = new List<Vector3> ();
	List<LineRenderer> lineRendererList = new List<LineRenderer> ();
	StringBuilder traceDataStr = new StringBuilder ();

	void OnPress (bool pressed)
	{
		if (pressed)
		{
			lineRendererList.Add (NGUITools.AddChild (transform.parent.gameObject, (GameObject)Resources.Load ("Prefabs/Line")).GetComponent<LineRenderer> ());
			traceList.Add (InputMouseVector2);
			lineList.Add (Camera.main.ScreenToWorldPoint (Input.mousePosition));
		}
		else
		{
			lineList.Clear ();
			traceList.Add (new Vector2 (-1, 0));
		}
	}

	void OnDrag (Vector2 delta)
	{
		Vector2 InputMouseV2 = InputMouseVector2;
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Ray ray = new Ray (worldPoint, transform.forward);
		if ((traceList [traceList.Count - 1] - InputMouseV2).sqrMagnitude > 4 && bounds.IntersectRay (ray))
		{
			traceList.Add (InputMouseV2);
			lineList.Add (worldPoint);
			int lineListNum = lineList.Count;
			int lineRendererListNum = lineRendererList.Count;
			lineRendererList [lineRendererListNum - 1].SetVertexCount (lineListNum);
			for (int i = 0; i < lineListNum; i++)
			{
				lineRendererList [lineRendererListNum - 1].SetPosition (i, new Vector3 (lineList [i].x, lineList [i].y, 5));
			}
			OK.isEnabled = true;
		}
	}

	void ListToString ()
	{
		if (traceList.Count > 0)
		{
			traceList.Add (new Vector2 (-1, -1));
			traceDataStr.Remove (0, traceDataStr.Length);
			for (int i = 0; i < traceList.Count; i++)
			{
				traceDataStr.Append (traceList [i].x);
				traceDataStr.Append (",");
				traceDataStr.Append (traceList [i].y);
				if (i != traceList.Count - 1)
				{
					traceDataStr.Append (",");
				}
			}
			traceList.Clear ();
			for (int i = 0; i < lineRendererList.Count; i++)
			{
				Destroy (lineRendererList [i].gameObject);
			}
			lineRendererList.Clear ();
		}
	}

	Vector2 InputMouseVector2
	{
		get
		{
			Vector3 v3 = Input.mousePosition;
			return new Vector2 ((int)v3.x, (int)(screenH - v3.y));
		}
	}
}
