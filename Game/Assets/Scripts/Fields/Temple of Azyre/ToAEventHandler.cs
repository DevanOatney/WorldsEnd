using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToAEventHandler : BaseEventSystemScript 
{
	public GameObject[] Phase1_waypoints;
	public GameObject[] Phase2_waypoints;
	public GameObject m_goBoar;
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
		else
		{
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			GameObject briol = GameObject.Find("Briol");
			briol.GetComponent<SpriteRenderer> ().enabled = true;
			briol.GetComponent<BoxCollider2D> ().enabled = true;
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
		case "Temple Bed":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("Temple Bed");
			if(messageSystem)
			{
				messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
			}
		}
			
			break;
		
		case "Boar_Battle":
		{
			GameObject src = GameObject.Find ("Player");
			src.GetComponent<FieldPlayerMovementScript> ().DHF_PlayerMoveToGameObject (GameObject.Find ("Boar_Battle"), true);
			GameObject.Find ("Boar_Battle").GetComponent<BoxCollider2D> ().enabled = true;
			GameObject boar = GameObject.Find ("Boar");
			boar.GetComponent<NPCScript> ().DHF_NPCMoveToGameobject (GameObject.Find ("Boar_Battle"), true);
			GameObject.Find ("Boar_Battle").GetComponent<BoxCollider2D> ().enabled = true;
			Camera.main.GetComponent<CameraFollowTarget>().m_bShouldSwirl = true;
			Camera.main.GetComponent<VEffects>().SendMessage("StartBlur");
			StartBoarBattle();
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
		case "Basement1":
			{
				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				if(player)
				{
					player.GetComponent<FieldPlayerMovementScript> ().BindInput ();
				}
				GameObject src = GameObject.Find ("Player");
				src.GetComponent<FieldPlayerMovementScript> ().DHF_PlayerMoveToGameObject (GameObject.Find ("PitPoint"), true);
				GameObject.Find ("PitPoint").GetComponent<BoxCollider2D> ().enabled = true;
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
				if (ds.m_dStoryFlagField.TryGetValue ("Lion_Statue", out lionRes) == false)
				{
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
					GameObject messageSystem = GameObject.Find ("Callan");
					messageSystem.GetComponent<MessageHandler> ().BeginDialogue ("C0");
				} 
				else 
				{
					GameObject messageSystem = GameObject.Find ("Lion Statue");
					messageSystem.GetComponent<MessageHandler> ().BeginDialogue ("A0");
				}
			}
			break;
		case "Temple_Alter_1":
			{
				ds.m_dStoryFlagField.Add ("Lion_Statue", 1);
				GameObject.Find ("Briol").GetComponent<NPCScript> ().DHF_NPCMoveIntoPlayer ();
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
		case "Interior":
		{
			GameObject player = GameObject.FindGameObjectWithTag ("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript> ().BindInput ();
			}
			GameObject briol = GameObject.Find ("Briol");
			briol.transform.position = player.transform.position;
			briol.GetComponent<SpriteRenderer> ().enabled = true;
			briol.GetComponent<BoxCollider2D> ().enabled = true;
			NPCScript bNpc = briol.GetComponent<NPCScript> ();
			bNpc.m_bIsComingOutOfPlayer = true;
			bNpc.m_bIsMoving = false;
			bNpc.m_bActive = true;
			bNpc.m_nFacingDir = (int)NPCScript.FACINGDIR.eUP;
			bNpc.ResetAnimFlagsExcept (bNpc.m_nFacingDir);
			GameObject messageSystem = GameObject.Find ("Briol");
			messageSystem.GetComponent<MessageHandler> ().BeginDialogue ("A0");
		}
			break;
		}
	}
	void StartBoarBattle()
	{
		GameObject dc = GameObject.Find("PersistantData");
		if(dc)
		{
			List<string> bossEncounter = new List<string>();
			bossEncounter.Add("Boar, Boar, Boar");
			//Set the names of the list of enemies the player is about to fight
			dc.GetComponent<DCScript>().SetEnemyNames(bossEncounter);
			//Set the position of the player before the battle starts
			GameObject go = GameObject.Find("PersistantData");
			GameObject m_goPlayer = GameObject.Find("Player");
			go.GetComponent<DCScript>().SetPreviousPosition(m_goPlayer.transform.position);
			go.GetComponent<DCScript>().SetPreviousFacingDirection(m_goPlayer.GetComponent<FieldPlayerMovementScript>().m_nFacingDir);
			go.GetComponent<DCScript>().SetPreviousFieldName(Application.loadedLevelName);
			go.GetComponent<DCScript>().SetBattleFieldBackgroundIter(3);
			
			GameObject Briol = Resources.Load<GameObject>("Units/Ally/Briol/Briol");
			Briol.GetComponent<PlayerBattleScript>().SetUnitStats();
			
			Application.LoadLevel("Battle_Scene");
		}
	}
}
