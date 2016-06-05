
public class LoginUI : GUIBase {

	Login m_scene;

	public LoginUI(string prefabRoot):base(prefabRoot)
	{
		if(ClonedPrefab != null)
		{
			m_scene = ClonedPrefab.GetComponent<Login>();
		}
	}

	protected override void AfterEnter ()
	{
		//todo
	}
}
