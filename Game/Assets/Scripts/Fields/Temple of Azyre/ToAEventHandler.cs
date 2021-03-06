﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ToAEventHandler : BaseEventSystemScript 
{
	public GameObject[] Phase1_waypoints;
	public GameObject m_goBoar;
	public GameObject m_goBoarBoss;
	public GameObject[] m_goBlackSmithShop;
	public GameObject m_RecruitPositionsRoot;
	public GameObject m_WorldMapTable;
	DCScript ds;
	public List<GameObject[]> m_lEvents;
	string m_szToABGM = "TempleOfAzyre_BGM";
	// Use this for initialization
	void Start()
	{
		GameObject _goAudioHelper = GameObject.Find("AudioHelper");
		_goAudioHelper.GetComponent<CAudioHelper>().vPlayMusic(_goAudioHelper.GetComponent<CAudioHelper>().eFromName(m_szToABGM),true, true);
		ds = GameObject.Find("PersistantData").GetComponent<DCScript>();
		SetWaypoints();

		GameObject player = GameObject.Find("Player");
		GameObject briol = GameObject.Find("Briol");
		Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), briol.GetComponent<BoxCollider2D>());
		AdjustBuildings ();
	}

	override public void AdjustBuildings()
	{
		int result = 0;
		ds.m_dBaseFlagField.TryGetValue ("BlacksmithLevel", out result);
		foreach (GameObject blacksmithLayer in m_goBlackSmithShop)
		{
			if (blacksmithLayer.GetComponent<BaseLayerLevelScript> ().m_nLevel <= result)
			{
				blacksmithLayer.SetActive (true);
			}
			else
			{
				blacksmithLayer.SetActive (false);
			}
		}
	}

	//putting this in it's own function so that when you cheat it can update it there without you having to re-start that scene.
	override public void SetWaypoints()
	{
		GameObject[] wypnts = GameObject.FindGameObjectsWithTag("Waypoint");
		foreach(GameObject wypnt in wypnts)
			wypnt.GetComponent<BoxCollider2D>().enabled = false;
		int result;
		if(ds.m_dStoryFlagField.TryGetValue("ToAEvent", out result) == false)
		{
			//This is the introduction, play it yo!
			GameObject.Find("OverWatcher").GetComponent<EncounterGroupLoaderScript>().m_bEncountersHappen = true;
			GameObject player = GameObject.Find("Player");
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
			player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
			player.GetComponentInChildren<MessageHandler>().BeginDialogue("A0");
			ds.m_dStoryFlagField.Add("ToAEvent", 0);
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			GameObject Briol = GameObject.Find("Briol");
			Briol.GetComponent<SpriteRenderer>().enabled = true;
			Briol.GetComponent<BoxCollider2D>().enabled = true;
			Camera.main.GetComponent<CameraFollowTarget>().m_goTarget = Briol;
		}
		else if (ds.m_dStoryFlagField.TryGetValue("ToAEvent", out result))
		{
			switch(result)
			{
			case 0:
			{
				//The player has gone through the opening event of this scene
				GameObject.Find("OverWatcher").GetComponent<EncounterGroupLoaderScript>().m_bEncountersHappen = true;
				GameObject.Find("Interior").GetComponent<BoxCollider2D>().enabled = true;
				m_goBoar.SetActive (true);
			}
				break;
			case 1:
			{
				//The player has gone into the entrance of the Temple for the first time
				GameObject.Find("OverWatcher").GetComponent<EncounterGroupLoaderScript>().m_bEncountersHappen = true;
				GameObject.Find("EncounterBoarBeforeRunOff").GetComponent<BoxCollider2D>().enabled = true;
				m_goBoar.SetActive (false);
			}
				break;
			case 2:
			{
				//The player has seen the boar run down into the basement
				GameObject.Find("OverWatcher").GetComponent<EncounterGroupLoaderScript>().m_bEncountersHappen = true;
				GameObject.Find("EncounterThePit").GetComponent<BoxCollider2D>().enabled = true;
			}
				break;
			case 3:
			{
				//The player has seen the pit
				GameObject.Find("OverWatcher").GetComponent<EncounterGroupLoaderScript>().m_bEncountersHappen = true;
				GameObject.Find("EncounterBoarBoss_Start").GetComponent<BoxCollider2D>().enabled = true;
				m_goBoarBoss.SetActive (true);
			}
				break;
			case 4:
			{
				//Player just returned from fighting the boar boss
				GameObject.Find("OverWatcher").GetComponent<EncounterGroupLoaderScript>().m_bEncountersHappen = false;
				HandleEvent("ReturnedFromBoarBossFight");
				ds.m_dStoryFlagField.Remove("ToAEvent");
				ds.m_dStoryFlagField.Add("ToAEvent", 5);
			}
				break;
			case 5:
			{
				//Temple was cleared out and a choice was made over Knight/Ranger
				GameObject rubble1 = GameObject.Find("Temple of Azyre int").transform.Find("rubble_04").gameObject;
				rubble1.SetActive(false);
				GameObject rubble2 = GameObject.Find("Temple of Azyre int2").transform.Find("rubble_04").gameObject;
				rubble2.SetActive(false);
				GameObject rubble3 = GameObject.Find("Temple of Azyre roof").transform.Find("rubble_04").gameObject;
				rubble3.SetActive(false);
				GameObject rubble4 = GameObject.Find("Temple of Azyre basment").transform.Find("rubble_03").gameObject;
				rubble4.SetActive(false);
				GameObject.Find("OverWatcher").GetComponent<EncounterGroupLoaderScript>().m_bEncountersHappen = false;
				m_goBoar.SetActive (false);
				m_goBoarBoss.SetActive (false);
			}
				break;
			}
			if (result >= 5)
			{
				//So passed this point , we should start doing some default things, like populating the base with recruits (and later some random npcs)
				List<DCScript.CharacterData> _roster = ds.GetRoster();
				foreach (DCScript.CharacterData c in _roster)
				{
					if (c.m_bHasBeenRecruited == true && c.m_bIsInParty == false)
					{
						//This character has been recruited, and is not in the party. Unless under some special circumstance, they SHOULD show up in the base.
						foreach (Transform child in m_RecruitPositionsRoot.transform)
						{
							if (child.gameObject.name == c.m_szCharacterName)
							{
								
								//All right, found the character, time to create one of them and put them in this position
								GameObject _unit = Resources.Load<GameObject>("Units/Ally/" + c.m_szCharacterName + "/Field/" + c.m_szCharacterName);
								_unit = Instantiate(_unit, child.position, Quaternion.identity);

							}
						}
					}
				}


				//So not sure when we do want this to actually activate, but for now let's give them the world map mission table once they unlock the base.
				m_WorldMapTable.SetActive(true);
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{

	}

	override public void HandleEvent(string eventID)
	{
		switch (eventID) 
		{
		case "Callan_runoff":
			{
				//Callan runs forward to bridge then stops and waits for Briol
				GameObject src = GameObject.Find ("Player");
				src.GetComponent<FieldPlayerMovementScript> ().DHF_PlayerMoveToGameObject (GameObject.Find ("StopAtBridge"), true);
				GameObject.Find ("StopAtBridge").GetComponent<BoxCollider2D> ().enabled = true;
			}
			break;
		case "Temple Bed":
			{
				//Callan has interacted with the bed, give him the option to rest.
				GameObject messageSystem = GameObject.Find ("CharacterTempleBed");
				if (messageSystem) {
					messageSystem.GetComponentInChildren<MessageHandler> ().BeginDialogue (0);
				}
			}
			
			break;
		
		case "BoarBattle":
			{

				//Callan begins to run upward after entering the temple for the first time.
				GameObject src = GameObject.Find ("Player");
				src.GetComponent<FieldPlayerMovementScript> ().DHF_PlayerMoveToGameObject (GameObject.Find ("PlayerStop_EncounterBoar"), true, 1);
				GameObject.Find ("PlayerStop_EncounterBoar").GetComponent<BoxCollider2D> ().enabled = true;
			}
			break;
		case "Pan_to_Boar_Azyre":
		{
			//Pan the camera to the boar and have the boar start running toward the stairs
			Camera.main.GetComponent<CameraFollowTarget>().m_goNextTarget = m_goBoar;
			m_goBoar.SetActive(true);
			m_goBoar.transform.position = GameObject.Find("BoarRunOffStartPosition").transform.position;
			GameObject.Find ("EncounterBoarDestination").GetComponent<BoxCollider2D>().enabled = true;
			m_goBoar.GetComponent<NPCScript>().DHF_NPCMoveToGameobject(GameObject.Find ("EncounterBoarDestination"), false, 2, false);
		}
			break;
		case "EndSeeBoar":
		{
			//move briol back to callan
			GameObject.Find("Briol").GetComponent<NPCScript>().DHF_NPCMoveIntoPlayer();
			GameObject.Find("EncounterThePit").GetComponent<BoxCollider2D>().enabled = true;
		}
			break;
		case "AfterBossBattle":
			{
				//messageSystem.GetComponent<MessageHandler> ().BeginDialogue ("C5");
			}
			break;
		case "Lion Statue":
			{
				GameObject messageSystem = GameObject.Find ("Lion Statue");
				if (messageSystem) {
					messageSystem.GetComponentInChildren<MessageHandler> ().BeginDialogue (0);
				}
			}
			break;
		case "InnKeeper_Sleep":
			{
				//Player has selected to sleep.
				GameObject[] keepers = GameObject.FindGameObjectsWithTag ("InnKeeper");
				foreach (GameObject keeper in keepers) 
				{
					if (keeper.GetComponent<NPCScript> ().m_bIsBeingInterractedWith == true) 
					{
						//found the game object that is the innkeeper, check if the player can afford it, if not.. ?   if you can, go to sleep after deducting the cost
						if (ds.m_nGold - keeper.GetComponent<NPCScript> ().m_nCost >= 0) 
						{
							ds.m_nGold = ds.m_nGold - keeper.GetComponent<NPCScript> ().m_nCost;
							GameObject.Find ("Inn Keeper").GetComponent<InnKeeperScript> ().BeginFade ();
						} 
						else 
						{
							HandleEvent ("EndDialogue");
						}
					}
				}
			
			}
			break;
		case "AfterOpen":
			{	
				GameObject briol = GameObject.Find ("Briol");
				NPCScript bNpc = briol.GetComponent<NPCScript> ();
				bNpc.DHF_NPCMoveIntoPlayer ();
			}
			break;
		case "PitSeen":
		{
			//The pit in the basement scene is over, have briol return to the character
			GameObject.Find("Briol").GetComponent<NPCScript>().DHF_NPCMoveIntoPlayer();
			GameObject.Find("EncounterBoarBoss_Start").GetComponent<BoxCollider2D>().enabled = true;
		}
			break;
		case "Boar_Boss":
			{
				//Caracters charge to waypoint "BoarBoss" and Battle Starts
				ds.m_dStoryFlagField.Remove ("ToAEvent");
				ds.m_dStoryFlagField.Add ("ToAEvent", 4);
				GameObject player = GameObject.Find ("Player");
				GameObject briol = GameObject.Find ("Briol");
				GameObject.Find("BoarBossChargePoint_Callan").GetComponent<BoxCollider2D>().enabled = true;
				GameObject.Find("BoarBossChargePoint_Briol").GetComponent<BoxCollider2D>().enabled = true;
				player.GetComponent<FieldPlayerMovementScript> ().DHF_PlayerMoveToGameObject (GameObject.Find ("BoarBossChargePoint_Callan"), true);
				briol.GetComponent<NPCScript> ().DHF_NPCMoveToGameobject (GameObject.Find ("BoarBossChargePoint_Briol"), true, 3, false);
				m_goBoarBoss.SetActive(true);
			}
			break;

		case "AfterBoar":
			{

				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				if (player) 
				{
					player.GetComponent<FieldPlayerMovementScript> ().BindInput ();
				}
				m_goBoar.SetActive (false);
				GameObject briol = GameObject.Find ("Briol");
				briol.GetComponent<SpriteRenderer> ().enabled = false;
				briol.GetComponentInChildren<MessageHandler> ().BeginDialogue ("A7");
			}
			break;
		case "ReturnedFromBoarBossFight":
		{
			//The player just returned from the boar boss fight Briol/Player face each other and dialogue starts
			GameObject player = GameObject.Find("Player");
			GameObject briol = GameObject.Find("Briol");
			player.GetComponent<FieldPlayerMovementScript>().BindInput();

			player.transform.position = GameObject.Find("BoarBossChargePoint_Callan").transform.position;
			briol.transform.position = GameObject.Find("BoarBossChargePoint_Briol").transform.position;
			player.GetComponent<Animator>().SetFloat("m_nFacingDir", 2);
			briol.GetComponent<SpriteRenderer>().enabled = true;
			briol.GetComponent<Animator>().SetFloat("m_nFacingDir", 1);
			m_goBoarBoss.SetActive (true);
			m_goBoarBoss.GetComponentInChildren<Animator> ().Play ("BoarBoss_Dead");
			briol.GetComponentInChildren<MessageHandler>().BeginDialogue("C5");

		}
			break;
		case "End_Boar_Boss_Battle":
		{
			/*
   				Start Callan_Temple_of_Azyre dialogue at B0
   				Hunter flag is added if Hunter path is chosen or Knight flag is added if Knight path is chosen
   				Characters form one unit and scene ends
			 */
			GameObject player = GameObject.Find("Player");
			GameObject briol = GameObject.Find("Briol");
			player.transform.position = GameObject.Find("BoarBossChargePoint_Callan").transform.position;
			briol.transform.position = GameObject.Find("BoarBossChargePoint_Briol").transform.position;
			player.GetComponent<Animator>().SetInteger("m_nFacingDir", 2);
			briol.GetComponent<Animator>().SetInteger("m_nFacingDir", 1);
			Camera.main.SendMessage("fadeOut");
			GameObject rubble1 = GameObject.Find("Temple of Azyre int").transform.Find("rubble_04").gameObject;
			rubble1.SetActive(false);
			GameObject rubble2 = GameObject.Find("Temple of Azyre int2").transform.Find("rubble_04").gameObject;
			rubble2.SetActive(false);
			GameObject rubble3 = GameObject.Find("Temple of Azyre roof").transform.Find("rubble_04").gameObject;
			rubble3.SetActive(false);
			GameObject rubble4 = GameObject.Find("Temple of Azyre basment").transform.Find("rubble_03").gameObject;
			rubble4.SetActive(false);
			Invoke("FadeBackIn", 1.0f);
		}
			break;

		case "End_Temple_Clean_Dialogue1":
			{
				//The hunter has been chosen as the first recruit!
				ds.m_dStoryFlagField.Add("FirstRecruit", 0);
				GameObject briol = GameObject.Find("Briol");
				briol.GetComponent<NPCScript> ().DHF_NPCMoveIntoPlayer ();
			}
			break;
		case "End_Temple_Clean_Dialogue2":
			{
				ds.m_dStoryFlagField.Add("FirstRecruit", 1);
				GameObject briol = GameObject.Find("Briol");
				briol.GetComponent<NPCScript> ().DHF_NPCMoveIntoPlayer ();
			}
			break;
			default:
				{
					base.HandleEvent (eventID);
				}
				break;
		}
	}


	override public void WaypointTriggered(Collider2D c)
	{
		c.enabled = false;
		switch(c.name)
		{
		case "StopAtBridge":
		{
			//Player has gotten to the bridge in the beginning.  Turn off the collider and have Briol begin to move to him
			GameObject src = GameObject.Find("Player");
			src.GetComponentInChildren<MessageHandler>().BeginDialogue("A6");
			GameObject.Find("Interior").GetComponent<BoxCollider2D>().enabled = true;
		}
			break;
		case "BoarStop_EncounterBoar":
		{
			//The boar has charged at the player after entering the temple for the first time, blur the camera and begin battle!
			GameObject.Find ("Boar").GetComponent<Animator>().SetBool("m_bAttack", true);
			Camera.main.GetComponent<CameraFollowTarget>().m_bShouldSwirl = true;
			Camera.main.GetComponent<VEffects>().SendMessage("StartBlur");
			StartBoarBattle();
		}
			break;
		case "PlayerStop_EncounterBoar":
		{
			//Player has ran upward after entering the temple for the first time.   Activate the boar and move it toward the waypoint near the player.
			m_goBoar.SetActive(true);
			m_goBoar.GetComponent<NPCScript>().DHF_NPCMoveToGameobject(GameObject.Find("BoarStop_EncounterBoar"), true, 2, false);
			GameObject.Find ("BoarStop_EncounterBoar").GetComponent<BoxCollider2D> ().enabled = true;
		}
			break;
		case "EncounterBoarBeforeRunOff":
		{
			//Encountering the boar that is going to run toward the stairs.
			ds.m_dStoryFlagField.Remove ("ToAEvent");
			ds.m_dStoryFlagField.Add("ToAEvent", 2);
			GameObject Player = GameObject.Find("Player");
			Player.GetComponent<FieldPlayerMovementScript>().BindInput();
			Player.GetComponent<FieldPlayerMovementScript>().DHF_PlayerMoveToGameObject(GameObject.Find("Callan_MoveTowardBoar"), false, 2);
			GameObject.Find("Callan_MoveTowardBoar").GetComponent<BoxCollider2D>().enabled = true;
		}
			break;
		case "Callan_MoveTowardBoar":
		{
			//Callan is now in position to encounter the boar before it runs downstairs, now move Briol.
			GameObject Player = GameObject.Find("Player");

			GameObject Briol  = GameObject.Find("Briol");
			Briol.transform.position = Player.transform.position;
			Briol.GetComponent<SpriteRenderer>().enabled = true;
			Briol.GetComponent<NPCScript>().DHF_NPCMoveToGameobject(GameObject.Find("Briol_MoveTowardBoar"), false, 2, true);
			GameObject.Find("Briol_MoveTowardBoar").GetComponent<BoxCollider2D>().enabled = true;
		}
			break;
		case "Briol_MoveTowardBoar":
		{
			//Briol exclaims about there being a boar
			GameObject Briol  = GameObject.Find("Briol");
			Briol.GetComponentInChildren<MessageHandler>().BeginDialogue("B0");
		}
			break;
		
		case "To Forest":
		{
			GameObject src = GameObject.Find("Player");
			src.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKUP);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveUp", true);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", false);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 1);
			src.GetComponent<FieldPlayerMovementScript>().SetIsRunning(false);
			GameObject.Find("StepBack").GetComponent<BoxCollider2D>().enabled = true;
			
		}
			break;
		case "Interior":
		{
			//Is entering the temple for the first time, Stop the player, move briol away a little bit, begin dialogue.
			GameObject player = GameObject.FindGameObjectWithTag ("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript> ().BindInput ();
			}
			GameObject briol = GameObject.Find ("Briol");
			briol.transform.position = player.transform.position;
			briol.GetComponent<SpriteRenderer> ().enabled = true;
			NPCScript bNpc = briol.GetComponent<NPCScript> ();
			bNpc.m_bIsComingOutOfPlayer = true;
			GameObject.Find ("Briol_AwayFromPlayerAtEntrance").GetComponent<BoxCollider2D> ().enabled = true;
			bNpc.DHF_NPCMoveToGameobject (GameObject.Find ("Briol_AwayFromPlayerAtEntrance"),false,2, true);
			ds.m_dStoryFlagField.Remove("ToAEvent");
			ds.m_dStoryFlagField.Add("ToAEvent", 1);
		}
			break;
		case "Briol_AwayFromPlayerAtEntrance":
		{
			//Briol has moved to the location, begin dialogue
			GameObject briol = GameObject.Find ("Briol");
			briol.GetComponentInChildren<MessageHandler> ().BeginDialogue ("A0");

		}
			break;
		case "EncounterBoarDestination":
		{
			//Boar has reached stairs, deactivate, set the camera target back to the player and play the last bit of dialogue for the scene
			m_goBoar.SetActive(false);
			Camera.main.GetComponent<CameraFollowTarget>().m_goNextTarget = GameObject.Find("Player");
			GameObject.Find("Briol").GetComponentInChildren<MessageHandler>().BeginDialogue("B2");
		}
			break;
		case "EncounterThePit":
		{
			/*
   					Briol emerges from the party and runs off toward the hole
   					Callan follows and Briol_Temple_Dialogue starts at line D0
   					Charcters form one unit and scene ends
				*/
			ds.m_dStoryFlagField.Remove ("ToAEvent");
			ds.m_dStoryFlagField.Add("ToAEvent", 3);
			GameObject player = GameObject.FindGameObjectWithTag ("Player");
			if (player) 
			{
				player.GetComponent<FieldPlayerMovementScript> ().BindInput ();
			}
			player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
			GameObject Briol = GameObject.Find("Briol");
			Briol.transform.position = player.transform.position;
			NPCScript bscrpt = Briol.GetComponent<NPCScript>();
			Briol.GetComponent<SpriteRenderer>().enabled = true;
			GameObject.Find("PitPoint_Briol").GetComponent<BoxCollider2D>().enabled = true;
			bscrpt.DHF_NPCMoveToGameobject(GameObject.Find("PitPoint_Briol"), false, 3, true);
		}
			break;
		case "PitPoint_Briol":
		{
			//Briol has moved to the edge of the pit, have the main character move up beside her.
			GameObject.Find("PitPoint_Callan").GetComponent<BoxCollider2D>().enabled = true;
			GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().DHF_PlayerMoveToGameObject(GameObject.Find ("PitPoint_Callan"), false);
		}
			break;
		case "PitPoint_Callan":
		{
			//Callan has moved beside briol to look at the pit, begin dialogue
			GameObject.Find("Briol").GetComponentInChildren<MessageHandler>().BeginDialogue("D0");
		}
			break;
		case "EncounterBoarBoss_Start":
		{
			//Move the player into starting position for encountering the boar boss.
			m_goBoarBoss.SetActive (true);
			GameObject player = GameObject.Find("Player");
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
			GameObject.Find("BoarBossPoint_Callan").GetComponent<BoxCollider2D>().enabled = true;
			player.GetComponent<FieldPlayerMovementScript>().DHF_PlayerMoveToGameObject(GameObject.Find("BoarBossPoint_Callan"), false, 3);
		}
			break;
		case "BoarBossPoint_Callan":
		{
			//Callan has moved into initial position for encountering the boar boss, now move Briol
			GameObject Briol = GameObject.Find("Briol");
			GameObject.Find("BoarBossPoint_Briol").GetComponent<BoxCollider2D>().enabled = true;
			Briol.transform.position = GameObject.Find("Player").transform.position;
			Briol.GetComponent<SpriteRenderer>().enabled = true;
			Briol.GetComponent<NPCScript>().DHF_NPCMoveToGameobject(GameObject.Find("BoarBossPoint_Briol"), false, 3, true);
		}
			break;
		case "BoarBossPoint_Briol":
		{
			//Briol has moved into initial position for encountering the boar boss, start the dialogue
			GameObject.Find("Briol").GetComponentInChildren<MessageHandler>().BeginDialogue("C0");
		}
			break;
		case "BoarBossChargePoint_Callan":
		{
			//Characters have charged at the boar, start the battle stuff
			Camera.main.GetComponent<CameraFollowTarget> ().m_bShouldSwirl = true;
			Camera.main.GetComponent<VEffects> ().SendMessage ("StartBlur");
			Invoke("StartBoarBossBattle", 0.5f);
		}
			break;
		}
	}



	//Helper functions for battles------
	void StartBoarBattle()
	{
		GameObject dc = GameObject.Find("PersistantData");
		if(dc)
		{
			List<EncounterGroupLoaderScript.cEnemyData> bossEncounter = new List<EncounterGroupLoaderScript.cEnemyData>();
			EncounterGroupLoaderScript.cEnemyData enemy = new EncounterGroupLoaderScript.cEnemyData();
			enemy.m_szEnemyName = "Boar";
			enemy.m_nFormationIter = 3;
			bossEncounter.Add(enemy);
			enemy = new EncounterGroupLoaderScript.cEnemyData();
			enemy.m_szEnemyName = "Boar";
			enemy.m_nFormationIter = 4;
			bossEncounter.Add(enemy);
			enemy = new EncounterGroupLoaderScript.cEnemyData();
			enemy.m_szEnemyName = "Boar";
			enemy.m_nFormationIter = 5;
			bossEncounter.Add(enemy);
			//Set the names of the list of enemies the player is about to fight
			dc.GetComponent<DCScript>().SetEnemyNames(bossEncounter);
			//Set the position of the player before the battle starts
			GameObject go = GameObject.Find("PersistantData");
			GameObject m_goPlayer = GameObject.Find("Player");
			go.GetComponent<DCScript>().SetPreviousPosition(m_goPlayer.transform.position);
			go.GetComponent<DCScript>().SetPreviousFacingDirection(m_goPlayer.GetComponent<FieldPlayerMovementScript>().m_nFacingDir);
			go.GetComponent<DCScript>().SetPreviousFieldName(SceneManager.GetActiveScene().name);
			go.GetComponent<DCScript>().SetBattleFieldBackgroundIter(1);


            SceneManager.LoadScene("Battle_Scene");
		}
	}
	void StartBoarBossBattle()
	{
		GameObject dc = GameObject.Find("PersistantData");
		if(dc)
		{
			List<EncounterGroupLoaderScript.cEnemyData> bossEncounter = new List<EncounterGroupLoaderScript.cEnemyData>();
			EncounterGroupLoaderScript.cEnemyData enemy = new EncounterGroupLoaderScript.cEnemyData();
			enemy.m_szEnemyName = "Boar";
			enemy.m_nFormationIter = 3;
			bossEncounter.Add(enemy);
			enemy = new EncounterGroupLoaderScript.cEnemyData();
			enemy.m_szEnemyName = "Boar";
			enemy.m_nFormationIter = 5;
			bossEncounter.Add(enemy);
			enemy = new EncounterGroupLoaderScript.cEnemyData();
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

	//Fading back in after the boar boss fight
	void FadeBackIn()
	{
		GameObject player = GameObject.Find("Player");
		GameObject briol = GameObject.Find("Briol");
		player.transform.position = GameObject.Find("AfterBoarBattlePosition").transform.position;
		briol.transform.position = GameObject.Find("AfterBoarBattlePositionBriol").transform.position;
		player.GetComponent<Animator>().SetFloat("m_nFacingDir", 2);
		briol.GetComponent<Animator>().SetFloat("m_nFacingDir", 1);
		Camera.main.SendMessage("fadeIn");
		Invoke("StartChat", 1.0f);
	}

	//has faded back in, start the dialogue where the player chooses knight or ranger
	void StartChat()
	{
		GameObject player = GameObject.Find("Player");
		player.GetComponentInChildren<MessageHandler>().BeginDialogue("B0");
	}
}
