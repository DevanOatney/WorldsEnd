using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EventHandler : BaseEventSystemScript 
{
	float musicFadeTime = 4.0f;
	DCScript ds;

	public GameObject[] Phase1_waypoints;
	public GameObject[] Phase2_waypoints;
	public GameObject[] Phase3_waypoints;
	public AudioClip m_aBossIntroClip;
	public AudioClip m_aBossAfterFightClip;
	// Use this for initialization
	void Start () 
	{
		ds = GameObject.Find("PersistantData").GetComponent<DCScript>();


		int result;
		//When started check to see if the player has just returned from the first battle where the boss defeats him.  If so deactivate all of the Phase1 waypoints, and activate the Phase2
		if(ds.m_dStoryFlagField.TryGetValue("Primus_ReturnFromFixFight", out result))
		{
			//also during iterating through p1 waypoints, catch the waypoint where the boss should be and put him in that location
			foreach(GameObject wypnt in Phase1_waypoints)
			{
				if(wypnt.name == "Primus_BossApproach")
				{
					GameObject.Find("Boss_Field").transform.position = wypnt.transform.position;
				}
				wypnt.GetComponent<BoxCollider>().enabled = false;
			}
			//Yep, we're on to phase 2!
			foreach(GameObject wypnt in Phase2_waypoints)
				wypnt.GetComponent<BoxCollider>().enabled = true;
			//disable 3
			foreach(GameObject wypnt in Phase3_waypoints)
				wypnt.GetComponent<BoxCollider>().enabled = false;
		}
		else if(ds.m_dStoryFlagField.TryGetValue("DefeatedApplicant", out result))
		{
			foreach(GameObject wypnt in Phase1_waypoints)
			{
				if(wypnt.name == "Primus_BossApproach")
				{
					Destroy(GameObject.Find("Boss_Field"));
				}
				wypnt.GetComponent<BoxCollider>().enabled = false;
			}
			foreach(GameObject wypnt in Phase2_waypoints)
				wypnt.GetComponent<BoxCollider>().enabled = false;
			foreach(GameObject wypnt in Phase3_waypoints)
				wypnt.GetComponent<BoxCollider>().enabled = true;
		}
		else
		{
			foreach(GameObject wypnt in Phase1_waypoints)
				wypnt.GetComponent<BoxCollider>().enabled = true;
			//Still hasn't happpened, disable phase 2.
			foreach(GameObject wypnt in Phase2_waypoints)
				wypnt.GetComponent<BoxCollider>().enabled = false;
			foreach(GameObject wypnt in Phase3_waypoints)
				wypnt.GetComponent<BoxCollider>().enabled = false;
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	override public void HandleEvent(string eventID)
	{
		switch(eventID)
		{
		case "Primus_MattFirstApproach":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
				player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKRIGHT);
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
				FadeInOutSound obj = Camera.main.GetComponent<FadeInOutSound>();
				StartCoroutine(obj.FadeAudio(musicFadeTime, FadeInOutSound.Fade.Out));
				Input.ResetInputAxes();
			}
		}
			break;
		case "Primus_EnteredBossRoom":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eIDLE);
				player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
				Invoke("BeginBossMusic", 1.0f);
			}
			GameObject boss = GameObject.Find("Boss_Field");
			if(boss)
			{
				GetComponent<AudioSource>().PlayOneShot(m_aBossIntroClip, 0.5f + ds.m_fVoiceVolume);
				boss.GetComponent<BossScript>().SetState((int)BossScript.States.eWALKLEFT);
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
		}
			break;
		case "Primus_BossApproach":
		{
			Invoke("WaitForBossDialogue", 10.0f);
			GameObject boss = GameObject.Find("Boss_Field");
			boss.GetComponent<BossScript>().BeginAttack();
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
		}
			break;
		case "Primus_DefeatedFirst":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
			GetComponent<AudioSource>().PlayOneShot(m_aBossAfterFightClip, 0.5f + ds.m_fVoiceVolume);
			Invoke("StartDialogueAfterBossFight", 2.0f);

		}
			break;
		case "FinalBattle":
		{
			//start the fixed fight with the applicant
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
			Camera.main.GetComponent<VEffects>().SendMessage("StartBlur");
			Invoke ("StartBossBattle", 2.0f);
			GameObject dataCanister = GameObject.Find("PersistantData");
			if(dataCanister)
			{
				dataCanister.GetComponent<DCScript>().m_dStoryFlagField.Add("FixedFight", 1);
			}
		}
			break;
		case "WalkUp":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKUP);
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
		}
			break;
		case "WalkDown":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKDOWN);
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
		}
			break;
		case "Second_Fire":
		{
			GameObject dataCanister = GameObject.Find("PersistantData");
			if(dataCanister)
			{
				//dataCanister.GetComponent<DCScript>().m_dStoryFlagField.Add("ElementalAffinity", 1);
				//dataCanister.GetComponent<DCScript>().m_lSpellsKnown.Add("Pyre");
			}
			HandleEvent("ThirdStory");
		}
			break;
		case "ReadyToFight":
		{
			//start the fixed fight with the applicant
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
			Camera.main.GetComponent<VEffects>().SendMessage("StartBlur");
			Invoke ("StartBossBattle", 2.0f);
			GameObject dataCanister = GameObject.Find("PersistantData");
			if(dataCanister)
			{
				dataCanister.GetComponent<DCScript>().m_dStoryFlagField.Add("FixedFight", 1);
			}
		}
			break;
		case "BossDefeated":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
			GameObject messageSystem = GameObject.Find("Message System");
			if(messageSystem)
			{
				messageSystem.GetComponent<MessageHandler>().BeginDialogue(9);
			}
		}
			break;
		case "Victory":
		{
			Camera.main.SendMessage("fadeOut");
			Invoke("WinTheGame", 4.0f);
		}
			break;

		}
	}

	override public void WaypointTriggered(Collider c)
	{
		
			switch(c.name)
			{
				case "Primus_MattFirstApproach":
				{
					//player has walked into the boss room, have them walk to the right a little (they'll get caught by next
					//waypoint
					HandleEvent("Primus_MattFirstApproach");
					c.enabled = false;
					GameObject wypnt = GameObject.Find("Primus_EnteredBossRoom");
					if(wypnt)
						wypnt.GetComponent<BoxCollider>().enabled = true;
				}
				break;
				case "Primus_EnteredBossRoom":
				{
					//player has entered the boss room, stop player movement/input, turn off this waypoint, send message to boss to begin his script
					HandleEvent("Primus_EnteredBossRoom");
					c.enabled = false;
					GameObject wypnt = GameObject.Find("Primus_BossApproach");
					if(wypnt)
					wypnt.GetComponent<BoxCollider>().enabled = true;
				}
				break;
				
				case "Primus_BossApproach":
				{
					HandleEvent("Primus_BossApproach");
					c.enabled = false;
					GameObject wypnt = GameObject.Find("Primus_MattApproach");
					if(wypnt)
					wypnt.GetComponent<BoxCollider>().enabled = true;
				}
				break;

				case "Primus_DefeatedFirst":
				{
					HandleEvent("Primus_DefeatedFirst");
					c.enabled = false;
				}
				break;
				case "Primus_Victory":
				{
					HandleEvent("BossDefeated");
					c.enabled = false;
				}
				break;
				
			}
	}
	void WinTheGame()
	{
		Application.LoadLevel("CreditsScreen_Scene");
	}
	void WaitForBossDialogue()
	{
		GameObject messageSystem = GameObject.Find("Message System");
		if(messageSystem)
		{
			messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
		}
	}
	void StartDialogueAfterBossFight()
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if(player)
		{
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
		}
		GameObject messageSystem = GameObject.Find("Message System");
		if(messageSystem)
		{
			messageSystem.GetComponent<MessageHandler>().BeginDialogue(5);
		}
	}
	public void BeginBossMusic()
	{
		GameObject.Find("Background_Map4").GetComponent<AudioSource>().volume = GameObject.Find("PersistantData").GetComponent<DCScript>().m_fMusicVolume;
		GameObject.Find("Background_Map4").GetComponent<AudioSource>().Play();
	}
	public void WaitToTalkAfterSit()
	{
		
		GameObject messageSystem = GameObject.FindGameObjectWithTag("Message System");
		if(messageSystem)
		{
			messageSystem.GetComponent<MessageHandler>().m_bShouldDisplayDialogue = true;
			messageSystem.GetComponent<MessageHandler>().ChangeDialogueEvent("A6");
		}
	}
	void StartBossBattle()
	{
		GameObject dc = GameObject.Find("PersistantData");
		if(dc)
		{
			List<string> bossEncounter = new List<string>();
			bossEncounter.Add("CharacterReference");
			bossEncounter.Add("OtherApplicant");
			bossEncounter.Add("CharacterReference");
			//Set the names of the list of enemies the player is about to fight
			dc.GetComponent<DCScript>().SetEnemyNames(bossEncounter);
			//Set the position of the player before the battle starts
			GameObject go = GameObject.Find("PersistantData");
			GameObject m_goPlayer = GameObject.Find("Player");
			go.GetComponent<DCScript>().SetPreviousPosition(m_goPlayer.transform.position);
			go.GetComponent<DCScript>().SetPreviousFacingDirection(m_goPlayer.GetComponent<FieldPlayerMovementScript>().m_nFacingDir);
			go.GetComponent<DCScript>().SetPreviousFieldName(Application.loadedLevelName);
			Application.LoadLevel("Battle_Scene");
		}
	}
}
