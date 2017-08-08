using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInWorldMapRosterScript : MonoBehaviour
{
	string m_szLocationName;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void Initialize(string _szLocation)
	{
		m_szLocationName = _szLocation;
	}

	public void OnClick()
	{
		//Send this unit to gather resources
		string _result = "";
		DCScript dc = GameObject.Find ("PersistantData").GetComponent<DCScript> ();
		if (dc.m_dUnitsGatheringResources.TryGetValue (m_szLocationName, out _result))
		{
			dc.m_dUnitsGatheringResources.Remove (m_szLocationName);
		}
		dc.m_dUnitsGatheringResources.Add (m_szLocationName, transform.Find ("CharacterName").GetComponent<Text> ().text);
	}
}
