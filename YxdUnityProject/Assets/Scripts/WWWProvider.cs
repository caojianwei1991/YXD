using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Text;
using System;
using System.Collections.Generic;
using System.Net;

public class WWWProvider : MonoBehaviour
{
	readonly string DownLoadURL = "http://ezlearn.kudospark.com/";
	readonly string GetServerURL = "/app2016/interface.php?schoolid=ezlearn&method=";
	string URL = "";
	public static string RedirectURL = "";
	static WWWProvider instance;

	public static WWWProvider Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = new GameObject ("WWWProvider");
				instance = go.AddComponent<WWWProvider> ();
				DontDestroyOnLoad (go);
			}
			return instance;
		}
	}

	void GetRedirectURL (string MethodName)
	{
		if (RedirectURL == "")
		{
			try
			{
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create (DownLoadURL);
				myHttpWebRequest.Referer = DownLoadURL;
				myHttpWebRequest.AllowAutoRedirect = false;
				using (WebResponse response = myHttpWebRequest.GetResponse())
				{
					RedirectURL = response.Headers ["Location"];
				}
				if(RedirectURL == null || RedirectURL == "")
				{
					RedirectURL = "http://52.221.227.248";
				}
			}
			catch (Exception e)
			{
				RedirectURL = "http://52.221.227.248";
				Debug.LogError (string.Format ("StartWWWCommunication.GetRedirect Fail! Exception:{0}", e.Message));
			}
		}
		if (MethodName == "GetServerURL")
		{
			URL = RedirectURL + GetServerURL;
		}
		else if (URL == "")
		{
			URL = RedirectURL + "/app2016/interface.php?schoolid=kudospark&method=";
		}
	}

	public void StartWWWCommunication (string MethodName, JSONClass JsonClass, Action<bool, string> OnSuccess = null)
	{
		GetRedirectURL (MethodName);
		StartCoroutine (WWWCommunication (MethodName, JsonClass, OnSuccess));
	}

	IEnumerator WWWCommunication (string MethodName, JSONClass JsonClass, Action<bool, string> OnSuccess)
	{
		WWW www = new WWW (URL + MethodName, UTF8Encoding.UTF8.GetBytes (JsonClass.ToString ()));
		yield return www;
		if (www.error != null)
		{
			Alert.Show (www.error);
			Debug.LogError (www.error);
			if (OnSuccess != null)
			{
				//OnSuccess (false, www.error);
			}
		}
		else
		{
			Debug.LogError (www.text);
			if (MethodName == "GetServerURL")
			{
				var jn = JSONNode.Parse (www.text);
				URL = jn ["URL"].Value + "&method=";
			}
			if (OnSuccess != null)
			{
				OnSuccess (true, www.text);
			}
		}
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			Exit ();
		}
	}

	public void Exit ()
	{
		SoundPlay.Instance.PlayLocal (23, LocalStorage.Language == "1");
		Alert.Show ("是否要退出游戏？", () => Application.Quit (), () => {});
	}
}
