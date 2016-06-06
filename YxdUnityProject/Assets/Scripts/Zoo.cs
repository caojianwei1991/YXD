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
			animalNames [i].gameObject.SetActive (false);

			animalTextures [i].mainTexture = null;
			textureLabels [i].text = "";
			animalNames [i].text = "";
		}
		animalTextures [0].transform.localPosition = new Vector3 (-535, 313, 0);

		speaker.gameObject.SetActive (false);
		voice.gameObject.SetActive (false);
	}

#region 看图识字
	string OnePictureAnswerID;

	IEnumerator ShowOnePicture ()
	{
		var jn = jsonNode ["Questions"] [questionIndex];
		int choiceID = jn ["Choice"].AsInt;
		OnePictureAnswerID = jn ["Characters"] [choiceID] ["CharacterID"].Value;
		bool isEnglish = jn ["IsEnglish"].AsBool;

		if (AssetData.AssetDataDic == null || !AssetData.AssetDataDic.ContainsKey (OnePictureAnswerID))
		{
			Debug.LogError ("AssetData.AssetDataDic is null or not contains key!");
			yield break;
		}

		animalTextures [0].gameObject.SetActive (true);
		animalTextures [0].transform.localPosition = new Vector3 (0, 313, 0);
		animalTextures [0].mainTexture = AssetData.AssetDataDic [OnePictureAnswerID].Image;

		textureLabels [0].gameObject.SetActive (jn ["DisplayText"].AsBool);
		textureLabels [0].text = isEnglish ? AssetData.AssetDataDic [OnePictureAnswerID].ChineseName : AssetData.AssetDataDic [OnePictureAnswerID].EnglishName;

		while (true)
		{
			for (int i = 0; i < AssetData.AssetDataDic [OnePictureAnswerID].AnimationImages.Count; i++)
			{
				animalTextures [0].mainTexture = AssetData.AssetDataDic [OnePictureAnswerID].AnimationImages [i];
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
			if (AssetData.AssetDataDic != null && AssetData.AssetDataDic.ContainsKey (characterID))
			{
				animalNames [i].gameObject.SetActive (true);
				animalNames [i].text = isEnglish ? AssetData.AssetDataDic [characterID].EnglishName : AssetData.AssetDataDic [characterID].ChineseName;
				EventDelegate.Set (animalNameBtn [i].onClick, delegate
				{
					ClickAnimalNames (characterID, isEnglish);
				});
			}
			else
			{
				Debug.LogError (string.Format ("AssetData.AssetDataDic is null or not contains key! characterID:{0}", characterID));
			}
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
	IEnumerator ShowSpeaker ()
	{
		speaker.gameObject.SetActive (true);
		while (true)
		{
			for (int i = 1; i < 4; i++)
			{
				speaker.normalSprite = 
				yield return new WaitForSeconds (0.5f);
			}
			//yield return new WaitForSeconds (1);
		}
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
				StartCoroutine (ShowSpeaker ());
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
	
	}
}
