using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Text;
using System;
using System.Collections.Generic;
using System.Net;

public class WWWProvider : MonoBehaviour
{
	public static readonly string DownLoadURL = "http://ezlearn.kudospark.com/";
	string URL = "{0}/app2016/interface.php?schoolid=ezlearn&method=";
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

	void GetRedirectURL ()
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
			}
			catch (Exception e)
			{
				RedirectURL = "http://52.221.227.248";
				Debug.LogError (string.Format ("StartWWWCommunication.GetRedirect Fail! Exception:{0}", e.Message));
			}
			URL = string.Format (URL, RedirectURL);
		}
	}

	public void StartWWWCommunication (string MethodName, JSONClass JsonClass, Action<bool, string> OnSuccess)
	{
		GetRedirectURL ();
		StartCoroutine (WWWCommunication (MethodName, JsonClass, OnSuccess));
	}

	IEnumerator WWWCommunication (string MethodName, JSONClass JsonClass, Action<bool, string> OnSuccess)
	{
		WWW www = new WWW (URL + MethodName, UTF8Encoding.UTF8.GetBytes (JsonClass.ToString ()));
		yield return www;
		if (www.error != null)
		{
			Debug.LogError (www.error);
			OnSuccess (false, www.error);
		}
		else
		{
			Debug.LogError (www.text);
			if (MethodName == "GetServerURL")
			{
				var jn = JSONNode.Parse (www.text);
				URL = jn ["URL"].Value + "&method=";
			}
			OnSuccess (true, www.text);
		}
	}
}
