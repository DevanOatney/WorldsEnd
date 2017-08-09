using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInWorldMapRosterScript : MonoBehaviour
{
	string m_szLocationName;
	GameObject m_goParent;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void Initialize(string _szLocation, GameObject _parent)
	{
		m_szLocationName = _szLocation;
		m_goParent = _parent;
	}

	public void OnClick()
	{
		//Send this unit to gather resources
		string _result = "";
		DCScript dc = GameObject.Find ("PersistantData").GetComponent<DCScript> ();
		if (dc.m_dUnitsGatheringResources.TryGetValue (m_szLocationName, out _result))
		{
			dc.m_dUnitsGatheringResources.Remove (m_szLocationName);
			foreach (Transform _go in transform.parent)
			{
				Transform _child = _go.Find ("CharacterName");
				if(_child != null)
				{
					string _childsName = _child.GetComponent<Text> ().text;
					if (_childsName == _result)
					{
						_go.GetComponent<Image> ().color = Color.grey;
					}
				}
			}
		}
		dc.m_dUnitsGatheringResources.Add (m_szLocationName, transform.Find ("CharacterName").GetComponent<Text> ().text);
		gameObject.GetComponent<Image> ().color = Color.black;
		m_goParent.GetComponent<WorldMissionMapScript> ().DeactivateRoster ();
	}
}
