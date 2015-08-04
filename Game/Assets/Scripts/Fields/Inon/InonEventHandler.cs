using UnityEngine;
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
			player.GetComponent<MessageHandler>().BeginDialogue(0);
			ds.m_dStoryFlagField.Add("Intro_in_Inon", 1);
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase2_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase3_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
		}
		if(ds.m_dStoryFlagField.TryGetValue("Inon_RitualBattleComplete", out result) == false)
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
			GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
			//have completed the ritual battle, but haven't completed the full ritual yet, begin the final bits of dialogue for the ritual
			GameObject.Find("Mattach").GetComponent<MessageHandler>().BeginDialogue("B1");
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
					player.GetComponent<MessageHandler>().BeginDialogue("B1");
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
				player.GetComponent<MessageHandler>().BeginDialogue("D1");
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
			GameObject.Find("Player").GetComponent<MessageHandler>().BeginDialogue("C1");
		}
			break;
		case "FemaleDancerDialogue":
			int result;
			if(ds.m_dStoryFlagField.TryGetValue("Inon_CrossedBridge", out result))
			{
				//If the player has previously gone over the bridge
				BeginDialogue(1);
			}
			else
			{
				//If the player hasn't gone over the bridge
				BeginDialogue(0);
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
					messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
				else
				{
					if(cythRes == 1)
						messageSystem.GetComponent<MessageHandler>().BeginDialogue("B0");
					else if(cythRes == 2)
						messageSystem.GetComponent<MessageHandler>().BeginDialogue("C0");
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
					messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
				else
				{
					messageSystem.GetComponent<MessageHandler>().BeginDialogue("B0");
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
					messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
				else
				{
					messageSystem.GetComponent<MessageHandler>().BeginDialogue("A3");
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
					messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
				else
				{
					messageSystem.GetComponent<MessageHandler>().BeginDialogue("A3");
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
				messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
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
				messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
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
					messageSystem.GetComponent<MessageHandler>().BeginDialogue("C0");
				else if(ds.m_dStoryFlagField.TryGetValue("Inon_Bartholomew", out briRes) == false)
					messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
				else
					messageSystem.GetComponent<MessageHandler>().BeginDialogue("B0");
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
					messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
				else
				{
						messageSystem.GetComponent<MessageHandler>().BeginDialogue("A3");
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
					messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
				else
					messageSystem.GetComponent<MessageHandler>().BeginDialogue("A5");
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
				messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
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
					messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
				else
				{
					messageSystem.GetComponent<MessageHandler>().BeginDialogue("A4");
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
					messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
				else
				{
					messageSystem.GetComponent<MessageHandler>().BeginDialogue("A3");
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
				messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
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
				messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
			}
		}

			break;
		case "End_Marcus":
		{
			GameObject[] gObjs = GameObject.FindObjectsOfType<GameObject>();
			foreach(GameObject g in gObjs)
			{
				if(g.GetComponent<MessageHandler>() != null)
				{
					if(g.GetComponent<NPCScript>() != null)
						g.GetComponent<NPCScript>().RestartPathing();
					g.GetComponent<MessageHandler>().m_bShouldDisplayDialogue = false;
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
				if(g.GetComponent<MessageHandler>() != null)
				{
					if(g.GetComponent<NPCScript>() != null)
						g.GetComponent<NPCScript>().m_bIsBeingInterractedWith = false;
					g.GetComponent<MessageHandler>().m_bShouldDisplayDialogue = false;
				}
			}
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
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
				if(g.GetComponent<MessageHandler>() != null)
				{
					if(g.GetComponent<NPCScript>() != null)
						g.GetComponent<NPCScript>().RestartPathing();
					g.GetComponent<MessageHandler>().m_bShouldDisplayDialogue = false;
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
			m_goBoar.GetComponent<BoarRitualScript>().m_bIsActive = true;

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
			GameObject.Find("Briol").GetComponent<BriolInonRitualScript>().MoveDownward();
		}
			break;
		case "BriolArriveAtRitual":
		{
			Debug.Log("bub");
			ds.m_dStoryFlagField.Add("Inon_CeremonyComplete", 1);
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			m_goDeadBoar.GetComponent<SpriteRenderer>().sprite = m_t2dDeadBoarWithoutTusk;
			GameObject.Find("Mattach").GetComponent<MessageHandler>().BeginDialogue("C1");
		}
			break;
		default:
			break;
		}
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
			messageSystem.GetComponent<MessageHandler>().BeginDialogue(iter);
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
			GameObject.Find("Mattach").GetComponent<MessageHandler>().BeginDialogue("A1");
			GameObject.Find("ArrivedAtRitualWaypoint").GetComponent<BoxCollider2D>().enabled = false;
		}
			break;
		case "BoarArriveAtRitual":
		{
			m_goBoar.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
			m_goBoar.GetComponent<Animator>().SetBool("m_bAttack", true);

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
			List<string> bossEncounter = new List<string>();
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

}