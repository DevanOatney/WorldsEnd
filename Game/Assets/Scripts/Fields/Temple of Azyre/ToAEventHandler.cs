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
			player.GetComponent<MessageHandler>().BeginDialogue(0);
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
		switch(eventID)
		{
		case "Callan_runoff":
			{
				GameObject src = GameObject.Find("Player");
			src.GetComponent<FieldPlayerMovementScript>().DHF_PlayerMoveToGameObject(GameObject.Find("XWaypoint"), false);
				//src.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKUP);
				//src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveUp", true);
				//src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", true);
				//src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 3);
				//src.GetComponent<FieldPlayerMovementScript>().SetIsRunning(true);
				GameObject.Find("XWaypoint").GetComponent<BoxCollider2D>().enabled = true;
				
			}
			break;
		case "AfterOpen":
			{	
				GameObject XWaypoint = GameObject.Find ("XWaypoint");
				XWaypoint.GetComponent<WaypointScript>().m_szTarget = "SzTarget";
				GameObject briol = GameObject.Find("Briol");
				NPCScript bNpc = briol.GetComponent<NPCScript>();
				bNpc.m_bReturnToPlayer = true;
				//bNpc.m_bIsMoving = true;
				//bNpc.m_bActive = true;
				//bNpc.m_nFacingDir = (int)NPCScript.FACINGDIR.eUP;
				//bNpc.ResetAnimFlagsExcept(bNpc.m_nFacingDir);
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
			FieldPlayerMovementScript fpmScript = src.GetComponent<FieldPlayerMovementScript>();
			fpmScript.SetState((int)FieldPlayerMovementScript.States.eIDLE);
			fpmScript.ResetAnimFlagsExcept(-1);
			fpmScript.GetAnimator().SetInteger("m_nFacingDir", 3);
			fpmScript.SetIsRunning(false);
			src.GetComponent<MessageHandler>().BeginDialogue("A6");
		}
			break;
		}
	}
}
