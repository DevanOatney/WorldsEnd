using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToAEventHandler : BaseEventSystemScript 
{
	public GameObject[] Phase1_waypoints;
	public GameObject m_goBoar;
	public GameObject m_goBoarBoss;
	DCScript ds;
	public List<GameObject[]> m_lEvents;
	// Use this for initialization
	void Start()
	{
		ds = GameObject.Find("PersistantData").GetComponent<DCScript>();
		SetWaypoints();

		GameObject player = GameObject.Find("Player");
		GameObject briol = GameObject.Find("Briol");
		Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), briol.GetComponent<BoxCollider2D>());
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
			GameObject player = GameObject.Find("Player");
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
			player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
			player.GetComponent<MessageHandler>().BeginDialogue("A0");
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
				GameObject.Find("Interior").GetComponent<BoxCollider2D>().enabled = true;
			}
				break;
			case 1:
			{
				//The player has gone into the entrance of the Temple for the first time
				GameObject.Find("EncounterBoarBeforeRunOff").GetComponent<BoxCollider2D>().enabled = true;
			}
				break;
			case 2:
			{
				//The player has seen the boar run down into the basement
				GameObject.Find("EncounterThePit").GetComponent<BoxCollider2D>().enabled = true;
			}
				break;
				
			}
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
				//Callan runs forward to bridge then stops and waits for Briol
				GameObject src = GameObject.Find ("Player");
				src.GetComponent<FieldPlayerMovementScript> ().DHF_PlayerMoveToGameObject (GameObject.Find ("StopAtBridge"), true);
				GameObject.Find ("StopAtBridge").GetComponent<BoxCollider2D> ().enabled = true;
			}
			break;
		case "Temple Bed":
			{
				//Callan has interacted with the bed, give him the option to rest.
				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				if (player) {
					player.GetComponent<FieldPlayerMovementScript> ().BindInput ();
				}
				GameObject messageSystem = GameObject.Find ("Temple Bed");
				if (messageSystem) {
					messageSystem.GetComponent<MessageHandler> ().BeginDialogue (0);
				}
			}
			
			break;
		
		case "BoarBattle":
			{

				//Callan begins to run upward after entering the temple for the first time.
				GameObject src = GameObject.Find ("Player");
				src.GetComponent<FieldPlayerMovementScript> ().DHF_PlayerMoveToGameObject (GameObject.Find ("PlayerStop_EncounterBoar"), true);
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
			m_goBoar.GetComponent<NPCScript>().DHF_NPCMoveToGameobject(GameObject.Find ("EncounterBoarDestination"), false);
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
		case "Boar_Boss":
			{
				ds.m_dStoryFlagField.Remove ("BoarBattle");
				ds.m_dStoryFlagField.Add ("AfterBoarBattle", 1);
				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				player.GetComponent<FieldPlayerMovementScript> ().DHF_PlayerMoveToGameObject (GameObject.Find ("BoarBoss"), true);
				GameObject.Find ("BoarBoss").GetComponent<BoxCollider2D> ().enabled = true;
				GameObject briol = GameObject.Find ("Briol");
				briol.GetComponent<NPCScript> ().DHF_NPCMoveToGameobject (GameObject.Find ("BoarBoss"), true);
				Camera.main.GetComponent<CameraFollowTarget> ().m_bShouldSwirl = true;
				Camera.main.GetComponent<VEffects> ().SendMessage ("StartBlur");
				StartBoarBossBattle ();
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
				briol.GetComponent<MessageHandler> ().BeginDialogue ("A7");
			}
			break;
		
		
		case "EndDialogue":
			{
				//turn off all dialogues happening, release bind on input.. umn.. i think that's it?
				GameObject[] gObjs = GameObject.FindObjectsOfType<GameObject> ();
				foreach (GameObject g in gObjs) 
				{
					if (g.GetComponent<MessageHandler> () != null) 
					{
						if (g.GetComponent<NPCScript> () != null)
						{
							g.GetComponent<NPCScript> ().m_bIsBeingInterractedWith = false;
						}
						g.GetComponent<MessageHandler> ().m_bShouldDisplayDialogue = false;
					}
				}
				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				if (player) 
				{
					player.GetComponent<FieldPlayerMovementScript> ().ReleaseBind ();
				}
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
			src.GetComponent<MessageHandler>().BeginDialogue("A6");
			GameObject.Find("Interior").GetComponent<BoxCollider2D>().enabled = true;
		}
			break;
		case "BoarStop_EncounterBoar":
		{
			//The boar has charged at the player after entering the temple for the first time, blur the camera and begin battle!
			Camera.main.GetComponent<CameraFollowTarget>().m_bShouldSwirl = true;
			Camera.main.GetComponent<VEffects>().SendMessage("StartBlur");
			StartBoarBattle();
		}
			break;
		case "PlayerStop_EncounterBoar":
		{
			//Player has ran upward after entering the temple for the first time.   Activate the boar and move it toward the waypoint near the player.
			m_goBoar.SetActive(true);
			m_goBoar.GetComponent<NPCScript>().DHF_NPCMoveToGameobject(GameObject.Find("BoarStop_EncounterBoar"), true);
			GameObject.Find ("BoarStop_EncounterBoar").GetComponent<BoxCollider2D> ().enabled = true;
		}
			break;
		case "EncounterBoarBeforeRunOff":
		{
			//Encountering the boar that is going to run toward the stairs.
			GameObject Player = GameObject.Find("Player");
			Player.GetComponent<FieldPlayerMovementScript>().BindInput();
			Player.GetComponent<FieldPlayerMovementScript>().DHF_PlayerMoveToGameObject(GameObject.Find("Callan_MoveTowardBoar"), false);
			GameObject.Find("Callan_MoveTowardBoar").GetComponent<BoxCollider2D>().enabled = true;
		}
			break;
		case "Callan_MoveTowardBoar":
		{
			//Callan is now in position to encounter the boar before it runs downstairs, now move Briol.
			GameObject Player = GameObject.Find("Player");
			Player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);

			GameObject Briol  = GameObject.Find("Briol");
			Briol.transform.position = Player.transform.position;
			Briol.GetComponent<SpriteRenderer>().enabled = true;
			Briol.GetComponent<NPCScript>().m_bIsComingOutOfPlayer = true;
			Briol.GetComponent<BoxCollider2D>().enabled = true;
			Briol.GetComponent<NPCScript>().DHF_NPCMoveToGameobject(GameObject.Find("Briol_MoveTowardBoar"), false);
			GameObject.Find("Briol_MoveTowardBoar").GetComponent<BoxCollider2D>().enabled = true;
		}
			break;
		case "Briol_MoveTowardBoar":
		{
			//Briol exclaims about there being a boar
			GameObject Player = GameObject.Find("Player");
			Player.GetComponent<Animator>().SetInteger("m_nFacingDir", 2);
			GameObject Briol  = GameObject.Find("Briol");
			Briol.GetComponent<MessageHandler>().BeginDialogue("B0");
		}
			break;
		
		case "To Forest":
		{
			GameObject src = GameObject.Find("Player");
			src.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKUP);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveUp", true);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", false);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 3);
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
			bNpc.DHF_NPCMoveToGameobject (GameObject.Find ("Briol_AwayFromPlayerAtEntrance"),false);
			briol.GetComponent<MessageHandler> ().BeginDialogue ("A0");
			ds.m_dStoryFlagField.Remove("ToAEvent");
			ds.m_dStoryFlagField.Add("ToAEvent", 1);
		}
			break;
		case "Briol_AwayFromPlayerAtEntrance":
		{
			//Briol has moved to the location, begin dialogue
			GameObject briol = GameObject.Find ("Briol");
			briol.GetComponent<MessageHandler> ().BeginDialogue ("A0");

		}
			break;
		case "EncounterBoarDestination":
		{
			//Boar has reached stairs, deactivate, set the camera target back to the player and play the last bit of dialogue for the scene
			m_goBoar.SetActive(false);
			Camera.main.GetComponent<CameraFollowTarget>().m_goNextTarget = GameObject.Find("Player");
			GameObject.Find("Briol").GetComponent<MessageHandler>().BeginDialogue("B2");
		}
			break;
		case "EncounterThePit":
		{
			/*
   					Briol emerges from the party and runs off toward the hole
   					Callan follows and Briol_Temple_Dialogue starts at line D0
   					Charcters form one unit and scene ends
				*/
			GameObject player = GameObject.FindGameObjectWithTag ("Player");
			if (player) 
			{
				player.GetComponent<FieldPlayerMovementScript> ().BindInput ();
			}
			GameObject Briol = GameObject.Find("Briol");
			NPCScript bscrpt = Briol.GetComponent<NPCScript>();
			Briol.GetComponent<SpriteRenderer>().enabled = true;
			bscrpt.m_bIsComingOutOfPlayer = true;
			GameObject.Find("PitPoint_Briol").GetComponent<BoxCollider2D>().enabled = true;
			bscrpt.DHF_NPCMoveToGameobject(GameObject.Find("PitPoint_Briol"), false);
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
			GameObject.Find("Briol").GetComponent<MessageHandler>().BeginDialogue("D0");
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
			List<string> bossEncounter = new List<string>();
			bossEncounter.Add("Boar");
			bossEncounter.Add("Boar");
			bossEncounter.Add("Boar");
			//Set the names of the list of enemies the player is about to fight
			dc.GetComponent<DCScript>().SetEnemyNames(bossEncounter);
			//Set the position of the player before the battle starts
			GameObject go = GameObject.Find("PersistantData");
			GameObject m_goPlayer = GameObject.Find("Player");
			go.GetComponent<DCScript>().SetPreviousPosition(m_goPlayer.transform.position);
			go.GetComponent<DCScript>().SetPreviousFacingDirection(m_goPlayer.GetComponent<FieldPlayerMovementScript>().m_nFacingDir);
			go.GetComponent<DCScript>().SetPreviousFieldName(Application.loadedLevelName);
			go.GetComponent<DCScript>().SetBattleFieldBackgroundIter(1);

			
			Application.LoadLevel("Battle_Scene");
		}
	}
	void StartBoarBossBattle()
	{
		GameObject dc = GameObject.Find("PersistantData");
		if(dc)
		{
			List<string> bossEncounter = new List<string>();
			bossEncounter.Add("BoarBoss");
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
}
