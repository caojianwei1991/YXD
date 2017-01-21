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
	readonly string NoNetWorkURL = "http://123.207.9.191";
	readonly string GetServerURL = "/app2016/interface.php?schoolid=ezlearn&method=";
	string URL = "http://123.207.9.191:8080/ezlearn-appservice";
	string RedirectURL = "";
	public static string DownLoadAssetURL = "http://123.207.9.191:8080/ezlearn-appservice";
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
			RedirectURL = NoNetWorkURL;
//			try
//			{
//				HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create (DownLoadURL);
//				myHttpWebRequest.Referer = DownLoadURL;
//				myHttpWebRequest.AllowAutoRedirect = false;
//				using (WebResponse response = myHttpWebRequest.GetResponse())
//				{
//					RedirectURL = response.Headers ["Location"];
//				}
//				if(RedirectURL == null || RedirectURL == "")
//				{
//					RedirectURL = NoNetWorkURL;
//				}
//			}
//			catch (Exception e)
//			{
//				RedirectURL = NoNetWorkURL;
//				Debug.LogError (string.Format ("StartWWWCommunication.GetRedirect Fail! Exception:{0}", e.Message));
//			}
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

	public void StartWWWCommunication (string MethodName, JSONClass wf, Action<bool, string> OnSuccess = null)
	{

	}

	public void StartWWWCommunication (string MethodName, WWWForm wf, Action<bool, string> OnSuccess = null)
	{
		//GetRedirectURL (MethodName);
		StartCoroutine (WWWCommunication (MethodName, wf, OnSuccess));
	}

	IEnumerator WWWCommunication (string MethodName, WWWForm wf, Action<bool, string> OnSuccess)
	{
		Debug.LogError (URL + MethodName);
		WWW www = new WWW (URL + MethodName, wf);
		yield return www;
		if (www.error != null)
		{
			//Alert.Show (www.error);
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
				string str = jn ["URL"].Value;
				URL = str + "&method=";
				str = str.Replace ("http://", "");
				string[] strs = str.Split ('/');
				DownLoadAssetURL = "http://" + strs [0];
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

	public static string GetMD5 (string s)
	{
		System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider ();            
		byte[] bytes = System.Text.Encoding.UTF8.GetBytes (s);
		bytes = md5.ComputeHash (bytes);
		md5.Clear ();
		string ret = "";
		for (int i = 0; i < bytes.Length; i++)
		{
			ret += Convert.ToString (bytes [i], 16).PadLeft (2, '0');
		}
		return ret.PadLeft (32, '0');
	}
}
