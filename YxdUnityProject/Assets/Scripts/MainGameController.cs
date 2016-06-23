﻿using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

public enum GAME_TYPE : int
{
	ReadPicture = 0,
	SpeechRecognizer = 1,
	HWR = 2,
	ListenPicture = 3,
	LinkPicture = 4
}


public class MainGameController : MonoBehaviour
{
	int RandomQuestionNum = 10;
	int totalQuestionSize;
	int currentQuestionPos = 0;//-1;
	string questionNumber = "50";
	string questionOrder = "-1";//"0";
	int returnQuestionNum;
	UIButton speaker, voice, mHWR;
	UITexture voiceUITexture;
	Texture[] voiceTexture = new Texture[3];
	UILabel mHWRLabel;
	GameObject tablet;
	List<IEnumerator> iEnumeratorList = new List<IEnumerator> ();

	public UIQuestion[] uiQuestions{ get; private set; }

	public UIAnswer[] uiAnswers{ get; private set; }

	public UIAnswer answer{ get; private set; }

	public JSONNode jsonNode{ get; private set; }

	public JSONNode allQuestions{ get; private set; }

	public int questionIndex{ get; private set; }

	public GAME_TYPE GameType{ get; private set; }

	void Awake ()
	{
		uiQuestions = new UIQuestion[4];
		uiAnswers = new UIAnswer[4];
		answer = new UIAnswer ();

		transform.FindChild ("Back").GetComponent<UIButton> ().onClick.Add (new EventDelegate (() => Application.LoadLevel ("SelectScene")));
		for (int i = 0; i < uiQuestions.Length; i++)
		{
			uiQuestions [i] = transform.FindChild ("AnimalTexture" + i).GetComponent<UIQuestion> ();
		}
		for (int i = 0; i < uiAnswers.Length; i++)
		{
			uiAnswers [i] = transform.FindChild ("AnimalName" + i).GetComponent<UIAnswer> ();
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
		GetQuestions ();
	}

	void InitUI ()
	{
		for (int i = 0; i < uiQuestions.Length; i++)
		{
			uiQuestions [i].Init ();
		}

		for (int i = 0; i < uiAnswers.Length; i++)
		{
			uiAnswers [i].Init ();
		}

		speaker.gameObject.SetActive (false);
		speaker.normalSprite = "yuyin1";
		speaker.onClick.Clear ();

		voice.gameObject.SetActive (false);
		voiceUITexture.mainTexture = voiceTexture [0];
		voice.onClick.Clear ();

		mHWR.gameObject.SetActive (false);
		mHWRLabel.text = "";
		mHWR.onClick.Clear ();

		tablet.SetActive (false);

		for (int i = 0; i < iEnumeratorList.Count; i++)
		{
			if (iEnumeratorList [i] != null)
			{
				StopCoroutine (iEnumeratorList [i]);
			}
		}
	}

	void ShowPictures (int Num)
	{
		List<int> result = RandomInt (Num);

		for (int i = 0; i < result.Count; i++)
		{
			string ID = jsonNode ["Characters"] [result [i]] ["CharacterID"].Value;
			if (Num == 1)
			{
				int choiceID = jsonNode ["Choice"].AsInt - 1;
				ID = jsonNode ["Characters"] [choiceID] ["CharacterID"].Value;
				Vector3 v3 = uiQuestions [0].LocalPosition;
				v3.x = 0;
				uiQuestions [0].LocalPosition = v3;
			}
			uiQuestions [i].SetCharacterID (ID);
		}
	}

	void SetAnswer ()
	{
		int choiceID = jsonNode ["Choice"].AsInt - 1;
		bool isEnglish = jsonNode ["IsEnglish"].AsBool;
		string characterID = jsonNode ["Characters"] [choiceID] ["CharacterID"].Value;
		answer.Name = AssetData.GetNameByID (characterID, isEnglish);
		answer.CharacterID = characterID;
		answer.IsEnglish = isEnglish;
		answer.mTransform = uiQuestions [0].transform;
	}

	public bool JudgeIsMatch (string CharacterID = "")
	{
		bool isFinish = true;
		if (GameType == GAME_TYPE.LinkPicture)
		{
			for (int i = 0; i < uiAnswers.Length; i++)
			{
				if (uiAnswers [i].gameObject.activeInHierarchy)
				{
					isFinish = false;
					break;
				}
			}
		}
		else
		{
			isFinish = false;
			if (answer.CharacterID == CharacterID)
			{
				isFinish = true;
			}
		}
		return isFinish;
	}

	public void AllRight ()
	{
		NextQuestion (false, "");
	}

	void ShowAnimalNames ()
	{
		var characters = jsonNode ["Characters"];

		List<int> result = RandomInt (uiAnswers.Length);

		for (int i = 0; i < result.Count; i++)
		{
			string characterID = characters [result [i]] ["CharacterID"].Value;
			uiAnswers [i].SetCharacterID (characterID);
		}
	}

#region 听音识字

	void ShowSpeaker ()
	{
		speaker.gameObject.SetActive (true);
		EventDelegate.Set (speaker.onClick, delegate
		{
			speaker.isEnabled = false;
			var ie = SpeakerAnim ();
			iEnumeratorList.Add (ie);
			StartCoroutine (ie);
			SoundPlay.Instance.Play (answer.CharacterID, answer.IsEnglish, () =>
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
			var ie = StartUITextureAnimal (voiceUITexture, "", voiceTexture);
			iEnumeratorList.Add (ie);
			StartCoroutine (ie);
		});
	}

	void ReceiveIse (string result)
	{
		string[] str = result.Split (',');
		var jc = new JSONClass ();
		jc.Add ("GameType", ((int)GAME_TYPE.SpeechRecognizer).ToString ());
		jc.Add ("Text", answer.Name);
		jc.Add ("RecogText", str [0]);
		jc.Add ("RecogScore", str [1]);
		WWWProvider.Instance.StartWWWCommunication ("GetCorrectAnswer", jc, NextQuestion);
	}

#endregion

#region 手写识别

	string wordHWR;

	void ShowHWR ()
	{
		mHWR.gameObject.SetActive (true);
		if (answer.IsEnglish)
		{
			wordHWR = answer.Name;
		}
		else
		{
			int len = answer.Name.Length;
			int subIndex = Random.Range (0, len);
			wordHWR = answer.Name.Substring (subIndex, 1);
			string temp = answer.Name.Remove (subIndex, 1);
			mHWRLabel.text = temp.Insert (subIndex, "（ ）");
		}

		EventDelegate.Set (mHWR.onClick, delegate
		{
			SoundPlay.Instance.Play (answer.CharacterID, answer.IsEnglish, () =>
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

#region 听音识字
	
	void ListenPicture ()
	{
		ShowPictures (4);
		speaker.gameObject.SetActive (true);
		EventDelegate.Set (speaker.onClick, delegate
		{
			speaker.isEnabled = false;
			IEnumerator ie = SpeakerAnim ();
			iEnumeratorList.Add (ie);
			StartCoroutine (ie);
			SoundPlay.Instance.Play (answer.CharacterID, answer.IsEnglish, () =>
			{
				speaker.isEnabled = true;
				if (ie != null)
				{
					StopCoroutine (ie);
				}
			});
		});
	}
	
#endregion
	
#region 连连看
	
	void LinkPicture ()
	{
		ShowPictures (4);
		ShowAnimalNames ();
		for (int i = 0; i < uiAnswers.Length; i++)
		{
			uiAnswers [i].SetDrag ();
		}
	}
	
#endregion

	void GetQuestions ()
	{
		if (LocalStorage.IsRandomPlay)
		{
			if (LocalStorage.StudentID == "")
			{
				LocalRandomQuestions ();
			}
			else
			{
				ServerRandomQuestions ();
			}
		}
		else
		{
			ServerRandomQuestions ();
		}
	}

	void LocalRandomQuestions ()
	{
		var jc = new JSONClass ();
		jc.Add ("ReturnQestionNum", RandomQuestionNum.ToString ());
		jc.Add ("SceneID", LocalStorage.SceneID);

		for (int i = 0; i < RandomQuestionNum; i++)
		{
			List<string> randomQestionIDs = new List<string> ();
			foreach (var add in AssetData.AssetDataDic)
			{
				if (add.Value.SceneID == LocalStorage.SceneID)
				{
					randomQestionIDs.Add (add.Key);
				}
			}
			for (int j = 0; j < 4; j++)
			{
				int randomID = Random.Range (0, randomQestionIDs.Count);
				for (int m = 0; m < randomQestionIDs.Count; m++)
				{
					if (m == randomID)
					{
						int id1 = j == 0 ? -1 : i;
						int id2 = j == 0 ? -1 : j;
						jc ["Questions"] [id1] ["Characters"] [id2] ["CharacterID"] = randomQestionIDs [m];
						randomQestionIDs.RemoveAt (m);
						break;
					}
				}
			}
			jc ["Questions"] [i] ["IsEnglish"] = LocalStorage.Language;
			int gameID = Random.Range (0, 3);
			switch (gameID)
			{
				case 0:
					jc ["Questions"] [i] ["Choice"] = Random.Range (1, 5).ToString ();
					jc ["Questions"] [i] ["DisplayText"] = Random.Range (0, 2).ToString ();
					jc ["Questions"] [i] ["GameType"] = Random.Range (1, 4).ToString ();
					break;
				case 1:
					jc ["Questions"] [i] ["Choice"] = Random.Range (1, 5).ToString ();
					jc ["Questions"] [i] ["DisplayText"] = Random.Range (0, 2).ToString ();
					break;
				default:
					break;
			}
			jc ["Questions"] [i] ["GameID"] = gameID.ToString ();
		}
		Debug.LogError (jc.ToString ());
		DealQuestionsData (true, jc.ToString ());
	}
	
	void ServerRandomQuestions ()
	{
		string gameName = "GetRandomQuestions";
		if (Application.internetReachability != NetworkReachability.NotReachable)
		{
			var jc = new JSONClass ();
			jc.Add ("StudentID", LocalStorage.StudentID);
			jc.Add ("SceneID", LocalStorage.SceneID);
			jc.Add ("QuestionNumber", questionNumber);
			if (!LocalStorage.IsRandomPlay)
			{
				jc.Add ("QuestionPos", currentQuestionPos.ToString ());
				jc.Add ("QuestionOrder", questionOrder);
				gameName = "GetQuizQuestions";
			}
			jc.Add ("Language", LocalStorage.Language);
			WWWProvider.Instance.StartWWWCommunication (gameName, jc, DealQuestionsData);
		}
		else
		{
			LocalRandomQuestions ();
		}
	}

	void DealQuestionsData (bool IsSuccess, string JsonData)
	{
		allQuestions = JSONNode.Parse (JsonData);
		returnQuestionNum = allQuestions ["ReturnQestionNum"].AsInt;
		if (!LocalStorage.IsRandomPlay)
		{
			currentQuestionPos = allQuestions ["CurrentQestionPos"].AsInt;
			totalQuestionSize = allQuestions ["TotalQuestionSize"].AsInt;
		}
		questionIndex = 0;
		DataToUI ();
	}

	void DataToUI ()
	{
		jsonNode = allQuestions ["Questions"] [questionIndex];
		switch (jsonNode ["GameID"].Value)
		{
			case "0":
				SetAnswer ();
				ReadPicture ();
				break;
			case "1":
				GameType = GAME_TYPE.ListenPicture;
				SetAnswer ();
				ListenPicture ();
				break;
			case "2":
				GameType = GAME_TYPE.LinkPicture;
				LinkPicture ();
				break;
			default:
				break;
		}
		questionIndex++;
	}

	void ReadPicture ()
	{
		switch (jsonNode ["GameType"].Value)
		{
			case "1":
				GameType = GAME_TYPE.ReadPicture;
				ShowPictures (1);
				ShowAnimalNames ();
				break;
			case "2":
				GameType = GAME_TYPE.SpeechRecognizer;
				ShowPictures (1);
				ShowSpeaker ();
				break;
			case "3":
				GameType = GAME_TYPE.HWR;
				ShowPictures (1);
				ShowHWR ();
				break;
			default:
				break;
		}
	}

	public void NextQuestion (bool IsSuccess = true, string JsonData = "")
	{
		InitUI ();
		if (questionIndex >= returnQuestionNum)
		{
			if (LocalStorage.IsRandomPlay)
			{
				FinishQuestion ();
			}
			else if (currentQuestionPos >= totalQuestionSize - 1)
			{
				FinishQuestion ();
			}
			else
			{
				GetQuestions ();
			}
		}
		else
		{
			DataToUI ();
		}
	}

	void FinishQuestion ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space))
		{
			AllRight ();
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

	List<int> RandomInt (int num)
	{
		List<int> result = new List<int> ();
		while (result.Count < num)
		{
			int i = Random.Range (0, num);
			if (!result.Contains (i))
			{
				result.Add (i);
			}
		}
		return result;
	}

}
