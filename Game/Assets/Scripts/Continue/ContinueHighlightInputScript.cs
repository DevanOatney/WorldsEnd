using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ContinueHighlightInputScript : MonoBehaviour {
	
	public List<GameObject> m_lSaveFilesObjects;
	List<SaveDataInformation> m_lSaveFilesData = new List<SaveDataInformation>();
	public int m_nSelectedIndex = 0;
	public AudioClip m_aMoveHighlight;
	public AudioClip m_aSelectionMade;
	bool m_bAllowInput = true;
	public Texture2D m_tWhiteSelection;

	public class SaveDataInformation
	{
		public string m_szName;
		public int m_nLevel;
		public string m_szFieldName;
		public string m_szSceneName;
		public int m_nGold;
		public float m_fTimePlayed;
		public int m_nSlotNumber;
	}

	// Use this for initialization
	void Start () 
	{
		//to know how many files were able to load
		int count = 0;

		for(int i = 0; i < 3; ++i)
		{
			SaveDataInformation gContainer = gameObject.GetComponent<LoadingScript>().GetSaveData(i+1);
			if(gContainer != null)
			{
				m_lSaveFilesData.Add(gContainer);
				count++;
			}
			else
			{
				m_lSaveFilesObjects [count].SetActive (false);
				m_lSaveFilesObjects.RemoveAt(count);
			}
		}
		if(count == 0)
		{
			gameObject.GetComponent<Image> ().enabled = false;
			m_bAllowInput = false;
		}
		else
			ChangeHighlightedPosition();
		Camera.main.GetComponent<Light>().intensity = GameObject.Find("PersistantData").GetComponent<DCScript>().m_fBrightness + 1;
		GetComponent<AudioSource>().Stop();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bAllowInput == true)
		{
			if(Input.GetKeyUp(KeyCode.DownArrow))
			{
				++m_nSelectedIndex;
				if(m_nSelectedIndex >= m_lSaveFilesData.Count)
					m_nSelectedIndex = 0;

				ChangeHighlightedPosition();
			}
			else if(Input.GetKeyUp(KeyCode.UpArrow))
			{
				--m_nSelectedIndex;
				if(m_nSelectedIndex < 0)
					m_nSelectedIndex = m_lSaveFilesData.Count - 1;

				ChangeHighlightedPosition();
			}
			else if(Input.GetKeyUp (KeyCode.Return))
			{
				GetComponent<AudioSource>().PlayOneShot(m_aSelectionMade, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
				gameObject.GetComponent<LoadingScript>().Load(m_nSelectedIndex + 1);
                SceneManager.LoadScene(m_lSaveFilesData[m_nSelectedIndex].m_szSceneName);
			}
		}
		if(Input.GetKeyUp(KeyCode.Escape) || Input.GetMouseButtonUp(1))
		{
            SceneManager.LoadScene("IntroMenu_Scene");
		}
	}

	public void ChangeHighlightedPosition(int _nIndex)
	{
		m_nSelectedIndex = _nIndex;
		ChangeHighlightedPosition ();
	}

	public void SelectSavePoint(int _nIndex)
	{
		m_nSelectedIndex = _nIndex;
		GetComponent<AudioSource>().PlayOneShot(m_aSelectionMade, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
		gameObject.GetComponent<LoadingScript>().Load(m_nSelectedIndex + 1);
		SceneManager.LoadScene(m_lSaveFilesData[m_nSelectedIndex].m_szSceneName);
	}

	void ChangeHighlightedPosition()
	{
		if(m_nSelectedIndex != -1)
		{
			float yOffset = 0;
			yOffset = 0.005f;
			GetComponent<AudioSource>().PlayOneShot(m_aMoveHighlight, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
			Vector3 position = new Vector3( m_lSaveFilesObjects[m_nSelectedIndex].transform.position.x,  m_lSaveFilesObjects[m_nSelectedIndex].transform.position.y - yOffset,  m_lSaveFilesObjects[m_nSelectedIndex].transform.position.z + 0.1f);
			gameObject.transform.position = position;

			//area to render more data.. not sure if I need it yet.. but..
			//ContinueStatPageScript csps = GameObject.Find("Stat Page").GetComponent<ContinueStatPageScript>();
			//iterate through each of the save slots available
			for(int i = 0; i < m_lSaveFilesData.Count; ++i)
			{
				m_lSaveFilesObjects[i].GetComponentInChildren<Text>().text = "";
				m_lSaveFilesObjects[i].GetComponentInChildren<Text>().text += m_lSaveFilesData[i].m_szName + "\t\t\tLevel: " + m_lSaveFilesData[i].m_nLevel + "\t\t\tGold: " + m_lSaveFilesData[i].m_nGold + '\n';



				//Save location name + time played
				float _fTimer = m_lSaveFilesData [i].m_fTimePlayed;
				float _fMinutes = Mathf.Floor (_fTimer / 60);
				float _fSeconds = Mathf.Round (_fTimer % 60);
				string _szMinutes = _fMinutes.ToString ("00");
				string _szSeconds = _fSeconds.ToString ("00");
				m_lSaveFilesObjects[i].GetComponentInChildren<Text>().text += m_lSaveFilesData[i].m_szFieldName + "    Time Played: " + _szMinutes + ":" + _szSeconds;
			}
		}
	}

	void OnGUI()
	{

	}
}
