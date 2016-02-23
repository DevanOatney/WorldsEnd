using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class InonForestEventHandler : BaseEventSystemScript 
{
	DCScript ds;
	public GameObject m_goBossBoar;
	public GameObject[] Phase1_waypoints;
	public GameObject[] Mushrooms;
	// Use this for initialization
	void Start () 
	{
		ds = GameObject.Find ("PersistantData").GetComponent<DCScript> ();
		int result;
		if(ds.m_dStoryFlagField.TryGetValue("ToAEvent", out result))
		{
			//hasn't been to the temple yet
			m_goBossBoar.SetActive(true);
			m_goBossBoar.GetComponent<Animator>().Play("BoarBoss_IdleLeft");
		}
		if (ds.m_dStoryFlagField.TryGetValue ("BoarBossPart1Finished", out result)) 
		{
			if(result == 1)
			{
				ds.m_dStoryFlagField.Remove("BoarBossPart1Finished");
				ds.m_dStoryFlagField.Add("BoarBossPart1Finished", 2);
				HandleEvent("BoarChase");
			}
			else
			{
				m_goBossBoar.SetActive(false);
			}
			foreach (GameObject go in Phase1_waypoints)
				go.SetActive (false);

		}
		if(ds.m_dStoryFlagField.TryGetValue("Inon_Lydia", out result))
		{
			if(result == 11)
			{
				foreach(GameObject mushroom in Mushrooms)
					mushroom.SetActive(true);
			}
		}

	}

	override public void HandleEvent(string eventID)
	{
		switch(eventID)
		{
		case "mushroom":
		{
			ds.m_lItemLibrary.AddItem("Rare Mushroom");
				//TODO: when you get all of the mushrooms maybe have some dialogue like "hey let's go return these to 'character'"
		}
			break;
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
			m_goBossBoar.SetActive(true);
			m_goBossBoar.GetComponent<NPCScript>().m_bActive = true;
			m_goBossBoar.GetComponent<NPCScript>().m_bIsMoving = true;
			m_goBossBoar.GetComponent<NPCScript>().m_nFacingDir = 2;
			m_goBossBoar.GetComponent<Animator>().SetBool("m_bIsMoving", true);
			m_goBossBoar.GetComponent<Animator>().Play("BoarBoss_ChargeRight");
			GameObject.Find("to Temple").GetComponent<BoxCollider2D>().enabled = true;
		}	
		break;
		}
	}


	void StartBossBattle()
	{
		GameObject dc = GameObject.Find("PersistantData");
		{
			List<EncounterGroupLoaderScript.cEnemyData> bossEncounter = new List<EncounterGroupLoaderScript.cEnemyData>();
			EncounterGroupLoaderScript.cEnemyData enemy = new EncounterGroupLoaderScript.cEnemyData();
			enemy.m_szEnemyName = "Boar Boss";
			enemy.m_nFormationIter = 4;
			bossEncounter.Add(enemy);
			
			//Set the names of the list of enemies the player is about to fight
			dc.GetComponent<DCScript>().SetEnemyNames(bossEncounter);
			//Set the position of the player before the battle starts
			GameObject go = GameObject.Find("PersistantData");
			GameObject m_goPlayer = GameObject.Find("Player");
			go.GetComponent<DCScript>().SetPreviousPosition(m_goPlayer.transform.position);
			go.GetComponent<DCScript>().SetPreviousFacingDirection(m_goPlayer.GetComponent<FieldPlayerMovementScript>().m_nFacingDir);
            go.GetComponent<DCScript>().SetPreviousFieldName(SceneManager.GetActiveScene().name);
			go.GetComponent<DCScript>().SetBattleFieldBackgroundIter(2);

            SceneManager.LoadScene("Battle_Scene");
		}
	}

	override public void WaypointTriggered(Collider2D c)
	{
		switch(c.name)
		{
		case "BriolWaypoint":
		{
			GameObject briol = GameObject.Find("Briol");
			NPCScript bNpc = briol.GetComponent<NPCScript>();
			bNpc.m_bIsMoving = false;
			bNpc.m_bActive = false;
			bNpc.m_nFacingDir = (int)NPCScript.FACINGDIR.eRIGHT;
			bNpc.ResetAnimFlagsExcept(-1);
			bNpc.m_aAnim.SetInteger("m_nFacingDir", (int)NPCScript.FACINGDIR.eRIGHT);
			GameObject.Find("BriolWaypoint").GetComponent<BoxCollider2D>().enabled = false;

			GameObject player = GameObject.Find("Player");
			player.GetComponent<MessageHandler>().BeginDialogue("A1");
		}
			break;
		case "BoarMove":
		{
			GameObject goBoar = GameObject.Find("BoarBoss");
			goBoar.SetActive(false);
			GameObject src = GameObject.Find("Player");
			FieldPlayerMovementScript fpmScript = src.GetComponent<FieldPlayerMovementScript>();
			fpmScript.BindInput();
			fpmScript.SetState((int)FieldPlayerMovementScript.States.eWALKRIGHT);
			fpmScript.GetAnimator().SetBool("m_bMoveRight", true);
			fpmScript.GetAnimator().SetBool("m_bRunButtonIsPressed", true);
			fpmScript.GetAnimator().SetInteger("m_nFacingDir", 2);
			fpmScript.SetIsRunning(true);
		}
			break;
		case "BoarChaseEndDialogue":
		{
			GameObject src = GameObject.Find("Player");
			src.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eRUNRIGHT);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveRight", true);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", true);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 2);
			src.GetComponent<FieldPlayerMovementScript>().SetIsRunning(false);
		}
			break;
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

			GameObject briol = GameObject.Find("Briol");
			briol.transform.position = player.transform.position;
			briol.GetComponent<SpriteRenderer>().enabled = true;
			briol.GetComponent<BoxCollider2D>().enabled = true;
			NPCScript bNpc = briol.GetComponent<NPCScript>();
			bNpc.m_bIsComingOutOfPlayer = true;
			bNpc.m_bIsMoving = true;
			bNpc.m_bActive = true;
			bNpc.m_nFacingDir = (int)NPCScript.FACINGDIR.eUP;
			bNpc.ResetAnimFlagsExcept(bNpc.m_nFacingDir);


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
