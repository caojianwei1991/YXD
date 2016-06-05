using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class HWR : MonoBehaviour
{
	string[] recognitionLanguage = {"hwr.cloud.freewrite","hwr.cloud.freewrite.english"};
	int screenH;

	void Awake ()
	{
		screenH = Screen.height;
	}

	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	int capKeyIndex;

	public void StartHWR ()
	{
		Debug.Log ("traceList.Count=" + traceList.Count);
		ListToString ();
		Debug.Log ("traceDataStr=" + traceDataStr.ToString ());
		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
		jo.Call ("StartHWR", traceDataStr.ToString (), recognitionLanguage [capKeyIndex]);
		if (capKeyIndex == 0)
		{
			capKeyIndex = 1;
		}
		else
		{
			capKeyIndex = 0;
		}
	}

	void ReceiveHWR (string str)
	{
		transform.FindChild ("Label").GetComponent<UILabel> ().text = str;
	}

	List<Vector2> traceList = new List<Vector2> ();
	List<Vector3> lineList = new List<Vector3> ();
	List<LineRenderer> lineRendererList = new List<LineRenderer> ();
	StringBuilder traceDataStr = new StringBuilder ();

	void OnPress (bool pressed)
	{
		if (pressed)
		{
			lineRendererList.Add (NGUITools.AddChild (transform.parent.gameObject, (GameObject)Resources.Load ("line")).GetComponent<LineRenderer> ());
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
		if ((traceList [traceList.Count - 1] - InputMouseV2).sqrMagnitude > 4)
		{
			traceList.Add (InputMouseV2);
			lineList.Add (Camera.main.ScreenToWorldPoint (Input.mousePosition));
			int lineListNum = lineList.Count;
			int lineRendererListNum = lineRendererList.Count;
			lineRendererList [lineRendererListNum - 1].SetVertexCount (lineListNum);
			for (int i = 0; i < lineListNum; i++)
			{
				lineRendererList [lineRendererListNum - 1].SetPosition (i, new Vector3 (lineList [i].x, lineList [i].y, 5));
			}
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
