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
		ATTACK_RETURNING,
		DEFENDING,		//unit starts animation for defending, then moves to STATUS_EFFECT
		USEITEM_CHOSEN, //player has chosen to use an item, display inventory on screen to allow player to select from useable items. (Cycle through items, select item, move to ITEM_PICKED)
		ITEM_PICKED_SINGLEDMG,    //item has been picked, now needs to select the target to use this item on.  (Cycle through targets, select target, move to USING_ITEM)
		ITEM_PICKED_AOEDMG,
		ITEM_PICKED_SINGLEHEAL,
		ITEM_PICKED_AOEHEAL,
		USING_ITEM,     //Player has chosen which item to use and what target to use it on, play animation, resolve item useage, then move to STATUS_EFFECT
		USEMAGIC_CHOSEN, //same as USEITEM_CHOSEN, but with magic, (cycle through and select with spell to cast, move to SPELL_PICKED)
		SPELL_PICKED_SINGLEDMG,	 //same at ITEM_PICKED, but with magic, (Cycle through targets, select target, move to CASTING_SPELL)
		SPELL_PICKED_AOEDMG,
		SPELL_PICKED_SINGLEHEAL,
		SPELL_PICKED_AOEHEAL,
		CHARGING_SPELL,  //player is charging up to cast the spell, wait for the charging animation, then move to CASTING_SPELL state
		CASTING_SPELL,   //player has chosen a spell and a target, play animation, resolve spell effect, move to STATUS_EFFECT
		SWITCHING,		 //Unit switches with either unit infront or behind (depending on formation) afterward move to STATUS_EFFECT
		SWITCHING_NOTMYTURN,     //State for switching when it's not your turn, this is to allow non-turn units to be able to animate/move but not go to STATUS_EFFECT afterward.
		ESCAPING_FAILED,		 //Unit tries to escape, if failed go to STATUS_EFFECT, if succeeded end the fight.
		ESCAPING_SUCCEEDED,
		STATUS_EFFECTS,	 //Cycle through status effects, tick off one cycle of the effects, remove/update effects, inform TURNWATCHER of end of turn and end your turn.
		DEAD
	}

	float m_fMovementSpeed = 8.0f; //The speed in which this unit moves accross the battlefield.
	bool m_bAmIDefending = false;

	//hooks to components/gameobjects
				
	public GameObject m_goFadingText; //for displaying damage taken (or any other fading text to float above this unit.
	DCScript m_dcPersistantData;

	public GameObject m_goShadowClone; //for shadow trails of this unit
	public Sprite m_tLargeBust;     //the portrait of this character  (to be displayed during victory screen)
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

	Transform m_tTargetPositionOnField = null;

	//For when this character is trying to use an item in battle
	[HideInInspector]
	public GameObject m_goItemBeingUsed = null;

	string m_szSpellToCast = "";
	//increments when the unit cast ends and when the spell runic circle ends.   Spell effect doesn't happen until the counter gets to 2 so that both effects end before the spell goes off.
	int m_nCastingCounter = 0;

	// Use this for initialization
	void Start () 
	{
		m_dcPersistantData = GameObject.Find("PersistantData").GetComponent<DCScript>();
		m_twTurnWatcher = GameObject.Find("TurnWatcher").GetComponent<TurnWatcherScript>();
		m_aAnim = GetComponent<Animator>();
		if(m_aAnim == null)
			m_aAnim = GetComponentInChildren<Animator>();
		m_nState = (int)ALLY_STATES.DIALOGUE;
		//Grab any status effects that are currently effecting this character.
		List<DCScript.StatusEffect> effects = GameObject.Find("PersistantData").GetComponent<DCScript>().GetStatusEffects();
		foreach(DCScript.StatusEffect se in effects)
		{
			foreach(string charName in se.m_lEffectedMembers)
			{
				if(charName == name)
				{
					AddStatusEffect(se);
				}
			}
		}
		AdjustTempStats();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bIsMyTurn == true)
		{
			HandleMainStates();
		}
	}

	void HandleMainStates()
	{
		switch(m_nState)
		{
		case (int)ALLY_STATES.DIALOGUE:
			{
				m_bAmIDefending = false;
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
					m_twTurnWatcher.m_goActionSelector.SetActive(true);
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
				HandleSingleTargetInput(false);
				if(Input.GetKeyDown(KeyCode.Return))
				{
					EnemyToAttackSelected(m_nTargetPositionOnField);
				}
			}
			break;
		case (int)ALLY_STATES.ATTACKING:
			{
				if(m_tTargetPositionOnField != null)
				{
					Vector3 toTarget = m_tTargetPositionOnField.position - transform.position;
					if(toTarget.sqrMagnitude > 0.1f)
					{
						toTarget.Normalize();
						transform.position += toTarget * m_fMovementSpeed * Time.deltaTime;
						if(m_fShadowTimer >= m_fShadowTimerBucket && m_goShadowClone != null)
						{
							GameObject newShadow = Instantiate(m_goShadowClone, transform.position, Quaternion.identity) as GameObject;
							newShadow.GetComponent<SpriteRenderer>().sprite = m_aAnim.gameObject.GetComponent<SpriteRenderer>().sprite;
							Vector3 cloneTransform = m_aAnim.gameObject.transform.localScale;
							newShadow.transform.localScale = cloneTransform;
							Vector3 pos = transform.position;
							//adjust so the clone is behind the unit
							pos.z += 0.1f;
							Destroy(newShadow, m_fShadowTimerBucket*3);
							m_fShadowTimer = 0.0f;
						}
						else
							m_fShadowTimer += Time.deltaTime;
					}
					else
					{
						m_tTargetPositionOnField = null;
						m_aAnim.SetBool("m_bIsMoving", false);
						m_aAnim.SetBool("m_bIsAttacking", true);
					}
				}
			}
			break;
		case (int)ALLY_STATES.ATTACK_RETURNING:
			{
				if(m_nUnitType == (int)UnitTypes.ALLY_MELEE)
				{
					Vector3 toTarget = m_vInitialPos - transform.position;
					if(toTarget.sqrMagnitude > 0.1f)
					{
						toTarget.Normalize();
						transform.position += toTarget * m_fMovementSpeed * Time.deltaTime;
					}
					else
					{
						transform.position = m_vInitialPos;
						m_aAnim.SetBool("m_bIsMoving", false);
						m_nState = (int)ALLY_STATES.STATUS_EFFECTS;
					}
				}
			}
			break;
		case (int)ALLY_STATES.DEFENDING:
			{
				//maybe some animation stuff? not sure, this could be a useless state
				m_nState = (int)ALLY_STATES.STATUS_EFFECTS;
			}
			break;
		case (int)ALLY_STATES.USEITEM_CHOSEN:
			{

				if(Input.GetKeyDown(KeyCode.Return))
				{
					m_twTurnWatcher.gameObject.GetComponent<ItemsAndSpellsContainer>().SelectionSelected();
				}
				else if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
				{
					m_twTurnWatcher.GetComponent<ItemsAndSpellsContainer>().m_goItemAndSpellSelector.SetActive(false);
					m_twTurnWatcher.GetComponent<ItemsAndSpellsContainer>().m_goItemAndSpellDescriptor.SetActive(false);
					m_twTurnWatcher.m_goActionSelector.SetActive(true);
					m_nState = (int)ALLY_STATES.ACTION_SELECTION;
				}
			}
			break;
		case (int)ALLY_STATES.ITEM_PICKED_SINGLEDMG:
			{
				HandleSingleTargetInput(false);
				if(Input.GetKeyDown(KeyCode.Return))
				{
					//turn off the flags for the item/inventory rendering
					m_nState = (int)CAllyBattleScript.ALLY_STATES.USING_ITEM;
					m_goItemBeingUsed.GetComponent<ItemSingleDamage>().m_bShouldActivate = true;
					ClearTargetReticles();
				}
			}
			break;
		case (int)ALLY_STATES.ITEM_PICKED_AOEDMG:
			{
				if(Input.GetKeyDown(KeyCode.Return))
				{
					m_nState = (int)CAllyBattleScript.ALLY_STATES.USING_ITEM;
					m_goItemBeingUsed.GetComponent<ItemGroupDamage>().m_bShouldActivate = true;
					ClearTargetReticles();
				}
				else if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
				{
					m_nState = (int)CAllyBattleScript.ALLY_STATES.USEITEM_CHOSEN;
					ClearTargetReticles();
					HandleActionSelected(4);
				}
			}
			break;
		case (int)ALLY_STATES.ITEM_PICKED_SINGLEHEAL:
			{
				HandleSingleTargetInput(true);
				if(Input.GetKeyDown(KeyCode.Return))
				{
					//turn off the flags for the item/inventory rendering
					m_nState = (int)CAllyBattleScript.ALLY_STATES.USING_ITEM;
					m_goItemBeingUsed.GetComponent<ItemSingleHeal>().m_bShouldActivate = true;
					ClearTargetReticles();
				}
			}
			break;
		case (int)ALLY_STATES.ITEM_PICKED_AOEHEAL:
			{
				if(Input.GetKeyDown(KeyCode.Return))
				{
					m_nState = (int)CAllyBattleScript.ALLY_STATES.USING_ITEM;
					m_goItemBeingUsed.GetComponent<ItemGroupHeal>().m_bShouldActivate = true;
					ClearTargetReticles();
				}
				else if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
				{
					m_nState = (int)CAllyBattleScript.ALLY_STATES.USEITEM_CHOSEN;
					ClearTargetReticles();
					HandleActionSelected(4);
				}
			}
			break;
		case (int)ALLY_STATES.USING_ITEM:    
			{
			}
			break;
	    case (int)ALLY_STATES.USEMAGIC_CHOSEN:
			{
				if(Input.GetKeyDown(KeyCode.Return))
				{
					m_twTurnWatcher.gameObject.GetComponent<ItemsAndSpellsContainer>().SelectionSelected();
				}
				else if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
				{
					m_twTurnWatcher.GetComponent<ItemsAndSpellsContainer>().m_goItemAndSpellSelector.SetActive(false);
					m_twTurnWatcher.GetComponent<ItemsAndSpellsContainer>().m_goItemAndSpellDescriptor.SetActive(false);
					m_twTurnWatcher.m_goActionSelector.SetActive(true);
					m_nState = (int)ALLY_STATES.ACTION_SELECTION;
				}
			}
			break;
		case (int)ALLY_STATES.SPELL_PICKED_SINGLEDMG:
			{
				HandleSingleTargetInput(false);
				if(Input.GetKeyDown(KeyCode.Return))
				{
					//turn off the flags for the item/inventory rendering
					m_nState = (int)CAllyBattleScript.ALLY_STATES.CHARGING_SPELL;
					m_aAnim.SetBool("m_bIsCasting", true);
					StartChargingSpell();
					ClearTargetReticles();
				}
			}
			break;
		case (int)ALLY_STATES.SPELL_PICKED_AOEDMG:
			{
				if(Input.GetKeyDown(KeyCode.Return))
				{
					m_nState = (int)CAllyBattleScript.ALLY_STATES.CHARGING_SPELL;
					m_aAnim.SetBool("m_bIsCasting", true);
					StartChargingSpell();
					ClearTargetReticles();
				}
				else if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
				{
					m_nState = (int)CAllyBattleScript.ALLY_STATES.USEMAGIC_CHOSEN;
					ClearTargetReticles();
					HandleActionSelected(5);
				}
			}
			break;
		case (int)ALLY_STATES.SPELL_PICKED_SINGLEHEAL:
			{
				HandleSingleTargetInput(true);
				if(Input.GetKeyDown(KeyCode.Return))
				{
					//turn off the flags for the item/inventory rendering
					m_nState = (int)CAllyBattleScript.ALLY_STATES.CHARGING_SPELL;
					m_aAnim.SetBool("m_bIsCasting", true);
					StartChargingSpell();
					ClearTargetReticles();
				}
			}
			break;
		case (int)ALLY_STATES.SPELL_PICKED_AOEHEAL:
			{
				if(Input.GetKeyDown(KeyCode.Return))
				{
					m_nState = (int)CAllyBattleScript.ALLY_STATES.CHARGING_SPELL;
					StartChargingSpell();
					m_aAnim.SetBool("m_bIsCasting", true);
					ClearTargetReticles();
				}
				else if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
				{
					m_nState = (int)CAllyBattleScript.ALLY_STATES.USEMAGIC_CHOSEN;
					ClearTargetReticles();
					HandleActionSelected(5);
				}
			}
			break;
		case (int)ALLY_STATES.CASTING_SPELL:  
			{
			}
			break;
		case (int)ALLY_STATES.CHARGING_SPELL:
			{
			}
			break;
		case (int)ALLY_STATES.SWITCHING:
			{
				if(m_tTargetPositionOnField == null)
				{
					m_tTargetPositionOnField = GameObject.Find("Ally_StartPos" + FieldPosition).transform;
				}
				
				Vector3 toTarget = m_tTargetPositionOnField.position - transform.position;
				if(toTarget.sqrMagnitude > 0.1f)
				{
					toTarget.Normalize();
					transform.position += toTarget * m_fMovementSpeed * Time.deltaTime;
				}
				else
				{
					transform.position = m_tTargetPositionOnField.position;
					m_tTargetPositionOnField = null;
					m_aAnim.SetBool("m_bIsMoving", false);
					m_nState = (int)ALLY_STATES.STATUS_EFFECTS;
				}
			}
			break;
		case (int)ALLY_STATES.SWITCHING_NOTMYTURN:
			{
				if(m_tTargetPositionOnField == null)
				{
					m_tTargetPositionOnField = GameObject.Find("Ally_StartPos" + FieldPosition).transform;
					if(GetCurHP() > 0)
						m_aAnim.SetBool("m_bIsMoving", true);
				}

				Vector3 toTarget = m_tTargetPositionOnField.position - transform.position;
				if(toTarget.sqrMagnitude > 0.1f)
				{
					toTarget.Normalize();
					transform.position += toTarget * m_fMovementSpeed * Time.deltaTime;
				}
				else
				{
					transform.position = m_tTargetPositionOnField.position;
					m_tTargetPositionOnField = null;
					m_bIsMyTurn = false;
					if(GetCurHP() > 0)
					{
						m_aAnim.SetBool("m_bIsMoving", false);
						m_nState = (int)ALLY_STATES.DIALOGUE;
					}
					else
					{
						m_nState = (int)ALLY_STATES.DEAD;
					}
					
				}
			}
			break;
		case (int)ALLY_STATES.ESCAPING_FAILED:
			{
				m_nState = (int)ALLY_STATES.STATUS_EFFECTS;
			}
			break;
		case (int)ALLY_STATES.ESCAPING_SUCCEEDED:
			{
				m_twTurnWatcher.Escape();
			}
			break;
		case (int)ALLY_STATES.STATUS_EFFECTS:	 
			{
				//Adjust the temp stats(if an enemy or something does something that should effect your stats, IT should call the function as well incase other things attack before you go(shouldn't cause any issues)
				AdjustTempStats();
				//Update any of the status effects. (use a new list, as some of the master list may get removed
				for(int i = 0; i < m_lStatusEffects.Count; ++i)
				{
					if(m_lStatusEffects[i].GetComponent<BattleBaseEffectScript>().m_bToBeRemoved == true)
					{

						m_dcPersistantData.RemoveMeFromStatus(name, i);
						RemoveStatusEffect(m_lStatusEffects[i].name);
						i--;
					}
					else
						m_lStatusEffects[i].GetComponent<BattleBaseEffectScript>().m_dFunc();
				}
				m_nState = (int)ALLY_STATES.DIALOGUE;
				m_twTurnWatcher.m_goActionSelector.GetComponent<CBattleActionsScript>().ChangeIndex(0);
				EndMyTurn();
			}
			break;
		}
	}
	public void HandleActionSelected(int p_nIndex)
	{
		switch(p_nIndex)
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
				m_nState = (int)ALLY_STATES.DEFENDING;
			}
			break;
		case 2:
			{
				//Switch

				//find if we can switch
				int nPosToSwitchTo = TryToSwitch(true);
				if(nPosToSwitchTo != -1)
				{
					//switch is possible.
					foreach(GameObject unit in m_twTurnWatcher.m_goUnits)
					{
						if(unit.GetComponent<UnitScript>().FieldPosition == nPosToSwitchTo && unit.GetComponent<UnitScript>().m_nUnitType <= (int)UnitScript.UnitTypes.NPC)
						{
							int nOldPos = FieldPosition;
							//set movement position to target position of new formation
							FieldPosition = nPosToSwitchTo;
							m_nState = (int)ALLY_STATES.SWITCHING;
							//set movement position for the other unit
							unit.GetComponent<UnitScript>().FieldPosition = nOldPos;
							unit.GetComponent<UnitScript>().m_nState = (int)ALLY_STATES.SWITCHING_NOTMYTURN;
							unit.GetComponent<UnitScript>().m_bIsMyTurn = true;
							break;
						}
					}
				}
				else
				{
					//enable action selection box
					m_twTurnWatcher.m_goActionSelector.SetActive(true);
					//put us back in the state to select actions
					m_nState = (int)ALLY_STATES.ACTION_SELECTION;
				}

			}
			break;
		case 3:
			{
				//Escape
				AttemptToEscape();
			}
			break;
		case 4:
			{
				//Use Item
				m_nState = (int)ALLY_STATES.USEITEM_CHOSEN;
				List<DCScript.CharacterData> allies = m_dcPersistantData.GetParty();
				foreach(DCScript.CharacterData g in allies)
				{
					if(g.m_szCharacterName == name)
					{
						m_twTurnWatcher.gameObject.GetComponent<ItemsAndSpellsContainer>().SwitchContainerTypeTo(g, 0);
						break;
					}
				}	
			}
			break;
		case 5:
			{
				//Magic
				m_nState = (int)ALLY_STATES.USEMAGIC_CHOSEN;
				List<DCScript.CharacterData> allies = m_dcPersistantData.GetParty();
				foreach(DCScript.CharacterData g in allies)
				{
					if(g.m_szCharacterName == name)
					{
						m_twTurnWatcher.gameObject.GetComponent<ItemsAndSpellsContainer>().SwitchContainerTypeTo(g, 1);
						break;
					}
				}
			}
			break;
		}
	}
	void HandleActionSelectionInput()
	{
		if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			m_twTurnWatcher.m_goActionSelector.GetComponent<CBattleActionsScript>().ChangeIndex(m_twTurnWatcher.m_goActionSelector.GetComponent<CBattleActionsScript>().ActionIndex()+1);
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			m_twTurnWatcher.m_goActionSelector.GetComponent<CBattleActionsScript>().ChangeIndex(m_twTurnWatcher.m_goActionSelector.GetComponent<CBattleActionsScript>().ActionIndex()-1);
		}
		else if(Input.GetKeyDown(KeyCode.Return))
		{
			m_twTurnWatcher.m_goActionSelector.SetActive(false);
			HandleActionSelected(m_twTurnWatcher.m_goActionSelector.GetComponent<CBattleActionsScript>().ActionIndex());
		}
	}

	void HandleSingleTargetInput(bool isAlly)
	{
		if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			List<int> lValidPos = new List<int>();
			GameObject[] units;
			if(isAlly == true)
			{
				units = GameObject.FindGameObjectsWithTag("Ally");
			}
			else
			{
				units = GameObject.FindGameObjectsWithTag("Enemy");
			}
			foreach(GameObject e in units)
			{
				lValidPos.Add(e.GetComponent<UnitScript>().FieldPosition);
			}
			foreach(int n in lValidPos)
			{
				int index = m_nTargetPositionOnField;
				while(index != 5)
				{
					index++;
					if(index == n)
					{
						m_nTargetPositionOnField = index;
						UpdateTargetReticles(isAlly);
						return;
					}
				}
				index = m_nTargetPositionOnField;
				while(index != 0)
				{
					index--;
					if(index == n)
					{
						m_nTargetPositionOnField = index;
						UpdateTargetReticles(isAlly);
						return;
					}
				}
			}
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			List<int> lValidPos = new List<int>();
			GameObject[] units;
			if(isAlly == true)
			{
				units = GameObject.FindGameObjectsWithTag("Ally");
			}
			else
			{
				units = GameObject.FindGameObjectsWithTag("Enemy");
			}
			foreach(GameObject e in units)
			{
				lValidPos.Add(e.GetComponent<UnitScript>().FieldPosition);
			}
			foreach(int n in lValidPos)
			{
				int index = m_nTargetPositionOnField;
				while(index != 0)
				{
					index--;
					if(index == n)
					{
						m_nTargetPositionOnField = index;
						UpdateTargetReticles(isAlly);
						return;
					}
				}
				index = m_nTargetPositionOnField;
				while(index != 5)
				{
					index++;
					if(index == n)
					{
						m_nTargetPositionOnField = index;
						UpdateTargetReticles(isAlly);
						return;
					}
				}

			}
		}
		else if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
		{
			List<int> lValidPos = new List<int>();
			GameObject[] units;
			if(isAlly == true)
			{
				units = GameObject.FindGameObjectsWithTag("Ally");
			}
			else
			{
				units = GameObject.FindGameObjectsWithTag("Enemy");
			}
			foreach(GameObject e in units)
			{
				lValidPos.Add(e.GetComponent<UnitScript>().FieldPosition);
			}
			if(m_nTargetPositionOnField <= 2)
			{
				

				foreach(int n in lValidPos)
				{
					if(m_nTargetPositionOnField + 3 == n)
					{
						m_nTargetPositionOnField = m_nTargetPositionOnField + 3;
						UpdateTargetReticles(isAlly);
						return;
					}
				}
				foreach(int n in lValidPos)
				{
					
					if(m_nTargetPositionOnField + 3 == n)
					{
						m_nTargetPositionOnField = m_nTargetPositionOnField + 3;
						UpdateTargetReticles(isAlly);
						return;
					}
					if(m_nTargetPositionOnField == 2)
					{
						if(m_nTargetPositionOnField + 1 == n)
						{
							//wrapped around, is top right available?
							m_nTargetPositionOnField = m_nTargetPositionOnField + 1;
							UpdateTargetReticles(isAlly);
							return;
						}
					}
					else if(m_nTargetPositionOnField + 4 == n)
					{
						m_nTargetPositionOnField = m_nTargetPositionOnField + 4;
						UpdateTargetReticles(isAlly);
						return;
					}
					if(m_nTargetPositionOnField == 0)
					{
						if(m_nTargetPositionOnField + 5 == n)
						{
							m_nTargetPositionOnField = m_nTargetPositionOnField + 5;
							UpdateTargetReticles(isAlly);
							return;
						}
					}
					else if(m_nTargetPositionOnField + 2 == n)
					{
						m_nTargetPositionOnField = m_nTargetPositionOnField + 2;
						UpdateTargetReticles(isAlly);
						return;
					}
				}
			}
			else
			{
				//right side
				foreach(int n in lValidPos)
				{
					if(m_nTargetPositionOnField - 3 == n)
					{
						m_nTargetPositionOnField = m_nTargetPositionOnField - 3;
						UpdateTargetReticles(isAlly);
						return;
					}
					if(m_nTargetPositionOnField == 3)
					{
						if(m_nTargetPositionOnField - 1 == n)
						{
							m_nTargetPositionOnField = m_nTargetPositionOnField - 1;
							UpdateTargetReticles(isAlly);
							return;
						}
					}
					else if(m_nTargetPositionOnField - 4 == n)
					{
						m_nTargetPositionOnField = m_nTargetPositionOnField - 4;
						UpdateTargetReticles(isAlly);
						return;
					}
					if(m_nTargetPositionOnField == 5)
					{
						if(m_nTargetPositionOnField - 5 == n)
						{
							m_nTargetPositionOnField = m_nTargetPositionOnField - 5;
							UpdateTargetReticles(isAlly);
							return;
						}
					}
					else if(m_nTargetPositionOnField - 2 == n)
					{
						m_nTargetPositionOnField = m_nTargetPositionOnField - 2;
						UpdateTargetReticles(isAlly);
						return;
					}

				}

			}
		}
		else if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
		{
			//Disable targeting cursors
			ClearTargetReticles();
			switch(m_nState)
			{
			case (int)ALLY_STATES.ATTACK_CHOSEN:
				{
					//enable action selection box
					m_twTurnWatcher.m_goActionSelector.SetActive(true);
					//put us back in the state to select actions
					m_nState = (int)ALLY_STATES.ACTION_SELECTION;
				}
				break;
			case (int)ALLY_STATES.ITEM_PICKED_SINGLEDMG:
				{
					m_nState = (int)CAllyBattleScript.ALLY_STATES.USEITEM_CHOSEN;
					ClearTargetReticles();
					HandleActionSelected(4);
				}
				break;
			case (int)ALLY_STATES.ITEM_PICKED_SINGLEHEAL:
				{
					m_nState = (int)CAllyBattleScript.ALLY_STATES.USEITEM_CHOSEN;
					ClearTargetReticles();
					HandleActionSelected(4);
				}
				break;
			case (int)ALLY_STATES.SPELL_PICKED_SINGLEDMG:
				{
					m_nState = (int)CAllyBattleScript.ALLY_STATES.USEMAGIC_CHOSEN;
					ClearTargetReticles();
					HandleActionSelected(5);
				}
				break;
			case (int)ALLY_STATES.SPELL_PICKED_SINGLEHEAL:
				{
					m_nState = (int)CAllyBattleScript.ALLY_STATES.USEMAGIC_CHOSEN;
					ClearTargetReticles();
					HandleActionSelected(5);
				}
				break;
			}

		}
  	}

	public void ChangeEnemyTarget(int p_nIndex)
	{
		if(m_nState == (int)ALLY_STATES.ATTACK_CHOSEN || m_nState == (int)ALLY_STATES.ITEM_PICKED_SINGLEDMG || m_nState == (int)ALLY_STATES.SPELL_PICKED_SINGLEDMG)
		{
			m_nTargetPositionOnField = p_nIndex;
			UpdateTargetReticles(false);
		}
	}

	public void ChangeAllyTarget(int p_nIndex)
	{
		//TODO: add in logic for targetting a dead ally to work only if you're trying to revive them
		if(m_nState == (int)ALLY_STATES.ITEM_PICKED_SINGLEHEAL || m_nState == (int)ALLY_STATES.SPELL_PICKED_SINGLEHEAL)
		{
			m_nTargetPositionOnField = p_nIndex;
			UpdateTargetReticles(true);
		}
	}

	public override void AdjustTempStats()
	{
		//must be called first
		base.AdjustTempStats();
		if(GetCurHP() <= 0)
			return;
		//Update the temp stats from gear (incase something broke?)
		if(m_idHelmSlot != null)
			AdjustTempStatFromItem(m_idHelmSlot);
		if(m_idShoulderSlot != null)
			AdjustTempStatFromItem(m_idShoulderSlot);
		if(m_idChestSlot != null)
			AdjustTempStatFromItem(m_idChestSlot);
		if(m_idGloveSlot != null)
			AdjustTempStatFromItem(m_idGloveSlot);
		if(m_idBeltSlot != null)
			AdjustTempStatFromItem(m_idBeltSlot);
		if(m_idLegSlot != null)
			AdjustTempStatFromItem(m_idLegSlot);
		if(m_idTrinket1 != null)
			AdjustTempStatFromItem(m_idTrinket1);
		if(m_idTrinket2 != null)
			AdjustTempStatFromItem(m_idTrinket2);

		//Adjust temp stats from weapon
		SetTempSTR(GetTempSTR() + m_dcPersistantData.GetCharacter(name).m_nWeaponDamageModifier);

		//Update the temp stats from status effects.
		foreach(GameObject effect in m_lStatusEffects)
		{
			AdjustTempStatFromEffects(effect.GetComponent<BattleBaseEffectScript>());
		}
		
	}
	void AdjustTempStatFromItem(ItemLibrary.ItemData item)
	{
		SetTempMaxHP(GetTempMaxHP() + item.m_nHPMod);
		SetTempMaxMP(GetTempMaxMP() + item.m_nMPMod);
		SetTempSTR(GetTempSTR() + item.m_nPowMod);
		SetTempDEF(GetTempDEF() + item.m_nDefMod);
		SetTempSPD(GetTempSPD() + item.m_nSpdMod);
		SetTempHIT(GetTempHIT() + item.m_nHitMod);
		SetTempEVA(GetTempEVA() + item.m_nEvaMod);
	}
	void AdjustTempStatFromEffects(BattleBaseEffectScript effect)
	{
		if(effect.m_bToBeRemoved == true)
			return;
		List<int> lStats = new List<int>();
		lStats.Add(GetTempMaxHP());
		lStats.Add(GetTempMaxMP());
		lStats.Add(GetTempSTR());
		lStats.Add(GetTempDEF());
		lStats.Add(GetTempSPD());
		lStats.Add(GetTempHIT());
		lStats.Add(GetTempEVA());
		lStats = effect.AdjustTempStats(lStats);
		SetTempMaxHP(lStats[0]);
		SetTempMaxMP(lStats[1]);
		SetTempSTR(lStats[2]);
		SetTempDEF(lStats[3]);
		SetTempSPD(lStats[4]);
		SetTempHIT(lStats[5]);
		SetTempEVA(lStats[6]);
	}

	//returns the amount of experience the player earned
	public int AwardExperience(List<int> _levelsOfEnemies, ref int nextExp, ref int nextLvl)
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
		nextExp = m_nCurrentExperience + nTotalExp;
		nextLvl = GetUnitLevel();
		while(nextExp >= m_nExperienceToLevel && nextLvl < 99)
		{
			nextLvl++;
			nextExp = nextExp - m_nExperienceToLevel;
		}
		if(nextLvl == 99 && nextExp > 999)
		{
			nextExp = 999;
			return 0;
		}
		return nTotalExp;
  	}
	public void LevelUp()
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

	//This should only be called once, when the character is very first being recruited. (not in battle or anywhere else)
	public void SetUnitStats()
	{
		if(m_taStartingStats)
		{
			DCScript.CharacterData c = new DCScript.CharacterData();
			//m_taStartingStats
			string[] _szlines = m_taStartingStats.text.Split('\n');
			List<string> lines = new List<string>();
			foreach(string s in _szlines)
			{
				//This is to ignore anything on the left side of :
				string[] data = s.Split(':');
				lines.Add(data[data.Length-1].Trim());
			}
			//Name
			c.m_szCharacterName = name;
			//Max HP
			c.m_nMaxHP = int.Parse(lines[0].Trim());
			c.m_nCurHP = c.m_nMaxHP;
			//Max MP
			c.m_nMaxMP = int.Parse(lines[1].Trim());
			c.m_nCurMP = c.m_nMaxMP;
			//STR
			c.m_nSTR = int.Parse(lines[2].Trim());
			//DEF
			c.m_nDEF = int.Parse(lines[3].Trim());
			//SPD
			c.m_nSPD = int.Parse(lines[4].Trim());
			//EVA
			c.m_nEVA = int.Parse(lines[5].Trim());
			//HIT
			c.m_nHIT = int.Parse(lines[6].Trim());
			//LEVEL
			c.m_nLevel = int.Parse(lines[7].Trim());
			//Race
			c.m_szCharacterRace = lines[8].Trim ();
			//ClassType
			c.m_szCharacterClassType = lines[9].Trim();
			//Weapon Name
			c.m_szWeaponName = lines[10].Trim();
			//Weapon Level
			c.m_nWeaponLevel = int.Parse(lines[11].Trim());
			//Weapon Damage
			c.m_nWeaponDamageModifier = int.Parse(lines[12].Trim());
			c.m_nSTR += c.m_nWeaponDamageModifier;
			//Weapon Mod name
			c.m_szWeaponModifierName = lines[13].Trim();
			if(c.m_szWeaponModifierName == "NULL")
				c.m_szWeaponModifierName = "";
			//Head
			if(lines[14].Trim() != "NULL")
			{
				c.m_idHelmSlot = (ItemLibrary.ArmorData)m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[13].Trim());
			}
			else
				c.m_idHelmSlot = null;
			//Shoulders
			if(lines[15].Trim() != "NULL")
			{
				c.m_idShoulderSlot = (ItemLibrary.ArmorData)m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[14].Trim());
			}
			else
				c.m_idShoulderSlot = null;
			//Chest
			if(lines[16].Trim() != "NULL")
			{
				c.m_idChestSlot = (ItemLibrary.ArmorData)m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[15].Trim());
			}
			else
				c.m_idChestSlot = null;
			//Arms
			if(lines[17].Trim() != "NULL")
			{
				c.m_idGloveSlot = (ItemLibrary.ArmorData)m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[16].Trim());
			}
			else
				c.m_idGloveSlot = null;
			//Waist
			if(lines[18].Trim() != "NULL")
			{
				c.m_idBeltSlot = (ItemLibrary.ArmorData)m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[17].Trim());
			}
			else
				c.m_idBeltSlot = null;
			//Legs
			if(lines[19].Trim() != "NULL")
			{
				c.m_idLegSlot = (ItemLibrary.ArmorData)m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[18].Trim());
			}
			else
				c.m_idLegSlot = null;
			//Trinket1
			if(lines[20].Trim() != "NULL")
			{
				c.m_idTrinket1 = m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[19].Trim());
			}
			else
				c.m_idTrinket1 = null;
			//Trinket2
			if(lines[21].Trim() != "NULL")
			{
				c.m_idTrinket2 = m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(lines[20].Trim());
			}
			else
				c.m_idTrinket2 = null;
			//Character Bio
			c.m_szCharacterBio = lines[22].Trim();
			//SpellCount
			int amntOfSpells = int.Parse(lines[23].Trim());
			c.m_lSpellsKnown = new List<string>();
			for(int i = 0; i < amntOfSpells; ++i)
			{
				c.m_lSpellsKnown.Add(lines[24+i].Trim());
			}
			UpdateStats();
			GameObject.Find("PersistantData").GetComponent<DCScript>().SetRosteredCharacterData(c);
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
	void UpdateTargetReticles(bool _isAlly)
	{
		for(int i = 0; i < 6; ++i)
		{
			if(i == m_nTargetPositionOnField)
			{
				if(_isAlly == false)
					GameObject.Find("Enemy_Cursor"+i).GetComponent<SpriteRenderer>().enabled = true;
				else 
					GameObject.Find("Ally_Cursor"+i).GetComponent<SpriteRenderer>().enabled = true;
			}
			else
			{
				if(_isAlly == false)
					GameObject.Find("Enemy_Cursor"+i).GetComponent<SpriteRenderer>().enabled = false;
				else
					GameObject.Find("Ally_Cursor"+i).GetComponent<SpriteRenderer>().enabled = false;
			}
		}
	}
	void ClearTargetReticles()
	{
		for(int i = 0; i < 6; ++i)
		{
			GameObject.Find("Enemy_Cursor"+i).GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find("Ally_Cursor"+i).GetComponent<SpriteRenderer>().enabled = false;
		}
	}
	public void UpdatePositionOnField()
	{
		if(m_aAnim == null)
		{
			m_aAnim = GetComponent<Animator>();
			if(m_aAnim == null)
				m_aAnim = GetComponentInChildren<Animator>();
		}
		string szgoName = "Ally_StartPos" + FieldPosition.ToString();
		GameObject go = GameObject.Find(szgoName);
		m_vInitialPos = new Vector3();
		m_vInitialPos.x = go.transform.position.x;
		m_vInitialPos.y = go.transform.position.y;
		m_vInitialPos.z = 0.0f;
		transform.position = m_vInitialPos;
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
				m_aAnim.gameObject.GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder + 1;
			}
			break;
		case 2:
			{
				//Bottom right
				m_aAnim.gameObject.GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder + 2;
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
				m_aAnim.gameObject.GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder + 1;
			}
			break;
		case 5:
			{
				//Bottom left
				m_aAnim.gameObject.GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder + 2;
			}
			break;
		}
	}

	public void EnemyToAttackSelected(int p_nTargetPosition)
	{
		if(m_nState == (int)ALLY_STATES.ATTACK_CHOSEN)
		{
			GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach(GameObject e in _enemies)
			{
				if(e.GetComponent<UnitScript>().FieldPosition == p_nTargetPosition)
				{
					ClearTargetReticles();
					m_tTargetPositionOnField = GameObject.Find("Near_Enemy" + p_nTargetPosition).transform;

					if(m_nUnitType == (int)UnitTypes.ALLY_MELEE)
					{
						m_nState = (int)ALLY_STATES.ATTACKING;
						m_aAnim.SetBool("m_bIsMoving", true);
					}
					else if(m_nUnitType == (int)UnitTypes.ALLY_RANGED)
					{
						m_nState = (int)ALLY_STATES.ATTACK_RETURNING;
						m_aAnim.SetBool("m_bIsAttacking", true);
					}
				}
			}
		}
		else if(m_nState == (int) ALLY_STATES.ITEM_PICKED_SINGLEDMG)
		{
			//turn off the flags for the item/inventory rendering
			ClearTargetReticles();
			m_nState = (int)CAllyBattleScript.ALLY_STATES.USING_ITEM;
			m_goItemBeingUsed.GetComponent<ItemSingleDamage>().m_bShouldActivate = true;
		}
		else if(m_nState == (int)ALLY_STATES.ITEM_PICKED_AOEDMG)
		{
			m_nState = (int)CAllyBattleScript.ALLY_STATES.USING_ITEM;
			m_goItemBeingUsed.GetComponent<ItemGroupDamage>().m_bShouldActivate = true;
			ClearTargetReticles();
		}
		else if(m_nState == (int)ALLY_STATES.SPELL_PICKED_SINGLEDMG)
		{
			StartChargingSpell();
			m_nState = (int)CAllyBattleScript.ALLY_STATES.CHARGING_SPELL;
			m_aAnim.SetBool("m_bIsCasting", true);
			ClearTargetReticles();
		}
		else if(m_nState == (int)ALLY_STATES.SPELL_PICKED_AOEDMG)
		{
			StartChargingSpell();
			m_nState = (int)CAllyBattleScript.ALLY_STATES.CHARGING_SPELL;
			m_aAnim.SetBool("m_bIsCasting", true);
			ClearTargetReticles();
		}
	}

	public void AllyToActSelected(int p_nTargetPosition)
	{
		if(m_nState == (int)ALLY_STATES.ITEM_PICKED_SINGLEHEAL)
		{
			ClearTargetReticles();
			m_nState = (int)CAllyBattleScript.ALLY_STATES.USING_ITEM;
			m_goItemBeingUsed.GetComponent<ItemSingleHeal>().m_bShouldActivate = true;
		}
		else if(m_nState == (int)ALLY_STATES.ITEM_PICKED_AOEHEAL)
		{
			ClearTargetReticles();
			m_nState = (int)CAllyBattleScript.ALLY_STATES.USING_ITEM;
			m_goItemBeingUsed.GetComponent<ItemGroupHeal>().m_bShouldActivate = true;
		}
		else if(m_nState == (int)ALLY_STATES.SPELL_PICKED_SINGLEHEAL)
		{
			StartChargingSpell();
			ClearTargetReticles();
			m_aAnim.SetBool("m_bIsCasting", true);
			m_nState = (int)CAllyBattleScript.ALLY_STATES.CHARGING_SPELL;
		}
		else if(m_nState == (int)ALLY_STATES.SPELL_PICKED_AOEHEAL)
		{
			StartChargingSpell();
			ClearTargetReticles();
			m_aAnim.SetBool("m_bIsCasting", true);
			m_nState = (int)CAllyBattleScript.ALLY_STATES.CHARGING_SPELL;
		}
	}
		
	public override void AttackAnimationEnd() 
	{
		if(m_nUnitType == (int)UnitTypes.ALLY_MELEE)
		{
			m_nState = (int)ALLY_STATES.ATTACK_RETURNING;
			m_aAnim.SetBool("m_bIsMoving", true);
			if(CheckIfHit())
			{
				//HIT
				int dmgAdjustment = UnityEngine.Random.Range((int)(m_nTempStr * -0.2f), (int)(m_nTempStr * 0.2f)) + m_nTempStr;
				GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Enemy");
				foreach(GameObject tar in posTargs)
				{
					if(tar.GetComponent<UnitScript>().FieldPosition == m_nTargetPositionOnField)
					{
						tar.GetComponent<UnitScript>().AdjustHP(dmgAdjustment);
					}
				}
			}
			else
			{
				//MISS
				GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Enemy");
				foreach(GameObject tar in posTargs)
				{
					if(tar.GetComponent<UnitScript>().FieldPosition == m_nTargetPositionOnField)
					{
						tar.GetComponent<UnitScript>().Missed();
					}
				}
			}
		}
		else if(m_nUnitType == (int)UnitTypes.ALLY_RANGED)
		{
			
			GameObject goArrow = Instantiate(Resources.Load<GameObject>("Animation Effects/Arrow")) as GameObject;
			goArrow.transform.position = transform.position;
			int dmg = 0;
			if(CheckIfHit())
				dmg  = UnityEngine.Random.Range(1, 5) + m_nStr;
			else
				dmg = -1;
			goArrow.GetComponent<ProjectileScript>().m_fSpeed = 20;
			goArrow.GetComponent<ProjectileScript>().m_fRotationSpeed = 50;
			GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Enemy");
			foreach(GameObject tar in posTargs)
			{
				if(tar.GetComponent<UnitScript>().FieldPosition == m_nTargetPositionOnField)
				{
					goArrow.GetComponent<ProjectileScript>().m_goTarget = tar;
				}
			}
			goArrow.GetComponent<ProjectileScript>().m_nDamageDealt = dmg;
			Invoke("ChangeStateToStatusEffect", 2.0f);
		}
		m_aAnim.SetBool("m_bIsAttacking", false);
	}

	public override void CastingAnimationEnd()
	{
		if(m_nState == (int)ALLY_STATES.CHARGING_SPELL)
		{
			m_nState = (int)ALLY_STATES.CASTING_SPELL;
			m_aAnim.SetBool("m_bIsCasting", false);
			m_nCastingCounter += 1;
			if(m_nCastingCounter >= 2)
			{
				m_nCastingCounter = 0;
				m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().m_bShouldActivate = true;
			}
		}
	}

	public override void DamagedAnimationOver()
	{
		m_aAnim.SetBool("m_bIsDamaged", false);
	}

	//Can't think of a better way for ranged units to wait until their projectile hits their target to wait.. since there could be more than one projectile.. and it seems silly to create a pointer/counter
	void ChangeStateToStatusEffect()
	{
		m_nState = (int)ALLY_STATES.STATUS_EFFECTS;
	}

	public void AttemptToEscape()
	{
		float _fChanceToEscape = m_twTurnWatcher.GetChanceToEscape();
		if(_fChanceToEscape < 1.0f)
		{
			//Can't escape this fight, so auto fail.
			FailedToEscape();
			m_nState = (int)ALLY_STATES.STATUS_EFFECTS;
		}
		else
		{
			GameObject[] _allies = GameObject.FindGameObjectsWithTag("Ally");
			int partyAveLevel = 0;
			foreach(GameObject ally in _allies)
			{
				UnitScript unit = ally.GetComponent<UnitScript>();
				if(unit.GetCurHP() > 0)
				{
					partyAveLevel += unit.GetUnitLevel();
				}
			}
			partyAveLevel = partyAveLevel / _allies.Length;
			GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");
			int enemyAveLevel = 0;
			foreach(GameObject enemy in _enemies)
			{
				UnitScript unit = enemy.GetComponent<UnitScript>();
				if(unit.GetCurHP() > 0)
				{
					enemyAveLevel += GetUnitLevel();
				}
			}
			enemyAveLevel = enemyAveLevel / _enemies.Length;
			float escChanceMod = partyAveLevel - enemyAveLevel;

			int escRoll = UnityEngine.Random.Range(0, 100);

			Debug.Log("Esc base : " + _fChanceToEscape + "  Esc mod : " + escChanceMod + " Esc Roll : " + escRoll);
			if(escRoll <= escChanceMod + _fChanceToEscape)
			{
				//succeeded
				SucceededToEscape();
			}
			else
			{
				//failed to escape
				FailedToEscape();
			}
		}
	}
	void SucceededToEscape()
	{
		//TODO: add in a message about succeeding to escape before ending the fight
		m_nState = (int)ALLY_STATES.ESCAPING_SUCCEEDED;
		m_twTurnWatcher.PlayMessage("Successfully escaped the battle!");
	}
	void FailedToEscape()
	{
		//TODO: add in a message about failing to escape before transitioning states
		m_nState = (int)ALLY_STATES.ESCAPING_FAILED;
		m_twTurnWatcher.PlayMessage("Failed to escape the battle...");
	}
	bool CheckIfHit()
	{
		GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject tar in posTargs)
		{
			if(tar.GetComponent<UnitScript>().FieldPosition == m_nTargetPositionOnField)
			{
				int nChanceToHit = UnityEngine.Random.Range(0,100);
				int nRange = 85 + m_nHit - tar.GetComponent<UnitScript>().GetEVA();
				if(nRange < 5)
					nRange = 5;
				Debug.Log("Chance: " + nChanceToHit + "    Range: " + nRange);
				if(nChanceToHit <	nRange)
				{
					//Target was hit
					return true;
				}
				else
				{
					//target was missed
					return false;
				}
			}
		}
		return false;
	}

	new public void Missed()
	{
		GameObject newText = Instantiate(m_goFadingText);
		newText.GetComponent<GUI_FadeText>().SetColor(true);
		newText.GetComponent<GUI_FadeText>().SetText("Miss");
		newText.GetComponent<GUI_FadeText>().SetShouldFloat(true);
		Vector3 textPos = transform.GetComponent<Collider>().transform.position;
		textPos.y += (gameObject.GetComponent<BoxCollider>().size.y * 0.75f);
		textPos = Camera.main.WorldToViewportPoint(textPos);
		newText.transform.position = textPos;
	}

	new public void AdjustHP(int dmg)
	{
		GameObject newText = Instantiate(m_goFadingText);
		if(dmg >= 0)
		{
			if(m_bAmIDefending == true)
				dmg = dmg - GetTempDEF()*2;
			else 
				dmg = dmg - GetTempDEF();

			if(dmg < 0)
				dmg = 0;
		}
		m_nCurHP = Mathf.Clamp(m_nCurHP - dmg, 0, GetTempMaxHP());
		if(m_nCurHP <= 0)
		{
			m_nCurHP = 0;
			m_aAnim.SetBool("m_bIsDying", true);
			m_nState = (int)ALLY_STATES.DEAD;
			newText.GetComponent<GUI_FadeText>().SetColor(true);
			GameObject tw = GameObject.Find("TurnWatcher");
			if(tw)
			{
				EndMyTurn();
			}
		}
		else if(dmg >= 0)
		{
			m_aAnim.SetBool("m_bIsDamaged", true);
			newText.GetComponent<GUI_FadeText>().SetColor(true);
		}
		else
		{
			//Make sure that the cur HP never goes above the max hp
			newText.GetComponent<GUI_FadeText>().SetColor(false);
		}

		newText.GetComponent<GUI_FadeText>().SetText((Mathf.Abs(dmg)).ToString());
		newText.GetComponent<GUI_FadeText>().SetShouldFloat(true);
		Vector3 textPos = transform.GetComponent<Collider>().transform.position;
		textPos.y += (gameObject.GetComponent<BoxCollider>().size.y * 0.75f);
		textPos = Camera.main.WorldToViewportPoint(textPos);
		newText.transform.position = textPos;

	}

	public void SpellToUseSelected(string _szSpellName)
	{
		m_szSpellToCast = _szSpellName;

		if(m_nState == (int)ALLY_STATES.USEMAGIC_CHOSEN)
		{
			SpellLibrary.cSpellData _Spell =  m_dcPersistantData.m_lSpellLibrary.GetSpellFromLibrary(m_szSpellToCast);
			if(_Spell != null)
			{
				switch(_Spell.m_nTargetType)
				{
				case 1:
					{
						//Single Heal
						m_nState = (int)ALLY_STATES.SPELL_PICKED_SINGLEHEAL;
						m_nTargetPositionOnField = FieldPosition;
						UpdateTargetReticles(true);
					}
					break;
				case 2:
					{
						//Group Heal
						m_nState = (int)ALLY_STATES.SPELL_PICKED_AOEHEAL;
					}
					break;
				case 3:
					{
						//Single Damage
						m_nState = (int)ALLY_STATES.SPELL_PICKED_SINGLEDMG;
						InitializeTargetReticle();
						UpdateTargetReticles(false);
					}
					break;
				case 4:
					{
						//Group Damage
						m_nState = (int)ALLY_STATES.SPELL_PICKED_AOEDMG;
					}
					break;
				}
				m_goItemBeingUsed = Instantiate(Resources.Load("Animation Effects/Spell Effects" + "/" + _Spell.m_szSpellName)) as GameObject;
				m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().m_szSpellName = _Spell.m_szSpellName;
				m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().m_nElementType = _Spell.m_nElementType;
				m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().m_nTargetType = _Spell.m_nTargetType;
				m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().m_nMPCost = _Spell.m_nMPCost;
				m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().m_nHPMod = _Spell.m_nHPMod;
				m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().m_nMPMod = _Spell.m_nMPMod;
				m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().m_nPOWMod = _Spell.m_nPOWMod;
				m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().m_nDEFMod = _Spell.m_nDEFMod;
				m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().m_nSPDMod = _Spell.m_nSPDMod;
				m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().m_nHITMod = _Spell.m_nHITMod;
				m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().m_nEVAMod = _Spell.m_nEVAMod;
				m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().m_lStatusEffect = _Spell.m_lStatusEffects;

				m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().Initialize(gameObject);

			}
		}
	}

	void StartChargingSpell()
	{
		SpellLibrary.cSpellData _Spell =  m_dcPersistantData.m_lSpellLibrary.GetSpellFromLibrary(m_szSpellToCast);
		switch(_Spell.m_nElementType)
		{
		case 0:
			{
				//Water
				Vector3 newPos = transform.position;
				newPos.y -= m_aAnim.gameObject.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f;
				GameObject WaterRune = Instantiate(Resources.Load("Animation Effects/Spell Effects/CastEffects/Cast_Water") as GameObject, newPos, Quaternion.identity) as GameObject;
				WaterRune.GetComponent<CastingScript>().m_goOwner = gameObject;
			}
			break;
		case 1:
			{
				//Earth
				Vector3 newPos = transform.position;
				newPos.y -= m_aAnim.gameObject.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f;
				GameObject EarthRune = Instantiate(Resources.Load("Animation Effects/Spell Effects/CastEffects/Cast_Earth") as GameObject, newPos, Quaternion.identity) as GameObject;
				EarthRune.GetComponent<CastingScript>().m_goOwner = gameObject;
			}
			break;
		case 2:
			{
				//Wind
				Vector3 newPos = transform.position;
				newPos.y -= m_aAnim.gameObject.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f;
				GameObject WindRune = Instantiate(Resources.Load("Animation Effects/Spell Effects/CastEffects/Cast_Wind") as GameObject, newPos, Quaternion.identity) as GameObject;
				WindRune.GetComponent<CastingScript>().m_goOwner = gameObject;
			}
			break;
		case 3:
			{
				//Fire
				Vector3 newPos = transform.position;
				newPos.y -= m_aAnim.gameObject.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f;
				GameObject FireRune = Instantiate(Resources.Load("Animation Effects/Spell Effects/CastEffects/Cast_Fire") as GameObject, newPos, Quaternion.identity) as GameObject;
				FireRune.GetComponent<CastingScript>().m_goOwner = gameObject;
			}
			break;
		case 4:
			{
				//Dark
				Vector3 newPos = transform.position;
				newPos.y -= m_aAnim.gameObject.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f;
				GameObject DarkRune = Instantiate(Resources.Load("Animation Effects/Spell Effects/CastEffects/Cast_Dark") as GameObject, newPos, Quaternion.identity) as GameObject;
				DarkRune.GetComponent<CastingScript>().m_goOwner = gameObject;
			}
			break;
		case 5:
			{
				//Light
				Vector3 newPos = transform.position;
				newPos.y -= m_aAnim.gameObject.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f;
				GameObject LightRune = Instantiate(Resources.Load("Animation Effects/Spell Effects/CastEffects/Cast_Light") as GameObject, newPos, Quaternion.identity) as GameObject;
				LightRune.GetComponent<CastingScript>().m_goOwner = gameObject;
			}
			break;
		}
	}

	public override void ChargingOver()
	{
		m_nCastingCounter += 1;
		if(m_nCastingCounter >= 2)
		{
			m_nCastingCounter = 0;
			m_goItemBeingUsed.GetComponent<BaseSpellBattleScript>().m_bShouldActivate = true;
		}
	}

	public void ItemToUseSelected(string _szItem)
	{
		if(m_nState == (int)ALLY_STATES.USEITEM_CHOSEN)
		{
			ItemLibrary.ItemData ItemData = m_dcPersistantData.m_lItemLibrary.GetItemFromDictionary(_szItem);

			if(ItemData != null)
			{
				switch(ItemData.m_nItemType)
				{
				case (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL:
					{
						m_goItemBeingUsed = Instantiate(Resources.Load("Items/SingleItemHeal")) as GameObject;
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetItemName(ItemData.m_szItemName);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetDescription(ItemData.m_szDescription);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetItemType(ItemData.m_nItemType);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetHPMod(ItemData.m_nHPMod);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetPowMod(ItemData.m_nPowMod);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetDefMod(ItemData.m_nDefMod);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetSpdMod(ItemData.m_nSpdMod);

						m_goItemBeingUsed.GetComponent<ItemSingleHeal>().Initialize();
						m_goItemBeingUsed.GetComponent<ItemSingleHeal>().m_dFunc(gameObject);
						m_nTargetPositionOnField = FieldPosition;
						UpdateTargetReticles(true);
						m_nState = (int)ALLY_STATES.ITEM_PICKED_SINGLEHEAL;
					}
					break;
				case (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL:
					{
						m_goItemBeingUsed = Instantiate(Resources.Load("Items/GroupItemHeal")) as GameObject;
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetItemName(ItemData.m_szItemName);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetDescription(ItemData.m_szDescription);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetItemType(ItemData.m_nItemType);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetHPMod(ItemData.m_nHPMod);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetPowMod(ItemData.m_nPowMod);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetDefMod(ItemData.m_nDefMod);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetSpdMod(ItemData.m_nSpdMod);

						m_goItemBeingUsed.GetComponent<ItemGroupHeal>().Initialize();
						m_goItemBeingUsed.GetComponent<ItemGroupHeal>().m_dFunc(gameObject);
						m_nTargetPositionOnField = FieldPosition;
						m_nState = (int)ALLY_STATES.ITEM_PICKED_AOEHEAL;
					}
					break;
				case (int)BaseItemScript.ITEM_TYPES.eSINGLE_DAMAGE:
					{
						m_goItemBeingUsed = Instantiate(Resources.Load("Items/SingleItemDamage")) as GameObject;
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetItemName(ItemData.m_szItemName);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetDescription(ItemData.m_szDescription);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetItemType(ItemData.m_nItemType);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetHPMod(ItemData.m_nHPMod);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetPowMod(ItemData.m_nPowMod);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetDefMod(ItemData.m_nDefMod);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetSpdMod(ItemData.m_nSpdMod);

						m_goItemBeingUsed.GetComponent<ItemSingleDamage>().Initialize();
						m_goItemBeingUsed.GetComponent<ItemSingleDamage>().m_dFunc(gameObject);
						InitializeTargetReticle();
						UpdateTargetReticles(false);
						m_nState = (int)ALLY_STATES.ITEM_PICKED_SINGLEDMG;
					}
					break;
				case (int)BaseItemScript.ITEM_TYPES.eGROUP_DAMAGE:
					{
						m_goItemBeingUsed = Instantiate(Resources.Load("Items/GroupItemDamage")) as GameObject;
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetItemName(ItemData.m_szItemName);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetDescription(ItemData.m_szDescription);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetItemType(ItemData.m_nItemType);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetHPMod(ItemData.m_nHPMod);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetPowMod(ItemData.m_nPowMod);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetDefMod(ItemData.m_nDefMod);
						m_goItemBeingUsed.GetComponent<BaseItemScript>().SetSpdMod(ItemData.m_nSpdMod);

						m_goItemBeingUsed.GetComponent<ItemGroupDamage>().Initialize();
						m_goItemBeingUsed.GetComponent<ItemGroupDamage>().m_dFunc(gameObject);
						m_nState = (int)ALLY_STATES.ITEM_PICKED_AOEDMG;
					}
					break;
				}

			}
		}
	}

	public Texture2D TextureFromSprite(Sprite sprite)
	{
		if(sprite.rect.width != sprite.texture.width)
		{
			Texture2D newText = new Texture2D((int)sprite.rect.width,(int)sprite.rect.height);
			Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
				(int)sprite.textureRect.y,
				(int)sprite.textureRect.width,
				(int)sprite.textureRect.height);
			newText.SetPixels(newColors);
			newText.Apply();
			return newText;
		}
		else
			return sprite.texture;
	}
}
