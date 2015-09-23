using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToAEventHandler : BaseEventSystemScript 
{
	public GameObject[] Phase1_waypoints;
	DCScript ds;

	// Use this for initialization
	void Start()
	{
		ds = GameObject.Find("PersistantData").GetComponent<DCScript>();
		int result;
		if(ds.m_dStoryFlagField.TryGetValue("AfterOpening", out result) == false)
		{
			//This is the introduction, play it yo!
			GameObject player = GameObject.Find("Player");
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
			player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
			player.GetComponent<MessageHandler>().BeginDialogue("A0");
			ds.m_dStoryFlagField.Add("AfterOpening", 1);
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			GameObject Briol = GameObject.Find("Briol");
			Camera.main.GetComponent<CameraFollowTarget>().m_goTarget = Briol;
		}
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	override public void HandleEvent(string eventID)
	{
		switch (eventID) {
		case "Callan_runoff":
			{
				GameObject src = GameObject.Find ("Player");
				src.GetComponent<FieldPlayerMovementScript> ().DHF_PlayerMoveToGameObject (GameObject.Find ("XWaypoint"), true);
				GameObject.Find ("XWaypoint").GetComponent<BoxCollider2D> ().enabled = true;
				src.GetComponent<MessageHandler> ().BeginDialogue ("A6");
			}
			break;
		case "AfterOpen":
			{	
				GameObject XWaypoint = GameObject.Find ("XWaypoint");
				XWaypoint.GetComponent<WaypointScript> ().m_szTarget = "SzTarget";
				GameObject briol = GameObject.Find ("Briol");
				NPCScript bNpc = briol.GetComponent<NPCScript> ();
				bNpc.DHF_NPCMoveIntoPlayer ();
			}
			break;
		
		case "Lion Statue":
			{
				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				if(player)
				{
					player.GetComponent<FieldPlayerMovementScript> ().BindInput ();
				}
				int lionRes = -1;
				if (ds.m_dStoryFlagField.TryGetValue ("Lion_Statue", out lionRes) == false) {
					GameObject briol = GameObject.Find ("Briol");
					briol.transform.position = player.transform.position;
					briol.GetComponent<SpriteRenderer> ().enabled = true;
					briol.GetComponent<BoxCollider2D> ().enabled = true;
					NPCScript bNpc = briol.GetComponent<NPCScript> ();
					bNpc.m_bIsComingOutOfPlayer = true;
					bNpc.m_bIsMoving = true;
					bNpc.m_bActive = true;
					bNpc.m_nFacingDir = (int)NPCScript.FACINGDIR.eUP;
					bNpc.ResetAnimFlagsExcept (bNpc.m_nFacingDir);
					GameObject messageSystem = GameObject.Find ("Briol");
					messageSystem.GetComponent<MessageHandler> ().BeginDialogue ("D0");
				} else {
					GameObject messageSystem = GameObject.Find ("Lion Statue");
					messageSystem.GetComponent<MessageHandler> ().BeginDialogue ("A0");
				}
			}
			break;
		case "Temple_Alter_1":
			{
				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				if (player) {
					player.GetComponent<FieldPlayerMovementScript> ().ReleaseBind ();
				}
				ds.m_dStoryFlagField.Add ("Lion_Statue", 1);
				GameObject.Find ("Briol").GetComponent<NPCScript> ().DHF_NPCMoveIntoPlayer ();
				GameObject.Find ("Briol").GetComponent<BoxCollider2D> ().enabled = false;
				Invoke ("ReleasePlayer", 2.0f);
			}
			break;
		case "EndDialogue":
			{
				//turn off all dialogues happening, release bind on input.. umn.. i think that's it?
				GameObject[] gObjs = GameObject.FindObjectsOfType<GameObject> ();
				foreach (GameObject g in gObjs) {
					if (g.GetComponent<MessageHandler> () != null) {
						if (g.GetComponent<NPCScript> () != null)
							g.GetComponent<NPCScript> ().m_bIsBeingInterractedWith = false;
						g.GetComponent<MessageHandler> ().m_bShouldDisplayDialogue = false;
					}
				}
				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				if (player) {
					player.GetComponent<FieldPlayerMovementScript> ().ReleaseBind ();
				}
			}
			break;
		}
	}


	override public void WaypointTriggered(Collider2D c)
	{
		switch(c.name)
		{
		case "XWaypoint":
		{
			GameObject src = GameObject.Find("Player");
			src.GetComponent<MessageHandler>().BeginDialogue("A6");
		}
			break;
		}
	}
}
