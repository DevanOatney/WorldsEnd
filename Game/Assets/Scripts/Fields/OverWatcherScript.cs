using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

//Kind of a bloated script so going to list what this one handles.
//Each tick calculating how far the character has moved and the chance for them to get into a random battle, set m_nEncounterChance to 0 if no random battles should happen
//Increments the day/night cycle, set m_fRateOfChange to 0 if you don't want to have day/night in that scene
//Party menu, no variable to turn this off as I can't see a circumstance where I want that disabled.

public class OverWatcherScript : MonoBehaviour {

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
	public Texture2D m_MenuSelectedTexture;
	//iter for the options "Change, Optimize, Unequipped All"
	int m_nEquipmentChangeIndex = 0;
	//flags for if the character chooses chng/opt/uneq
	bool m_bChangeSelected = false;
	bool m_bOptimizeSelected = false;
	bool m_bClearSelected = false;
	//iter for selecting which slot of gear to change
	int m_nEquipmentSlotChangeIndex = 0;
	//flag for if a slot to change has been chosen
	bool m_bSlotChangeChosen = false;
	//iter for which item in the list is being highlighted
	int m_nItemSlotIndex = 0;

	public Texture2D m_ChestArmorIcon;
	public Texture2D m_LegArmorIcon;
	public Texture2D m_BeltArmorIcon;
	public Texture2D m_ShoulderArmorIcon;
	public Texture2D m_HelmArmorIcon;
	public Texture2D m_GloveArmorIcon;
	public Texture2D m_Trinket1Icon;
	public Texture2D m_Trinket2Icon;

	public Texture2D m_PoisonIcon;

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

	//iter for which menu option is highlighted
	int m_nMenuSelectionIndex = 0;
	//iter for which character is highlighted
	int m_nCharacterSelectionIndex = 0;
	//flag for if the highlighter should be on the characters
	bool m_bCharacterBeingSelected = false;
	//flag for showing a different screen other than the main party meny
	bool m_bShowDifferentMenuScreen = false;
	//delegate for handling the different party menu renderings
	delegate void m_delegate(DCScript.CharacterData g);
	//iterator for item type currently selected
	int m_nItemTypeIter = 0;
	//flag for if input should change item type or item
	bool m_bInputFlagForInventory = false;
	//flag for if the player presses enter on an item in the inventory (leads to window with use/discard
	bool  m_bInventoryItemSelected = false;
	//flag for if "use" is selected
	bool m_bInventoryUseSelected = false;
	//iter for which character is being selected during "use"
	int m_nInventoryUseIter = 0;
	//iter for use/discard
	int m_nInventoryItemSelectedIndex = 0;
	//iterator for item currently being selected.
	int m_nItemIter = 0;
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
			GameObject.Find("PersistantData").GetComponent<DCScript>().GetParty().Clear();
			GameObject Callan = Resources.Load<GameObject>("Units/Ally/Callan/Callan");
			Callan.GetComponent<CAllyBattleScript>().SetUnitStats();
			GameObject Briol = Resources.Load<GameObject>("Units/Ally/Briol/Briol");
			Briol.GetComponent<CAllyBattleScript>().SetUnitStats();
			DCScript.StatusEffect se = new DCScript.StatusEffect();
			//"Poison", 2, 20
			se.m_szEffectName = "Poison";
			se.m_nAmountOfTicks = 2;
			se.m_nHPMod = 20;
			se.m_lEffectedMembers.Add("Callan");
			se.m_lEffectedMembers.Add("Briol");
			pdata.GetComponent<DCScript>().AddStatusEffect(se);
		}
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
				//temp for status effect test
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
		CheckCheatKeys();

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


	void OnGUI()
	{
		//just make sure that the player will NOT move in the background.
		if(m_bShouldPause == true && m_bShowDifferentMenuScreen == false)
		{
			GUI.BeginGroup(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.9f, Screen.height * 0.9f));
			GUI.Box(new Rect(0, 0, Screen.width * 0.7f, Screen.height * 0.7f), "");

			float yOffset = Screen.height * 0.1f;
			float xOffset = 5.0f;
			float boxWidth = 125.0f;
			float boxHeight = 40.0f;
			float boxSpacing = 7.0f;
			GUI.Box(new Rect(xOffset, yOffset, boxWidth, boxHeight), "Inventory");
			yOffset += boxHeight + boxSpacing;
			GUI.Box(new Rect(xOffset, yOffset, boxWidth, boxHeight), "Status");
			yOffset += boxHeight + boxSpacing;
			GUI.Box(new Rect(xOffset, yOffset, boxWidth, boxHeight), "Equipment");
			yOffset += boxHeight + boxSpacing;
			if(SceneManager.GetActiveScene().name != "Regilance")
				GUI.contentColor = new Color(0.6f, 0.6f, 0.6f, 0.85f);
			GUI.Box(new Rect(xOffset, yOffset, boxWidth, boxHeight), "Save Game");
			GUI.contentColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			GUI.color= new Color(1.0f, 1.0f, 1.0f, 1.0f);
			yOffset += boxHeight + boxSpacing;
			GUI.Box(new Rect(xOffset, yOffset, boxWidth, boxHeight), "Quit to Main Menu");
			yOffset += boxHeight + boxSpacing;
			GUI.Box(new Rect(xOffset, yOffset, boxWidth, boxHeight), "Exit");


			//draw selector
			GUIStyle selectorStyle = new GUIStyle(GUI.skin.box);
			selectorStyle.normal.background = m_MenuSelectedTexture;
			//draw the selector box for the dialogue choice
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
			GUI.Box(new Rect(xOffset - 2.0f, 
			                 (Screen.height * 0.1f) + m_nMenuSelectionIndex * boxHeight + (boxSpacing * m_nMenuSelectionIndex) - 2.0f,
			                 boxWidth + 4.0f,
			                 boxHeight + 4), "",
			        		 selectorStyle);


			//display character information to the right of the menu options
			float characterXOffset = xOffset + boxWidth + 10;
			float characterYOffset = Screen.height * 0.1f;
			float charBoxWidth = 400;
			float charBoxHeight = 150;
			float charBoxSpacing = 7.0f;

			float textHeight = 18.0f;
			List<DCScript.CharacterData> lParty = dc.GetParty();
			int charCounter = 0;
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			foreach(DCScript.CharacterData g in lParty)
			{
				GUI.BeginGroup(new Rect(characterXOffset, (Screen.height * 0.1f) + charCounter * charBoxHeight + (charBoxSpacing * charCounter),
				                        charBoxWidth, charBoxHeight));
				//(draw twice to make it more visible (TODO: remove second draw call when UI texture is put in)
				GUI.Box(new Rect(0, 0, charBoxWidth,charBoxHeight), "");
				GUI.Box(new Rect(0,0,charBoxWidth,charBoxHeight), "");

				//name
				GUI.Label(new Rect(5.0f, 5.0f, 150, textHeight), g.m_szCharacterName);
				//level
				GUI.Label(new Rect(5.0f, 5.0f + textHeight, 150, textHeight), "Level : " + g.m_nLevel);
				//hp
				GUI.Label(new Rect(5.0f, 5.0f + textHeight*2, 150, textHeight), "Hit Points : " + g.m_nCurHP + " / " + g.m_nMaxHP);

				//portrait
				Texture2D tBust;
				if(GameObject.Find("Portraits Container").GetComponent<PortraitContainerScript>().m_dPortraits.TryGetValue(g.m_szCharacterName+1 , out tBust))
				{
					GUIStyle portrait = new GUIStyle(GUI.skin.box);
					portrait.normal.background = tBust;
					GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
					GUI.Box(new Rect(charBoxWidth * 0.7f, charBoxHeight * 0.1f, 96, 96),"", portrait);
				}

				//status effects
				List<DCScript.StatusEffect> ses = dc.GetStatusEffects();
				float seXOffset  = charBoxWidth * 0.7f;
				foreach(DCScript.StatusEffect se in ses)
				{
					foreach(string s in se.m_lEffectedMembers)
					{
						if(s == g.m_szCharacterName)
						{
							if(se.m_szEffectName == "Poison")
							{
								//this character is effected by this status, display the icon, increment the offset for the next icon (if it exists, lol)
								GUI.DrawTexture(new Rect(seXOffset,  charBoxHeight * 0.1f + 98, 32, 32), m_PoisonIcon);
								seXOffset += 32;
							}
						}
					}
				}

				//exp
				GUI.Label(new Rect(5.0f, charBoxHeight * 0.85f, 300, textHeight), "EXP : " + g.m_nCurrentEXP + "	\tNext: " + 
				          (1000 - g.m_nCurrentEXP).ToString());

				GUI.EndGroup();
				charCounter++;
			}

			if(m_bCharacterBeingSelected == true)
			{
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
				GUI.Box(new Rect(characterXOffset - 2.0f, 
				                 characterYOffset - 2.0f + m_nCharacterSelectionIndex * charBoxHeight + charBoxSpacing * m_nCharacterSelectionIndex,
				                 charBoxWidth + 4.0f,
				                 charBoxHeight + 4), "",
				        selectorStyle);
			}
			GUI.EndGroup();
		}
		else if(m_bShouldPause == true && m_bShowDifferentMenuScreen == true)
		{
			GUI.BeginGroup(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.9f, Screen.height * 0.9f));
			//GUI.Box(new Rect(0, 0, Screen.width * 0.7f, Screen.height * 0.7f), "");

			DCScript.CharacterData character = null;
			if(m_bCharacterBeingSelected == true)
			{
				List<DCScript.CharacterData> lParty = dc.GetParty();
				character = lParty[m_nCharacterSelectionIndex];
			}

			GUI.EndGroup();
		}
	}


	//doesn't use the passed in argument, but needed it to work with delegate
	void InventoryScreen(DCScript.CharacterData character)
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(m_bInventoryUseSelected == true)
			{
				m_bInventoryUseSelected = false;
				Input.ResetInputAxes();
			}
			else if(m_bInventoryItemSelected == true)
			{
				m_bInventoryItemSelected = false;
				Input.ResetInputAxes();
			}
			else if(m_bInputFlagForInventory == true)
			{
				m_bInputFlagForInventory = false;
				m_nItemIter = 0;
				Input.ResetInputAxes();
			}
			else
			{
				m_bShowDifferentMenuScreen = false;
				m_nCharacterSelectionIndex = 0;
				m_bCharacterBeingSelected = false;
				m_nItemTypeIter = 0;
				Input.ResetInputAxes();
			}
		}
		else if(Input.GetKeyDown(KeyCode.Return))
		{
			if(m_bInputFlagForInventory == true)
			{
				if(m_bInventoryItemSelected == false)
				{
					m_bInventoryItemSelected = true;
					List<ItemLibrary.CharactersItems> theInv = new List<ItemLibrary.CharactersItems>();
					theInv = GetItemsOfType(m_nItemTypeIter);
					if(theInv[m_nItemIter].m_nItemType < (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL || theInv[m_nItemIter].m_nItemType > (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL)
						m_nInventoryItemSelectedIndex = 1;
					Input.ResetInputAxes();
				}
				else if(m_bInventoryUseSelected == false)
				{
					//selecting use/discard
					if(m_nInventoryItemSelectedIndex == 0)
					{
						//use
						m_bInventoryUseSelected = true;
					}
					else if(m_nInventoryItemSelectedIndex == 1)
					{
						//discard
						List<ItemLibrary.CharactersItems> theInv = new List<ItemLibrary.CharactersItems>();
						theInv = GetItemsOfType(m_nItemTypeIter);
						m_bInventoryItemSelected = false;
						dc.m_lItemLibrary.RemoveItemAll(theInv[m_nItemIter]);
						theInv.RemoveAt(m_nItemIter);
						if(m_nItemIter >= theInv.Count)
							m_nItemIter--;
						if(theInv.Count <= 0)
						{
							m_bInputFlagForInventory = false;
							m_bInventoryItemSelected = false;
						}

					}
					Input.ResetInputAxes();
				}
				else
				{
					//using an item on a character.
					List<ItemLibrary.CharactersItems> theInv = new List<ItemLibrary.CharactersItems>();
					theInv = GetItemsOfType(m_nItemTypeIter);
					ItemLibrary.ItemData ni = dc.m_lItemLibrary.GetItemFromDictionary(theInv[m_nItemIter].m_szItemName);
					switch(ni.m_szDescription)
					{
					case "Cures Poison.":
					{
						//check to see if it's healing all targets or just one.
						if(ni.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL)
						{
							//check to see if anyone is effected by poison
							int removeIter = -1;
							int counter = 0;
							foreach(DCScript.StatusEffect se in dc.GetStatusEffects())
							{
								if(se.m_szEffectName == "Poison")
								{
									removeIter = counter;
									se.m_lEffectedMembers.Clear();
								}
								counter++;
							}
							if(removeIter != -1)
							{
								//some people were effected by poison, decrement the item count by one, remove the status effect.

								//is this the last of this item?
								if(theInv[m_nItemIter].m_nItemCount == 1)
								{
									m_bInventoryUseSelected = false;
									m_bInventoryItemSelected = false;
									dc.m_lItemLibrary.RemoveItem(theInv[m_nItemIter]);
									theInv.RemoveAt(m_nItemIter);
									if(m_nItemIter >= theInv.Count)
										m_nItemIter--;
									else if(theInv.Count == 0)
										m_bInputFlagForInventory = false;
								}
								//this isn't the last item? just remove one of it then.
								else
									dc.m_lItemLibrary.RemoveItem(theInv[m_nItemIter]);
								//remove the status effect from the party
								dc.GetStatusEffects().RemoveAt(removeIter);
								GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().RemoveStatusEffect("Poison");
							}
							else
							{
								//no unit was effected by this status, do nothing
							}
							
						}
						else
						{
							//only trying to remove poison from a single target
							//check to see if anyone is effected by poison
							int removeIter = -1;
							int counter = 0;
							bool effectFound = false;
							foreach(DCScript.StatusEffect se in dc.GetStatusEffects())
							{
								if(se.m_szEffectName == "Poison")
								{
									removeIter = counter;
									if(se.m_lEffectedMembers.Remove(dc.GetParty()[m_nInventoryUseIter].m_szCharacterName) == true)
									{
										//this unit WAS effected by the status
										effectFound = true;
									}
								}
								counter++;
							}
							if(removeIter != -1 && effectFound == true)
							{
								//is this the last of this item?
								if(theInv[m_nItemIter].m_nItemCount == 1)
								{
									m_bInventoryUseSelected = false;
									m_bInventoryItemSelected = false;
									dc.m_lItemLibrary.RemoveItem(theInv[m_nItemIter]);
									theInv.RemoveAt(m_nItemIter);
									if(m_nItemIter >= theInv.Count)
										m_nItemIter--;
									else if(theInv.Count == 0)
										m_bInputFlagForInventory = false;
								}
								//this isn't the last item? just remove one of it then.
								else
									dc.m_lItemLibrary.RemoveItem(theInv[m_nItemIter]);
								//if there are no more effect members, remove the status effect from the party.
								if(dc.GetStatusEffects()[removeIter].m_lEffectedMembers.Count == 0)
								{
									dc.GetStatusEffects().RemoveAt(removeIter);
									GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().RemoveStatusEffect("Poison");
								}
							}
							else
							{
								//no unit had the status effect we're trying to remove.
							}

						}

					}
						break;
					case "Cures Stone.":
					{
					}
						break;
					case "Cures Paralyze":
					{
					}
						break;
					case "Cures Ailments.":
					{
					}
						break;
					default:
					{
						//If we landed in here it means that it's just a heal item.

						int healingAmnt = ni.m_nHPMod;
						//check to see if it's healing all targets or just one.
						if(ni.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL)
						{
							//yep, heal everyone in the party, then go back to the inventory screen.
							foreach(DCScript.CharacterData unit in dc.GetParty())
							{
								unit.m_nCurHP += healingAmnt;
								if(unit.m_nCurHP > unit.m_nMaxHP)
									unit.m_nCurHP = unit.m_nMaxHP;
							}
						}
						else
						{
							//heal whichever unit is selected
							DCScript.CharacterData unit = dc.GetParty()[m_nInventoryUseIter];
							unit.m_nCurHP += healingAmnt;
							if(unit.m_nCurHP > unit.m_nMaxHP)
								unit.m_nCurHP = unit.m_nMaxHP;
						}

						//is this the last of this item?
						if(theInv[m_nItemIter].m_nItemCount == 1)
						{
							m_bInventoryUseSelected = false;
							m_bInventoryItemSelected = false;
							dc.m_lItemLibrary.RemoveItem(theInv[m_nItemIter]);
							theInv.RemoveAt(m_nItemIter);
							if(m_nItemIter >= theInv.Count)
								m_nItemIter--;
							else if(theInv.Count == 0)
								m_bInputFlagForInventory = false;
						}
						//this isn't the last item? just remove one of it then.
						else
							dc.m_lItemLibrary.RemoveItem(theInv[m_nItemIter]);



					}
						break;
					}

				}
			}
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			if(m_bInventoryItemSelected == true)
			{
				//first check to see if we're already using the item, or still deciding whether it's use/discard
				if(m_bInventoryUseSelected == false)
				{
					//first check the item to see if it's even useable, if it's not, the only option is to discard
					List<ItemLibrary.CharactersItems> theInv = new List<ItemLibrary.CharactersItems>();
					theInv = GetItemsOfType(m_nItemTypeIter);
					if(theInv[m_nItemIter].m_nItemType >= (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL && theInv[m_nItemIter].m_nItemType <= (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL)
					{
						m_nInventoryItemSelectedIndex--;
						if(m_nInventoryItemSelectedIndex < 0)
							m_nInventoryItemSelectedIndex = 1;
					}
					else
					{
						//not a useable item, discard is the only option
						m_nInventoryItemSelectedIndex = 1;
					}
				}
				else
				{
					//player has selected use item, cycle through the players in the party.
					m_nInventoryUseIter--;
					if(m_nInventoryUseIter < 0)
						m_nInventoryUseIter = dc.GetParty().Count - 1;
				}
				Input.ResetInputAxes();
			}
		}
		else if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			if(m_bInventoryItemSelected == true)
			{
				if(m_bInventoryUseSelected == false)
				{
					//first check the item to see if it's even useable, if it's not, the only option is to discard
					List<ItemLibrary.CharactersItems> theInv = new List<ItemLibrary.CharactersItems>();
					theInv = GetItemsOfType(m_nItemTypeIter);
					if(theInv[m_nItemIter].m_nItemType >= (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL && theInv[m_nItemIter].m_nItemType <= (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL)
					{
						m_nInventoryItemSelectedIndex++;
						if(m_nInventoryItemSelectedIndex > 1)
							m_nInventoryItemSelectedIndex = 0;
					}
					else
					{
						//not a useable item, discard is the only option
						m_nInventoryItemSelectedIndex = 1;
					}
				}
				else
				{
					//player has selected use item, cycle through the players in the party.
					m_nInventoryUseIter++;
					if(m_nInventoryUseIter >= dc.GetParty().Count)
						m_nInventoryUseIter = 0;
				}
				Input.ResetInputAxes();
			}
		}
		GUI.Box(new Rect(0, 0, Screen.width * 0.7f, Screen.height * 0.7f), "");
		GUI.Box(new Rect(0, 0, Screen.width * 0.7f, Screen.height * 0.7f), "");
		GUI.Box(new Rect(0, 0, Screen.width * 0.7f, Screen.height * 0.7f), "");
		//render item type icons.
		float iconXOffset = 50.0f;
		float iconWidth = 40.0f;
		float iconWidthSpacing = 25.0f;
		float iconYOffset = 25.0f;
		for(int i = 0; i < m_tItemTypeTextures.Length; ++i)
		{
			GUI.DrawTexture(new Rect(iconXOffset, iconYOffset, iconWidth, iconWidth), m_tItemTypeTextures[i]);
			iconXOffset += iconWidth + iconWidthSpacing;
		}
		//draw a line underneath the item icons
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
		GUI.DrawTexture(new Rect(0, iconYOffset+iconWidth+2, Screen.width * 0.7f, 0.5f), m_MenuSelectedTexture);
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		//Display in text the current item type being selected to the left and below the line
		string szTypeName = "";
		switch(m_nItemTypeIter)
		{
		case 0:
			szTypeName = "Useable Items";
			break;
		case 1:
			szTypeName = "Armor";
			break;
		case 2:
			szTypeName = "Trinkets";
			break;
		case 3:
			szTypeName = "Junk";
			break;
		case 4:
			szTypeName = "Key Items";
			break;
		}
		GUI.Label(new Rect(2, iconYOffset+iconWidth+8, 100, 21), szTypeName);
		//draw selector
		GUIStyle selectorStyle = new GUIStyle(GUI.skin.box);
		selectorStyle.normal.background = m_MenuSelectedTexture;
		//draw the selector box for the dialogue choice
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
		GUI.Box(new Rect( 48 + iconWidth*m_nItemTypeIter + iconWidthSpacing*m_nItemTypeIter, 
		                 iconYOffset - 2.0f,
		                 iconWidth + 4.0f,
		                 iconWidth + 4.0f), "",
		        selectorStyle);
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);



		
		//create the current inv list based on what item type is currently being selected
		List<ItemLibrary.CharactersItems> inv = new List<ItemLibrary.CharactersItems>();
		inv = GetItemsOfType(m_nItemTypeIter);
		if(m_bInputFlagForInventory == false)
		{
			//if an item type hasn't been selected yet.
			if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
			{
				m_nItemTypeIter++;
				if(m_nItemTypeIter >= 5)
					m_nItemTypeIter = 0;
			}
			else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
			{
				m_nItemTypeIter--;
				if(m_nItemTypeIter < 0)
					m_nItemTypeIter = 4;
			}
			else if(Input.GetKeyDown(KeyCode.Return))
			{
				if(inv.Count > 0)
					m_bInputFlagForInventory = true;
			}
			Input.ResetInputAxes();
		}
		else if(m_bInventoryItemSelected == false)
		{
			//if an item type has been selected
			if(Input.GetKeyDown(KeyCode.DownArrow))
			{
				if(m_nItemIter + 3 < inv.Count)
					m_nItemIter += 3;
			}
			else if(Input.GetKeyDown(KeyCode.UpArrow))
			{
				if(m_nItemIter - 3 >= 0)
					m_nItemIter -= 3;
			}
			else if(Input.GetKeyDown(KeyCode.LeftArrow))
			{
				if(m_nItemIter-1 >= 0)
					m_nItemIter--;
			}
			else if(Input.GetKeyDown(KeyCode.RightArrow))
			{
				if(m_nItemIter +1 < inv.Count)
					m_nItemIter++;
			}



			Input.ResetInputAxes();
		}


		//Display the items

		float baseXOffset = 100.0f;
		float characterYOffset = 100.0f;
		float characterXOffset = baseXOffset;
		foreach(ItemLibrary.CharactersItems item in inv)
		{
			if(characterXOffset > baseXOffset + 500)
			{
				characterXOffset = baseXOffset;
				characterYOffset += 20;
			}
			GUI.Label(new Rect(characterXOffset, characterYOffset, 100, 21.0f), item.m_szItemName);
			characterXOffset += 100;
			GUI.Label(new Rect(characterXOffset, characterYOffset, 100, 21.0f), item.m_nItemCount.ToString());
			characterXOffset += 100;

		}


		if(m_bInputFlagForInventory == true)
		{
			//draw selector for which item is currently being selected
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
			GUI.Box(new Rect(baseXOffset + 200*(m_nItemIter % 3) - 2, 
			                 baseXOffset + 21*((int)(m_nItemIter/3)) - 2,
			                 115 + 4.0f,
			                 21 + 4.0f), "",
			        selectorStyle);
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);


			//Draw the box for the description
			GUI.Box(new Rect(Screen.width * 0.1f, Screen.height * 0.7f, 500, 30), "");
			GUI.Box(new Rect(Screen.width * 0.1f, Screen.height * 0.7f, 500, 30), "");
			ItemLibrary.ItemData item = dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemIter].m_szItemName);
			GUI.Label(new Rect(Screen.width * 0.1f + 5, Screen.height * 0.7f + 2, 500, 21), item.m_szDescription);



			//code for use/discard
			if(m_bInventoryItemSelected == true)
			{
				//draw background for use/discard
				GUI.Box(new Rect(Screen.width * 0.1f, Screen.height * 0.3f, 150, 150), "");

				//"use"
				GUI.Label(new Rect(Screen.width * 0.1f + 7, Screen.height * 0.3f + 2, 75, 21), "Use");
				//"discard"
				GUI.Label(new Rect(Screen.width * 0.1f + 7, Screen.height * 0.3f + 23, 75, 21), "Discard");

				//draw selector for use/discard
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
				GUI.Box(new Rect(Screen.width * 0.1f + 5, 
				                 Screen.height * 0.3f + 21*((int)m_nInventoryItemSelectedIndex),
				                 120,
				                 21), "",
				        selectorStyle);
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);


				if(m_bInventoryUseSelected == true)
				{
					//character has chosen to use the item, display characters for the player to decide which character to use the item on
					//display character information to the right of the menu options
					float XOffset = Screen.width * 0.1f + 5;
					float YOffset = Screen.height * 0.05f;
					float charBoxWidth = 400;
					float charBoxHeight = 150;
					float charBoxSpacing = 7.0f;
					
					float textHeight = 18.0f;
					List<DCScript.CharacterData> lParty = dc.GetParty();
					int charCounter = 0;
					GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
					foreach(DCScript.CharacterData g in lParty)
					{
						GUI.BeginGroup(new Rect(XOffset, YOffset + charCounter * charBoxHeight + (charBoxSpacing * charCounter),
						                        charBoxWidth, charBoxHeight));
						//(draw twice to make it more visible (TODO: remove second draw call when UI texture is put in)
						GUI.Box(new Rect(0, 0, charBoxWidth,charBoxHeight), "");
						GUI.Box(new Rect(0,0,charBoxWidth,charBoxHeight), "");
						
						//name
						GUI.Label(new Rect(5.0f, 5.0f, 150, textHeight), g.m_szCharacterName);
						//level
						GUI.Label(new Rect(5.0f, 5.0f + textHeight, 150, textHeight), "Level : " + g.m_nLevel);
						//hp
						GUI.Label(new Rect(5.0f, 5.0f + textHeight*2, 150, textHeight), "Hit Points : " + g.m_nCurHP + " / " + g.m_nMaxHP);
						
						//portrait
						Texture2D tBust;
						if(GameObject.Find("Portraits Container").GetComponent<PortraitContainerScript>().m_dPortraits.TryGetValue(g.m_szCharacterName+1 , out tBust))
						{
							GUIStyle portrait = new GUIStyle(GUI.skin.box);
							portrait.normal.background = tBust;
							GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
							GUI.Box(new Rect(charBoxWidth * 0.7f, charBoxHeight * 0.1f, 96, 96),"", portrait);
						}
						
						//status effects
						List<DCScript.StatusEffect> ses = dc.GetStatusEffects();
						float seXOffset  = charBoxWidth * 0.7f;
						foreach(DCScript.StatusEffect se in ses)
						{
							foreach(string s in se.m_lEffectedMembers)
							{
								if(s == g.m_szCharacterName)
								{
									if(se.m_szEffectName == "Poison")
									{
										//this character is effected by this status, display the icon, increment the offset for the next icon (if it exists, lol)
										GUI.DrawTexture(new Rect(seXOffset,  charBoxHeight * 0.1f + 98, 32, 32), m_PoisonIcon);
										seXOffset += 32;
									}
								}
							}
						}
						
						//exp
						GUI.Label(new Rect(5.0f, charBoxHeight * 0.85f, 300, textHeight), "EXP : " + g.m_nCurrentEXP + "	\tNext: " + 
						          (1000 - g.m_nCurrentEXP).ToString());
						
						GUI.EndGroup();
						charCounter++;
					}


					//draw selector for use on character
					GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
					GUI.Box(new Rect(XOffset + 5, 
					                 YOffset + charBoxHeight*m_nInventoryUseIter + charBoxSpacing * m_nInventoryUseIter,
					                 charBoxWidth - 10,
					                 charBoxHeight), "",
					        selectorStyle);
					GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				}
			}
		}

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


	void StatusScreen(DCScript.CharacterData character)
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			m_bShowDifferentMenuScreen = false;
			Input.ResetInputAxes();
		}
		GUI.Box(new Rect(0, 0, Screen.width * 0.7f, Screen.height * 0.7f), "");
		GUI.Box(new Rect(0, 0, Screen.width * 0.7f, Screen.height * 0.7f), "");
		GUI.Box(new Rect(0, 0, Screen.width * 0.7f, Screen.height * 0.7f), "");

		//display character information on the top left
		float characterXOffset = (Screen.width * 0.1f);
		float characterYOffset = (Screen.height * 0.05f);
		float charBoxSpacing = 1.0f;
		float textHeight = 18.0f;

		GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		//name
		GUI.Label(new Rect(characterXOffset, characterYOffset, 
		                   150, textHeight), 
		          			character.m_szCharacterName);
		characterYOffset += (charBoxSpacing + textHeight);
		//LEVEL 
		GUI.Label(new Rect(characterXOffset, characterYOffset, 
		                   150, textHeight), 
		          "Level : " + character.m_nLevel);
		characterYOffset += (charBoxSpacing + textHeight);
		//Race
		GUI.Label(new Rect(characterXOffset, characterYOffset, 
		                   150, textHeight+2), 
		          "[" + character.m_szCharacterRace + " ]");
		characterYOffset += (charBoxSpacing + textHeight);
		//Class Type
		GUI.Label(new Rect(characterXOffset, characterYOffset, 
		                  150, textHeight+3), 
		          "Class : " + character.m_szCharacterClassType);


		GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
		//draw slashed line
		characterYOffset += (charBoxSpacing + textHeight + 10);
		Matrix4x4 catchMatrix = GUI.matrix;
		GUIUtility.RotateAroundPivot(-45.0f, new Vector2(0, 0));
		GUI.DrawTexture(new Rect(-75, 240, 300.0f, 0.5f), m_MenuSelectedTexture);
		GUI.matrix = catchMatrix;

		//Draw line underneath class type to seperate from stats
		GUI.DrawTexture(new Rect(5, (int)characterYOffset + 5, 200.0f, 0.5f), m_MenuSelectedTexture);
		characterYOffset += (charBoxSpacing + textHeight);

		characterXOffset = 5;
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		//HP
		GUI.Label(new Rect(characterXOffset, characterYOffset, 
		                   150, textHeight), 
		          "HP : " + character.m_nCurHP + " / " + character.m_nMaxHP);
		characterYOffset += (textHeight);
		//STR
		GUI.Label(new Rect(characterXOffset, characterYOffset, 
		                   150, textHeight), 
		          "STR : " + character.m_nSTR);
		characterYOffset += (textHeight);
		//DEF
		GUI.Label(new Rect(characterXOffset, characterYOffset, 
		                   150, textHeight), 
		          "DEF : " + character.m_nDEF);
		characterYOffset += (textHeight);
		//SPD
		GUI.Label(new Rect(characterXOffset, characterYOffset, 
		                   150, textHeight), 
		          "SPD : " + character.m_nSPD);
		characterYOffset = (Screen.height * 0.05f) + (int)(charBoxSpacing + textHeight);

		//Current EXP
		characterXOffset = Screen.width * 0.6f;
		GUI.Label(new Rect(characterXOffset, characterYOffset, 
		                   150, textHeight), 
		          "CUR EXP: " + character.m_nCurrentEXP);
		characterYOffset += textHeight + charBoxSpacing;
		//Exp to level
		GUI.Label(new Rect(characterXOffset-25, characterYOffset, 
		                   150, textHeight), 
		          "Till Next Level: " + (1000- character.m_nCurrentEXP).ToString());

		characterYOffset += textHeight + charBoxSpacing;
		//Draw line under NEXT LEVEL
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
		GUI.DrawTexture(new Rect(characterXOffset - 30, (int)characterYOffset + 5, 150.0f, 0.5f), m_MenuSelectedTexture);

		GUI.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
		GUI.DrawTexture(new Rect(characterXOffset - 30, (int)characterYOffset + textHeight*4 + charBoxSpacing, 75.0f, 0.5f), m_MenuSelectedTexture);

		GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		characterYOffset += textHeight*3 + charBoxSpacing;
		GUI.Label(new Rect(characterXOffset-25, characterYOffset, 
		                   150, textHeight+3), 
		          "Equipment");
		characterYOffset += textHeight*2 + charBoxSpacing;

		//Helm Armor
		GUI.DrawTexture(new Rect(characterXOffset-40, characterYOffset, 40, 40), m_HelmArmorIcon);
		if(character.m_idHelmSlot != null)
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), character.m_idHelmSlot.m_szItemName);
		else
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), "--EMPTY--");
		characterYOffset += textHeight*2 + charBoxSpacing;

		//Shoulder Armor
		GUI.DrawTexture(new Rect(characterXOffset-40, characterYOffset, 40, 40), m_ShoulderArmorIcon);
		if(character.m_idShoulderSlot != null)
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), character.m_idShoulderSlot.m_szItemName);
		else
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), "--EMPTY--");
		characterYOffset += textHeight*2 + charBoxSpacing;

		//Chest Armor
		GUI.DrawTexture(new Rect(characterXOffset-40, characterYOffset, 40, 40), m_ChestArmorIcon);
		if(character.m_idChestSlot != null)
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), character.m_idChestSlot.m_szItemName);
		else
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), "--EMPTY--");
		characterYOffset += textHeight*2 + charBoxSpacing;

		//Glove Armor
		GUI.DrawTexture(new Rect(characterXOffset-40, characterYOffset, 40, 40), m_GloveArmorIcon);
		if(character.m_idGloveSlot != null)
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), character.m_idGloveSlot.m_szItemName);
		else
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), "--EMPTY--");
		characterYOffset += textHeight*2 + charBoxSpacing;

		//Belt Armor
		GUI.DrawTexture(new Rect(characterXOffset-40, characterYOffset, 40, 40), m_BeltArmorIcon);
		if(character.m_idBeltSlot != null)
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), character.m_idBeltSlot.m_szItemName);
		else
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), "--EMPTY--");
		characterYOffset += textHeight*2 + charBoxSpacing;

		//Leg Armor
		GUI.DrawTexture(new Rect(characterXOffset-40, characterYOffset, 40, 40), m_LegArmorIcon);
		if(character.m_idLegSlot != null)
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), character.m_idLegSlot.m_szItemName);
		else
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), "--EMPTY--");
		characterYOffset += textHeight*2 + charBoxSpacing;

		//Trinket1
		GUI.DrawTexture(new Rect(characterXOffset-40, characterYOffset, 40, 40), m_Trinket1Icon);
		if(character.m_idTrinket1 != null)
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), character.m_idTrinket1.m_szItemName);
		else
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), "--EMPTY--");
		characterYOffset += textHeight*2 + charBoxSpacing;

		//Trinket2
		GUI.DrawTexture(new Rect(characterXOffset-40, characterYOffset, 40, 40), m_Trinket2Icon);
		if(character.m_idTrinket2 != null)
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), character.m_idTrinket2.m_szItemName);
		else
			GUI.Label(new Rect(characterXOffset, characterYOffset, 150, textHeight+3), "--EMPTY--");


		//Character Bio
		characterXOffset = 20;
		characterYOffset = Screen.height * 0.5f;
		GUI.Label(new Rect(characterXOffset, characterYOffset, 
		                   150, textHeight), 
		          "Character Bio: ");
		GUI.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
		characterYOffset += textHeight;
		GUI.DrawTexture(new Rect(characterXOffset, (int)characterYOffset, 75.0f, 0.5f), m_MenuSelectedTexture);
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		GUI.Label(new Rect(characterXOffset, characterYOffset, 
		                   700, textHeight+3), 
		          character.m_szCharacterBio);

		//Draw Character
		GUIStyle portrait = new GUIStyle(GUI.skin.box);
		GameObject unit = Resources.Load<GameObject>("Units/Ally/" + character.m_szCharacterName + "/" + character.m_szCharacterName);
		portrait.normal.background = unit.GetComponent<CAllyBattleScript>().m_tLargeBust;
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		GUI.Box(new Rect(Screen.width * 0.25f, Screen.height *0.2f, 134, 256),"", portrait);
	}

	void AdjustStats(ItemLibrary.ItemData item, bool bRemove)
	{
		if(item != null)
		{
			DCScript.CharacterData c = null;
			if(m_bCharacterBeingSelected == true)
			{
				List<DCScript.CharacterData> lParty = dc.GetParty();
				c = lParty[m_nCharacterSelectionIndex];
			}
			if(c != null)
			{
				if(bRemove == true)
				{
					int diff = c.m_nMaxHP - c.m_nCurHP;
					c.m_nMaxHP -= item.m_nHPMod;
					c.m_nCurHP = c.m_nMaxHP - diff;
					c.m_nSTR -= item.m_nPowMod;
					c.m_nDEF -= item.m_nDefMod;
					c.m_nSPD -= item.m_nSpdMod;
					c.m_nEVA -= item.m_nEvaMod;
					c.m_nHIT -= item.m_nHitMod;
				}
				else
				{
					int diff = c.m_nMaxHP - c.m_nCurHP;
					c.m_nMaxHP += item.m_nHPMod;
					c.m_nCurHP = c.m_nMaxHP - diff;
					c.m_nSTR += item.m_nPowMod;
					c.m_nDEF += item.m_nDefMod;
					c.m_nSPD += item.m_nSpdMod;
					c.m_nEVA += item.m_nEvaMod;
					c.m_nHIT += item.m_nHitMod;
				}
			}
		}
	}

	void EquipmentScreen(DCScript.CharacterData character)
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(m_bSlotChangeChosen == true)
			{
				m_bSlotChangeChosen = false;
			}
			else if(m_bChangeSelected == true)
			{
				m_nEquipmentSlotChangeIndex = 0;
				m_bChangeSelected = false;
			}
			else
			{
				m_nEquipmentChangeIndex = 0;
				m_nEquipmentSlotChangeIndex = 0;
				m_bShowDifferentMenuScreen = false;
			}
			Input.ResetInputAxes();
		}
		if(Input.GetKeyDown(KeyCode.Return))
		{
			if(m_bChangeSelected == false && m_bOptimizeSelected == false && m_bClearSelected == false)
			{
				//haven't selected an option yet, select an option.
				if(m_nEquipmentChangeIndex == 0)
					m_bChangeSelected = true;
				else if(m_nEquipmentChangeIndex == 1)
					m_bOptimizeSelected = true;
				else if(m_nEquipmentChangeIndex == 2)
					m_bClearSelected = true;
			}
			else if(m_bSlotChangeChosen == false)
			{
				//choosing a slot to change item equipped
				m_bSlotChangeChosen = true;
			}
			else
			{
				//attempting to equip currently highlighted item
				int nItemType = m_nEquipmentSlotChangeIndex + (int)BaseItemScript.ITEM_TYPES.eHELMARMOR;
				if(nItemType > (int)BaseItemScript.ITEM_TYPES.eTRINKET)
					nItemType = (int)BaseItemScript.ITEM_TYPES.eTRINKET;
				List<ItemLibrary.CharactersItems> inv = new List<ItemLibrary.CharactersItems>();
				foreach(ItemLibrary.CharactersItems item in dc.m_lItemLibrary.m_lInventory)
				{
					if(nItemType == item.m_nItemType)
						inv.Add(item);
				}
				if(inv.Count > 0)
				{
					//there is something to equipped and they're wanting it to be equipped, unnequip current item, equip highlighted item
					switch(m_nEquipmentSlotChangeIndex)
					{
						//which slot is about to get replaced?
					case 0:
						//helm
						if(character.m_idHelmSlot != null)
						{
							//character already has something equipped, put it in the inventory and replace.
							ItemLibrary.CharactersItems item = new ItemLibrary.CharactersItems();
							item.m_nItemCount = 1;
							item.m_nItemType = character.m_idHelmSlot.m_nItemType;
							item.m_szItemName = character.m_idHelmSlot.m_szItemName;
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							dc.m_lItemLibrary.AddItem(item);
							AdjustStats(character.m_idHelmSlot, true);
							character.m_idHelmSlot = (ItemLibrary.ArmorData)dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idHelmSlot, false);
						}
						else
						{
							//character doesn't have anything in that slot, remove item from inventory and put it on character
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							character.m_idHelmSlot = (ItemLibrary.ArmorData)dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idHelmSlot, false);
						}
						break;
					case 1:
						//shoulders
						if(character.m_idShoulderSlot != null)
						{
							//character already has something equipped, put it in the inventory and replace.
							ItemLibrary.CharactersItems item = new ItemLibrary.CharactersItems();
							item.m_nItemCount = 1;
							item.m_nItemType = character.m_idShoulderSlot.m_nItemType;
							item.m_szItemName = character.m_idShoulderSlot.m_szItemName;
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							dc.m_lItemLibrary.AddItem(item);
							AdjustStats(character.m_idShoulderSlot, true);
							character.m_idShoulderSlot = (ItemLibrary.ArmorData)dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idShoulderSlot, false);
						}
						else
						{
							//character doesn't have anything in that slot, remove item from inventory and put it on character
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							character.m_idShoulderSlot = (ItemLibrary.ArmorData)dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idShoulderSlot, false);
						}

						break;
					case 2:
						//chest
						if(character.m_idChestSlot != null)
						{
							//character already has something equipped, put it in the inventory and replace.
							ItemLibrary.CharactersItems item = new ItemLibrary.CharactersItems();
							item.m_nItemCount = 1;
							item.m_nItemType = character.m_idChestSlot.m_nItemType;
							item.m_szItemName = character.m_idChestSlot.m_szItemName;
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							dc.m_lItemLibrary.AddItem(item);
							AdjustStats(character.m_idChestSlot, true);
							character.m_idChestSlot = (ItemLibrary.ArmorData)dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idChestSlot, false);
						}
						else
						{
							//character doesn't have anything in that slot, remove item from inventory and put it on character
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							character.m_idChestSlot = (ItemLibrary.ArmorData)dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idChestSlot, false);
						}
						break;
					case 3:
						//gloves
						if(character.m_idGloveSlot != null)
						{
							//character already has something equipped, put it in the inventory and replace.
							ItemLibrary.CharactersItems item = new ItemLibrary.CharactersItems();
							item.m_nItemCount = 1;
							item.m_nItemType = character.m_idGloveSlot.m_nItemType;
							item.m_szItemName = character.m_idGloveSlot.m_szItemName;
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							dc.m_lItemLibrary.AddItem(item);
							AdjustStats(character.m_idGloveSlot, true);
							character.m_idGloveSlot = (ItemLibrary.ArmorData)dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idGloveSlot, false);
						}
						else
						{
							//character doesn't have anything in that slot, remove item from inventory and put it on character
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							character.m_idGloveSlot = (ItemLibrary.ArmorData)dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idGloveSlot, false);
						}
						break;
					case 4:
						//belt
						if(character.m_idBeltSlot != null)
						{
							//character already has something equipped, put it in the inventory and replace.
							ItemLibrary.CharactersItems item = new ItemLibrary.CharactersItems();
							item.m_nItemCount = 1;
							item.m_nItemType = character.m_idBeltSlot.m_nItemType;
							item.m_szItemName = character.m_idBeltSlot.m_szItemName;
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							dc.m_lItemLibrary.AddItem(item);
							AdjustStats(character.m_idBeltSlot, true);
							character.m_idBeltSlot = (ItemLibrary.ArmorData)dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idBeltSlot, false);
						}
						else
						{
							//character doesn't have anything in that slot, remove item from inventory and put it on character
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							character.m_idBeltSlot = (ItemLibrary.ArmorData)dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idBeltSlot, false);
						}
						break;
					case 5:
						//legs
						if(character.m_idLegSlot != null)
						{
							//character already has something equipped, put it in the inventory and replace.
							ItemLibrary.CharactersItems item = new ItemLibrary.CharactersItems();
							item.m_nItemCount = 1;
							item.m_nItemType = character.m_idLegSlot.m_nItemType;
							item.m_szItemName = character.m_idLegSlot.m_szItemName;
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							dc.m_lItemLibrary.AddItem(item);
							AdjustStats(character.m_idLegSlot, true);
							character.m_idLegSlot = (ItemLibrary.ArmorData)dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idLegSlot, false);
						}
						else
						{
							//character doesn't have anything in that slot, remove item from inventory and put it on character
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							character.m_idLegSlot = (ItemLibrary.ArmorData)dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idLegSlot, false);
						}
						break;
					case 6:
						//trinket1
						if(character.m_idTrinket1 != null)
						{
							//character already has something equipped, put it in the inventory and replace.
							ItemLibrary.CharactersItems item = new ItemLibrary.CharactersItems();
							item.m_nItemCount = 1;
							item.m_nItemType = character.m_idTrinket1.m_nItemType;
							item.m_szItemName = character.m_idTrinket1.m_szItemName;
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							dc.m_lItemLibrary.AddItem(item);
							AdjustStats(character.m_idTrinket1, true);
							character.m_idTrinket1 = dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idTrinket1, false);
						}
						else
						{
							//character doesn't have anything in that slot, remove item from inventory and put it on character
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							character.m_idTrinket1 = dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idTrinket1, false);
						}
						break;
					case 7:
						//trinket2
						if(character.m_idTrinket2 != null)
						{
							//character already has something equipped, put it in the inventory and replace.
							ItemLibrary.CharactersItems item = new ItemLibrary.CharactersItems();
							item.m_nItemCount = 1;
							item.m_nItemType = character.m_idTrinket2.m_nItemType;
							item.m_szItemName = character.m_idTrinket2.m_szItemName;
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							dc.m_lItemLibrary.AddItem(item);
							AdjustStats(character.m_idTrinket2, true);
							character.m_idTrinket2 = dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idTrinket2, false);
						}
						else
						{
							//character doesn't have anything in that slot, remove item from inventory and put it on character
							dc.m_lItemLibrary.RemoveItem(inv[m_nItemSlotIndex]);
							character.m_idTrinket2 = dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
							AdjustStats(character.m_idTrinket2, false);
						}
						break;
					}
					m_nItemSlotIndex = 0;
					m_bSlotChangeChosen = false;
				}
			}
			Input.ResetInputAxes();
		}
		GUI.Box(new Rect(0, 0, Screen.width * 0.7f, Screen.height * 0.85f), "");
		float xOffset = Screen.width * 0.1f;
		float yOffset = 10.0f;
		float textHeight = 18.0f;
		float charSpacing = 3.0f;

		//Draw box for Change/optimize/unequip all
		GUI.Box(new Rect(xOffset, yOffset, 200, 100), "");
		yOffset += 10.0f;
		xOffset += 15.0f;
		GUI.Label(new Rect(xOffset, yOffset, 200, textHeight + charSpacing), "Change");
		yOffset += textHeight + charSpacing;
		GUI.Label(new Rect(xOffset, yOffset, 200, textHeight + charSpacing), "Optimize");
		yOffset += textHeight + charSpacing;
		GUI.Label(new Rect(xOffset, yOffset, 200, textHeight + charSpacing), "Unequipped All");

		if(m_bChangeSelected == false)
		{
			//Draw selector for change/opt/unequip
			GUIStyle selectorStyle = new GUIStyle(GUI.skin.box);
			selectorStyle.normal.background = m_MenuSelectedTexture;
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
			GUI.Label(new Rect(Screen.width * 0.1f, 20 + textHeight * m_nEquipmentChangeIndex + charSpacing * m_nEquipmentChangeIndex, 180, textHeight + charSpacing),"", selectorStyle);
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
		else if(m_bChangeSelected == true && m_bSlotChangeChosen == false)
		{
			float yOff = 120.0f;
			//draw selector over which slot to change
			GUIStyle selectorStyle = new GUIStyle(GUI.skin.box);
			selectorStyle.normal.background = m_MenuSelectedTexture;
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
			GUI.Label(new Rect(Screen.width * 0.15f,
			                   yOff + 45.0f * m_nEquipmentSlotChangeIndex, 
			                   100, 45),"", selectorStyle);
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
		else if(m_bSlotChangeChosen == true)
		{
			float yOff = 120.0f;
			//draw selector over which slot to change
			GUIStyle selectorStyle = new GUIStyle(GUI.skin.box);
			selectorStyle.normal.background = m_MenuSelectedTexture;
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
			GUI.Label(new Rect(Screen.width * 0.1f + 2.0f,
			                   yOff + textHeight* m_nItemSlotIndex, 
			                   100, textHeight+2),"", selectorStyle);
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
		//optimize
		if(m_bOptimizeSelected == true)
		{
			m_bOptimizeSelected = false;
			int nItemType = (int)BaseItemScript.ITEM_TYPES.eHELMARMOR;
			//helm
			ItemLibrary.ItemData bis = GetBISFromInventory(nItemType);
			if(bis != null)
			{
				if(character.m_idHelmSlot != null)
				{
					int eSum = bis.m_nHPMod + bis.m_nPowMod + bis.m_nDefMod + bis.m_nSpdMod;
					int nSum = character.m_idHelmSlot.m_nHPMod + character.m_idHelmSlot.m_nPowMod + character.m_idHelmSlot.m_nDefMod +  character.m_idHelmSlot.m_nSpdMod;
					if(eSum > nSum)
						character.m_idHelmSlot = (ItemLibrary.ArmorData)bis;
				}
				else
				{
					Debug.Log(bis.m_szItemName);
					character.m_idHelmSlot = (ItemLibrary.ArmorData)bis;
				}
			}

			//Shoulders
			nItemType = (int)BaseItemScript.ITEM_TYPES.eSHOULDERARMOR;
			bis = GetBISFromInventory(nItemType);
			if(bis != null)
			{
				if(character.m_idShoulderSlot != null)
				{
					int eSum = bis.m_nHPMod + bis.m_nPowMod + bis.m_nDefMod + bis.m_nSpdMod;
					int nSum = character.m_idShoulderSlot.m_nHPMod + character.m_idShoulderSlot.m_nPowMod + character.m_idShoulderSlot.m_nDefMod +  character.m_idShoulderSlot.m_nSpdMod;
					if(eSum > nSum)
						character.m_idShoulderSlot = (ItemLibrary.ArmorData)bis;
				}
				else
					character.m_idShoulderSlot = (ItemLibrary.ArmorData)bis;
			}

			//Chest
			nItemType = (int)BaseItemScript.ITEM_TYPES.eCHESTARMOR;
			bis = GetBISFromInventory(nItemType);
			if(bis != null)
			{
				if(character.m_idChestSlot != null)
				{
					int eSum = bis.m_nHPMod + bis.m_nPowMod + bis.m_nDefMod + bis.m_nSpdMod;
					int nSum = character.m_idChestSlot.m_nHPMod + character.m_idChestSlot.m_nPowMod + character.m_idChestSlot.m_nDefMod +  character.m_idChestSlot.m_nSpdMod;
					if(eSum > nSum)
						character.m_idChestSlot = (ItemLibrary.ArmorData)bis;
				}
				else
					character.m_idChestSlot = (ItemLibrary.ArmorData)bis;
			}

			//Gloves
			nItemType = (int)BaseItemScript.ITEM_TYPES.eGLOVEARMOR;
			bis = GetBISFromInventory(nItemType);
			if(bis != null)
			{
				if(character.m_idGloveSlot != null)
				{
					int eSum = bis.m_nHPMod + bis.m_nPowMod + bis.m_nDefMod + bis.m_nSpdMod;
					int nSum = character.m_idGloveSlot.m_nHPMod + character.m_idGloveSlot.m_nPowMod + character.m_idGloveSlot.m_nDefMod +  character.m_idGloveSlot.m_nSpdMod;
					if(eSum > nSum)
						character.m_idGloveSlot = (ItemLibrary.ArmorData)bis;
				}
				else
					character.m_idGloveSlot = (ItemLibrary.ArmorData)bis;
			}

			//Belt
			nItemType = (int)BaseItemScript.ITEM_TYPES.eBELTARMOR;
			bis = GetBISFromInventory(nItemType);
			if(bis != null)
			{
				if(character.m_idBeltSlot != null)
				{
					int eSum = bis.m_nHPMod + bis.m_nPowMod + bis.m_nDefMod + bis.m_nSpdMod;
					int nSum = character.m_idBeltSlot.m_nHPMod + character.m_idBeltSlot.m_nPowMod + character.m_idBeltSlot.m_nDefMod +  character.m_idBeltSlot.m_nSpdMod;
					if(eSum > nSum)
						character.m_idBeltSlot = (ItemLibrary.ArmorData)bis;
				}
				else
					character.m_idBeltSlot = (ItemLibrary.ArmorData)bis;
			}

			//Leg
			nItemType = (int)BaseItemScript.ITEM_TYPES.eLEGARMOR;
			bis = GetBISFromInventory(nItemType);
			if(bis != null)
			{
				if(character.m_idLegSlot != null)
				{
					int eSum = bis.m_nHPMod + bis.m_nPowMod + bis.m_nDefMod + bis.m_nSpdMod;
					int nSum = character.m_idLegSlot.m_nHPMod + character.m_idLegSlot.m_nPowMod + character.m_idLegSlot.m_nDefMod +  character.m_idLegSlot.m_nSpdMod;
					if(eSum > nSum)
						character.m_idLegSlot = (ItemLibrary.ArmorData)bis;
				}
				else
					character.m_idLegSlot = (ItemLibrary.ArmorData)bis;
			}


		}
		//clear
		else if(m_bClearSelected == true)
		{
			m_bClearSelected = false;
			ItemLibrary.CharactersItems item = new ItemLibrary.CharactersItems();

			//helm
			if(character.m_idHelmSlot != null)
			{
				item.m_szItemName = character.m_idHelmSlot.m_szItemName;
				item.m_nItemType = character.m_idHelmSlot.m_nItemType;
				item.m_nItemCount = 1;
				dc.m_lItemLibrary.AddItem(item);
				AdjustStats(character.m_idHelmSlot, true);
				character.m_idHelmSlot = null;
			}
			//shoulder
			if(character.m_idShoulderSlot != null)
			{
				item.m_szItemName = character.m_idShoulderSlot.m_szItemName;
				item.m_nItemType = character.m_idShoulderSlot.m_nItemType;
				item.m_nItemCount = 1;
				dc.m_lItemLibrary.AddItem(item);
				AdjustStats(character.m_idShoulderSlot, true);
				character.m_idShoulderSlot = null;
			}
			//chest
			if(character.m_idChestSlot != null)
			{
				item.m_szItemName = character.m_idChestSlot.m_szItemName;
				item.m_nItemType = character.m_idChestSlot.m_nItemType;
				item.m_nItemCount = 1;
				dc.m_lItemLibrary.AddItem(item);
				AdjustStats(character.m_idChestSlot, true);
				character.m_idChestSlot = null;
			}
			//gloves
			if(character.m_idGloveSlot != null)
			{
				item.m_szItemName = character.m_idGloveSlot.m_szItemName;
				item.m_nItemType = character.m_idGloveSlot.m_nItemType;
				item.m_nItemCount = 1;
				dc.m_lItemLibrary.AddItem(item);
				AdjustStats(character.m_idGloveSlot, true);
				character.m_idGloveSlot = null;
			}
			//belt
			if(character.m_idBeltSlot != null)
			{
				item.m_szItemName = character.m_idBeltSlot.m_szItemName;
				item.m_nItemType = character.m_idBeltSlot.m_nItemType;
				item.m_nItemCount = 1;
				dc.m_lItemLibrary.AddItem(item);
				AdjustStats(character.m_idBeltSlot, true);
				character.m_idBeltSlot = null;
			}
			//legs
			if(character.m_idLegSlot != null)
			{
				item.m_szItemName = character.m_idLegSlot.m_szItemName;
				item.m_nItemType = character.m_idLegSlot.m_nItemType;
				item.m_nItemCount = 1;
				dc.m_lItemLibrary.AddItem(item);
				AdjustStats(character.m_idLegSlot, true);
				character.m_idLegSlot = null;
			}
			//trinket 1
			if(character.m_idTrinket1 != null)
			{
				item.m_szItemName = character.m_idTrinket1.m_szItemName;
				item.m_nItemType = character.m_idTrinket1.m_nItemType;
				item.m_nItemCount = 1;
				dc.m_lItemLibrary.AddItem(item);
				AdjustStats(character.m_idTrinket1, true);
				character.m_idTrinket1 = null;
			}
			//trinket 2
			if(character.m_idTrinket2 != null)
			{
				item.m_szItemName = character.m_idTrinket2.m_szItemName;
				item.m_nItemType = character.m_idTrinket2.m_nItemType;
				item.m_nItemCount = 1;
				dc.m_lItemLibrary.AddItem(item);
				AdjustStats(character.m_idTrinket2, true);
				character.m_idTrinket2 = null;
			}

		}



		//Draw box for icons/slots on the left (need to handle changing this box for item types)
		xOffset = Screen.width * 0.1f;
		yOffset = 110.0f;
		GUI.Box(new Rect(xOffset, yOffset, 200, 400), "");
		yOffset += 10.0f;
		xOffset += 15.0f;
		if(m_bSlotChangeChosen == false)
		{
			//Helm Armor
			GUI.Label(new Rect(xOffset, yOffset, 40, 40), m_HelmArmorIcon);
			xOffset += 45.0f;
			if(character.m_idHelmSlot != null)
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), character.m_idHelmSlot.m_szItemName); 
			else
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), "--EMPTY--"); 
			yOffset += 45.0f;
			xOffset -= 45.0f;
			
			//Shoulder Armor
			GUI.Label(new Rect(xOffset, yOffset, 40, 40), m_ShoulderArmorIcon);
			xOffset += 45.0f;
			if(character.m_idShoulderSlot != null)
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), character.m_idShoulderSlot.m_szItemName); 
			else
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), "--EMPTY--"); 
			yOffset += 45.0f;
			xOffset -= 45.0f;
			
			//chest armor
			GUI.Label(new Rect(xOffset, yOffset, 40, 40), m_ChestArmorIcon);
			xOffset += 45.0f;
			if(character.m_idChestSlot != null)
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), character.m_idChestSlot.m_szItemName); 
			else
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), "--EMPTY--"); 
			yOffset += 45.0f;
			xOffset -= 45.0f;
			
			//Glove Armor
			GUI.Label(new Rect(xOffset, yOffset, 40, 40), m_GloveArmorIcon);
			xOffset += 45.0f;
			if(character.m_idGloveSlot != null)
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), character.m_idGloveSlot.m_szItemName); 
			else
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), "--EMPTY--"); 
			yOffset += 45.0f;
			xOffset -= 45.0f;
			
			//Belt Armor
			GUI.Label(new Rect(xOffset, yOffset, 40, 40), m_BeltArmorIcon);
			xOffset += 45.0f;
			if(character.m_idBeltSlot != null)
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), character.m_idBeltSlot.m_szItemName); 
			else
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), "--EMPTY--"); 
			yOffset += 45.0f;
			xOffset -= 45.0f;
			
			//Leg Armor
			GUI.Label(new Rect(xOffset, yOffset, 40, 40), m_LegArmorIcon);
			xOffset += 45.0f;
			if(character.m_idLegSlot != null)
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), character.m_idLegSlot.m_szItemName); 
			else
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), "--EMPTY--"); 
			yOffset += 45.0f;
			xOffset -= 45.0f;
			
			//Trinket 1
			GUI.Label(new Rect(xOffset, yOffset, 40, 40), m_Trinket1Icon);
			xOffset += 45.0f;
			if(character.m_idTrinket1 != null)
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), character.m_idTrinket1.m_szItemName); 
			else
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), "--EMPTY--"); 
			yOffset += 45.0f;
			xOffset -= 45.0f;
			
			//Trinket 2
			GUI.Label(new Rect(xOffset, yOffset, 40, 40), m_Trinket2Icon);
			xOffset += 45.0f;
			if(character.m_idTrinket2 != null)
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), character.m_idTrinket2.m_szItemName); 
			else
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), "--EMPTY--"); 
		}
		else if(m_bSlotChangeChosen == true)
		{
			//list out the items in the character's inventory that could be worn in this slot.
			int nItemType = m_nEquipmentSlotChangeIndex + (int)BaseItemScript.ITEM_TYPES.eHELMARMOR;
			if(nItemType > (int)BaseItemScript.ITEM_TYPES.eTRINKET)
				nItemType = (int)BaseItemScript.ITEM_TYPES.eTRINKET;
			List<ItemLibrary.CharactersItems> inv = new List<ItemLibrary.CharactersItems>();
			foreach(ItemLibrary.CharactersItems item in dc.m_lItemLibrary.m_lInventory)
			{
				if(nItemType == item.m_nItemType)
					inv.Add(item);
			}
			foreach(ItemLibrary.CharactersItems item in inv)
			{
				int prevMod = -1;
				switch(m_nEquipmentSlotChangeIndex)
				{
				case 0:
					if(character.m_idHelmSlot != null)
						prevMod = character.m_idHelmSlot.m_nHPMod + character.m_idHelmSlot.m_nPowMod + character.m_idHelmSlot.m_nDefMod +  character.m_idHelmSlot.m_nSpdMod;
					break;
				case 1:
					if(character.m_idShoulderSlot != null)
						prevMod = character.m_idShoulderSlot.m_nHPMod + character.m_idShoulderSlot.m_nPowMod + character.m_idShoulderSlot.m_nDefMod +  character.m_idShoulderSlot.m_nSpdMod;
					break;
				case 2:
					if(character.m_idChestSlot != null)
						prevMod = character.m_idChestSlot.m_nHPMod + character.m_idChestSlot.m_nPowMod + character.m_idChestSlot.m_nDefMod +  character.m_idChestSlot.m_nSpdMod;
					break;
				case 3:
					if(character.m_idGloveSlot != null)
						prevMod = character.m_idGloveSlot.m_nHPMod + character.m_idGloveSlot.m_nPowMod + character.m_idGloveSlot.m_nDefMod +  character.m_idGloveSlot.m_nSpdMod;
					break;
				case 4:
					if(character.m_idBeltSlot != null)
						prevMod = character.m_idBeltSlot.m_nHPMod + character.m_idBeltSlot.m_nPowMod + character.m_idBeltSlot.m_nDefMod +  character.m_idBeltSlot.m_nSpdMod;
					break;
				case 5:
					if(character.m_idLegSlot != null)
						prevMod = character.m_idLegSlot.m_nHPMod + character.m_idLegSlot.m_nPowMod + character.m_idLegSlot.m_nDefMod +  character.m_idLegSlot.m_nSpdMod;
					break;
				case 6:
					if(character.m_idTrinket1 != null)
						prevMod = character.m_idTrinket1.m_nHPMod + character.m_idTrinket1.m_nPowMod + character.m_idTrinket1.m_nDefMod +  character.m_idTrinket1.m_nSpdMod;
					break;
				case 7:
					if(character.m_idTrinket2 != null)
						prevMod = character.m_idTrinket2.m_nHPMod + character.m_idTrinket2.m_nPowMod + character.m_idTrinket2.m_nDefMod +  character.m_idTrinket2.m_nSpdMod;
					break;
				}
				int newMod = dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName).m_nHPMod + 
							 dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName).m_nPowMod + 
							 dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName).m_nDefMod + 
							 dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName).m_nSpdMod;
				Color toolTip = Color.white;
				if(prevMod > newMod)
					toolTip = Color.red;
				if(prevMod < newMod)
					toolTip = Color.green;
				GUI.color = toolTip;
				GUI.Label(new Rect(xOffset, yOffset, 150, textHeight + charSpacing), item.m_szItemName);
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				yOffset += textHeight + charSpacing;
			}

			if(inv.Count > 0)
			{
				ItemLibrary.ItemData item = dc.m_lItemLibrary.GetItemFromDictionary(inv[m_nItemSlotIndex].m_szItemName);
				GUI.Box(new Rect(Screen.width * 0.1f, 510.0f, 500, 100), item.m_szDescription);
			}
		}
		//Draw box for character picture, icon boxes for slots
		xOffset = 200.0f + Screen.width * 0.1f;
		yOffset = 10.0f;
		GUI.Box(new Rect(xOffset, yOffset, 300, 500), "");
		//Draw Character
		GUIStyle portrait = new GUIStyle(GUI.skin.box);
		GameObject unit = Resources.Load<GameObject>("Units/Ally/" + character.m_szCharacterName + "/" + character.m_szCharacterName);
		portrait.normal.background = unit.GetComponent<CAllyBattleScript>().m_tLargeBust;
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		GUI.Box(new Rect(Screen.width * 0.4f - 30, Screen.height *0.2f, 185, 300),"", portrait);
		//box for helm
		GUI.Box(new Rect(Screen.width * 0.4f+50, Screen.height * 0.2f-20, 40, 40), "");
		//if there is a helm equipped, show it's icon inside the box
		if(character.m_idHelmSlot != null)
			GUI.Label(new Rect(Screen.width * 0.4f+50, Screen.height * 0.2f-20, 40, 40), m_HelmArmorIcon);
		//box for shoulders
		GUI.Box(new Rect(Screen.width * 0.4f, Screen.height * 0.2f+40, 40, 40), "");
		if(character.m_idShoulderSlot != null)
			GUI.Label(new Rect(Screen.width * 0.4f, Screen.height * 0.2f+40, 40, 40), m_ShoulderArmorIcon);
		//box for chest
		GUI.Box(new Rect(Screen.width * 0.4f + 100, Screen.height * 0.2f + 80, 40, 40), "");
		if(character.m_idChestSlot != null)
			GUI.Label(new Rect(Screen.width * 0.4f + 100, Screen.height * 0.2f + 80, 40, 40), m_ChestArmorIcon);
		//box for gloves
		GUI.Box(new Rect(Screen.width * 0.4f-60, Screen.height * 0.2f+120, 40, 40), "");
		if(character.m_idGloveSlot != null)
			GUI.Label(new Rect(Screen.width * 0.4f-60, Screen.height * 0.2f+120, 40, 40), m_GloveArmorIcon);
		//box for belt
		GUI.Box(new Rect(Screen.width * 0.4f+50, Screen.height * 0.2f + 250, 40, 40), "");
		if(character.m_idBeltSlot != null)
			GUI.Label(new Rect(Screen.width * 0.4f+50, Screen.height * 0.2f + 250, 40, 40), m_BeltArmorIcon);
		//box for trinket1
		GUI.Box (new Rect(Screen.width * 0.4f, Screen.height * 0.2f + 300, 40, 40), "");
		if(character.m_idTrinket1 != null)
			GUI.Label(new Rect(Screen.width * 0.4f, Screen.height * 0.2f + 300, 40, 40), m_Trinket1Icon);
		//box for trinket2
		GUI.Box (new Rect(Screen.width * 0.4f+60, Screen.height * 0.2f + 300, 40, 40), "");
		if(character.m_idTrinket2 != null)
			GUI.Label(new Rect(Screen.width * 0.4f+60, Screen.height * 0.2f + 300, 40, 40), m_Trinket2Icon);

		//Draw box for item description (blank if no item selected)
		xOffset = Screen.width * 0.1f;
		yOffset = 510.0f;

		if(m_bChangeSelected == true && m_bSlotChangeChosen == false)
		{
			//a slot is currently being highlighted, the user hasn't selected a slot yet.
			int itemSelected = m_nEquipmentSlotChangeIndex + (int)BaseItemScript.ITEM_TYPES.eHELMARMOR;
			switch(itemSelected)
			{
			case (int)BaseItemScript.ITEM_TYPES.eHELMARMOR:
				if(character.m_idHelmSlot != null)
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), character.m_idHelmSlot.m_szDescription);
				else
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), "");
				break;
			case (int)BaseItemScript.ITEM_TYPES.eSHOULDERARMOR:
				if(character.m_idShoulderSlot != null)
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), character.m_idShoulderSlot.m_szDescription);
				else
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), "");
				break;
			case (int)BaseItemScript.ITEM_TYPES.eCHESTARMOR:
				if(character.m_idChestSlot != null)
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), character.m_idChestSlot.m_szDescription);
				else
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), "");
				break;
			case (int)BaseItemScript.ITEM_TYPES.eGLOVEARMOR:
				if(character.m_idGloveSlot != null)
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), character.m_idGloveSlot.m_szDescription);
				else
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), "");
				break;
			case (int)BaseItemScript.ITEM_TYPES.eBELTARMOR:
				if(character.m_idBeltSlot != null)
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), character.m_idBeltSlot.m_szDescription);
				else
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), "");
				break;
			case (int)BaseItemScript.ITEM_TYPES.eLEGARMOR:
				if(character.m_idLegSlot != null)
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), character.m_idLegSlot.m_szDescription);
				else
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), "");
				break;
			case (int)BaseItemScript.ITEM_TYPES.eTRINKET:
				if(character.m_idTrinket1 != null)
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), character.m_idTrinket1.m_szDescription);
				else
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), "");
				break;
			case (int)BaseItemScript.ITEM_TYPES.eTRINKET +1:
				if(character.m_idTrinket2 != null)
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), character.m_idTrinket2.m_szDescription);
				else
					GUI.Box(new Rect(xOffset, yOffset, 500, 100), "");
				break;
			}


		}
		else if(m_bSlotChangeChosen == true)
		{
			//Do nothing, rendering for this is handled above
		}
		else
		{
			//any other time, leave it blank.
			GUI.Box(new Rect(xOffset, yOffset, 500, 100), "");
		}

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
	//doesn't use the passed in argument, but needed it to work with delegate
	void SaveGame(DCScript.CharacterData character)
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			m_bShowDifferentMenuScreen = false;
			m_nCharacterSelectionIndex = 0;
			m_bCharacterBeingSelected = false;
		}
		GUI.Box(new Rect(0, 0, 200, 200), "");
	}
		

	void CheckCheatKeys()
	{
		if(Input.GetKeyUp(KeyCode.Keypad0))
		{
			//Greatly increase grate of day/night cycle rate to test and make sure it works, lol.
			Debug.Log("Let there be light!..or dark?");
			m_fRateOfChange = 0.5f;
			m_fTickBucket = 0.0f;
		}
		if(Input.GetKeyUp(KeyCode.Alpha0))
		{
			//hack to disable chance to get into an encounter
			Debug.Log("No combat allowed!");
			m_nEncounterChance = -1;
		}
		else if(Input.GetKeyUp(KeyCode.Alpha9))
		{
			//hack to turn encounters back on
			Debug.Log("Combat now allowed!");
			m_nEncounterChance = 10;
		}
	}



}
