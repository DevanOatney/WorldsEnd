using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InonForestEventHandler : BaseEventSystemScript 
{
	DCScript ds;
	public GameObject m_goBossBoar;
	public GameObject[] Phase1_waypoints;
	// Use this for initialization
	void Start () 
	{
		ds = GameObject.Find ("PersistantData").GetComponent<DCScript> ();
		int result;
		if (ds.m_dStoryFlagField.TryGetValue ("BoarBossPart1Finished", out result)) 
		{
			if(result == 1)
			{
				ds.m_dStoryFlagField.Remove("BoarBossPart1Finished");
				ds.m_dStoryFlagField.Add("BoarBossPart1Finished", 2);
				HandleEvent("BoarChase");
			}
			foreach (GameObject go in Phase1_waypoints)
				go.SetActive (false);

		}
		else
		{
			//boar needs to be turned invisible/put off screen
			m_goBossBoar.GetComponent<SpriteRenderer>().enabled = false;
		}

	}

	override public void HandleEvent(string eventID)
	{
		switch(eventID)
		{
		case "BoarBossEndDialogue":
			{
				ds.m_dStoryFlagField.Add("BoarBossPart1Finished", 1);
				Camera.main.GetComponent<CameraFollowTarget>().m_bShouldSwirl = true;
				Camera.main.GetComponent<VEffects>().SendMessage("StartBlur");
			StartBossBattle();
			}
		break;
		case "BoarChase":
		{
			GameObject goBoar = GameObject.Find("BoarBoss");
			goBoar.GetComponent<NPCScript>().m_bActive = true;
			goBoar.GetComponent<NPCScript>().m_bIsMoving = true;
			goBoar.GetComponent<NPCScript>().m_nFacingDir = 2;
			GameObject src = GameObject.Find("Player");
			src.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKRIGHT);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveRight", true);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", true);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 2);
			src.GetComponent<FieldPlayerMovementScript>().SetIsRunning(true);
			GameObject.Find("to Temple").GetComponent<BoxCollider2D>().enabled = true;
		}	
		break;
		}
	}


	void StartBossBattle()
	{
		GameObject dc = GameObject.Find("PersistantData");
		{
			List<string> bossEncounter = new List<string>();
			bossEncounter.Add("Boar Boss");
			
			//Set the names of the list of enemies the player is about to fight
			dc.GetComponent<DCScript>().SetEnemyNames(bossEncounter);
			//Set the position of the player before the battle starts
			GameObject go = GameObject.Find("PersistantData");
			GameObject m_goPlayer = GameObject.Find("Player");
			go.GetComponent<DCScript>().SetPreviousPosition(m_goPlayer.transform.position);
			go.GetComponent<DCScript>().SetPreviousFacingDirection(m_goPlayer.GetComponent<FieldPlayerMovementScript>().m_nFacingDir);
			go.GetComponent<DCScript>().SetPreviousFieldName(Application.loadedLevelName);
			go.GetComponent<DCScript>().SetBattleFieldBackgroundIter(2);

			Application.LoadLevel("Battle_Scene");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}


	override public void WaypointTriggered(Collider2D c)
	{
		switch(c.name)
		{
		case "Forest start scene":
			c.enabled = false;
			ds.m_dStoryFlagField.Add("BoarBossPart1Finished", 1);
			HandleEvent("ENCOUNTERED_BOAR");
			break;
		case "YWaypoint":
		{
			GameObject player = GameObject.Find("Player");
			player.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 2);
			player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eIDLE);
			player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
			GameObject.Find("Player").GetComponent<MessageHandler>().BeginDialogue("A1");
			GameObject.Find("YWaypoint").GetComponent<BoxCollider2D>().enabled = false;
		}
			break;
		case "XWaypoint":
		{
			GameObject src = GameObject.Find("Player");
			src.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKUP);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveUp", true);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", true);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 3);
			src.GetComponent<FieldPlayerMovementScript>().SetIsRunning(true);
			GameObject.Find("XWaypoint").GetComponent<BoxCollider2D>().enabled = false;
			GameObject.Find("YWaypoint").GetComponent<BoxCollider2D>().enabled = true;
		}
			break;
		case "Forest scene start":
		{
			
			GameObject.Find("Forest scene start").GetComponent<BoxCollider2D>().enabled = false;
			GameObject dest = GameObject.Find("XWaypoint");
			GameObject src = GameObject.Find("Player");
			src.GetComponent<FieldPlayerMovementScript>().BindInput();
			Vector2 dir = dest.transform.position - src.transform.position;
			if(dir.x > 0)
			{
				src.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKRIGHT);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveRight", true);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", true);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 2);
				src.GetComponent<FieldPlayerMovementScript>().SetIsRunning(true);
				GameObject.Find("XWaypoint").GetComponent<BoxCollider2D>().enabled = true;
			}
			else
			{
				src.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKUP);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveUp", true);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", true);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 3);
				src.GetComponent<FieldPlayerMovementScript>().SetIsRunning(true);
				GameObject.Find("YWaypoint").GetComponent<BoxCollider2D>().enabled = true;
			}
		}
			break;
		default:
			break;
		}
	}
}
