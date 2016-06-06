using UnityEngine;
using System.Collections;

public static class DirectoryTool
{
	public static string persistentDataPath
	{
		get
		{
			// 恶心的机型适配问题。
			// 对于部分安卓机型， Application.persistentDataPath 真的可能取到空字符串。
			// 此时尝试一下自己构造安卓机型的存储位置。
#if UNITY_ANDROID
			return Application.persistentDataPath == null || Application.persistentDataPath == "" ? "/data/data/com.kudospark.yxd" : Application.persistentDataPath;
#else
			return Application.persistentDataPath;
#endif
		}
	}

	public static string LocalPathToStreamingAssetsPath (string localPath)
	{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		return "file://" + Application.dataPath + "/StreamingAssets/" + localPath;
#elif UNITY_IPHONE
        return "file://" + Application.dataPath + "/Raw/" + localPath;
#elif UNITY_ANDROID
		return "jar:file://" + Application.dataPath + "!/assets/" + localPath;
#endif
	}
}
