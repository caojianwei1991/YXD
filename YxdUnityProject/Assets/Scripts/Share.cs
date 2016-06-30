using UnityEngine;
using System.Collections;
using SimpleJSON;
using cn.sharesdk.unity3d;

public class Share : MonoBehaviour
{

	private ShareSDK ssdk;
	string[] shareName =
	{
		"微信好友",
		"微信朋友圈",
		"手机QQ",
		"QQ空间",
		"新浪微博",
		"Facebook",
		"Twitter"
	};
	PlatformType[] platformType = 
	{
		PlatformType.WeChat,
		PlatformType.WeChatMoments,
		PlatformType.QQ,
		PlatformType.QZone,
		PlatformType.SinaWeibo,
		PlatformType.Facebook,
		PlatformType.Twitter
	};
	UIButton[] shareBtns;
	string text;

	public static void Show ()
	{
		UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot> ();
		NGUITools.AddChild (uiRoot.gameObject, (GameObject)Resources.Load ("Prefabs/Share"));
	}

	void Awake ()
	{
		shareBtns = new UIButton[shareName.Length];
		for (int i=0; i < shareBtns.Length; i++)
		{
			int index = i;
			shareBtns [i] = transform.FindChild (shareName [i]).GetComponent<UIButton> ();
			shareBtns [i].onClick.Add (new EventDelegate (() => StartShare (index)));
		}
		transform.FindChild ("Texture/Close").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Destroy (gameObject)));
		ssdk = GetComponent<ShareSDK> ();
		ssdk.shareHandler = OnShareResultHandler;
	}

	void OnShareResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		SetBtnsEnabled (true);
		if (state == ResponseState.Success)
		{
			Debug.Log ("share result :");
			Debug.Log (MiniJSON.jsonEncode (result));
			Destroy (gameObject);
		}
		else if (state == ResponseState.Fail)
		{
			Debug.LogError ("fail! error code = " + result ["error_code"] + "; error msg = " + result ["error_msg"]);
			Alert.Show ("分享失败，请重试！");
		}
		else if (state == ResponseState.Cancel)
		{
			Debug.Log ("cancel !");
		}
	}

	void SetBtnsEnabled (bool isEnabled)
	{
		for (int i=0; i < shareBtns.Length; i++)
		{
			shareBtns [i].isEnabled = isEnabled;
		}
	}

	void StartShare (int id)
	{
		SetBtnsEnabled (false);
		ShareContent content = new ShareContent ();
		content.SetText (text);
		//content.SetImageUrl ("https://f1.webshare.mob.com/code/demo/img/1.jpg");
		content.SetTitle ("易学岛");
		content.SetTitleUrl ("https://www.baidu.com/");
		//content.SetSite ("Mob-ShareSDK");
		//content.SetSiteUrl ("https://www.baidu.com/");
		//content.SetUrl ("https://www.baidu.com/");
		//content.SetComment ("test description");
		//content.SetMusicUrl ("http://mp3.mwap8.com/destdir/Music/2009/20090601/ZuiXuanMinZuFeng20090601119.mp3");
		content.SetShareType (ContentType.Text);
		ssdk.ShareContent (platformType [id], content);
	}
	
	void Start ()
	{
		var jc = new JSONClass ();
		string UserID = LocalStorage.StudentID;
		if (UserID == "")
		{
			UserID = SystemInfo.deviceUniqueIdentifier;
		}
		string SchoolID = LocalStorage.SchoolID;
		if (SchoolID == "")
		{
			SchoolID = "kudospark";
		}
		jc.Add ("UserID", UserID);
		jc.Add ("SchoolID", SchoolID);
		jc.Add ("APPStatus", LocalStorage.SchoolID == "" ? "1" : "0");
		WWWProvider.Instance.StartWWWCommunication ("GetShareText", jc, (x, y) =>
		{
			var jn = JSONNode.Parse (y);
			transform.FindChild ("Texture/Name").GetComponent<UILabel> ().text = LocalStorage.StudentID;
			transform.FindChild ("Texture/Label").GetComponent<UILabel> ().text = text = jn ["shareText"].Value;
		});
	}
}
