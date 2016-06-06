﻿using System;
using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class Download : MonoBehaviour
{
	string[] cachePath = new string[3];
	UISlider uiSlider;
	readonly string timeFormat = "yyyy-MM-dd HH:mm:ss";
	readonly string minUpdateTime = "1970-01-01 00:00:00";
	readonly string description = "正在下载资源：{0}...";
	UILabel descriptionLabel;
	float uiSliderValue, assetNum;
	JSONNode jsonNode = new JSONNode ();
	DateTime lastUpdateTime;
	bool isUpdateNeeded, isDownLoadFail;

	void Awake ()
	{
		cachePath [0] = DirectoryTool.persistentDataPath + "/AudioCache/";
		cachePath [1] = DirectoryTool.persistentDataPath + "/ImageCache/";
		cachePath [2] = DirectoryTool.persistentDataPath + "/AnimationFolderCache/";
		uiSlider = transform.FindChild ("Loading").GetComponent<UISlider> ();
		descriptionLabel = transform.FindChild ("Loading/Label").GetComponent<UILabel> ();
		lastUpdateTime = DateTime.ParseExact (PlayerPrefs.GetString ("LastUpdateTime", minUpdateTime), timeFormat, null);
	}

	void Start ()
	{
		var jc = new JSONClass ();
		jc.Add ("UpdateTime", lastUpdateTime.ToString (timeFormat));
		jc.Add ("SchoolID", "kudospark");
		WWWProvider.Instance.StartWWWCommunication ("UpdateCharactersInfo", jc, DownLoadCharactersInfo);
	}

	void DownLoadCharactersInfo (bool IsSuccess, string JsonData)
	{
		var jn = JSONNode.Parse (JsonData);
		isUpdateNeeded = jn ["UpdateNeeded"].Value == "1";
		var jc = new JSONClass ();
		jc.Add ("UpdateTime", minUpdateTime);
		WWWProvider.Instance.StartWWWCommunication ("DownLoadCharactersInfo", jc, (x , y) =>
		{
			jsonNode = JSONNode.Parse (y);
			StartCoroutine (DealDownLoadData ());
		});
	}

	IEnumerator DealDownLoadData ()
	{
		Dictionary<string, ASSET_TYPE> assetNameAndTypeDic = new Dictionary<string, ASSET_TYPE> ();
		StringBuilder localPath = new StringBuilder ();
		StringBuilder urlPath = new StringBuilder ();
		StringBuilder animationFolderPath = new StringBuilder ();
		isDownLoadFail = false;

		//计算资源总数
		for (int i = 0; i < jsonNode.Count; i++)
		{
			if (jsonNode [i] ["ChineseVoice"].Value != "")
			{
				assetNum++;
			}
			if (jsonNode [i] ["EnglishVoice"].Value != "")
			{
				assetNum++;
			}
			if (jsonNode [i] ["ImageURL"].Value != "")
			{
				assetNum++;
			}
			for (int n = 0; jsonNode [i] ["AnimationNumber"].Value != "" && n < jsonNode [i] ["AnimationNumber"].AsInt; n++)
			{
				assetNum++;
			}
			//比较最晚更新时间
			if (isUpdateNeeded)
			{
				if (i == 0)
				{
					lastUpdateTime = DateTime.ParseExact (jsonNode [i] ["UpdateTime"].Value, timeFormat, null);
				}
				else if (lastUpdateTime.CompareTo (DateTime.ParseExact (jsonNode [i] ["UpdateTime"].Value, timeFormat, null)) < 0)
				{
					lastUpdateTime = DateTime.ParseExact (jsonNode [i] ["UpdateTime"].Value, timeFormat, null);
				}
			}
		}

		//开始下载.....
		for (int i = 0; i < jsonNode.Count; i++)
		{
			string id = jsonNode [i] ["ID"].Value;
			var jn = jsonNode [i];
			AssetData.Add (id, ASSET_TYPE.Name, null, jn);
			for (int j = 0; j < cachePath.Length; j++)
			{
				assetNameAndTypeDic.Clear ();

				localPath.Remove (0, localPath.Length);
				localPath.Append (cachePath [j]);
				localPath.Append (id);
				localPath.Append ("/");
				try
				{
					if (!Directory.Exists (localPath.ToString ()))
					{
						Directory.CreateDirectory (localPath.ToString ());
					}
				}
				catch (Exception e)
				{
					Debug.LogError (string.Format ("Directory.CreateDirectory Fail! localPath={0},Exception={1}", localPath, e.Message)); 
				}
				if (j == 0)
				{
					//assetNameAndTypeDic.Add (jn ["ChineseVoice"].Value, ASSET_TYPE.ChineseVoice);
					//assetNameAndTypeDic.Add (jn ["EnglishVoice"].Value, ASSET_TYPE.EnglishVoice);
				}
				else if (j == 1)
				{
					assetNameAndTypeDic.Add (jn ["ImageURL"].Value, ASSET_TYPE.Image);
				}
				else if (j == 2)
				{
					for (int n = 1; n <= jn ["AnimationNumber"].AsInt; n++)
					{
						animationFolderPath.Remove (0, animationFolderPath.Length);
						animationFolderPath.Append (jn ["AnimationFolder"].Value);
						animationFolderPath.Append (n);
						animationFolderPath.Append (".png");
						assetNameAndTypeDic.Add (animationFolderPath.ToString (), ASSET_TYPE.AnimationImages);
					}
				}

				foreach (var item in assetNameAndTypeDic)
				{
					urlPath.Remove (0, urlPath.Length);
					urlPath.Append (WWWProvider.RedirectURL);
					urlPath.Append ("/");
					urlPath.Append (item.Key);
					localPath.Append (urlPath.ToString ().GetHashCode ());

					yield return StartCoroutine (DownLoadAsset (id, item, localPath.ToString (), urlPath.ToString ()));
				}
			}
		}

		if (isDownLoadFail)
		{
			descriptionLabel.text = "下载失败，请检查网络后重试！";
		}
		else
		{
			if (isUpdateNeeded)
			{
				PlayerPrefs.SetString ("LastUpdateTime", lastUpdateTime.ToString (timeFormat));
			}
			uiSlider.value = 1;
			descriptionLabel.text = "下载完成！";
			yield return new WaitForSeconds (1);
			Application.LoadLevel ("SelectScene");
		}
	}

	IEnumerator DownLoadAsset (string ID, KeyValuePair<string, ASSET_TYPE> ItemAssetNameAndType, string LocalPath, string UrlPath)
	{
		if (isDownLoadFail)
		{
			yield break;
		}

		bool isExists = File.Exists (LocalPath) && !isUpdateNeeded;
		string path = isExists ? @"file:///" + LocalPath : UrlPath;
		WWW www = new WWW (path);
		descriptionLabel.text = string.Format (description, ItemAssetNameAndType.Key);
		yield return www;
		if (www.error != null)
		{
			Debug.LogError (string.Format ("DownLoadAsset Fail! path={0}, error={1}", path, www.error));
			//isDownLoadFail = true;
			yield break;
		}
		if (!isExists)
		{
			// 防止空图片造成额外BUG风险
			if (www.bytes.Length >= 8)
			{
				try
				{
					// 保存图片
					File.WriteAllBytes (LocalPath, www.bytes);
				}
				catch (Exception e)
				{
					Debug.LogError ("File.WriteAllBytes Fail ===>" + e.Message);
				}
			}
		}
		AssetData.Add (ID, ItemAssetNameAndType.Value, www);

		uiSliderValue += 1.0f / assetNum;
		uiSlider.value = Mathf.Clamp (uiSliderValue, 0.1f, uiSliderValue);
	}
}
