using UnityEngine;
using System.Collections;

public class WaypointScript : MonoBehaviour 
{
	public string m_szTarget = "Player";
	public bool m_bShouldReset = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == m_szTarget)
		{
			GameObject.Find("Event system").GetComponent<BaseEventSystemScript>().WaypointTriggered(GetComponent<BoxCollider2D>());
			GetComponent<BoxCollider2D>().enabled = false;
			if (m_bShouldReset == true)
				Invoke ("DelayedReset", 1.0f);
		}
	}

	void DelayedReset()
	{
		GetComponent<BoxCollider2D>().enabled = true;
	}

}
