using UnityEngine;
using System.Collections;

public class ApplicationInit : MonoBehaviour {

	public float Process = 0f;

	public void StartInitApplication()
	{
		StartCoroutine (Init());
	}

	IEnumerator Init()
	{
		Process = 0.1f;
		yield return 0.1f;
		Process = 0.2f;
		yield return 0.1f;
		Process = 0.3f;
		yield return 0.1f;
		Process = 0.4f;
		yield return 0.1f;
		Process = 0.5f;
		yield return 0.1f;
		Process = 0.6f;
		yield return 0.1f;
		Process = 0.7f;
		yield return 0.1f;
		Process = 0.8f;
		yield return 0.1f;
		Process = 0.9f;
		yield return 0.1f;

		GUIRoot.Instance.InitAllScene ();
		yield return new WaitForSeconds (0.1f);
		Process = 1f;
	}
}
