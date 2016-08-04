using System;
using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class Download : MonoBehaviour
{
	string[] sdCachePath = new string[3];
	string[] streamingAssetsPath = new string[3];
	UISlider uiSlider;
	readonly string timeFormat = "yyyy-MM-dd HH:mm:ss";
	//readonly string minUpdateTime = "1970-01-01 00:00:00";
	string description = "正在下载资源：{0}...";
	string sdAssetList, streamingAssetsList;
	UILabel descriptionLabel;
	float uiSliderValue, assetNum;
	JSONNode jsonNode = new JSONNode ();
	JSONNode lastJsonNode = new JSONNode ();
	JSONNode NewJsonNode = new JSONNode ();
	bool isUpdateNeeded, isDownLoadFail;

	void Awake ()
	{
		streamingAssetsList = Application.streamingAssetsPath + "/assetList";
		sdAssetList = DirectoryTool.persistentDataPath + "/assetList";
		sdCachePath [0] = DirectoryTool.persistentDataPath + "/AudioCache/";
		sdCachePath [1] = DirectoryTool.persistentDataPath + "/ImageCache/";
		sdCachePath [2] = DirectoryTool.persistentDataPath + "/AnimationFolderCache/";
		streamingAssetsPath [0] = Application.streamingAssetsPath + "/AudioCache/";
		streamingAssetsPath [1] = Application.streamingAssetsPath + "/ImageCache/";
		streamingAssetsPath [2] = Application.streamingAssetsPath + "/AnimationFolderCache/";
		uiSlider = transform.FindChild ("Loading").GetComponent<UISlider> ();
		descriptionLabel = transform.FindChild ("Loading/Label").GetComponent<UILabel> ();
	}

	void Start ()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		try
		{
			lastJsonNode = jsonNode = JSONNode.LoadFromCompressedFile (sdAssetList);
			UpdateCharactersInfo ();
		}
		catch (Exception e)
		{
			Debug.LogError (string.Format ("JSONNode.LoadFromCompressedFile Fail! assetList={0},Exception={1}", sdAssetList, e.Message));
			StartCoroutine (ReadLocalAssetList ());
		}
	}

	IEnumerator ReadLocalAssetList ()
	{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		WWW www = new WWW (@"file:///" + streamingAssetsList);
#else
		WWW www = new WWW (streamingAssetsList);
#endif
		yield return www;
		lastJsonNode = jsonNode = JSONNode.LoadFromCompressedStream (new MemoryStream (www.bytes));
		UpdateCharactersInfo ();
	}

	void UpdateCharactersInfo ()
	{
		isUpdateNeeded = false;
		if ((LocalStorage.IsRandomPlay && LocalStorage.StudentID == "") || Application.internetReachability == NetworkReachability.NotReachable)
		{
			StartCoroutine (DealDownLoadData ());
		}
		else
		{
			var jc = new JSONClass ();
			jc.Add ("UpdateTime", jsonNode ["LastUpdateTime"].Value);
			jc.Add ("SchoolID", LocalStorage.SchoolID);
			WWWProvider.Instance.StartWWWCommunication ("UpdateCharactersInfo", jc, DownLoadCharactersInfo);
		}
	}

	void DownLoadCharactersInfo (bool IsSuccess, string JsonData)
	{
		var jn = JSONNode.Parse (JsonData);
		isUpdateNeeded = jn ["UpdateNeeded"].Value == "1";
		if (isUpdateNeeded)
		{
			var jc = new JSONClass ();
			jc.Add ("UpdateTime", jsonNode ["LastUpdateTime"].Value);
			WWWProvider.Instance.StartWWWCommunication ("DownLoadCharactersInfo", jc, (x , y) =>
			{
				try
				{
					jsonNode = JSONNode.Parse (y);
					float _assetNum = 0;
					DateTime lastUpdateTime = new DateTime ();
					//计算资源总数
					for (int i = 0; i < jsonNode.Count; i++)
					{
						if (jsonNode [i] ["ChineseVoice"].Value != "")
						{
							_assetNum++;
						}
						if (jsonNode [i] ["EnglishVoice"].Value != "")
						{
							_assetNum++;
						}
						if (jsonNode [i] ["ImageURL"].Value != "")
						{
							_assetNum++;
						}
						for (int n = 0; jsonNode [i] ["AnimationNumber"].Value != "" && n < jsonNode [i] ["AnimationNumber"].AsInt; n++)
						{
							_assetNum++;
						}
						//比较最晚更新时间
						if (i == 0)
						{
							lastUpdateTime = DateTime.ParseExact (jsonNode [i] ["UpdateTime"].Value, timeFormat, null);
						}
						else if (lastUpdateTime.CompareTo (DateTime.ParseExact (jsonNode [i] ["UpdateTime"].Value, timeFormat, null)) < 0)
						{
							lastUpdateTime = DateTime.ParseExact (jsonNode [i] ["UpdateTime"].Value, timeFormat, null);
						}
					}
					var jsonClass = new JSONClass ();
					jsonClass.Add ("CurrentDownloadURL", WWWProvider.RedirectURL);
					jsonClass.Add ("AssetNum", _assetNum.ToString ());
					jsonClass.Add ("LastUpdateTime", lastUpdateTime.ToString (timeFormat));
					jsonClass.Add ("ArrayData", "ZhangYi");
					string str = jsonClass.ToString ();
					str = str.Replace ("\"ZhangYi\"", y);
					NewJsonNode = jsonNode = JSONNode.Parse (str);
					//jsonNode.SaveToCompressedFile (sdAssetList);
					StartCoroutine (DealDownLoadData ());
				}
				catch (Exception e)
				{
					Debug.LogError (string.Format ("File.WriteAllBytes Fail! assetList={0},Exception={1}", sdAssetList, e.Message)); 
				}
			});
		}
		else
		{
			StartCoroutine (DealDownLoadData ());
		}
	}

	IEnumerator DealDownLoadData ()
	{
		Dictionary<string, ASSET_TYPE> assetNameAndTypeDic = new Dictionary<string, ASSET_TYPE> ();
		StringBuilder sdPath = new StringBuilder ();
		StringBuilder urlPath = new StringBuilder ();
		StringBuilder animationFolderPath = new StringBuilder ();
		StringBuilder sdFile = new StringBuilder ();
		isDownLoadFail = false;
		string currentDownloadURL = jsonNode ["CurrentDownloadURL"].Value;
		assetNum = jsonNode ["AssetNum"].AsFloat;
		jsonNode = jsonNode ["ArrayData"];
		if (isUpdateNeeded)
		{
			lastJsonNode = lastJsonNode ["ArrayData"];
		}

		//开始下载.....
		for (int i = 0; i < jsonNode.Count; i++)
		{
			string id = jsonNode [i] ["ID"].Value;
			var jn = jsonNode [i];
			var lastJn = lastJsonNode [i];
			bool isUpdate = false;
			if (isUpdateNeeded)
			{
				isUpdate = DateTime.ParseExact (jn ["UpdateTime"].Value, timeFormat, null) > DateTime.ParseExact (lastJn ["UpdateTime"].Value, timeFormat, null);
			}
			AssetData.Add (id, ASSET_TYPE.Name, null, jn);
			for (int j = 0; j < sdCachePath.Length; j++)
			{
				assetNameAndTypeDic.Clear ();

				sdPath.Remove (0, sdPath.Length);
				sdPath.Append (sdCachePath [j]);
				sdPath.Append (id);
				sdPath.Append ("/");
				try
				{
					if (!Directory.Exists (sdPath.ToString ()))
					{
						Directory.CreateDirectory (sdPath.ToString ());
					}
				}
				catch (Exception e)
				{
					Debug.LogError (string.Format ("Directory.CreateDirectory Fail! localPath={0},Exception={1}", sdPath, e.Message)); 
				}
				if (j == 0)
				{
					assetNameAndTypeDic.Add (jn ["ChineseVoice"].Value, ASSET_TYPE.ChineseVoice);
					assetNameAndTypeDic.Add (jn ["EnglishVoice"].Value, ASSET_TYPE.EnglishVoice);
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
					urlPath.Append (currentDownloadURL);
					urlPath.Append ("/");
					urlPath.Append (item.Key);

					sdFile.Remove (0, sdFile.Length);
					sdFile.Append (sdPath);
					sdFile.Append (urlPath.ToString ().GetHashCode ());

					if (item.Value == ASSET_TYPE.ChineseVoice || item.Value == ASSET_TYPE.EnglishVoice)
					{
						sdFile.Append (".mp3");
					}

					yield return StartCoroutine (DownLoadAsset (j, id, item, sdFile.ToString (), urlPath.ToString (), isUpdate));
				}
			}
		}

		if (isDownLoadFail)
		{
			descriptionLabel.text = "下载失败，请检查网络后重试！";
		}
		else
		{
			if(isUpdateNeeded)
			{
				NewJsonNode.SaveToCompressedFile (sdAssetList);
			}
			uiSlider.value = 1;
			descriptionLabel.text = description.Contains ("加载") ? "资源加载完成！" : "资源下载完成！";
			yield return new WaitForSeconds (1);
			LocalStorage.IsSwitchBG = false;
			Application.LoadLevel ("SelectScene");
		}
	}

	IEnumerator DownLoadAsset (int j, string ID, KeyValuePair<string, ASSET_TYPE> ItemAssetNameAndType, string sdFile, string UrlPath, bool isUpdate)
	{
		if (isDownLoadFail)
		{
			yield break;
		}
		string path = "";

		if (isUpdate)
		{
			path = UrlPath;
			description = "正在下载资源：{0}...";
		}
		else
		{
			description = "正在加载资源：{0}...";
			if (File.Exists (sdFile))
			{
				path = @"file:///" + sdFile;
			}
			else
			{
				StringBuilder sdPath = new StringBuilder ();
				sdPath.Append (streamingAssetsPath [j]);
				sdPath.Append (ID);
				sdPath.Append ("/");
				sdPath.Append (UrlPath.ToString ().GetHashCode ());
				if (ItemAssetNameAndType.Value == ASSET_TYPE.ChineseVoice || ItemAssetNameAndType.Value == ASSET_TYPE.EnglishVoice)
				{
					sdPath.Append (".mp3");
				}
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
				path = @"file:///" + sdPath.ToString();
#else
				path = sdPath.ToString ();
#endif
			}
		}

		WWW www = new WWW (path);
		descriptionLabel.text = string.Format (description, ItemAssetNameAndType.Key);
		yield return www;
		if (www.error != null)
		{
			Debug.LogError (string.Format ("DownLoadAsset Fail! path={0}, error={1}", path, www.error));
			//isDownLoadFail = true;
			yield break;
		}
		if (isUpdate)
		{
			// 防止空图片造成额外BUG风险
			if (www.bytes.Length >= 8)
			{
				try
				{
					// 保存图片
					File.WriteAllBytes (sdFile, www.bytes);
				}
				catch (Exception e)
				{
					Debug.LogError ("File.WriteAllBytes Fail ===>" + e.Message);
				}
			}
		}
		AssetData.Add (ID, ItemAssetNameAndType.Value, www);

		uiSliderValue += 1.0f / assetNum;
		uiSlider.value = Mathf.Clamp (uiSliderValue, 0.01f, uiSliderValue);
	}
}
