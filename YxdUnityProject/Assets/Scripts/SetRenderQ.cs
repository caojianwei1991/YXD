using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class SetRenderQ : MonoBehaviour
{
    public int setRendererQ = 3100;
    Renderer myRenderer;
    Renderer[] m_childRenderList;
    // Use this for initialization
    void Awake()
    {
        try
        {
            myRenderer = GetComponent<Renderer>();
        }
        catch
        {
 
        }
        if (myRenderer != null)
        {
            myRenderer.material.renderQueue = setRendererQ;
        }
        m_childRenderList = gameObject.GetComponentsInChildren<Renderer>();
        if (m_childRenderList.Length == 0)
        {
            return;
        }
        for (int i = 0; i < m_childRenderList.Length; ++i )
        {
            m_childRenderList[i].material.renderQueue = setRendererQ;
        }
    }

    // Update is called once per frame
    /*void Update()
    {
       
    }*/
}
