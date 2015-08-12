using UnityEngine;
using System.Collections;

public class WaypointScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == "Player")
		{
			Debug.Log("something");
			GameObject.Find("Event system").GetComponent<BaseEventSystemScript>().WaypointTriggered(GetComponent<BoxCollider2D>());
		}
	}

}
