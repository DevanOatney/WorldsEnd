using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMissionMapScript : MonoBehaviour 
{
	public List<GameObject> m_lWorldNodes = new List<GameObject>();
	public GameObject m_goCanvas;
	public GameObject[] m_goMenuTabs;
	public GameObject m_goRoster;
	public GameObject m_goCharacterInRosterPrefab;
	public GameObject m_goRosterListRoot;
	DCScript dc;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ((Input.GetMouseButtonUp (1) || Input.GetKeyUp(KeyCode.Escape)) && gameObject.activeSelf == true)
		{
			DeactivateMap ();
		}
	}

	public void ActivateMap(List<string> _lactiveNodes)
	{
		dc = GameObject.Find ("PersistantData").GetComponent<DCScript> ();
		GameObject.Find ("Player").GetComponent<FieldPlayerMovementScript> ().BindInput ();
		foreach (string _name in _lactiveNodes)
		{
			foreach (GameObject _go in m_lWorldNodes)
			{
				if (_go.GetComponent<WorldResourceNodeScript> ().m_szNodeLocationName == _name)
				{
					_go.SetActive (true);
				}
			}
		}
		gameObject.SetActive (true);
		m_goCanvas.SetActive (true);
		foreach (GameObject _tabs in m_goMenuTabs)
		{
			_tabs.SetActive (false);
		}

	}


	void AdjustRoster(string _szLocationName)
	{
		//First- clear it up incase any were left over from before
		foreach(Transform go in m_goRosterListRoot.transform)
		{
			Destroy (go.gameObject);
		}


		foreach(DCScript.CharacterData character in dc.GetRoster())
		{
			if(character.m_bHasBeenRecruited == true)
			{
				GameObject characterInList = Instantiate(m_goCharacterInRosterPrefab);
				characterInList.transform.Find("CharacterName").GetComponent<Text>().text = character.m_szCharacterName;
				if (character.m_bCombatCharacter == true)
				{
					//This is a combat character
					characterInList.transform.Find ("CharacterLVL").GetComponent<Text> ().text = character.m_nLevel.ToString ();
				}
				else
				{
					//This is a support character.
					characterInList.transform.Find ("CharacterLVL").GetComponent<Text> ().text = "--";
				}

				characterInList.transform.SetParent(m_goRosterListRoot.transform);
				characterInList.transform.localScale = new Vector3(1, 1, 1);
				characterInList.GetComponent<CharacterInWorldMapRosterScript> ().Initialize (_szLocationName);
			}
		}
	}

	public void DeactivateMap()
	{
		GameObject.Find ("Player").GetComponent<FieldPlayerMovementScript> ().ReleaseBind ();
		m_goCanvas.SetActive (false);
		foreach (GameObject _tabs in m_goMenuTabs)
		{
			_tabs.SetActive (true);
		}
		gameObject.SetActive (false);

	}

	public void ActivateRoster (string _szLocationName)
	{
		m_goRoster.SetActive (true);
		AdjustRoster (_szLocationName);
	}

	public void DeactivateRoster()
	{
		m_goRoster.SetActive (false);
	}
}
