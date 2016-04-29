using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

//Kind of a bloated script so going to list what this one handles.
//Each tick calculating how far the character has moved and the chance for them to get into a random battle, set m_nEncounterChance to 0 if no random battles should happen
//Increments the day/night cycle, set m_fRateOfChange to 0 if you don't want to have day/night in that scene
//Party menu, no variable to turn this off as I can't see a circumstance where I want that disabled.

public class OverWatcherScript : MonoBehaviour 
{

	GameObject m_goPlayer;
	Vector3 m_vLastPos;

	//The distance the player can go before there's a chance for a random battle
	float m_fThreshold = 0.2f;
	//The distance the player has gone since the last threshold tick
	float m_fDistanceStep = 0.0f;
	//The initial percent chance that an encounter will occur (0-100)
	public float m_nEncounterChance = 10;
	//The modifier for incrementing the encounter chance
	public float m_nEncounterTick = 1.2f;

	//Stuff for Menu screen
	//flag for stopping day/night tick
	bool m_bShouldPause = false;


	//Stuff for day/night cycle
	//increase to increase the rate of which it lightens/darkens each time step
	float m_fRateOfChange = 0.075f;
	//The maximum values in which the brightness can be adjusted
	float m_fMaxDecay = 1.5f;
	//The value impacting the brightness
	public float m_fBrightnessAdjuster;
	float m_fTickBucket = 0.05f;
	float m_fTickTimer = 0.0f;
	bool m_bDayTime = true;
	float m_fTimer = 0.0f;
	//the intial intensity of the camera on this map
	public float m_fInitialBrightness = 1.0f;

	//flag for if there is going to  be an encounter 
	bool m_bEncounterToHappen = false;
	public AudioClip m_acFoundEncounter;

	//iter for which background for battle during daytime
	public int nDayBattleBackgroundIter = 0;
	//iter for which background for battle during nighttime
	public int nNightBattleBackgroundIter = 1;

	//Textures for item type icons
	public Texture2D[] m_tItemTypeTextures;

	public GameObject m_goAudioPlayer;
	//game data
	DCScript dc;

	void Awake()
	{
		GameObject pdata = GameObject.Find("PersistantData");
		if(pdata == null)
		{
			
			//This is a debug play then.   Create a data canister, and put the main character in the party
			pdata = Instantiate(Resources.Load("Misc/PersistantData", typeof(GameObject))) as GameObject;
			pdata.name = pdata.name.Replace("(Clone)", "");
			dc = pdata.GetComponent<DCScript>();
		}
		else
			dc = GameObject.Find("PersistantData").GetComponent<DCScript>();
		if(CAudioHelper.Instance == null)
		{
			GameObject audioPlayer = (GameObject)Instantiate(m_goAudioPlayer);
			DontDestroyOnLoad(audioPlayer);
		}
	}
	// Use this for initialization
	void Start () 
	{
		m_goPlayer = GameObject.Find("Player");

		//check to see if the last scene was a battle, if it was the position data of where the player should go is in the datacanister. Also check if this was the last scene (loading game)
        if (dc.GetPreviousFieldName() == "Battle_Scene" || dc.GetPreviousFieldName() == SceneManager.GetActiveScene().name)
			m_goPlayer.transform.position = dc.GetPreviousPosition();
		//else, the player just entered the scene, put the player where the scene wants the player to start
		else
		{
			if(dc.GetStartingPos() == null && GameObject.Find("CallanStartPosition") != null)
				m_goPlayer.transform.position = GameObject.Find("CallanStartPosition").transform.position;
			else if( GameObject.Find(dc.GetStartingPos()) != null)
			{
				m_goPlayer.transform.position = GameObject.Find(dc.GetStartingPos()).transform.position;
			}
		}
		m_goPlayer.GetComponent<FieldPlayerMovementScript>().m_nFacingDir = dc.GetPreviousFacingDirection();
		m_vLastPos = m_goPlayer.transform.position;
		//set the previous scene to this one now
		dc.SetPreviousFieldName(SceneManager.GetActiveScene().name);
		Camera.main.GetComponent<Light>().intensity =dc.m_fBrightness + m_fInitialBrightness;

		//Fade in the music
		FadeInOutSound obj = Camera.main.GetComponent<FadeInOutSound>();
		StartCoroutine(obj.FadeAudio(2.0f, FadeInOutSound.Fade.In));

		//Adjust the master volume
		dc.SetMasterVolume();

	}

	void Encounter()
	{
		//Encounter!
		//grab the list of encounters, pick one of them at random, save the encounter information to the DataCanister, then load the randomBattle scene
		EncounterGroupLoaderScript es = gameObject.GetComponent<EncounterGroupLoaderScript>();
		if(es)
		{
			List<List<EncounterGroupLoaderScript.cEnemyData>> encGrps = new List<List<EncounterGroupLoaderScript.cEnemyData>>();
			if(m_bDayTime == true)
				encGrps = es.GetDayEncounterGroups();
			else
			{
				//Try to grab the night time enemy options
				encGrps = es.GetNightEncounterGroups();
				//if there are none, just grab the day time enemy choices
				if(encGrps == null)
					encGrps = es.GetDayEncounterGroups();
			}
			if(encGrps.Count > 0)
			{
				//adjust the status effect list.

				int rndmGrp = Random.Range(0, encGrps.Count);
				//Set the names of the list of enemies the player is about to fight
				dc.SetEnemyNames(encGrps[rndmGrp]);
				//Set the iter for which background to display during battle
				if(m_bDayTime == true)
					dc.SetBattleFieldBackgroundIter(nDayBattleBackgroundIter);
				else
					dc.SetBattleFieldBackgroundIter(nNightBattleBackgroundIter);
				//Set the position of the player before the battle starts
				dc.SetPreviousPosition(m_goPlayer.transform.position);
				dc.SetPreviousFacingDirection(m_goPlayer.GetComponent<FieldPlayerMovementScript>().m_nFacingDir);
				dc.SetPreviousFieldName(SceneManager.GetActiveScene().name);
                SceneManager.LoadScene("Battle_Scene");
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		//Check to see if there should be an encounter happening
		if(m_bEncounterToHappen == true)
		{
			if(Camera.main.GetComponent<VEffects>().fade >0.99f)
			{
				//I think it's over?
				Encounter();
			}
		}


		//Get the distance from the last tick
		Vector3 newPos = m_vLastPos - m_goPlayer.transform.position;
		float distance = newPos.sqrMagnitude;
		//don't add distance if the player is teleporting
		if(distance > 1.0f)
			distance = 0;
		//walking barely ever puts player in combat.. more bloodshed required!
		else if( distance < 0.0002 && distance > 0)
			distance = 0.0005f;
		//Increment the distance traveled
		m_fDistanceStep += distance;
		//Adjust the previous position
		m_vLastPos = m_goPlayer.transform.position;

		//Check to see if input is currently being allowed (to make sure we're not in an event/menu/something
		if(GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().GetAllowInput() == true)
		{
			//Check to make sure that encounters can happen
			if(gameObject.GetComponent<EncounterGroupLoaderScript>().m_bEncountersHappen == true)
			{
				//Check if it passed the threshold
				if(m_fDistanceStep >= m_fThreshold)
				{
					m_fDistanceStep = 0.0f;
					int randomChance = Random.Range(0, 100);
					//first check to see if Devan is a cheater and has turned off the chance to get into a fight
					if(m_nEncounterChance != -1)
					if(randomChance <= m_nEncounterChance)
					{
						m_bEncounterToHappen = true;
						Camera.main.GetComponent<CameraFollowTarget>().m_bShouldSwirl = true;
						Camera.main.GetComponent<VEffects>().SendMessage("StartBlur");
						GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
						Input.ResetInputAxes();
						m_nEncounterChance = 10;
						GetComponent<AudioSource>().PlayOneShot(m_acFoundEncounter, 0.5f + dc.m_fSFXVolume);
					}
					else
					{
						m_nEncounterChance += m_nEncounterChance * m_nEncounterTick;;
					}
				}
			}
		}
		//for day/night cycle
		#region Day and Night Cycle

		if(m_bShouldPause == false)
		{
			m_fTimer += Time.deltaTime;
			m_fTickTimer += Time.deltaTime;
			if(m_fTickTimer >= m_fTickBucket)
			{
				m_fBrightnessAdjuster  = Mathf.Sin(m_fTimer * m_fRateOfChange) * m_fMaxDecay;
				if(m_fBrightnessAdjuster > 0.25f)
					m_fBrightnessAdjuster = 0.25f;
			
				if(m_fBrightnessAdjuster > -0.5f)
					m_bDayTime = true;
				else
					m_bDayTime = false;
			

				Camera.main.GetComponent<Light>().intensity = m_fBrightnessAdjuster + dc.m_fBrightness + m_fInitialBrightness;
				m_fTickTimer = 0.0f;
			}
		}
		#endregion
	}


	
	List<ItemLibrary.CharactersItems> GetItemsOfType(int type)
	{
		List<ItemLibrary.CharactersItems> inv = new List<ItemLibrary.CharactersItems>();
		List<ItemLibrary.CharactersItems> fullInv =  dc.m_lItemLibrary.m_lInventory;
		foreach(ItemLibrary.CharactersItems item in fullInv)
		{
			// 0 - useable item, 1- Armor, 2- Trinkets, 3- Junk, 4: key
			//1-4 : useable item, 5 : weapon, 6: armor, 7: junk, 8: key
			switch(type)
			{
			case 0:
			{
				if(item.m_nItemType >= (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL && item.m_nItemType <= (int)BaseItemScript.ITEM_TYPES.eGROUP_DAMAGE)
					inv.Add(item);
			}
				break;
			case 1:
			{
				if(item.m_nItemType >= (int)BaseItemScript.ITEM_TYPES.eHELMARMOR && item.m_nItemType <= (int)BaseItemScript.ITEM_TYPES.eLEGARMOR)
					inv.Add(item);
			}
				break;
			case 2:
			{
				if(item.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eTRINKET)
					inv.Add(item);
			}
				break;
			case 3: 
			{
				if(item.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eJUNK)
					inv.Add(item);
			}
				break;
			case 4:
			{
				if(item.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eKEYITEM)
					inv.Add(item);
			}
				break;
			}
		}
		return inv;
	}



	//Helper function, retrieves the best in slot item of specified item type from the characters inventory
	ItemLibrary.ItemData GetBISFromInventory(int nItemType)
	{
		ItemLibrary.ItemData bis = null;
		int highestValue = -1;
		List<ItemLibrary.CharactersItems> inv = dc.m_lItemLibrary.m_lInventory;
		foreach(ItemLibrary.CharactersItems item in inv)
		{
			if(item.m_nItemType == nItemType)
			{
				ItemLibrary.ItemData temp = dc.m_lItemLibrary.GetItemFromDictionary(item.m_szItemName);
				int tSum = temp.m_nHPMod + temp.m_nPowMod + temp.m_nDefMod + temp.m_nSpdMod;
				if(tSum > highestValue)
				{
					highestValue = tSum;
					bis = temp;
				}
			}
		}
		return bis;
	}



}
