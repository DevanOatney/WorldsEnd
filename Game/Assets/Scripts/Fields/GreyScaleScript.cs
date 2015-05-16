using UnityEngine;
using System.Collections;

public class GreyScaleScript : MonoBehaviour 
{

	//Stuff for greyscaling background as you in the party menu
	private Texture2D m_tRenderTarget;
	private Material m_mGreyMat;
	public Shader m_sGreyShader;
	private bool m_bToGreyScale = false;
	//flag for when the current screen needs to be caught
	private bool m_bNeedToGrabScreen = false;


	float m_fBlurAmount = 0.0f;
	float m_fGreyAmount = 0.0f;


	// Use this for initialization
	void Start () 
	{
		//Set the data for the greyscale texture during party menu
		m_mGreyMat = new Material (m_sGreyShader);
		m_tRenderTarget = new Texture2D (Screen.width, Screen.height, TextureFormat.RGB24, true);
		m_tRenderTarget.wrapMode = TextureWrapMode.Clamp;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bToGreyScale == true)
		{
			m_mGreyMat.SetTexture ("_MainTex", m_tRenderTarget);
			if(m_fBlurAmount <= 1.5f)
				m_fBlurAmount += 2 * Time.deltaTime;
			if(m_fGreyAmount < 1.0f)
				m_fGreyAmount += 1 * Time.deltaTime;
			m_mGreyMat.SetFloat ("_Amount", m_fBlurAmount);
			m_mGreyMat.SetFloat("_GreyAmount", m_fGreyAmount);
			m_mGreyMat.SetFloat("_Fade", 0);
		}
	}
	void StartGreyScale()
	{
		m_bNeedToGrabScreen = true;
	}
	void EndGreyScale()
	{
		m_bNeedToGrabScreen = false;
		m_bToGreyScale = false;
	}
	void OnPostRender()
	{
		if(m_bNeedToGrabScreen == true)
		{
			GrabScreen();
			m_bToGreyScale = true;
			m_bNeedToGrabScreen = false;
		}
		
		if (m_bToGreyScale == true)
		{
			GL.PushMatrix ();
			for (var i = 0; i < m_mGreyMat.passCount; ++i) 
			{
				m_mGreyMat.SetPass (i);
				
				GL.LoadOrtho ();
				GL.Begin (GL.QUADS); // Quad
				GL.Color (new Color (1f, 1f, 1f, 1f));
				GL.MultiTexCoord (0, new Vector3 (0f, 0f, 0f));
				GL.Vertex3 (0, 0, 0);
				GL.MultiTexCoord (0, new Vector3 (0f, 1f, 0f));
				GL.Vertex3 (0, 1, 0);
				GL.MultiTexCoord (0, new Vector3 (1f, 1f, 0f));
				GL.Vertex3 (1, 1, 0);
				GL.MultiTexCoord (0, new Vector3 (1f, 0f, 0f));
				GL.Vertex3 (1, 0, 0);
				GL.End ();
			}
			GL.PopMatrix ();
		}
	}

	//Catch the current screen for greyscaling before the menu screen pops up
	void GrabScreen()
	{
		m_tRenderTarget.ReadPixels (new Rect (0, 0, Screen.width, Screen.height), 0, 0, true);
		m_tRenderTarget.Apply (true);
		m_fBlurAmount = 0.0f;
		m_fGreyAmount = 0.0f;
	}
}
