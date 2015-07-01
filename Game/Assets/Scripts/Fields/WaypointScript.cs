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
			GameObject.Find("Event System").GetComponent<BaseEventSystemScript>().WaypointTriggered(GetComponent<BoxCollider2D>());
		}
	}

}
