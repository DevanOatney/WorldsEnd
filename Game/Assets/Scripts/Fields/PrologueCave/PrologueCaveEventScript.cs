using UnityEngine;
using System.Collections;

public class PrologueCaveEventScript : BaseEventSystemScript 
{

	DCScript ds;
	
	public GameObject[] Phase1_waypoints;
	public GameObject[] Phase2_waypoints;
	public GameObject[] Phase3_waypoints;


	int SwitchCounter = 7;
	// Use this for initialization
	void Start () 
	{
		ds = GameObject.Find("PersistantData").GetComponent<DCScript>();
		int result;
		if(ds.m_dStoryFlagField.TryGetValue("PrologueCave_CheckedAllSpots", out result))
		{
			foreach(GameObject wypnt in Phase1_waypoints)
				wypnt.GetComponent<BoxCollider>().enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	override public void HandleEvent(string eventID)
	{
		switch(eventID)
		{
		default:
			break;
		}
	}

	override public void WaypointTriggered(Collider2D c)
	{
		switch(c.name)
		{
		case "FurtherIntoCave":
		{
			//The player needs to stay in this room, disable input and have him begin walking downwards, will get caught by child waypoint to release input and stop movement.
			GameObject player = GameObject.Find("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
				player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKDOWN);
				player.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", false);
				GameObject.Find("StepBack").GetComponent<BoxCollider>().enabled = true;
				//TODO: Dialogue explaining that the player needs to investigate this room further
			}
		}
			break;
		case "StepBack":
		{
			//player has stepped back from going deeper into the cave, release the bind on input, disable collision box, umm.. change state to idle.
			GameObject player = GameObject.Find("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
				player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eIDLE);
				GameObject.Find("StepBack").GetComponent<BoxCollider>().enabled = false;
			}
		}
			break;
		default:
			break;
		}
	}

	public void ReduceCounter()
	{
		SwitchCounter--;
		if(SwitchCounter <= 0)
		{
			ds.m_dStoryFlagField.Add("PrologueCave_CheckedAllSpots", 1);
			foreach(GameObject wypnt in Phase1_waypoints)
				wypnt.GetComponent<BoxCollider>().enabled = false;
		}
	}
}
