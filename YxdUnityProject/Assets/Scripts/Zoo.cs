using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Zoo : MonoBehaviour
{
	string currentQestionPos = "-1";
	string questionNumber = "50";
	string questionOrder = "0";
	JSONNode jsonNode = new JSONNode ();
	int questionIndex, returnQestionNum;
	UITexture[] animalTextures = new UITexture[4];
	UILabel[] textureLabels = new UILabel[4];
	UILabel[] animalNames = new UILabel[4];
	UIButton[] animalNameBtn = new UIButton[4];
	UIButton speaker, voice;
	UITexture voiceUITexture;
	Texture[] voiceTexture = new Texture[3];

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
	}

	void Start ()
	{
//		LocalStorage.StudentID = "stu1";
//		LocalStorage.SceneID = "0";
//		LocalStorage.Language = "0";
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
		}
		animalTextures [0].transform.localPosition = new Vector3 (-535, 313, 0);

		speaker.gameObject.SetActive (false);
		speaker.normalSprite = "yuyin1";
		voice.gameObject.SetActive (false);
		voiceUITexture.mainTexture = voiceTexture [0];
	}

#region 看图识字
	string OnePictureAnswerID, OnePictureName;
	bool OnePictureIsEnglish;

	IEnumerator ShowOnePicture ()
	{
		var jn = jsonNode ["Questions"] [questionIndex];
		int choiceID = jn ["Choice"].AsInt;
		OnePictureAnswerID = jn ["Characters"] [choiceID] ["CharacterID"].Value;
		OnePictureIsEnglish = jn ["IsEnglish"].AsBool;

		if (AssetData.AssetDataDic == null || !AssetData.AssetDataDic.ContainsKey (OnePictureAnswerID))
		{
			Debug.LogError ("AssetData.AssetDataDic is null or not contains key!");
			yield break;
		}

		OnePictureName = AssetData.GetNameByID (OnePictureAnswerID, OnePictureIsEnglish);

		animalTextures [0].gameObject.SetActive (true);
		animalTextures [0].transform.localPosition = new Vector3 (0, 313, 0);
		animalTextures [0].mainTexture = AssetData.GetImageByID (OnePictureAnswerID);

		textureLabels [0].gameObject.SetActive (jn ["DisplayText"].AsBool);
		textureLabels [0].text = AssetData.GetNameByID (OnePictureAnswerID, !OnePictureIsEnglish);

		while (animalTextures [0].gameObject.activeInHierarchy)
		{
			for (int i = 0; i < AssetData.GetAnimationImageByID(OnePictureAnswerID).Count; i++)
			{
				animalTextures [0].mainTexture = AssetData.GetAnimationImageByID (OnePictureAnswerID) [i];
				yield return new WaitForSeconds (0.5f);
			}
			//yield return new WaitForSeconds (1);
		}
	}

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
			StartCoroutine (VoiceAnim ());
		});
	}

	IEnumerator VoiceAnim ()
	{
		while (voice.gameObject.activeInHierarchy)
		{
			for (int i = 0; i < voiceTexture.Length; i++)
			{
				voiceUITexture.mainTexture = voiceTexture [i];
				yield return new WaitForSeconds (0.5f);
			}
			//yield return new WaitForSeconds (1);
		}
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
	void ShowHWR ()
	{

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
		questionIndex = 1;
		DataToUI ();
	}

	void NextQuestion  (bool IsSuccess, string JsonData)
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
			case "0":
				StartCoroutine (ShowOnePicture ());
				ShowAnimalNames ();
				break;
			case "1":
				StartCoroutine (ShowOnePicture ());
				ShowSpeaker ();
				break;
			case "2":
				LinkPictures ();
				break;
			default:
				break;
		}
	}

	void ListenPictures ()
	{
		var characters = jsonNode ["Questions"] [questionIndex] ["Characters"];
		speaker.gameObject.SetActive (true);
	}

	void LinkPictures ()
	{
		var characters = jsonNode ["Questions"] [questionIndex] ["Characters"];
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			ReceiveIse ("goat,1");
		}
	}
}
