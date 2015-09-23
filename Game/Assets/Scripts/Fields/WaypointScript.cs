using UnityEngine;
using System.Collections;

public class WaypointScript : MonoBehaviour 
{
	public string m_szTarget = "Player";
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
			gameObject.SetActive(false);
		}
	}

}
