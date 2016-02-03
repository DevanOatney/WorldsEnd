using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CAllyBattleScript : UnitScript 
{
	public enum ALLY_STATES
	{
		DIALOGUE, //checks to see if there should be dialogue played (used for scripted battles or the tutorial, (Check to see if this fight has any special events, add them to dictionary, and if one happens 
		          //immediately, play it, but then move to ACTION_SELECTION state once it's done)
		ACTION_SELECTION, //player chooses between (Attack, Defend, Magic, Use Item, Switch, or Escape) (Cycle through options, select one, move to whichever state was selected)
		ATTACK_CHOSEN,  //player has chosen to attack, now needs to select which enemy to attack. (cycle through targets, select target, move to ATTACKING state)
		ATTACKING,		//player has selected target to attack, play animation, resolve attack, move unit back to point of origin, the move to STATUS_EFFECT state)
		USEITEM_CHOSEN, //player has chosen to use an item, display inventory on screen to allow player to select from useable items. (Cycle through items, select item, move to ITEM_PICKED)
		ITEM_PICKED,    //item has been picked, now needs to select the target to use this item on.  (Cycle through targets, select target, move to USING_ITEM)
		USING_ITEM,     //Player has chosen which item to use and what target to use it on, play animation, resolve item useage, then move to STATUS_EFFECT
		USEMAGIC_CHOSEN, //same as USEITEM_CHOSEN, but with magic, (cycle through and select with spell to cast, move to SPELL_PICKED)
		SPELL_PICKED,	 //same at ITEM_PICKED, but with magic, (Cycle through targets, select target, move to CASTING_SPELL)
		CASTING_SPELL,   //player has chosen a spell and a target, play animation, resolve spell effect, move to STATUS_EFFECT
		STATUS_EFFECTS	 //Cycle through status effects, tick off one cycle of the effects, remove/update effects, inform TURNWATCHER of end of turn and end your turn.
	}


	//Variables for choosing which action to do.
	int m_nMaxActionCount = 6; //(attack, defend, magic, use item, switch, escape)
	int m_nActionSelectionIndex = 0; //what is the player currently selecting in the main action selection
	int m_nMagicSelectionIndex = 0;  //which spell does the player have selected?
	Vector2 m_vMagicScrollPosition = Vector2.zero; //the position of the scroll bar in the spell list
	Vector2 m_vMagicSelectorPosition = Vector2.zero; //positional data for the selector in the spell list
	int m_nItemSelectionIndex = 0;   //which item does the player have selected?
	Vector2 m_vItemScrollPosition = Vector2.zero; //the position of the scroll bar in the inventory
	Vector2 m_vItemSelectorPosition = Vector2.zero; //positional data for the selector in the inventory
	float m_fTextHeight = 35.0f; //the height of the text for the inventory/spell list

	float m_fMovementSpeed = 8.0f; //The speed in which this unit moves accross the battlefield.
	bool m_bAmIDefending = false;  //flag for if the unit was defending the previous turn

	//hooks to components/gameobjects
	Animator m_aAnim;				
	public GameObject m_goFadingText; //for displaying damage taken (or any other fading text to float above this unit.
	DCScript m_dcPersistantData;
	TurnWatcherScript m_twTurnWatcher;
	public GameObject m_goShadowClone; //for shadow trails of this unit
	public Texture2D m_tLargeBust;     //the portrait of this character  (to be displayed during victory screen)
	public Texture2D m_t2dSelector;    //texture to display selector when choosing an action/spell/item
	public TextAsset m_taStartingStats;//The starting stats of this character when they are recruited.

	[HideInInspector]
	public List<string> m_lSpellList = new List<string>();  //Names of spells this unit can cast.

	float m_fShadowTimer = 0.0f; //when the last time a shadow clone was spawned
	float m_fShadowTimerBucket = 0.1f; //how frequently a shadow clone can spawn


	//Variables for unit stats/levels
	[HideInInspector]
	public int m_nExperienceToLevel = 1000;
	[HideInInspector]
	public int m_nCurrentExperience; //Each time the unit levels, reset this to 0

	[HideInInspector]
	public string m_szClassName;   //The class that this character is, this will adjust how the character gains stats when they level
	[HideInInspector]
	public ItemLibrary.ArmorData m_idHelmSlot;
	[HideInInspector]
	public ItemLibrary.ArmorData m_idShoulderSlot;
	[HideInInspector]
	public ItemLibrary.ArmorData m_idChestSlot;
	[HideInInspector]
	public ItemLibrary.ArmorData m_idGloveSlot;
	[HideInInspector]
	public ItemLibrary.ArmorData m_idBeltSlot;
	[HideInInspector]
	public ItemLibrary.ArmorData m_idLegSlot;
	[HideInInspector]
	public ItemLibrary.ItemData m_idTrinket1;
	[HideInInspector]
	public ItemLibrary.ItemData m_idTrinket2;


	// Use this for initialization
	void Start () 
	{
		m_dcPersistantData = GameObject.Find("PersistantData").GetComponent<DCScript>();
		m_twTurnWatcher = GameObject.Find("TurnWatcher").GetComponent<TurnWatcherScript>();
		m_aAnim = GetComponent<Animator>();
		m_vInitialPos = new Vector3();
		InitializeTargetReticle();
		m_nState = (int)ALLY_STATES.DIALOGUE;

		//Grab any status effects that are currently effecting this character.
		List<DCScript.StatusEffect> effects = GameObject.Find("PersistantData").GetComponent<DCScript>().GetStatusEffects();
		foreach(DCScript.StatusEffect se in effects)
		{
			foreach(string charName in se.m_lEffectedMembers)
			{
				if(charName == name)
				{
					switch(se.m_szName)
					{
					case "Poison":
						{
							m_poison.GetComponent<BattlePoisonEffectScript>().Initialize(gameObject, 1, se.m_nCount);
							m_lStatusEffects.Add(m_poison);
						}
						break;
					case "Paralyze":
						{
						}
						break;
					case "Stone":
						{
						}
						break;
					case "Confuse":
						{
						}
						break;
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void HandleMainStates()
	{
		switch(m_nState)
		{
		case (int)ALLY_STATES.DIALOGUE:
			{
				bool _bShouldAct = true;
				foreach(KeyValuePair<string,int> kvp in m_twTurnWatcher.m_dEventTriggers)
				{
					//check each key, if anything specific needs to be done, do it, else, move to action_selection
					switch(kvp.Key)
					{

					}
				}
				if(_bShouldAct == true)
				{
					m_nState = (int)ALLY_STATES.ACTION_SELECTION;
				}
			}
			break;
		case (int)ALLY_STATES.ACTION_SELECTION:
			{
				HandleActionSelectionInput();
			}
			break;
		case (int)ALLY_STATES.ATTACK_CHOSEN:
			{
			}
			break;
		case (int)ALLY_STATES.ATTACKING:
			{
			}
			break;
		case (int)ALLY_STATES.USEITEM_CHOSEN:
			{
			}
			break;
		case (int)ALLY_STATES.ITEM_PICKED:
			{
			}
			break;
		case (int)ALLY_STATES.USING_ITEM:    
			{
			}
			break;
	    case (int)ALLY_STATES.USEMAGIC_CHOSEN:
			{
			}
			break;
		case (int)ALLY_STATES.SPELL_PICKED:
			{
			}
			break;
		case (int)ALLY_STATES.CASTING_SPELL:  
			{
			}
			break;
		case (int)ALLY_STATES.STATUS_EFFECTS:	 
			{
				//Update any of the status effects. (use a new list, as some of the master list may get removed
				for(int i = 0; i < m_lStatusEffects.Count; ++i)
				{
					if(m_lStatusEffects[i].GetComponent<BattleBaseEffectScript>().m_bToBeRemoved == true)
					{

						m_dcPersistantData.RemoveMeFromStatus(name, i);
						m_lStatusEffects.RemoveAt(i);
						i--;
					}
					else
						m_lStatusEffects[i].GetComponent<BattleBaseEffectScript>().m_dFunc();
				}
				m_nState = (int)ALLY_STATES.DIALOGUE;
				m_nItemSelectionIndex = 0;
				m_nMagicSelectionIndex = 0;
				m_nActionSelectionIndex = 0;
				EndMyTurn();
			}
			break;
		}
	}

	void HandleActionSelectionInput()
	{
		if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			m_nActionSelectionIndex++;
			if(m_nActionSelectionIndex >= m_nMaxActionCount)
				m_nActionSelectionIndex = 0;
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			m_nActionSelectionIndex--;
			if(m_nActionSelectionIndex < 0)
				m_nActionSelectionIndex = m_nMaxActionCount - 1;
		}
		else if(Input.GetKeyDown(KeyCode.Return))
		{
			switch(m_nActionSelectionIndex)
			{
			case 0:
				{
					//Attack
					m_nState = (int)ALLY_STATES.ATTACK_CHOSEN;
					InitializeTargetReticle();
					for(int i = 0; i < 6; ++i)
					{
						if(i == m_nTargetPositionOnField)
						{
							GameObject.Find("Enemy_Cursor"+i).GetComponent<SpriteRenderer>().enabled = true;
						}
						else
						{
							GameObject.Find("Enemy_Cursor"+i).GetComponent<SpriteRenderer>().enabled = false;
						}
					}
				}
				break;
			case 1:
				{
					//Defend
					m_bAmIDefending = true;
					m_nState = (int)ALLY_STATES.STATUS_EFFECTS;
				}
				break;
			case 2:
				{
					//Magic
					m_nState = (int)ALLY_STATES.USEMAGIC_CHOSEN;
				}
				break;
			case 3:
				{
					//Use Item
					m_nState = (int)ALLY_STATES.USEITEM_CHOSEN;
				}
				break;
			case 4:
				{
					//Switch

				}
				break;
			case 5:
				{
					//Escape
				}
				break;
			}
		}
	}

	void HandleSingleTargetInput()
	{
		if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			int lowPos = 5, hiPos = 0;
			foreach(GameObject e in enemies)
			{
				if(e.GetComponent<UnitScript>().FieldPosition < lowPos)
					lowPos = e.GetComponent<UnitScript>().FieldPosition;
				if(e.GetComponent<UnitScript>().FieldPosition > hiPos)
					hiPos = e.GetComponent<UnitScript>().FieldPosition;
			}
			if(m_nTargetPositionOnField++ < hiPos)
			{
				//TODO: adjust for units Range and targets position in formation so that you can't target units that are out of range.
				m_nTargetPositionOnField++;
			}
			//else wrap around to the lowest formation counter active on the field.
			else
			{
				//TODO: adjust for units Range and targets position in formation so that you can't target units that are out of range.
				m_nTargetPositionOnField = lowPos;
			}

			UpdateTargetReticles();
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			int lowPos = 5, hiPos = 0;
			foreach(GameObject e in enemies)
			{
				if(e.GetComponent<UnitScript>().FieldPosition < lowPos)
					lowPos = e.GetComponent<UnitScript>().FieldPosition;
				if(e.GetComponent<UnitScript>().FieldPosition > hiPos)
					hiPos = e.GetComponent<UnitScript>().FieldPosition;
			}
			if(m_nTargetPositionOnField-- >= lowPos)
			{
				//TODO: adjust for units Range and targets position in formation so that you can't target units that are out of range.
				m_nTargetPositionOnField--;
			}
			//else wrap around to the highest formation counter active on the field.
			else
			{
				//TODO: adjust for units Range and targets position in formation so that you can't target units that are out of range.
				m_nTargetPositionOnField = hiPos;
			}
			UpdateTargetReticles();
		}
	}

	void HandleMagicSelectionInput()
	{

		if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			m_nMagicSelectionIndex++;
			if(m_nMagicSelectionIndex >= m_lSpellList.Count)
				m_nMagicSelectionIndex = 0;
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			m_nMagicSelectionIndex--;
			if(m_nMagicSelectionIndex < 0)
				m_nMagicSelectionIndex = m_lSpellList.Count - 1;
		}
	}

	void HandleItemSelectionInput()
	{
		if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			List<ItemLibrary.CharactersItems> theInv = m_dcPersistantData.m_lItemLibrary.GetInstance().GetItemsOfType(0);
			m_nItemSelectionIndex++;
			if(m_nItemSelectionIndex >= theInv.Count)
				m_nItemSelectionIndex = 0;
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			List<ItemLibrary.CharactersItems> theInv = m_dcPersistantData.m_lItemLibrary.GetInstance().GetItemsOfType(0);
			m_nItemSelectionIndex--;
			if(m_nItemSelectionIndex < 0)
				m_nItemSelectionIndex = theInv.Count - 1;
		}
	}

	//returns the amount of experience the player earned
	public int AwardExperience(List<int> _levelsOfEnemies)
	{
		int nTotalExp = 0;

		foreach(int eLvl in _levelsOfEnemies)
		{

			int baseExp = 5;
			int differenceInLevels = eLvl - GetUnitLevel();
			if(differenceInLevels > 0)
				baseExp = (int)(baseExp + (Mathf.Pow(differenceInLevels, 2.0f) * 25));
			else
				baseExp = 5;
			nTotalExp += baseExp;
		}
		m_nCurrentExperience = m_nCurrentExperience + nTotalExp;
		while(m_nCurrentExperience >= m_nExperienceToLevel && GetUnitLevel() < 99)
		{
			LevelUp();
			m_nCurrentExperience = m_nCurrentExperience - m_nExperienceToLevel;
		}
		if(GetUnitLevel() == 99 && m_nCurrentExperience > 999)
		{
			m_nCurrentExperience = 999;
			return 0;
		}
		return nTotalExp;
  	}
	void LevelUp()
	{
		SetUnitLevel(GetUnitLevel() +1);
		DCScript.ClassStats c = m_dcPersistantData.GetClassType(m_szClassName);
		if(c != null)
		{
			SetMaxHP(GetMaxHP() + c.m_nHPProg);
			SetCurHP(GetMaxHP());
			SetSTR(GetSTR() + c.m_nStrProg);
			SetDEF(GetDEF() + c.m_nDefProg);
			SetSPD(GetSPD() + c.m_nSpdProg);
			SetEVA(GetEVA() + c.m_nEvaProg);
			SetHIT(GetHIT() + c.m_nHitProg);
		}
		else
		{
			Debug.Log("Class not found during level up!");
			SetMaxHP(GetMaxHP() + 1);
			SetCurHP(GetMaxHP());
			SetSTR(GetSTR() + 1);
			SetDEF(GetDEF() + 1);
			SetSPD(GetSPD() + 1);
			SetEVA(GetEVA() + 1);
			SetHIT(GetHIT() + 1);
		}
  	}
	public void SetUnitStats()
	{
		if(m_taStartingStats)
		{
			DCScript.CharacterData c = new DCScript.CharacterData();
			//m_taStartingStats
			string[] lines = m_taStartingStats.text.Split('\n');
			//Name
			c.m_szCharacterName = name;
			//Max HP
			c.m_nMaxHP = int.Parse(lines[0].Trim());
			c.m_nCurHP = c.m_nMaxHP;
			//STR
			c.m_nSTR = int.Parse(lines[1].Trim());
			//DEF
			c.m_nDEF = int.Parse(lines[2].Trim());
			//SPD
			c.m_nSPD = int.Parse(lines[3].Trim());
			//EVA
			c.m_nEVA = int.Parse(lines[4].Trim());
			//HIT
			c.m_nHIT = int.Parse(lines[5].Trim());
			//LEVEL
			c.m_nLevel = int.Parse(lines[6].Trim());
			//Race
			c.m_szCharacterRace = lines[7].Trim ();
			//ClassType
			c.m_szCharacterClassType = lines[8].Trim();
			//Weapon Name
			c.m_szWeaponName = lines[9].Trim();
			//Weapon Level
			c.m_nWeaponLevel = int.Parse(lines[10].Trim());
			//Weapon Damage
			c.m_nWeaponDamageModifier = int.Parse(lines[11].Trim());
			c.m_nSTR += c.m_nWeaponDamageModifier;
			//Weapon Mod name
			c.m_szWeaponModifierName = lines[12].Trim();
			if(c.m_szWeaponModifierName == "NONE")
				c.m_szWeaponModifierName = "";
			//Head
			if(lines[13].Trim() != "NULL")
			{
				c.m_idHelmSlot = (ItemLibrary.ArmorData)m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[13].Trim());
			}
			else
				c.m_idHelmSlot = null;
			//Shoulders
			if(lines[14].Trim() != "NULL")
			{
				c.m_idShoulderSlot = (ItemLibrary.ArmorData)m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[14].Trim());
			}
			else
				c.m_idShoulderSlot = null;
			//Chest
			if(lines[15].Trim() != "NULL")
			{
				c.m_idChestSlot = (ItemLibrary.ArmorData)m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[15].Trim());
			}
			else
				c.m_idChestSlot = null;
			//Arms
			if(lines[16].Trim() != "NULL")
			{
				c.m_idGloveSlot = (ItemLibrary.ArmorData)m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[16].Trim());
			}
			else
				c.m_idGloveSlot = null;
			//Waist
			if(lines[17].Trim() != "NULL")
			{
				c.m_idBeltSlot = (ItemLibrary.ArmorData)m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[17].Trim());
			}
			else
				c.m_idBeltSlot = null;
			//Legs
			if(lines[18].Trim() != "NULL")
			{
				c.m_idLegSlot = (ItemLibrary.ArmorData)m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[18].Trim());
			}
			else
				c.m_idLegSlot = null;
			//Trinket1
			if(lines[19].Trim() != "NULL")
			{
				c.m_idTrinket1 = m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[19].Trim());
			}
			else
				c.m_idTrinket1 = null;
			//Trinket2
			if(lines[20].Trim() != "NULL")
			{
				c.m_idTrinket2 = m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[20].Trim());
			}
			else
				c.m_idTrinket2 = null;
			//Character Bio
			c.m_szCharacterBio = lines[21].Trim();
			//SpellCount
			int amntOfSpells = int.Parse(lines[22].Trim());
			c.m_lSpellsKnown = new List<string>();
			for(int i = 0; i < amntOfSpells; ++i)
			{
				c.m_lSpellsKnown.Add(lines[23+i].Trim());
			}
			UpdateStats();
			GameObject.Find("PersistantData").GetComponent<DCScript>().AddPartyMember(c);
		}
	}
		
	void UpdateStats()
	{
		//TODO: impliment updating of stats based on what items you have equipped
	}

	//Initialize the target reticle so that there isn't a crash (preference to front row enemies
	void InitializeTargetReticle()
	{
		
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		for(int i = enemies.Length-1; i >=0; i--)
		{
			if(m_nTargetPositionOnField == enemies[i].GetComponent<UnitScript>().FieldPosition && enemies[i].GetComponent<UnitScript>().GetCurHP() > 0)
				return;
		}
		for(int i = enemies.Length-1; i >=0; i--)
		{
			if(enemies[i].GetComponent<UnitScript>().GetCurHP() > 0)
			{
				m_nTargetPositionOnField = enemies[i].GetComponent<UnitScript>().FieldPosition;

			}
		}

	}
	void UpdateTargetReticles()
	{
		for(int i = 0; i < 6; ++i)
		{
			if(i == m_nTargetPositionOnField)
			{
				GameObject.Find("Enemy_Cursor"+i).GetComponent<SpriteRenderer>().enabled = true;
			}
			else
			{
				GameObject.Find("Enemy_Cursor"+i).GetComponent<SpriteRenderer>().enabled = false;
			}
		}
	}
	public void UpdatePositionOnField()
	{
		string szgoName = "Ally_StartPos" + FieldPosition.ToString();
		GameObject go = GameObject.Find(szgoName);
		m_vInitialPos.x = go.transform.position.x;
		m_vInitialPos.y = go.transform.position.y;
		m_vInitialPos.z = 0.0f;
		switch(FieldPosition)
		{
		case 0:
			{
				//Top right
			}
			break;
		case 1:
			{
				//Middle right
				GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder + 1;
			}
			break;
		case 2:
			{
				//Bottom right
				GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder + 2;
			}
			break;
		case 3:
			{
				//Top left
			}
			break;
		case 4:
			{
				//Middle Left
				GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder + 1;
			}
			break;
		case 5:
			{
				//Bottom left
				GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder + 2;
			}
			break;
		}
		transform.position = m_vInitialPos;
	}
}
