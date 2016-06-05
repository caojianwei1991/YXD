using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GUIState
{
	public const int GUI_StartLoading = 1;
	public const int GUI_Login = 2;
}

public class GUIRoot : MonoBehaviour {

	private static GUIRoot instance;
	public static GUIRoot Instance
	{
		get
		{
			return instance;
		}
	}


	private Dictionary<int, GUIBase> m_UIs = new Dictionary<int, GUIBase> (); 
	private GUIBase m_currentUI;

	void Awake()
	{
		instance = this;
		GameObject.DontDestroyOnLoad (this);
	}

	void Start()
	{
		m_UIs.Add (GUIState.GUI_StartLoading, new StartLoadingUI("ScenePrefab/StartLoadingPrefab"));

		m_currentUI = m_UIs [GUIState.GUI_StartLoading];
		m_currentUI.Enter ();
	}

	void Update()
	{
		m_currentUI.Update ();
	}

	void OnGUI()
	{
		m_currentUI.OnGUI ();
	}

	public void InitAllScene()
	{
		m_UIs.Add (GUIState.GUI_Login, new LoginUI("ScenePrefab/LoginPrefab"));
	}

	public void FadeState(int state)
	{
		if(m_currentUI == m_UIs[state])
		{
			return;
		}

		m_currentUI.Exit ();
		m_currentUI = m_UIs [state];
		m_currentUI.Enter ();
	}

	public T GetStateUI<T>(int state) where T:GUIBase
	{
		return m_UIs [state] as T;
	}
}
