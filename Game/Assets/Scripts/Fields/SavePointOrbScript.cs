using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SavePointOrbScript : MonoBehaviour 
{
	List<ContinueHighlightInputScript.SaveDataInformation> m_lSaveFiles = new List<ContinueHighlightInputScript.SaveDataInformation>();
	bool m_bDisplaySaveScreen = false;
	public Texture2D m_tDisplayTexture;
	public Texture2D m_tSelectionTexture;
	int m_nSelectedIndex = 0;


	public AudioClip m_acSaveSucceed;
	public AudioClip m_acSaveSelectMovement;
	public string m_szSaveLocationName = "";


	GameObject m_goRootParent;
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
				m_goRootParent.SetActive (false);
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
				SaveFile (m_nSelectedIndex + 1);
			}
		}
	}

	void UpdateUI()
	{
		m_lSaveFiles.Clear();
		//Load in the valid levels, to show which files are available and if there'd be something 
		//being overwritten
		for(int i = 1; i < 4; ++i)
		{
			ContinueHighlightInputScript.SaveDataInformation gContainer = gameObject.GetComponent<LoadingScript>().GetSaveData(i);
			if(gContainer != null)
			{
				gContainer.m_nSlotNumber = i;
				m_lSaveFiles.Add(gContainer);

			}
		}
		m_goRootParent = GameObject.Find ("SaveScreen").transform.Find ("Background").gameObject;
		List<GameObject> _saveFiles = new List<GameObject> ();
		foreach (Transform _child in m_goRootParent.transform)
		{
			if (_child.gameObject.name.Contains ("Slot"))
			{
				_saveFiles.Add (_child.gameObject);
				_child.gameObject.GetComponent<SaveSlotScript> ().m_goSavePointOrbOrigin = gameObject;
			}
		}
		if (m_lSaveFiles.Count > 0)
		{
			List<int> _lCounters = new List<int> ();
			_lCounters.Add (0);_lCounters.Add (1); _lCounters.Add (2);
			foreach (ContinueHighlightInputScript.SaveDataInformation _svfile in m_lSaveFiles)
			{
				_lCounters.Remove (_svfile.m_nSlotNumber - 1);
				//Okay so this may not be chronological so use the 'i' iterator and don't make assumptions.
				//No save files present, just hide all of the slot information.
				float _fTimer = _svfile.m_fTimePlayed;
				float _fMinutes = Mathf.Floor (_fTimer / 60);
				float _fSeconds = Mathf.Round (_fTimer % 60);
				string _szMinutes = _fMinutes.ToString ("00");
				string _szSeconds = _fSeconds.ToString ("00");
				_saveFiles [_svfile.m_nSlotNumber - 1].transform.Find ("Location").gameObject.GetComponent<Text> ().text = _svfile.m_szFieldName;
				_saveFiles [_svfile.m_nSlotNumber - 1].transform.Find ("Level").gameObject.GetComponent<Text> ().text = _svfile.m_nLevel.ToString ();
				_saveFiles [_svfile.m_nSlotNumber - 1].transform.Find ("TimePlayed").gameObject.GetComponent<Text> ().text = "Time Played: " + _szMinutes + ":" + _szSeconds;
			}
			foreach (int n in _lCounters)
			{
				_saveFiles[n].transform.Find ("Location").gameObject.GetComponent<Text> ().text = "";
				_saveFiles[n].transform.Find ("Level").gameObject.GetComponent<Text> ().text = "";
				_saveFiles[n].transform.Find ("TimePlayed").gameObject.GetComponent<Text> ().text = "";
			}
		}
		else
		{
			//No save files present, just hide all of the slot information.
			foreach (GameObject _file in _saveFiles)
			{
				_file.transform.Find ("Location").gameObject.GetComponent<Text>().text = "";
				_file.transform.Find ("Level").gameObject.GetComponent<Text> ().text = "";
				_file.transform.Find ("TimePlayed").gameObject.GetComponent<Text> ().text = "";
			}
		}
	}

	public void SaveFile(int _nSlotNumber)
	{
		FieldPlayerMovementScript go = GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>();
		DCScript dcs = GameObject.Find("PersistantData").GetComponent<DCScript>();
		dcs.m_szSaveLocationName = m_szSaveLocationName;
		dcs.SetPreviousFieldName (SceneManager.GetActiveScene().name);
		dcs.SetPreviousPosition(go.transform.position);
		dcs.SetPreviousFacingDirection(go.m_nFacingDir);
		GetComponent<SavingScript>().Save(_nSlotNumber);
		UpdateUI ();
		m_bDisplaySaveScreen = false;
		m_goRootParent.SetActive (false);

		GetComponent<AudioSource>().PlayOneShot(m_acSaveSucceed, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
		GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
		Input.ResetInputAxes ();

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
			UpdateUI ();
			m_goRootParent.SetActive (true);
			Input.ResetInputAxes();
		}
	}

	/*
	void OnGUI()
	{
		if(m_bDisplaySaveScreen == true)
		{
			Vector2 size = new Vector2(450, 300);
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
				float _fTimer =  m_lSaveFiles[i].m_fTimePlayed;
				float _fMinutes = Mathf.Floor (_fTimer / 60);
				float _fSeconds = Mathf.Round (_fTimer % 60);
				string _szMinutes = _fMinutes.ToString ("00");
				string _szSeconds = _fSeconds.ToString ("00");

				string szContent  = m_lSaveFiles[i].m_szName + "     Level: " + m_lSaveFiles[i].m_nLevel + "\n" +
					m_lSaveFiles[i].m_szFieldName + "     Time Played: " + _szMinutes + ":" + _szSeconds;

				GUI.Label(new Rect(25, size.y * 0.33f *i + 15.0f, size.x, size.y * 0.33f), szContent);
			}
			GUI.color = catchColor;
			GUI.EndGroup();
		}
	}
	*/

}
