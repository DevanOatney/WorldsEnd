using UnityEngine;
using System.Collections;

public class BaseEventSystemScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	virtual public void SetWaypoints()
	{
	}
	virtual public void HandleEvent(string eventID)
	{
	}
	virtual public void WaypointTriggered(Collider2D c)
	{
	}
}
