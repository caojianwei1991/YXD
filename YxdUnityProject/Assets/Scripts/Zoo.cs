using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Zoo : MonoBehaviour
{
	string currentQestionPos = "0";//"-1";
	string questionNumber = "50";
	string questionOrder = "-1";//"0";
	JSONNode jsonNode = new JSONNode ();
	int questionIndex = 1, returnQestionNum;
	UITexture[] animalTextures = new UITexture[4];
	UILabel[] textureLabels = new UILabel[4];
	UILabel[] animalNames = new UILabel[4];
	UIButton[] animalNameBtn = new UIButton[4];
	UIButton speaker, voice, mHWR;
	UITexture voiceUITexture;
	Texture[] voiceTexture = new Texture[3];
	UILabel mHWRLabel;
	GameObject tablet;

	enum GAME_TYPE : int
	{
		SpeechRecognizer = 1,
		HWR = 2
	}

	void Awake ()
	{
		transform.FindChild ("Back").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Application.LoadLevel ("SelectScene")));
		for (int i = 0; i < animalTextures.Length; i++)
		{
			animalTextures [i] = transform.FindChild ("AnimalTexture" + i).GetComponent<UITexture> ();
			textureLabels [i] = transform.FindChild ("AnimalTexture" + i + "/Label").GetComponent<UILabel> ();
		}
		for (int i = 0; i < animalNames.Length; i++)
		{
			int ID = i;
			animalNames [i] = transform.FindChild ("AnimalName" + i + "/Label").GetComponent<UILabel> ();
			animalNameBtn [i] = transform.FindChild ("AnimalName" + i).GetComponent<UIButton> ();
		}

		speaker = transform.FindChild ("Speaker").GetComponent<UIButton> ();
		voice = transform.FindChild ("Voice").GetComponent<UIButton> ();
		voiceUITexture = transform.FindChild ("Voice").GetComponent<UITexture> ();

		for (int i = 1; i <= voiceTexture.Length; i++)
		{
			voiceTexture [i - 1] = (Texture)Resources.Load ("Texture/Voice" + i);
		}

		mHWR = transform.FindChild ("HWR").GetComponent<UIButton> ();
		mHWRLabel = transform.FindChild ("HWR/Label").GetComponent<UILabel> ();
		tablet = transform.FindChild ("Tablet").gameObject;
	}

	void Start ()
	{
		InitUI ();
		GetQuizQuestions ();
	}

	void InitUI ()
	{
		for (int i = 0; i < 4; i++)
		{
			animalTextures [i].gameObject.SetActive (false);
			textureLabels [i].gameObject.SetActive (false);
			animalNameBtn [i].gameObject.SetActive (false);

			animalTextures [i].mainTexture = null;
			textureLabels [i].text = "";
			animalNames [i].text = "";
			animalNameBtn [i].onClick.Clear();
		}
		animalTextures [0].transform.localPosition = new Vector3 (-535, 313, 0);

		speaker.gameObject.SetActive (false);
		speaker.normalSprite = "yuyin1";
		speaker.onClick.Clear();

		voice.gameObject.SetActive (false);
		voiceUITexture.mainTexture = voiceTexture [0];
		voice.onClick.Clear();

		mHWR.gameObject.SetActive (false);
		mHWRLabel.text = "";
		mHWR.onClick.Clear();

		tablet.SetActive (false);
	}
	
	string OnePictureAnswerID, OnePictureName;
	bool OnePictureIsEnglish;

	void ShowPictures (int Num)
	{
		var jn = jsonNode ["Questions"] [questionIndex];
		int choiceID = jn ["Choice"].AsInt - 1;
		OnePictureAnswerID = jn ["Characters"] [choiceID] ["CharacterID"].Value;
		OnePictureIsEnglish = jn ["IsEnglish"].AsBool;

		if (AssetData.AssetDataDic == null || !AssetData.AssetDataDic.ContainsKey (OnePictureAnswerID))
		{
			Debug.LogError ("AssetData.AssetDataDic is null or not contains key!");
			return;
		}

		OnePictureName = AssetData.GetNameByID (OnePictureAnswerID, OnePictureIsEnglish);

		for (int i = 0; i < Num; i++)
		{
			animalTextures [i].gameObject.SetActive (true);
			textureLabels [i].gameObject.SetActive (jn ["DisplayText"].AsBool);
			string ID = jn ["Characters"] [i] ["CharacterID"].Value;
			if (Num == 1)
			{
				ID = OnePictureAnswerID;
				animalTextures [i].transform.localPosition = new Vector3 (0, 313, 0);
			}
			animalTextures [i].mainTexture = AssetData.GetImageByID (ID);
			textureLabels [i].text = AssetData.GetNameByID (ID, !OnePictureIsEnglish);
			StartCoroutine (StartUITextureAnimal (animalTextures [i], ID));
		}
	}

	IEnumerator StartUITextureAnimal (UITexture uiTexture, string CharacterID, Texture[] textures = null)
	{
		while (uiTexture.gameObject.activeInHierarchy)
		{
			if (textures == null)
			{
				for (int i = 0; i < AssetData.GetAnimationImageByID(CharacterID).Count; i++)
				{
					uiTexture.mainTexture = AssetData.GetAnimationImageByID (CharacterID) [i];
					yield return new WaitForSeconds (0.5f);
				}
			}
			else
			{
				for (int i = 0; i < textures.Length; i++)
				{
					uiTexture.mainTexture = textures [i];
					yield return new WaitForSeconds (0.5f);
				}
			}
			//yield return new WaitForSeconds (1);
		}
	}

#region 看图识字
	void ShowAnimalNames ()
	{
		var characters = jsonNode ["Questions"] [questionIndex] ["Characters"];
		bool isEnglish = jsonNode ["Questions"] [questionIndex] ["IsEnglish"].AsBool;

		for (int i = 0; i < animalNames.Length; i++)
		{
			string characterID = characters [i] ["CharacterID"].Value;
			animalNameBtn [i].gameObject.SetActive (true);
			animalNames [i].text = AssetData.GetNameByID (characterID, isEnglish);
			EventDelegate.Set (animalNameBtn [i].onClick, delegate
			{
				ClickAnimalNames (characterID, isEnglish);
			});
		}
	}

	void ClickAnimalNames (string CharacterID, bool IsEnglish)
	{
		SoundPlay.Instance.Play (CharacterID, IsEnglish);
		if (OnePictureAnswerID == CharacterID)
		{
			DataToUI ();
		}
	}
#endregion

#region 听音识字
	void ShowSpeaker ()
	{
		var sr = voice.GetComponent<SpeechRecognizer> ();
		sr.SetLanguageAndAnswer (OnePictureIsEnglish, OnePictureName);

		speaker.gameObject.SetActive (true);
		EventDelegate.Set (speaker.onClick, delegate
		{
			speaker.isEnabled = false;
			StartCoroutine (SpeakerAnim ());
			SoundPlay.Instance.Play (OnePictureAnswerID, OnePictureIsEnglish, () =>
			{
				speaker.isEnabled = true;
				speaker.gameObject.SetActive (false);
				ShowVoice ();
			});
		});
	}

	IEnumerator SpeakerAnim ()
	{
		while (speaker.gameObject.activeInHierarchy)
		{
			for (int i = 1; i < 4; i++)
			{
				speaker.normalSprite = "yuyin" + i;
				yield return new WaitForSeconds (0.5f);
			}
			//yield return new WaitForSeconds (1);
		}
	}

	void ShowVoice ()
	{
		voice.gameObject.SetActive (true);
		EventDelegate.Set (voice.onClick, delegate
		{
			voice.isEnabled = false;
			StartCoroutine (StartUITextureAnimal (voiceUITexture, "", voiceTexture));
		});
	}

	void ReceiveIse (string result)
	{
		string[] str = result.Split (',');
		var jc = new JSONClass ();
		jc.Add ("GameType", ((int)GAME_TYPE.SpeechRecognizer).ToString ());
		jc.Add ("Text", OnePictureName);
		jc.Add ("RecogText", str [0]);
		jc.Add ("RecogScore", str [1]);
		WWWProvider.Instance.StartWWWCommunication ("GetCorrectAnswer", jc, NextQuestion);
	}
#endregion

#region 手写识别
	string wordHWR;

	void ShowHWR ()
	{
		var hwr = tablet.GetComponent<HWR> ();
		hwr.SetLanguage (OnePictureIsEnglish);

		mHWR.gameObject.SetActive (true);
		if (OnePictureIsEnglish)
		{
			wordHWR = OnePictureName;
		}
		else
		{
			int len = OnePictureName.Length;
			int subIndex = Random.Range (0, len);
			wordHWR = OnePictureName.Substring (subIndex, 1);
			string temp = OnePictureName.Remove (subIndex, 1);
			mHWRLabel.text = temp.Insert (subIndex, "（ ）");
		}

		EventDelegate.Set (mHWR.onClick, delegate
		{
			SoundPlay.Instance.Play (OnePictureAnswerID, OnePictureIsEnglish, () =>
			{
				mHWR.gameObject.SetActive (false);
				tablet.SetActive (true);
			});
		});
	}

	void ReceiveHWR (string result)
	{
		string[] str = result.Split (',');
		int RecogScore = 0;
		string RecogText = wordHWR;
		if (result.Contains (wordHWR))
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (str [i].Contains (wordHWR))
				{
					RecogScore = (str.Length - i) * 10;
				}
			}
		}
		else
		{
			RecogText = str [0];
		}
		var jc = new JSONClass ();
		jc.Add ("GameType", ((int)GAME_TYPE.HWR).ToString ());
		jc.Add ("Text", wordHWR);
		jc.Add ("RecogText", RecogText);
		jc.Add ("RecogScore", RecogScore.ToString ());
		WWWProvider.Instance.StartWWWCommunication ("GetCorrectAnswer", jc, NextQuestion);
	}
#endregion

	void GetQuizQuestions ()
	{
		var jc = new JSONClass ();
		jc.Add ("StudentID", LocalStorage.StudentID);
		jc.Add ("SceneID", LocalStorage.SceneID);
		jc.Add ("QuestionPos", currentQestionPos);
		jc.Add ("QuestionNumber", questionNumber);
		jc.Add ("QuestionOrder", questionOrder);
		jc.Add ("Language", LocalStorage.Language);
		WWWProvider.Instance.StartWWWCommunication ("GetQuizQuestions", jc, DealQuizQuestionsData);
	}

	void DealQuizQuestionsData (bool IsSuccess, string JsonData)
	{
		jsonNode = JSONNode.Parse (JsonData);
		currentQestionPos = jsonNode ["CurrentQestionPos"].Value;
		returnQestionNum = jsonNode ["ReturnQestionNum"].AsInt;
		DataToUI ();
	}

	void NextQuestion (bool IsSuccess, string JsonData)
	{
		DataToUI ();
	}

	void DataToUI ()
	{
		InitUI ();
		switch (jsonNode ["Questions"] [questionIndex] ["GameID"].Value)
		{
			case "0":
				ReadPictures ();
				break;
			case "1":
				ListenPictures ();
				break;
			case "2":
				LinkPictures ();
				break;
			default:
				break;
		}
		questionIndex++;
	}

	void ReadPictures ()
	{
		switch (jsonNode ["Questions"] [questionIndex] ["GameType"].Value)
		{
			case "1":
				ShowPictures (1);
				ShowAnimalNames ();
				break;
			case "2":
				ShowPictures (1);
				ShowSpeaker ();
				break;
			case "3":
				ShowPictures (1);
				ShowHWR ();
				break;
			default:
				break;
		}
	}

#region 听音识字

	void ListenPictures ()
	{
		ShowPictures (4);
		speaker.gameObject.SetActive (true);
		EventDelegate.Set (speaker.onClick, delegate
		{
			StartCoroutine (SpeakerAnim ());
			SoundPlay.Instance.Play (OnePictureAnswerID, OnePictureIsEnglish);
		});
	}

#endregion

#region 连连看

	void LinkPictures ()
	{

	}

#endregion
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space))
		{
			NextQuestion (false, "");
		}
	}
}
