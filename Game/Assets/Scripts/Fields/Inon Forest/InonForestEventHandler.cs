using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class InonForestEventHandler : BaseEventSystemScript 
{
	DCScript ds;
	public GameObject m_goBossBoar;
	public GameObject[] Phase1_waypoints;
	public GameObject[] Phase2_waypoints;
	public GameObject[] Mushrooms;
	string m_szInonForestBGM = "InonForest_BGM";
	public GameObject m_goResourceHarvestLocation;
	// Use this for initialization
	void Start () 
	{
		GameObject _goAudioHelper = GameObject.Find("AudioHelper");
		_goAudioHelper.GetComponent<CAudioHelper>().vPlayMusic(_goAudioHelper.GetComponent<CAudioHelper>().eFromName(m_szInonForestBGM),true, true);
		GameObject.Find("Briol").GetComponent<SpriteRenderer>().enabled = false;
		ds = GameObject.Find ("PersistantData").GetComponent<DCScript> ();
		ds.AddPartyMember ("Briol");

		SetWaypoints ();
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.H)) {
			GameObject.Find ("Player").GetComponentInChildren<MessageHandler> ().BeginDialogue ("This is a test", "Callan", 2);
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
				Camera.main.GetComponent<CameraFollowTarget> ().StartSwirlAndBlur ();
				Invoke ("StartBossBattle", 1.0f);
			}
		break;
		case "BoarChase":
		{
			m_goBossBoar.SetActive(true);
			m_goBossBoar.GetComponent<NPCScript>().DHF_NPCMoveToGameobject(GameObject.Find("BoarMove"),true);
			m_goBossBoar.GetComponent<Animator>().SetBool("m_bIsMoving", true);
			
			GameObject.Find("to Temple").GetComponent<BoxCollider2D>().enabled = true;
		}	
		break;
		case "EndDialogue":
			{
				//turn off all dialogues happening, release bind on input.. umn.. i think that's it?
				GameObject[] gObjs = GameObject.FindObjectsOfType<GameObject>();
				foreach(GameObject g in gObjs)
				{
					if(g.GetComponentInChildren<MessageHandler>() != null)
					{
						if(g.GetComponent<NPCScript>() != null)
							g.GetComponent<NPCScript>().m_bIsBeingInterractedWith = false;
						g.GetComponentInChildren<MessageHandler>().m_bShouldDisplayDialogue = false;
					}
				}
				GameObject player = GameObject.FindGameObjectWithTag("Player");
				if(player)
				{
					player.GetComponent<FieldPlayerMovementScript>().ReleaseAllBinds();
				}
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
			GameObject.Find("BriolWaypoint").GetComponent<BoxCollider2D>().enabled = false;
			GameObject player = GameObject.Find("Player");
			player.GetComponentInChildren<MessageHandler>().BeginDialogue("A1");
		}
			break;
		case "BoarMove":
		{
			GameObject goBoar = GameObject.Find("BoarBoss");
			goBoar.SetActive(false);
			GameObject src = GameObject.Find("Player");
			FieldPlayerMovementScript fpmScript = src.GetComponent<FieldPlayerMovementScript>();
			fpmScript.DHF_PlayerMoveToGameObject(GameObject.Find("BoarMove"),false);
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
			NPCScript bNpc = briol.GetComponent<NPCScript>();
			bNpc.DHF_NPCMoveToGameobject(GameObject.Find("BriolWaypoint"),false, 2, true);


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
			//prevents the player from leaving InonForest 
		case "ToWorldMapCheck":
			{
				GameObject src = GameObject.Find("Player");
				src.GetComponent<FieldPlayerMovementScript>().BindInput();
				src.GetComponent<FieldPlayerMovementScript>().DHF_PlayerMoveToGameObject(GameObject.Find("ToWorldMapBackup"), false);
				GameObject.Find("ToWorldMapBackup").GetComponent<BoxCollider2D>().enabled = true;
			}
			break;
			//catches the player after they try to leave the forest, says some dialogue, then returns control to the player
		case "ToWorldMapBackup":
			{
				GameObject src = GameObject.Find("Player");
				src.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eIDLE);
				src.GetComponentInChildren<MessageHandler>().BeginDialogue("A7");
				GameObject.Find("ToWorldMapBackup").GetComponent<BoxCollider2D>().enabled = false;
			}
			break;
		default:
			break;
		}
	}

	override public void SetWaypoints()
	{
		int result;
		if(ds.m_dStoryFlagField.TryGetValue("ToAEvent", out result) == false)
		{
			//hasn't been to the temple yet
			m_goBossBoar.SetActive(true);
			m_goBossBoar.GetComponent<Animator>().Play("BoarBoss_IdleLeft");
			foreach (GameObject wypnt in Phase1_waypoints)
				wypnt.SetActive (true);

		}
		else if(result >= 5)
		{
			//Player has finished the Temple of Azyre initial events and can now leave to the world map from the north of the forest
			foreach(GameObject wypnt in Phase2_waypoints)
			{
				wypnt.SetActive(false);
			}

			//At this point it's also possible for there to be a recruited character at this harvest location- so let's find out.
			foreach (string _location in ds.m_lFieldResourceLocationsFound)
			{
				if (_location == "Inon Forest")
				{
					//this is a possible location-
					string _resourceResult;
					if (ds.m_dUnitsGatheringResources.TryGetValue ("Inon Forest", out _resourceResult))
					{
						//This location is currently being used, spawn whichever unit is supposed to be here at the resource location!
						GameObject _unit = Resources.Load<GameObject>("Units/Ally/" + _resourceResult + "/Field/" + _resourceResult);
						_unit = Instantiate(_unit, m_goResourceHarvestLocation.transform.position, Quaternion.identity);
					}
				}
			}

		}
		if (ds.m_dStoryFlagField.TryGetValue ("BoarBossPart1Finished", out result)) 
		{
			if(result == 1)
			{
				GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
				ds.m_dStoryFlagField.Remove("BoarBossPart1Finished");
				ds.m_dStoryFlagField.Add("BoarBossPart1Finished", 2);
				HandleEvent("BoarChase");
			}
			else
			{
				m_goBossBoar.SetActive(false);
			}
			foreach (GameObject go in Phase1_waypoints)
				if(go)
					go.SetActive (false);

		}
		if(ds.m_dStoryFlagField.TryGetValue("Inon_Lydia", out result))
		{
			//If we're on the part of the side-quest with Lydia in Inon to collect Rare Mushrooms, set the mushrooms as active.
			if(result == 11)
			{
				foreach(GameObject mushroom in Mushrooms)
					mushroom.SetActive(true);
			}
		}
	}
}
