using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapTableScript : MonoBehaviour 
{
	public GameObject m_goWorldMap;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == "Action Box(Clone)")
		{
			
			m_goWorldMap.GetComponent<WorldMissionMapScript> ().ActivateMap (GameObject.Find ("PersistantData").GetComponent<DCScript> ().m_lFieldResourceLocationsFound);
		}
	}
}
