using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class InonEventHandler : BaseEventSystemScript 
{
	DCScript ds;
	
	public GameObject[] Phase1_waypoints;
	public GameObject[] Phase2_waypoints;
	public GameObject[] Phase3_waypoints;
	public GameObject[] Phase4_waypoints;
	public GameObject m_goBoar;
	public GameObject m_goDeadBoar;
	public Sprite m_t2dDeadBoarWithoutTusk;
	public GameObject m_goForestLine;

	bool m_bUpDir = false, m_bDownDir = false, m_bLeftDir = false, m_bRightDir = false;

	// Use this for initialization
	void Start () 
	{
		ds = GameObject.Find("PersistantData").GetComponent<DCScript>();

		int result;
		if(ds.m_dStoryFlagField.TryGetValue("Intro_in_Inon", out result) == false)
		{
			//This is the introduction, play it yo!
			GameObject player = GameObject.Find("Player");
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
			player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
			player.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
			ds.m_dStoryFlagField.Add("Intro_in_Inon", 1);
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase2_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase3_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			GameObject.Find("NoteFromFamily").GetComponent<BoxCollider2D>().enabled = false;
		}
		else if(ds.m_dStoryFlagField.TryGetValue("Inon_RitualBattleComplete", out result) == false)
		{
			//Haven't started the ritual yet.
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase2_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase3_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
		}
		else if(ds.m_dStoryFlagField.TryGetValue("Inon_CeremonyComplete", out result) == false && 
		        ds.m_dStoryFlagField.TryGetValue("Inon_RitualBattleComplete", out result) == true)
		{
			GameObject player = GameObject.Find("Player");
			player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
			player.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 0);
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
			//have completed the ritual battle, but haven't completed the full ritual yet, begin the final bits of dialogue for the ritual
			GameObject.Find("Mattach").GetComponentInChildren<MessageHandler>().BeginDialogue("B1");
			ds.m_dStoryFlagField.Remove("Inon_RitualBattleComplete");
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase2_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase3_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			m_goDeadBoar.SetActive(true);
		}
		else if(ds.m_dStoryFlagField.TryGetValue("Inon_CeremonyComplete", out result) == true)
		{
			//The player has finished all of the intro events in Inon.
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase2_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase3_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;

		}
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		int result;
		if(ds.m_dStoryFlagField.TryGetValue("Inon_HasMoved", out result) == false)
		{
			//player hasn't yet moved in all of the cardinal directions
			GameObject player = GameObject.Find("Player");
			if(player.GetComponent<FieldPlayerMovementScript>().GetAllowInput() == true)
			{
				if(Input.GetKey(KeyCode.UpArrow) && m_bUpDir == false)
					m_bUpDir = true;
				if(Input.GetKey(KeyCode.DownArrow) && m_bDownDir == false)
					m_bDownDir = true;
				if(Input.GetKey(KeyCode.LeftArrow) && m_bLeftDir == false)
					m_bLeftDir = true;
				if(Input.GetKey(KeyCode.RightArrow) && m_bRightDir == false)
					m_bRightDir = true;
				if(m_bUpDir == true && m_bRightDir == true && m_bLeftDir == true && m_bDownDir == true)
				{
					//player has moved in all of the directions
					ds.m_dStoryFlagField.Add("Inon_HasMoved", 1);
					player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
					player.GetComponent<FieldPlayerMovementScript>().BindInput();
					player.GetComponentInChildren<MessageHandler>().BeginDialogue("B1");
					GameObject.Find("NoteFromFamily").GetComponent<BoxCollider2D>().enabled = true;
				}
			}
		}
	}
	
	override public void HandleEvent(string eventID)
	{
		switch(eventID)
		{
		case "Callan_Movement":
		{
			//For when callan can begin moving
			GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
		}
			break;
		case "NoteInterractedWith":
		{
			//Callan is now looking at the note
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
				player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
				player.GetComponentInChildren<MessageHandler>().BeginDialogue("D1");
				GameObject.Find("NoteFromFamily").GetComponent<BoxCollider2D>().enabled = false;
			}
		}
			break;
		case "ReadNote":
		{
			//Callan has read the note from his family and can now leave the room.
			GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			foreach(GameObject go in Phase1_waypoints)
				go.SetActive(false);
			GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
			GameObject.Find("Player").GetComponentInChildren<MessageHandler>().BeginDialogue("C1");

		}
			break;
		case "Cytheria":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("Cytheria");
			if(messageSystem)
			{
				int cythRes = -1;
				if(ds.m_dStoryFlagField.TryGetValue("Inon_Cytheria", out cythRes) == false)
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
				else
				{
					if(cythRes == 1)
						messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("B0");
					else if(cythRes == 2)
						messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("C0");
				}
			}
		}
			break;
		case "Cytheria_EndDialogue1":
		{
			//Player told Cytheria to tell the boy she likes how she feels
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
			ds.m_dStoryFlagField.Add("Inon_Cytheria", 1);
		}
			break;
		case "Cytheria_EndDialogue2":
		{
			//Player told Cytheria to keep her feelings hidden from the boy she likes.
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
			ds.m_dStoryFlagField.Add("Inon_Cytheria", 2);
		}
			break;
		case "Delaria":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("Delaria");
			if(messageSystem)
			{
				int delRes = -1;
				if(ds.m_dStoryFlagField.TryGetValue("Inon_Delaria", out delRes) == false)
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
				else
				{
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("B0");
				}
			}
		}
			break;
		case "Delaria_EndDialogue":
		{
			//Player has finished his first conversation with Delaria, mark it so she never says the same thing again
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
			ds.m_dStoryFlagField.Add("Inon_Delaria", 1);
		}
			break;
		case "Timmy":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("Timmy");
			if(messageSystem)
			{
				int timRes = -1;
				if(ds.m_dStoryFlagField.TryGetValue("Inon_Timmy", out timRes) == false)
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
				else
				{
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("A3");
				}
			}
		}
			break;
		case "Timothy_EndDialogue":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
			ds.m_dStoryFlagField.Add("Inon_Timmy", 1);
		}
			break;
		case "Matthew":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("Matthew");
			if(messageSystem)
			{
				
				int mattRes = -1;
				if(ds.m_dStoryFlagField.TryGetValue("Inon_CeremonyComplete", out mattRes) == false)
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
				else
				{
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("A3");
				}
			}
		}
			break;
		case "Marcus":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("Marcus");
			if(messageSystem)
			{
				messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
			}
		}

			break;
		case "Bedrest":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("Bedrest");
			if(messageSystem)
			{
				messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
			}
		}
			
			break;
		case "Briar":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("Briar");
			if(messageSystem)
			{
				int briRes = -1;
				if(ds.m_dStoryFlagField.TryGetValue("Inon_CeremonyComplete", out briRes))
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("C0");
				else if(ds.m_dStoryFlagField.TryGetValue("Inon_Bartholomew", out briRes) == false)
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
				else
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("B0");
			}
		}
			break;
		case "Briar_EndDialogue":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
			foreach(GameObject go in Phase2_waypoints)
				go.GetComponent<BoxCollider2D>().enabled = false;
		}
			break;
		case "OldTuck":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("Old Tuck");
			if(messageSystem)
			{

				int oldRes = -1;
				if(ds.m_dStoryFlagField.TryGetValue("Inon_CeremonyComplete", out oldRes) == false)
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
				else
				{
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("A3");
				}
			}
		}
			break;
		case "Cassandra":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("Cassandra");
			if(messageSystem)
			{
				int cassRes = -1;
				if(ds.m_dStoryFlagField.TryGetValue("Inon_Cassandra", out cassRes) == false)
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
				else
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("A5");
			}
		}
			break;
		case "Cassandra_EndDialogue":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
			ds.m_dStoryFlagField.Add("Inon_Cassandra", 1);
		}
			break;
		case "Lydia":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("Lydia");
			if(messageSystem)
			{
				int lydRes = -1;
				if(ds.m_dStoryFlagField.TryGetValue("Inon_Lydia", out lydRes))
				{
					if(lydRes == 12)
					{
						messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("D0");
					}
					else if(lydRes == 11)
					{
						//TODO: check player inventory for mushrooms
						ItemLibrary.CharactersItems mushroom = ds.m_lItemLibrary.GetItemFromInventory("Rare Mushroom");
						if(mushroom != null)
						{
							if( mushroom.m_nItemCount <= 5)
							{
								//has gotten all of the items.
								ds.m_lItemLibrary.RemoveItemAll(mushroom);
								ds.m_dStoryFlagField["Inon_Lydia"] = 12;
								messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("D0");
							}
							else
								messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("C0");
						}
						else
							messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("C0");
					}
					else if(lydRes == 3)
					{
						lydRes++;
						ds.m_dStoryFlagField["Inon_Lydia"] = lydRes;
							messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("B0");
					}
					else if(lydRes < 10)
					{
						lydRes++;
						ds.m_dStoryFlagField["Inon_Lydia"] = lydRes;
						messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
					}
				}
				else
				{
					ds.m_dStoryFlagField.Add("Inon_Lydia", 1);
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
				}

			}
		}
			break;
		case "Lydia_EndDialogue":
		{
			//turn off all dialogues happening, release bind on input.. umn.. i think that's it?
			GameObject[] gObjs = GameObject.FindObjectsOfType<GameObject>();
			foreach(GameObject g in gObjs)
			{
				if(g.GetComponent<MessageHandler>() != null)
				{
					if(g.GetComponent<NPCScript>() != null)
						g.GetComponent<NPCScript>().m_bIsBeingInterractedWith = false;
					g.GetComponentInChildren<MessageHandler>().m_bShouldDisplayDialogue = false;
				}
			}
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
		}
			break;
		case "Bartholomew":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("Bartholomew");
			if(messageSystem)
			{
				
				int bartRes = -1;
				if(ds.m_dStoryFlagField.TryGetValue("Inon_Bartholomew", out bartRes) == false)
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
				else
				{
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("A4");
				}
			}
		}
			break;
		case "Bartholomew_EndDialogue":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
			ds.m_dStoryFlagField.Add("Inon_Bartholomew", 1);
		}
			break;
		case "Constantinople":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("Constantinople");
			if(messageSystem)
			{

				int constRes = -1;
				if(ds.m_dStoryFlagField.TryGetValue("Inon_CeremonyComplete", out constRes) == false)
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
				else
				{
					messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("A3");
				}
			}
		}
			break;
		case "NPC_Dancer":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("NPC_Dancer");
			if(messageSystem)
			{
				messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
			}
		}
			break;
		case "Inon_Merchant1":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
						if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("NPC_Merchant1");
			if(messageSystem)
			{
				messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
			}
		}

			break;
		case "End_Marcus":
		{
			GameObject[] gObjs = GameObject.FindObjectsOfType<GameObject>();
			foreach(GameObject g in gObjs)
			{
				if(g.GetComponentInChildren<MessageHandler>() != null)
				{
					g.GetComponentInChildren<MessageHandler>().m_bShouldDisplayDialogue = false;
				}
			}
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
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
		case "Merchant_EndDialogue":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
			GameObject[] gObjs = GameObject.FindGameObjectsWithTag("Merchant");
			foreach(GameObject g in gObjs)
			{
				g.GetComponent<NPC_ArmorMerchantScript>().ActivateMerchantScreen();
			}

		}
			break;
		case "InnKeeper_Sleep":
		{
			GameObject[] keepers = GameObject.FindGameObjectsWithTag("InnKeeper");
			foreach(GameObject keeper in keepers)
			{
				if(keeper.GetComponent<NPCScript>().m_bIsBeingInterractedWith == true)
				{
					//found the game object that is the innkeeper, check if the player can afford it, if not.. ?   if you can, go to sleep after deducting the cost
					if(ds.m_nGold - keeper.GetComponent<NPCScript>().m_nCost >= 0)
					{
						ds.m_nGold = ds.m_nGold - keeper.GetComponent<NPCScript>().m_nCost;
						GameObject.Find("Inn Keeper").GetComponent<InnKeeperScript>().BeginFade();
					}
					else
					{
						HandleEvent("EndDialogue");
					}
				}
			}

		}
			break;

		case "ItemShoppe":
		{
			GameObject[] gObjs = GameObject.FindObjectsOfType<GameObject>();
			foreach(GameObject g in gObjs)
			{
				if(g.GetComponentInChildren<MessageHandler>() != null)
				{
					g.GetComponentInChildren<MessageHandler>().m_bShouldDisplayDialogue = false;
				}
			}
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
		}
			break;
		case "Inon_Blacksmith1":
		{
			GameObject blackSmith = GameObject.Find("Briar");
			blackSmith.GetComponent<NPC_BlacksmithScript>().ActivateScreen();

		}
			break;
		case "BoarTutorial":
		{
			m_goForestLine.GetComponent<EdgeCollider2D>().enabled = false;
			m_goBoar.SetActive(true);
			m_goBoar.GetComponent<BoarRitualScript>().ActivateBoar();

		}
			break;
		case "StartBoarBattle":
		{
			//battles stuff
			ds.m_dStoryFlagField.Add("Inon_RitualBattleComplete", 1);
			ds.m_dStoryFlagField.Add("Battle_ReadMessage", 1);
			StartBossBattle();
		}
			break;
		case "RetrieveTusks":
		{
			GameObject.Find("Briol").GetComponent<NPCScript>().DHF_NPCMoveToGameobject(Phase4_waypoints[0],false);
		}
			break;
		case "BriolArriveAtRitual":
		{
			//This event is for Briol having arrived next to the boar during the first ritual event to retrieve the boar tusk.
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			m_goDeadBoar.GetComponent<SpriteRenderer>().sprite = m_t2dDeadBoarWithoutTusk;
			GameObject.Find("UI_Alerts").GetComponent<UIAlertWindowScript>().ActivateWindow(UIAlertWindowScript.MESSAGEID.eITEMREWARD, "1 Boar Tusk", gameObject);
			
		}
			break;
		case "RITUALEVENT_ObtainedBoarTusk":
		{
			//Player has acquired the boar tusk, now have Briol move into the player to join the team.
			ds.m_dStoryFlagField.Add("Inon_CeremonyComplete", 1);
			m_goDeadBoar.SetActive(false);
			GameObject.Find("Briol").GetComponent<NPCScript>().DHF_NPCMoveIntoPlayer();
			GameObject.Find("Briol").GetComponent<BoxCollider2D>().enabled = false;
			Invoke("RecruitBriol", 2.0f);
		}
			break;
		case "RitualEnd":
		{
			
			ds.m_dStoryFlagField["Inon_CeremonyComplete"] = 2;
			GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseAllBinds();
			

		}
			break;
		default:
			break;
		}
	}

	void MessageWindowDeactivated()
	{
		int result = 0;
		if(ds.m_dStoryFlagField.TryGetValue("Inon_CeremonyComplete", out result))
		{
			if(result == 1)
			{
				//Briol has now moved into the player and they've seen the window for her having been recruited.
				GameObject.Find("Mattach").GetComponentInChildren<MessageHandler>().BeginDialogue("D0");
			}
		}
		else
		{
			//Player has not done any of the event yet, so this is for just after he acquires the boar tusk
			HandleEvent("RITUALEVENT_ObtainedBoarTusk");
		}
	}

	void RecruitBriol()
	{
		GameObject.Find("UI_Alerts").GetComponent<UIAlertWindowScript>().ActivateWindow(UIAlertWindowScript.MESSAGEID.eRECRUITED, "Briol", gameObject);
	}

	void BeginDialogue(int iter)
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if(player)
		{
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
		}
		GameObject messageSystem = GameObject.Find("Female Dancer");
		if(messageSystem)
		{
			messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(iter);
		}
	}

	override public void WaypointTriggered(Collider2D c)
	{
		switch(c.name)
		{
		case "IntoHallwayCheck":
		{
			//The player needs to stay in this room, disable input and have him begin walking downwards, will get caught by child waypoint to release input and stop movement.
			GameObject player = GameObject.Find("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
				player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKDOWN);
				player.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveDown", true);
				player.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", false);
				player.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 0);
				player.GetComponent<FieldPlayerMovementScript>().SetIsRunning(false);
				GameObject.Find("StepBackWaypoint").GetComponent<BoxCollider2D>().enabled = true;
			}
		}
			break;
		case "StepBackWaypoint":
		{
			//player has stepped back from going deeper into the cave, release the bind on input, disable collision box, umm.. change state to idle.
			GameObject player = GameObject.Find("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
				player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eIDLE);
				GameObject.Find("StepBackWaypoint").GetComponent<BoxCollider2D>().enabled = false;
			}
		}
			break;
		case "TowardRitualCheck":
		{
			//Stops the player from going up to the ritual site before doing the previous events.
			GameObject player = GameObject.Find("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
				player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKLEFT);
				player.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveLeft", true);
				player.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", false);
				player.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 1);
				player.GetComponent<FieldPlayerMovementScript>().SetIsRunning(false);
				GameObject.Find("StepBackRitual").GetComponent<BoxCollider2D>().enabled = true;
			}
		}
			break;
		case "StepBackRitual":
		{
			//player has stepped back from going deeper into the cave, release the bind on input, disable collision box, umm.. change state to idle.
			GameObject player = GameObject.Find("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
				player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eIDLE);
				GameObject.Find("StepBackRitual").GetComponent<BoxCollider2D>().enabled = false;
			}
		}
			break;
		case "StartArriveAtRitual":
		{

			//player has gotten close enough to the ritual for us to take over, first we need to make sure that the player is in the right x alignment, move toward that waypoint depending on the direction

			GameObject.Find("StartArriveAtRitual").GetComponent<BoxCollider2D>().enabled = false;
			GameObject dest = GameObject.Find("XAlignedAtRitual");
			GameObject src = GameObject.Find("Player");
			src.GetComponent<FieldPlayerMovementScript>().BindInput();
			Vector2 dir = dest.transform.position - src.transform.position;
			if(dir.x > 0)
			{
				src.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKRIGHT);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveRight", true);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", false);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 2);
				src.GetComponent<FieldPlayerMovementScript>().SetIsRunning(false);
				GameObject.Find("XAlignedAtRitual").GetComponent<BoxCollider2D>().enabled = true;
			}
			else if(dir.x < 0)
			{
				src.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKLEFT);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveLeft", true);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", false);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 1);
				src.GetComponent<FieldPlayerMovementScript>().SetIsRunning(false);
				GameObject.Find("XAlignedAtRitual").GetComponent<BoxCollider2D>().enabled = true;
			}
			else
			{
				src.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKUP);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveUp", true);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", false);
				src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 3);
				src.GetComponent<FieldPlayerMovementScript>().SetIsRunning(false);
				GameObject.Find("ArrivedAtRitualWaypoint").GetComponent<BoxCollider2D>().enabled = true;
			}
		}
			break;
		case "XAlignedAtRitual":
		{
			GameObject src = GameObject.Find("Player");
			src.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKUP);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveUp", true);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", false);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 3);
			src.GetComponent<FieldPlayerMovementScript>().SetIsRunning(false);
			GameObject.Find("XAlignedAtRitual").GetComponent<BoxCollider2D>().enabled = false;
			GameObject.Find("ArrivedAtRitualWaypoint").GetComponent<BoxCollider2D>().enabled = true;
		}
			break;
		case "ArrivedAtRitualWaypoint":
		{
			GameObject player = GameObject.Find("Player");
			player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eIDLE);
			player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
			GameObject.Find("Mattach").GetComponentInChildren<MessageHandler>().BeginDialogue("A1");
			GameObject.Find("ArrivedAtRitualWaypoint").GetComponent<BoxCollider2D>().enabled = false;
		}
			break;
		case "BoarArriveAtRitual":
		{
			m_goBoar.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
			m_goBoar.GetComponent<Animator>().SetBool("m_bAttack", true);
		}
			break;
		case "StepBackNorth":
		{
			GameObject player = GameObject.Find("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eIDLE);
				GameObject.Find("StepBackNorth").GetComponent<BoxCollider2D>().enabled = false;
				player.GetComponentInChildren<MessageHandler>().BeginDialogue("F0");
				GameObject.Find("LeaveFromNorthWaypoint").GetComponent<BoxCollider2D>().enabled = true;
			}
		}
			break;
		case "StepBackSouth":
		{
			GameObject player = GameObject.Find("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eIDLE);
				GameObject.Find("StepBackSouth").GetComponent<BoxCollider2D>().enabled = false;
				player.GetComponentInChildren<MessageHandler>().BeginDialogue("F0");
				GameObject.Find("LeaveFromSouthWaypoint").GetComponent<BoxCollider2D>().enabled = true;
			}
		}
			break;
		case "LeaveFromSouthWaypoint":
		{
			GameObject src = GameObject.Find("Player");
			src.GetComponent<FieldPlayerMovementScript>().BindInput();
			src.GetComponent<FieldPlayerMovementScript>().DHF_PlayerMoveToGameObject(GameObject.Find("StepBackSouth"), false);
			GameObject.Find("StepBackSouth").GetComponent<BoxCollider2D>().enabled = true;

		}
			break;
		case "LeaveFromNorthWaypoint":
		{
			GameObject src = GameObject.Find("Player");
			src.GetComponent<FieldPlayerMovementScript>().BindInput();
			src.GetComponent<FieldPlayerMovementScript>().DHF_PlayerMoveToGameObject(GameObject.Find("StepBackNorth"), false);
			GameObject.Find("StepBackNorth").GetComponent<BoxCollider2D>().enabled = true;
		}
			break;
		
		default:
			break;
		}
	}

	void StartBossBattle()
	{
		GameObject dc = GameObject.Find("PersistantData");
		if(dc)
		{
			List<EncounterGroupLoaderScript.cEnemyData> bossEncounter = new List<EncounterGroupLoaderScript.cEnemyData>();
			EncounterGroupLoaderScript.cEnemyData enemy = new EncounterGroupLoaderScript.cEnemyData();
			enemy.m_szEnemyName = "Boar";
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
			go.GetComponent<DCScript>().SetBattleFieldBackgroundIter(1);

			GameObject Briol = Resources.Load<GameObject>("Units/Ally/Briol/Briol");
			Briol.GetComponent<CAllyBattleScript>().SetUnitStats();

            SceneManager.LoadScene("Battle_Scene");
		}
	}

}