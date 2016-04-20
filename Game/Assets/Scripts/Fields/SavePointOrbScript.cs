using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SavePointOrbScript : MonoBehaviour 
{
	List<ContinueHighlightInputScript.SaveDataInformation> m_lSaveFiles = new List<ContinueHighlightInputScript.SaveDataInformation>();
	bool m_bDisplaySaveScreen = false;
	public Texture2D m_tDisplayTexture;
	public Texture2D m_tSelectionTexture;
	int m_nSelectedIndex = 0;
	public AudioClip m_acSaveSucceed;
	public AudioClip m_acSaveSelectMovement;
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bDisplaySaveScreen == true)
		{
			if(Input.GetKeyUp(KeyCode.Escape))
			{
				m_bDisplaySaveScreen = false;
				GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
				Input.ResetInputAxes();
			}

			if(Input.GetKeyUp(KeyCode.DownArrow))
			{
				GetComponent<AudioSource>().PlayOneShot(m_acSaveSelectMovement, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
				++m_nSelectedIndex;
				if(m_nSelectedIndex >= 3)
					m_nSelectedIndex = 0;
			}
			else if(Input.GetKeyUp(KeyCode.UpArrow))
			{
				GetComponent<AudioSource>().PlayOneShot(m_acSaveSelectMovement, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
				--m_nSelectedIndex;
				if(m_nSelectedIndex < 0)
					m_nSelectedIndex = 2;
			}
			else if(Input.GetKey(KeyCode.Return))
			{
				FieldPlayerMovementScript go = GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>();
				DCScript dcs = GameObject.Find("PersistantData").GetComponent<DCScript>();

				dcs.SetPreviousPosition(go.transform.position);
				dcs.SetPreviousFacingDirection(go.m_nFacingDir);
				GetComponent<SavingScript>().Save(m_nSelectedIndex+1);
				m_bDisplaySaveScreen = false;
				GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
				GetComponent<AudioSource>().PlayOneShot(m_acSaveSucceed, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == "Action Box(Clone)" && m_bDisplaySaveScreen == false)
		{
			GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
			//turn on flag to render the saving screen
			m_bDisplaySaveScreen = true;
			//turn off the collider because I don't want a bunch of spam
			c.enabled = false;
			m_lSaveFiles.Clear();
			//Load in the valid levels, to show which files are available and if there'd be something 
			//being overwritten
			for(int i = 1; i < 4; ++i)
			{
				ContinueHighlightInputScript.SaveDataInformation gContainer = gameObject.GetComponent<LoadingScript>().GetSaveData(i);
				if(gContainer != null)
				{
					m_lSaveFiles.Add(gContainer);
				}
			}
			Input.ResetInputAxes();
		}
	}

	void OnGUI()
	{
		if(m_bDisplaySaveScreen == true)
		{
			//turn off field player movement



			Vector2 size = new Vector2(300, 200);
			Rect boxRect = new Rect(Screen.width * 0.5f - size.x * 0.5f, Screen.height * 0.5f - size.y * 0.5f, size.x, size.y);
			GUI.BeginGroup(boxRect);
			GUI.Box(new Rect(0, 0, size.x, size.y), m_tDisplayTexture);
			GUIStyle selectorStyle = new GUIStyle(GUI.skin.box);
			selectorStyle.normal.background = m_tSelectionTexture;
			Color catchColor = GUI.color;
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
			GUI.Box(new Rect(0, size.y*0.33f*m_nSelectedIndex, size.x, size.y *0.33f), "",selectorStyle);
			GUI.color = Color.black;
			for(int i = 0; i < m_lSaveFiles.Count; ++i)
			{
				string szContent  = m_lSaveFiles[i].m_szName + "     Level: " + m_lSaveFiles[i].m_nLevel + "\n" +
					m_lSaveFiles[i].m_szFieldName;

				GUI.Label(new Rect(25, size.y * 0.33f *i + 15.0f, size.x, size.y * 0.33f), szContent);
			}
			GUI.color = catchColor;
			GUI.EndGroup();
		}
	}


}
