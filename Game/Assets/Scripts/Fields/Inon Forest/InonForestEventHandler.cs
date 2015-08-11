using UnityEngine;
using System.Collections;

public class InonForestEventHandler : BaseEventSystemScript 
{
	DCScript ds;
	public GameObject[] Phase1_waypoints;
	// Use this for initialization
	void Start () 
	{
		ds = GameObject.Find("PersistantData").GetComponent<DCScript>();
		int result;
		if(ds.m_dStoryFlagField.TryGetValue("InonForest_EncounteredBoar", out result))
		{
			foreach(GameObject go in Phase1_waypoints)
				go.SetActive(false);
		}

	}
	case "ForestSceneStart";
	{
		GameObject player = GameObject.Find("Player");
		player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eIDLE);
		player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
		GameObject.Find("Callan").GetComponent<MessageHandler>().BeginDialogue("A1");
		GameObject.Find("ForestSceneStart").GetComponent<BoxCollider2D>().enabled = false;
	}
	break;
	case "BoarBossEndDialogue";
	{
		ds.m_dStoryFlagField.Add("BoarBossPart1Finished", 1);
		Camera.main.GetComponent<CameraFollowTarget>().m_bShouldSwirl = true;
		Camera.main.GetComponent<VEffects>().SendMessage("StartBlur");
		StartBossBattle()
	}
	break;
	void StartBossBattle()
	{
		GameObject dc = GameObject.Find("PersistantData");
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
			go.GetComponent<DCScript>().SetBattleFieldBackgroundIter(2);

			Application.LoadLevel("Battle_Scene");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	override public void HandleEvent(string eventID)
	{
		switch(eventID)
		{

		}
	}

	override public void WaypointTriggered(Collider2D c)
	{
		switch(c.name)
		{
		case "EncounterBoarWaypoint":
			c.enabled = false;
			ds.m_dStoryFlagField.Add("InonForest_EncounteredBoar", 1);
			HandleEvent("ENCOUNTERED_BOAR");
			break;
		default:
			break;
		}
	}
}
