using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToAEventHandler : BaseEventSystemScript 
{
	public GameObject[] Phase1_waypoints;
	public GameObject[] Phase2_waypoints;
	public GameObject[] Phase3_waypoints;
	public GameObject[] Phase4_waypoints;
	public GameObject[] Phase5_waypoints;
	public GameObject m_goBoar;
	public GameObject m_goBoar2;
	public GameObject m_goBoarBoss;
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
			foreach(GameObject wpnt in Phase2_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase3_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase5_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			GameObject Briol = GameObject.Find("Briol");
			Camera.main.GetComponent<CameraFollowTarget>().m_goTarget = Briol;
		}
		else if (ds.m_dStoryFlagField.TryGetValue("AfterOpening", out result))
		{
			if (result == 1)
			{
				ds.m_dStoryFlagField.Remove("AfterOpening");
				HandleEvent ("AfterBoar");
			}
			else
			{
				m_goBoar.SetActive(false);
			}
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase2_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase3_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase5_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
		}

		else if (ds.m_dStoryFlagField.TryGetValue("BoarBattle", out result))
		{
			if (result == 1)
			{
				ds.m_dStoryFlagField.Remove("BoarBattle");
				ds.m_dStoryFlagField.Add("BoarBattle", 2);
				m_goBoar.SetActive(false);
				HandleEvent("AfterBoar");
			}
			else
			{
			m_goBoar.SetActive(false);
			m_goBoar2.SetActive(false);
			m_goBoarBoss.SetActive(false);
			}	
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase2_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase3_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			foreach(GameObject wpnt in Phase5_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
		}
		else if (ds.m_dStoryFlagField.TryGetValue ("AfterBoarBattle", out result)) 
		{
			if (result == 1) {
				ds.m_dStoryFlagField.Remove ("AfterBoarBattle");
				ds.m_dStoryFlagField.Add ("AfterCleanup", 1);
				HandleEvent ("AfterBossBattle");
			} 
			else 
			{
				m_goBoar.SetActive (false);
				m_goBoar2.SetActive (false);
				m_goBoarBoss.SetActive (false);
			}	
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase2_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase3_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase5_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
		}
		else if (ds.m_dStoryFlagField.TryGetValue ("AfterCleanup", out result)) 
		{
			if (result == 1) 
			{
				ds.m_dStoryFlagField.Remove ("AfterBoarBattle");
				HandleEvent ("FinishedCleanup");
			}
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase2_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase3_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase5_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
		}
		else if (ds.m_dStoryFlagField.TryGetValue ("Hunter", out result))
		{
			if (result == 1) 
			{
				m_goBoar.SetActive (false);
				m_goBoar2.SetActive (false);
				m_goBoarBoss.SetActive (false);
				GameObject overwatcher = GameObject.Find ("OverWatcher");
				overwatcher.GetComponent<EncounterGroupLoaderScript> ().enabled = false;
			}
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase2_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase3_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase5_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
		}
		else if (ds.m_dStoryFlagField.TryGetValue ("Knight", out result))
		{
			if (result == 1) 
			{
				m_goBoar.SetActive (false);
				m_goBoar2.SetActive (false);
				m_goBoarBoss.SetActive (false);
				GameObject overwatcher = GameObject.Find ("OverWatcher");
				overwatcher.GetComponent<EncounterGroupLoaderScript> ().enabled = false;
			}
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase2_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase3_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase5_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
		}
		else
		{
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase2_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase3_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase4_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
			foreach(GameObject wpnt in Phase5_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = false;
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
				GameObject src = GameObject.Find ("Player");
				src.GetComponent<FieldPlayerMovementScript> ().DHF_PlayerMoveToGameObject (GameObject.Find ("Boar_Battle"), true);
				GameObject.Find ("Boar_Battle").GetComponent<BoxCollider2D> ().enabled = true;
			}
			break;
		case "AfterBossBattle":
			{
				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				if (player) {
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
				bNpc.m_nFacingDir = (int)NPCScript.FACINGDIR.eDOWN;
				bNpc.ResetAnimFlagsExcept (bNpc.m_nFacingDir);
				GameObject messageSystem = GameObject.Find ("Briol");
				messageSystem.GetComponent<MessageHandler> ().BeginDialogue ("C5");
			}
			break;
		case "InnKeeper_Sleep":
			{
				GameObject[] keepers = GameObject.FindGameObjectsWithTag ("InnKeeper");
				foreach (GameObject keeper in keepers) {
					if (keeper.GetComponent<NPCScript> ().m_bIsBeingInterractedWith == true) {
						//found the game object that is the innkeeper, check if the player can afford it, if not.. ?   if you can, go to sleep after deducting the cost
						if (ds.m_nGold - keeper.GetComponent<NPCScript> ().m_nCost >= 0) {
							ds.m_nGold = ds.m_nGold - keeper.GetComponent<NPCScript> ().m_nCost;
							GameObject.Find ("Inn Keeper").GetComponent<InnKeeperScript> ().BeginFade ();
						} else {
							HandleEvent ("EndDialogue");
						}
					}
				}
			
			}
			break;
		case "Basement1":
			{
				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				if (player) {
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
		case "FinishedCleanup":
			{
				GameObject ABP = GameObject.Find ("AfterBattlePosition");
				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				player.transform.position = ABP.transform.position;
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
				messageSystem.GetComponent<MessageHandler> ().BeginDialogue ("B0");
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
		case "EndSeeBoar":
			{
				GameObject briol = GameObject.Find ("Briol");
				NPCScript bNpc = briol.GetComponent<NPCScript> ();
				bNpc.DHF_NPCMoveIntoPlayer ();
				GameObject[] gObjs = GameObject.FindObjectsOfType<GameObject> ();
				foreach (GameObject g in gObjs) {
					if (g.GetComponent<MessageHandler> () != null) {
						if (g.GetComponent<NPCScript> () != null)
							g.GetComponent<NPCScript> ().m_bIsBeingInterractedWith = false;
						g.GetComponent<MessageHandler> ().m_bShouldDisplayDialogue = false;
					}
				}
				foreach (GameObject wpnt in Phase3_waypoints)
					wpnt.GetComponent<BoxCollider2D> ().enabled = false;
				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				if (player) {
					player.GetComponent<FieldPlayerMovementScript> ().ReleaseBind ();
				}
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
			
		case "Pan_to_Boar_Azyre":
			{
				GameObject Boar2 = GameObject.Find ("Boar2");
				Camera.main.GetComponent<CameraFollowTarget> ().m_goTarget = Boar2;
				Boar2.GetComponent<NPCScript> ().DHF_NPCMoveToGameobject (GameObject.Find ("to basement"), true);
				GameObject.Find ("to basement").GetComponent<BoxCollider2D> ().enabled = true;
				GameObject player = GameObject.Find ("Player");
				Camera.main.GetComponent<CameraFollowTarget> ().m_goTarget = player;
				GameObject messageSystem = GameObject.Find ("Briol");
				messageSystem.GetComponent<MessageHandler> ().BeginDialogue ("B2");
			}
			break;

		case "AfterBoar":
			{
				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				if (player) {
					player.GetComponent<FieldPlayerMovementScript> ().BindInput ();
				}
				m_goBoar.SetActive (false);
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
				messageSystem.GetComponent<MessageHandler> ().BeginDialogue ("A7");
			}
			break;
		case "End Temple_Clean_Dialogue1":
			{
				ds.m_dStoryFlagField.Remove ("AfterCleanup");
				ds.m_dStoryFlagField.Add ("Hunter", 1);
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
		case "PitSeen":
			{
				GameObject briol = GameObject.Find ("Briol");
				NPCScript bNpc = briol.GetComponent<NPCScript> ();
				bNpc.DHF_NPCMoveIntoPlayer ();
				GameObject[] gObjs = GameObject.FindObjectsOfType<GameObject> ();
				foreach (GameObject g in gObjs) {
					if (g.GetComponent<MessageHandler> () != null) {
						if (g.GetComponent<NPCScript> () != null)
							g.GetComponent<NPCScript> ().m_bIsBeingInterractedWith = false;
						g.GetComponent<MessageHandler> ().m_bShouldDisplayDialogue = false;
					}
				}
				foreach (GameObject wpnt in Phase4_waypoints)
					wpnt.GetComponent<BoxCollider2D> ().enabled = false;
				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				if (player) {
					player.GetComponent<FieldPlayerMovementScript> ().ReleaseBind ();
				}
			}
			break;
		case "End Temple_Clean_Dialogue2":
			{
				ds.m_dStoryFlagField.Remove ("AfterCleanup");
				ds.m_dStoryFlagField.Add ("Knight", 1);
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
		case "Lion Statue":
			{
				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				if (player) {
					player.GetComponent<FieldPlayerMovementScript> ().BindInput ();
				}
				int lionRes = -1;
				if (ds.m_dStoryFlagField.TryGetValue ("Lion_Statue", out lionRes) == false) {
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
				} else {
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
		case "Basement2":
		{
			GameObject player = GameObject.FindGameObjectWithTag ("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript> ().BindInput ();
			}
			player.GetComponent<FieldPlayerMovementScript> ().DHF_PlayerMoveToGameObject (GameObject.Find ("BoarBossPoint"), true);
			GameObject.Find ("BoarBossPoint").GetComponent<BoxCollider2D> ().enabled = true;
			GameObject briol = GameObject.Find ("Briol");
			briol.transform.position = player.transform.position;
			briol.GetComponent<SpriteRenderer> ().enabled = true;
			briol.GetComponent<BoxCollider2D> ().enabled = true;
			NPCScript bNpc = briol.GetComponent<NPCScript> ();
			bNpc.m_bIsComingOutOfPlayer = true;
			bNpc.m_bIsMoving = false;
			bNpc.m_bActive = true;
			bNpc.m_nFacingDir = (int)NPCScript.FACINGDIR.eRIGHT;
			bNpc.ResetAnimFlagsExcept (bNpc.m_nFacingDir);
			briol.GetComponent<MessageHandler> ().BeginDialogue ("B0");
		}
			break;
		case "BoarBattle2":
		{
			Camera.main.GetComponent<CameraFollowTarget>().m_bShouldSwirl = true;
			Camera.main.GetComponent<VEffects>().SendMessage("StartBlur");
			StartBoarBattle();
		}
			break;
		case "Boar_Battle":
		{
			m_goBoar.SetActive(true);
			GameObject boar = GameObject.Find ("Boar");
			boar.GetComponent<NPCScript>().m_bIsMoving = true;
			boar.GetComponent<NPCScript>().DHF_NPCMoveToGameobject(GameObject.Find("BoarBattle2"), true);
			GameObject.Find ("BoarBattle2").GetComponent<BoxCollider2D> ().enabled = true;
			ds.m_dStoryFlagField.Add("BoarBattle", 1);
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
		case "StepBack":
		{
			GameObject player = GameObject.Find("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
				player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eIDLE);
				GameObject.Find("StepBack").GetComponent<BoxCollider2D>().enabled = false;
			}
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
			NPCScript bNpc = briol.GetComponent<NPCScript> ();
			bNpc.m_bIsComingOutOfPlayer = true;
			bNpc.DHF_NPCMoveToGameobject (GameObject.Find ("Briol1"),false);
			bNpc.m_nFacingDir = (int)NPCScript.FACINGDIR.eUP;
			bNpc.ResetAnimFlagsExcept (bNpc.m_nFacingDir);
			briol.GetComponent<MessageHandler> ().BeginDialogue ("A0");
		}
			break;
		case "BoarRunoff":
		{
			GameObject player = GameObject.FindGameObjectWithTag ("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript> ().BindInput ();
			}
			m_goBoar2.SetActive(true);
			GameObject briol = GameObject.Find ("Briol");
			briol.transform.position = player.transform.position;
			briol.GetComponent<SpriteRenderer> ().enabled = true;
			NPCScript bNpc = briol.GetComponent<NPCScript> ();
			bNpc.m_bIsComingOutOfPlayer = true;
			bNpc.DHF_NPCMoveToGameobject (GameObject.Find ("BRIOL"),false);
			bNpc.m_nFacingDir = (int)NPCScript.FACINGDIR.eRIGHT;
			bNpc.ResetAnimFlagsExcept (bNpc.m_nFacingDir);
			briol.GetComponent<MessageHandler> ().BeginDialogue ("B0");
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
			go.GetComponent<DCScript>().SetBattleFieldBackgroundIter(2);

			
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
