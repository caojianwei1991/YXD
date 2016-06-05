using UnityEngine;
using System.Collections;

public class GUIBase{

	protected UnityEngine.Object UIRes; 
	protected string PrefabRoot;
	protected GameObject ClonedPrefab;

	public GUIBase(string prefabRoot)
	{
		PrefabRoot = prefabRoot;
	}

	public void Enter()
	{
		CheckAndInitPrefab ();
		AfterEnter ();
	}

	public void CheckAndInitPrefab()
	{
		if(!string.IsNullOrEmpty(PrefabRoot))
		{
			if(UIRes == null)
			{
				UIRes = Resources.Load(PrefabRoot);
			}
		}
		else
		{
			Debug.LogWarning("GUIBase prefabroot is empty!");
			return;
		}

		if(UIRes != null)
		{
			ClonedPrefab = GameObject.Instantiate(UIRes) as GameObject;
			GameObject.DontDestroyOnLoad(ClonedPrefab);
		}
	}

	public void Exit()
	{
		BeforExit ();

		if(ClonedPrefab != null)
		{
			GameObject.Destroy(ClonedPrefab);
		}
	}

	public virtual void Update()
	{

	}

	public virtual void OnGUI()
	{

	}

	protected virtual void AfterEnter()
	{
	}

	protected virtual void BeforExit()
	{
	}


}
