
public class StartLoadingUI : GUIBase {

	StartLoading m_scene;
	ApplicationInit m_application;

	public StartLoadingUI (string prefabRoot): base(prefabRoot)
	{

	}

	protected override void AfterEnter ()
	{
		if(ClonedPrefab != null)
		{
			m_scene = ClonedPrefab.GetComponent<StartLoading>();
			m_application = ClonedPrefab.AddComponent<ApplicationInit>();
			m_application.StartInitApplication();
		}
	}

	public override void Update ()
	{
		m_scene.UpdateSliderProcess (m_application.Process);
		if(m_application.Process >= 1f)
		{
			GUIRoot.Instance.FadeState(GUIState.GUI_Login);
		}
	}
}
