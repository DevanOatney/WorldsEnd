using UnityEngine;
using System.Collections;

public class InonForestEventHandler : MonoBehaviour 
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
	void Update () {
	
	}
}
