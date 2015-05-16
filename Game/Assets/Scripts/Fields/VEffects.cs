using UnityEngine;
using System.Collections;

public class VEffects : MonoBehaviour {


	public Texture2D RenderTarget;
	public int textureSize;
	public Camera myCamera;
	private Material blurMat;
	public Shader blurShader;

	public float blurAmount;

	//Flag for if the blur effect is over
	public bool bBlur = false;
	public bool bSwirl = false;
	public float fade = 0.0f;
	public float duration = 2.0f;

	bool m_bShouldBlur = false;

	private bool fadein = false;

	private Material swirlMat;
	public Shader swirlShader;
	public Vector2 vSwirlCenter;
	public float fRadius;
	public float fAngle;
	private bool bNeedToReadFrameBuffer = true;

	// Use this for initialization
	void Start () 
	{
		blurMat = new Material (blurShader);
		swirlMat = new Material (swirlShader);
		RenderTarget = new Texture2D (Screen.width, Screen.height, TextureFormat.RGB24, true);
		RenderTarget.wrapMode = TextureWrapMode.Clamp;
	}

	void Update()
	{		
		if (duration > 0.0f) 
		{
			duration -= Time.deltaTime;
			if( bBlur )
			{
				if( fade >= 1.0f )
				{
					fadein = true;
					bNeedToReadFrameBuffer = true;
					blurAmount = 0.0f;
				}
				if( fadein )
					fade -= Time.deltaTime * 0.25f;
				else
				{
					fade += Time.deltaTime *0.25f;
					blurAmount += Time.deltaTime * 20.0f;
				}

				blurMat.SetTexture ("_MainTex", RenderTarget);
				blurMat.SetFloat ("_Amount", blurAmount);
				blurMat.SetFloat("_Fade", fade);
			}
			else if( bSwirl )
			{
				swirlMat.SetTexture("_MainTex", RenderTarget);
				swirlMat.SetFloat ("_Center_X", vSwirlCenter.x);
				swirlMat.SetFloat("_Center_Y", vSwirlCenter.y);
				swirlMat.SetFloat("_Radius", fRadius);
				fAngle += Time.deltaTime;
				swirlMat.SetFloat("_Angle", fAngle);
	  
//_Center_X("CenterX", Float) = 0.0
//	    _Center_Y("CenterY", Float) = 0.0
//	    _Radius("Radius", Float) = 300.0
//	    _Angle("Angle", Float) = 40.0

				//swirl shader stuffs
			}
		} 
		else if(m_bShouldBlur == true)
		{
			if(blurAmount <= 1.5f)
				blurAmount += 2 * Time.deltaTime;
			blurMat.SetTexture ("_MainTex", RenderTarget);
			blurMat.SetFloat ("_Amount", blurAmount);
			blurMat.SetFloat("_Fade", 0);
		}
		else
			bBlur = bSwirl = false;




		if (Input.GetKeyDown (KeyCode.B)) {
						StartBlur ();
				}
		if (Input.GetKeyDown (KeyCode.I)) {
						StartMenuBlur();
				}
				
		if (Input.GetKeyDown (KeyCode.O)) {
				StartSwirlAtPoint(Input.mousePosition.x, Input.mousePosition.y);
		}
	}

	//Function to call when the blur starts
	void StartBlur ()
	{
		bNeedToReadFrameBuffer = true;
		duration = 8.0f;
		bBlur = true;
		blurAmount = 3.5f;
		fade = 0.0f;
		fadein = false;
	}

	//for opening the menu?
	void StartMenuBlur()
	{
		bNeedToReadFrameBuffer = true;
		blurAmount = 0.0f;
		fade = 0.0f;
		fadein = false;
		m_bShouldBlur = !m_bShouldBlur;
	}

	void StartSwirl()
	{
		bNeedToReadFrameBuffer = true;
		duration = 6.0f;
		bSwirl = true;

		vSwirlCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
	//	fRadius = Screen.width * 0.5f;
	//	fAngle = 40.0f;
	}

	void StartSwirlAtPoint(float XPos, float YPos)
	{
		bNeedToReadFrameBuffer = true;
		duration = 6.0f;
		bSwirl = true;
		
		vSwirlCenter = new Vector2(XPos, YPos);
		fRadius = Screen.width * 0.5f;
		fAngle = 40.0f;
	}

	// Update is called once per frame
	void OnPostRender () 
	{		
		if (bNeedToReadFrameBuffer)
		{
			GrabScreen();
		}
		if (bBlur || m_bShouldBlur) {
			GL.PushMatrix ();
			for (var i = 0; i < blurMat.passCount; ++i) {
				blurMat.SetPass (i);
			
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

		if( bSwirl)
		{
			GL.PushMatrix ();
			for (var i = 0; i < swirlMat.passCount; ++i) {
				swirlMat.SetPass (i);
				
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

	void GrabScreen()
	{
		RenderTarget.ReadPixels (new Rect (0, 0, Screen.width, Screen.height), 0, 0, true);
		RenderTarget.Apply (true);
		bNeedToReadFrameBuffer = false;
	}
}
