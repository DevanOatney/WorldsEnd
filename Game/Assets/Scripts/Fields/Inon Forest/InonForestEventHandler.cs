using UnityEngine;
using System.Collections;

public class InonForestEventHandler : BaseEventSystemScript 
{
	DCScript ds;
	public GameObject[] Phase1_waypoints;
	// Use this for initialization
	void Start () 
	{
		ds = GameObject.Find("PersistantData").GetComponent<DCScript>();
		int result;
		if(ds.m_dStoryFlagField.TryGetValue("InonForest_EncounteredBoar", out result))
		{
			foreach(GameObject go in Phase1_waypoints)
				go.SetActive(false);
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	override public void HandleEvent(string eventID)
	{
		switch(eventID)
		{

		}
	}

	override public void WaypointTriggered(Collider2D c)
	{
		switch(c.name)
		{
		case "EncounterBoarWaypoint":
			c.enabled = false;
			ds.m_dStoryFlagField.Add("InonForest_EncounteredBoar", 1);
			HandleEvent("ENCOUNTERED_BOAR");
			break;
		default:
			break;
		}
	}
}
