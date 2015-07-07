using UnityEngine;
using System.Collections;

public class InonEventHandler : BaseEventSystemScript 
{
	DCScript ds;
	
	public GameObject[] Phase1_waypoints;
	public GameObject[] Phase2_waypoints;
	public GameObject[] Phase3_waypoints;

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
					player.GetComponent<MessageHandler>().BeginDialogue("A4");
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
				player.GetComponent<MessageHandler>().BeginDialogue("A7");
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
			GameObject.Find("Player").GetComponent<MessageHandler>().BeginDialogue("A10");
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
				messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
			}
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
				messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
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
			Debug.Log("end");
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
			GameObject blackSmith = GameObject.Find("Blacksmith");
			blackSmith.GetComponent<NPC_BlacksmithScript>().ActivateScreen();

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
				//player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
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
				//player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
				GameObject.Find("StepBackWaypoint").GetComponent<BoxCollider2D>().enabled = false;
			}
		}
			break;
		default:
			break;
		}
	}

}