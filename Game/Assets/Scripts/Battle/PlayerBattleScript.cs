using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class PlayerBattleScript : UnitScript {
	public Texture2D m_t2dTexture;
	int m_nSelectionIndex = 0;
	int m_nMaxChoices = 4;
	//flag for if the character has chosen to pick a spell to cast
	bool m_bMagicChosen = false;
	public void SetMagicChosen(bool flag) {m_bMagicChosen = flag;}
	int m_nMagicSelectionIndex = 0;
	//flag for if the character has chosen to pick an item to cast
	bool m_bItemChosen = false;
	public void SetItemChosen(bool flag) {m_bItemChosen = flag;}
	//flag for if the character has chosen which spell to cast
	bool m_bChoosingMagic = false;
	public void SetChoosingMagicFlag(bool flag) {m_bChoosingMagic = flag;}

	//The position of the scroll bar when the player selects "item"
	Vector2 m_vItemScrollPosition = Vector2.zero;
	//Positional data for which item is currently being selected (which item will be used if the user presses the enter key)
	Vector2 m_vItemSecondScroll = Vector2.zero;
	//float for how tall each item box is
	float m_fItemHeight = 35.0f;
	//flag for if the character has chosen to choose a target to attack
	bool m_bAttackChosen = false;
	//flag for if the character has selected what Item to use
	bool m_bChoosingItem = false;
	public void SetChoosingItemFlag(bool flag) {m_bChoosingItem = flag;}


	enum States{eIDLE, eCHARGE, eRETURN, eATTACK, eDAMAGED, eDEAD, eDEFEND, eMAGIC, eITEM};
	float m_fMovementSpeed = 8.0f;
	float m_fAttackBucket = 0.0f;
	float m_fDeadBucket = 0.0f;
	float m_fDamagedBucket = 0.0f;
	Animator anim;
	//animation for the unit dying
	public AnimationClip m_acDyingAnim;
	//animation for the unit taking damage
	public AnimationClip m_acDamagedAnim;
	//animatino for the units basic attack
	public AnimationClip m_acAttackAnim;

	//bool for if the player needs to switch back to the defend state after taking damage
	bool m_bWasDefending = false;

	//for fading text
	public GameObject m_goFadingText;

	//for casting/using items/being healed/getting buffed... other.. positive things? lol
	public GameObject m_goUnitArm;

	//for audios for attack effects
	public AudioClip m_acAttackAudio;
	//Sound before character charges
	public AudioClip m_acChargeClip;

	DCScript dataCan;

	//Spells this unit can cast
	List<string> m_lSpellList = new List<string>();
	public List<string> GetSpellList() {return m_lSpellList;}
	public void SetSpellList(List<string> list) {m_lSpellList = list;}
	public void AddSpell(string spell) {m_lSpellList.Add(spell);}
	public void RemoveSpell(string spell) {m_lSpellList.Remove(spell);}



	//Stuff for the shadow clones that spawn during movement... maybe special attacks if I have time?
	public GameObject m_goShadowClone;
	float m_fShadowTimer = 0.0f;
	float m_fShadowTimerBucket = 0.1f;

	//For starting stats
	public TextAsset m_taStartingStats;
	//The large bust of the character
	public Texture2D m_tLargeBust;

	//hardcoded experience marker, to make experience harder to gain as you level... decriment the incoming exp depending on
	//what level you are?  That would require enemies to have a "level"
	int m_nExperienceToLevel = 1000;
	public int GetExperienceToLevel() {return m_nExperienceToLevel;}

	//The class that this character is, this will adjust how the character gains stats when they level
	public string m_szClassName;

	//Each time the unit levels, reset this to 0
	int m_nCurrentExperience = 0;
	public int GetCurrentExperience() {return m_nCurrentExperience;}
	public void SetCurrentExperience(int exp) {m_nCurrentExperience = exp;}

	DCScript.ArmorData m_idHelmSlot;
	public DCScript.ArmorData GetHelmSlot() {return m_idHelmSlot;}
	public void SetHelmSlotData(DCScript.ArmorData armor) {m_idHelmSlot = armor;}

	DCScript.ArmorData m_idShoulderSlot;
	public DCScript.ArmorData GetShoulderSlot() {return m_idShoulderSlot;}
	public void SetShoulderSlotData(DCScript.ArmorData armor) {m_idShoulderSlot = armor;}

	DCScript.ArmorData m_idChestSlot;
	public DCScript.ArmorData GetChestSlot() {return m_idChestSlot;}
	public void SetChestSlotData(DCScript.ArmorData armor) {m_idChestSlot = armor;}

	DCScript.ArmorData m_idGloveSlot;
	public DCScript.ArmorData GetGloveSlot() {return m_idGloveSlot;}
	public void SetGloveSlotData(DCScript.ArmorData armor) {m_idGloveSlot = armor;}

	DCScript.ArmorData m_idBeltSlot;
	public DCScript.ArmorData GetBeltSlot() {return m_idBeltSlot;}
	public void SetBeltSlotData(DCScript.ArmorData armor) {m_idBeltSlot = armor;}

	DCScript.ArmorData m_idLegSlot;
	public DCScript.ArmorData GetLegSlot() {return m_idLegSlot;}
	public void SetLegSlotData(DCScript.ArmorData armor) {m_idLegSlot = armor;}


	//Trinket that the character is wearing
	DCScript.ItemData m_idTrinket1;
	public DCScript.ItemData GetTrinket1() {return m_idTrinket1;}
	public void SetTrinket1Data(DCScript.ItemData trinket) {m_idTrinket1 = trinket;}

	DCScript.ItemData m_idTrinket2;
	public DCScript.ItemData GetTrinket2() {return m_idTrinket2;}
	public void SetTrinket2Data(DCScript.ItemData trinket) {m_idTrinket2 = trinket;}

	public void LevelUp()
	{
		SetUnitLevel(GetUnitLevel() +1);
		DCScript.ClassStats c = dataCan.GetClassType(m_szClassName);
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

	// Use this for initialization
	void Start () 
	{
		dataCan = GameObject.Find("PersistantData").GetComponent<DCScript>();
		anim = GetComponent<Animator>();
		if(m_acDyingAnim)
			m_fDeadBucket = m_acDyingAnim.length;
		m_vInitialPos = new Vector3();
		string szgoName = "Ally_StartPos" + m_nPositionOnField.ToString();
		GameObject go = GameObject.Find(szgoName);
		m_vInitialPos.x = go.transform.position.x;
		m_vInitialPos.y = go.transform.position.y;
		m_vInitialPos.z = 0.0f;
		switch(m_nPositionOnField)
		{
		case 0:
		{
			//Middle
		}
			break;
		case 1:
		{
			//Top
			if(GetComponent<SpriteRenderer>() != null)
				GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
			else 
				GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder - 1;
		}
			break;
		case 2:
		{
			//Bottom
			if(GetComponent<SpriteRenderer>() != null)
				GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
			else 
				GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder + 1;
		}
			break;
		}
		transform.position = m_vInitialPos;
		InitializeTargetReticle();


		//if the MaxHP is zero, the character wasn't loaded in from a previous save file, and stats need to be initialized to the base stats of that character
		if(GetMaxHP() <= 0)
			SetUnitStats();

		//Grab any status effects that are currently effect this character.
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

	void UpdateStats(DCScript.ItemData armor, DCScript.CharacterData c)
	{
		if(c != null && armor != null)
		{
			c.m_nMaxHP += armor.m_nHPMod;
			c.m_nCurHP = c.m_nMaxHP;
			c.m_nSTR += armor.m_nPowMod;
			c.m_nDEF += armor.m_nDefMod;
			c.m_nSPD += armor.m_nSpdMod;
			c.m_nEVA += armor.m_nEvaMod;
			c.m_nHIT += armor.m_nHitMod;
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
				c.m_idHelmSlot = (DCScript.ArmorData)GameObject.Find("PersistantData").GetComponent<DCScript>().GetItemFromDictionary(lines[13].Trim());
				UpdateStats(c.m_idHelmSlot, c);
			}
			else
				c.m_idHelmSlot = null;
			//Shoulders
			if(lines[14].Trim() != "NULL")
			{
				c.m_idShoulderSlot = (DCScript.ArmorData)GameObject.Find("PersistantData").GetComponent<DCScript>().GetItemFromDictionary(lines[14].Trim());
				UpdateStats(c.m_idShoulderSlot, c);
			}
			else
				c.m_idShoulderSlot = null;
			//Chest
			if(lines[15].Trim() != "NULL")
			{
				c.m_idChestSlot = (DCScript.ArmorData)GameObject.Find("PersistantData").GetComponent<DCScript>().GetItemFromDictionary(lines[15].Trim());
				UpdateStats (c.m_idChestSlot, c);
			}
			else
				c.m_idChestSlot = null;
			//Arms
			if(lines[16].Trim() != "NULL")
			{
				c.m_idGloveSlot = (DCScript.ArmorData)GameObject.Find("PersistantData").GetComponent<DCScript>().GetItemFromDictionary(lines[16].Trim());
				UpdateStats(c.m_idGloveSlot, c);
			}
			else
				c.m_idGloveSlot = null;
			//Waist
			if(lines[17].Trim() != "NULL")
			{
				c.m_idBeltSlot = (DCScript.ArmorData)GameObject.Find("PersistantData").GetComponent<DCScript>().GetItemFromDictionary(lines[17].Trim());
				UpdateStats(c.m_idBeltSlot, c);
			}
			else
				c.m_idBeltSlot = null;
			//Legs
			if(lines[18].Trim() != "NULL")
			{
				c.m_idLegSlot = (DCScript.ArmorData)GameObject.Find("PersistantData").GetComponent<DCScript>().GetItemFromDictionary(lines[18].Trim());
				UpdateStats(c.m_idLegSlot, c);
			}
			else
				c.m_idLegSlot = null;
			//Trinket1
			if(lines[19].Trim() != "NULL")
			{
				c.m_idTrinket1 = GameObject.Find("PersistantData").GetComponent<DCScript>().GetItemFromDictionary(lines[19].Trim());
				UpdateStats(c.m_idTrinket1, c);
			}
			else
				c.m_idTrinket1 = null;
			//Trinket2
			if(lines[20].Trim() != "NULL")
			{
				c.m_idTrinket2 = GameObject.Find("PersistantData").GetComponent<DCScript>().GetItemFromDictionary(lines[20].Trim());
				UpdateStats(c.m_idTrinket2, c);
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
			GameObject.Find("PersistantData").GetComponent<DCScript>().AddPartyMember(c);
		}
	}
	// Update is called once per frame
	void Update () 
	{
		//If I was defending, and now I'm in idle, switch back to defend state and set my flag to false
		if(m_bWasDefending == true && m_nState == (int)States.eIDLE)
		{
			m_nState = (int)States.eDEFEND;
			m_bWasDefending = false;
		}

		if(m_bIsMyTurn)
		{
			if(m_bFirstPass == true)
			{
				m_bFirstPass = false;
				m_bAllowInput = false;
				StartMyTurn();
			}
			if(m_nState == (int)States.eDEFEND)
				m_nState = (int)States.eIDLE;
			if(m_bAllowInput == true)
				HandleInput();

		}
		HandleState();
	
	}

	void HandleInput()
	{
		if(Input.GetKeyUp(KeyCode.DownArrow))
		{
			if(m_bMagicChosen == false && m_bItemChosen == false && m_bAttackChosen == false)
			{
				m_nSelectionIndex++;
				if(m_nSelectionIndex >= m_nMaxChoices)
					m_nSelectionIndex = 0;
			}
			else if(m_bItemChosen == true && m_bChoosingItem == false)
			{
				List<DCScript.CharactersItems> theInv = GetItemsOfType(0);
				m_vItemSecondScroll.y += m_fItemHeight;
				if(m_vItemSecondScroll.y > (theInv.Count-1) * m_fItemHeight)
					m_vItemSecondScroll.y = (theInv.Count-1) * m_fItemHeight;
				if(m_vItemSecondScroll.y < m_fItemHeight * 4)
					m_vItemScrollPosition.y = 0.0f;
				else if(m_vItemSecondScroll.y < m_fItemHeight * 8)
					m_vItemScrollPosition.y = m_fItemHeight * 4;
				else if(m_vItemSecondScroll.y < m_fItemHeight * 12)
					m_vItemScrollPosition.y = m_fItemHeight * 8;
			}
			else if(m_bAttackChosen == true)
			{
				
				
				switch(m_nTargetPositionOnField)
				{
				case 0:
					m_nTargetPositionOnField = 2;
					break;
				case 1:
					m_nTargetPositionOnField = 0;
					break;
				case 2:
					m_nTargetPositionOnField = 1;
					break;
				}
				PositionTargetReticleDown();
				
			}
			else if(m_bMagicChosen == true && m_bChoosingMagic == false)
			{
				m_nMagicSelectionIndex++;
				if(m_nMagicSelectionIndex >= m_lSpellList.Count)
					m_nMagicSelectionIndex = 0;
			}
		}
		else if(Input.GetKeyUp(KeyCode.UpArrow))
		{
			if(m_bMagicChosen == false && m_bItemChosen == false && m_bAttackChosen == false)
			{
				m_nSelectionIndex--;
				if(m_nSelectionIndex < 0)
					m_nSelectionIndex = m_nMaxChoices - 1;
			}
			else if(m_bItemChosen == true && m_bChoosingItem == false)
			{
				m_vItemSecondScroll.y -= m_fItemHeight;
				if(m_vItemSecondScroll.y < 0)
					m_vItemSecondScroll.y = 0;
				
				if(m_vItemSecondScroll.y < m_fItemHeight * 4)
					m_vItemScrollPosition.y = 0.0f;
				else if(m_vItemSecondScroll.y < m_fItemHeight * 8)
					m_vItemScrollPosition.y = m_fItemHeight * 4;
				else if(m_vItemSecondScroll.y < m_fItemHeight * 12)
					m_vItemScrollPosition.y = m_fItemHeight * 8;
			}
			else if(m_bAttackChosen == true)
			{
				
				//Set the new targetting iter and make sure it stays in bounds
				switch(m_nTargetPositionOnField)
				{
				case 0:
					m_nTargetPositionOnField = 1;
					break;
				case 1:
					m_nTargetPositionOnField = 2;
					break;
				case 2:
					m_nTargetPositionOnField = 0;
					break;
				}
				PositionTargetReticleUp();
				
			}
			else if(m_bMagicChosen == true && m_bChoosingMagic == false)
			{
				m_nMagicSelectionIndex--;
				if(m_nMagicSelectionIndex < 0)
					m_nMagicSelectionIndex = m_lSpellList.Count - 1;
			}
		}
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(m_bChoosingMagic == true)
				m_bChoosingMagic = false;
			else if(m_bMagicChosen == true)
				m_bMagicChosen = false;
			m_bAttackChosen = false;
			if(m_bChoosingItem == true)
				m_bChoosingItem = false;
			else if(m_bItemChosen == true)
				m_bItemChosen = false;
			
			DisableAllCursors();
		}
		if(Input.GetKeyUp(KeyCode.Return))
		{
			if(m_bMagicChosen == false && m_bItemChosen == false && m_bAttackChosen == false)
			{
				InitializeTargetReticle();
				switch(m_nSelectionIndex)
				{
				case 0:
				{
					//Attack
					m_bAttackChosen = true;
					#region Enable/Disable Cursors
					if(m_nTargetPositionOnField == 0)
					{
						//disable 1, 2
						GameObject.Find("Enemy_Cursor1").GetComponent<SpriteRenderer>().enabled = false;
						GameObject.Find("Enemy_Cursor2").GetComponent<SpriteRenderer>().enabled = false;
						//enable 0
						GameObject.Find("Enemy_Cursor0").GetComponent<SpriteRenderer>().enabled = true;
					}
					else if(m_nTargetPositionOnField == 1)
					{
						//disable 0, 2
						GameObject.Find("Enemy_Cursor0").GetComponent<SpriteRenderer>().enabled = false;
						GameObject.Find("Enemy_Cursor2").GetComponent<SpriteRenderer>().enabled = false;
						//enable 1
						GameObject.Find("Enemy_Cursor1").GetComponent<SpriteRenderer>().enabled = true;
					}
					else if(m_nTargetPositionOnField == 2)
					{
						//disable 1, 0
						GameObject.Find("Enemy_Cursor1").GetComponent<SpriteRenderer>().enabled = false;
						GameObject.Find("Enemy_Cursor0").GetComponent<SpriteRenderer>().enabled = false;
						//enable 0
						GameObject.Find("Enemy_Cursor2").GetComponent<SpriteRenderer>().enabled = true;
					}
					#endregion
				}
					break;
				case 1:
				{
					//Magic
					m_bMagicChosen = true;
				}
					break;
				case 2:
				{
					//Item
					m_bItemChosen = true;
				}
					break;
					
				case 3:
				{
					//Defend
					m_nState = (int)States.eDEFEND;
					m_bIsMyTurn = false;
					m_bWasDefending = true;
					TurnOffFlags();
					EndMyTurn();
					//Having an issue where the enter key gets spammed after a player defends.. let's see if this jank fix resolves it!
					Input.ResetInputAxes();
				}
					break;
				}
				
			}
			else if(m_bAttackChosen == true)
			{
				DisableAllCursors();
				if(m_nUnitType == (int)UnitTypes.ALLY_MELEE)
				{
					m_nState = (int)States.eCHARGE;
					if(m_acChargeClip != null)
						GetComponent<AudioSource>().PlayOneShot(m_acChargeClip, 0.5f + dataCan.m_fSFXVolume);
					anim.SetBool("m_bIsMoving", true);
				}
				else if(m_nUnitType == (int)UnitTypes.ALLY_RANGED)
				{
					anim.SetBool("m_bIsAttacking", true);
					m_nState = (int)States.eATTACK;
					m_fAttackBucket = m_acAttackAnim.length;

				}
				m_bIsMyTurn = false;
			}
			else if(m_bItemChosen == true && m_bChoosingItem == false)
			{
				m_bChoosingItem = true;
				DisableAllCursors();
				int itemIter = (int)(m_vItemSecondScroll.y / m_fItemHeight);
				DCScript dcs = dataCan.GetComponent<DCScript>();

				DCScript.ItemData ItemData = dcs.GetItemFromDictionary(dcs.GetInventory()[itemIter].m_szItemName);
				if(ItemData != null)
				{
					switch(ItemData.m_nItemType)
					{
					case (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL:
					{
						GameObject gItem = Instantiate(Resources.Load("Items/SingleItemHeal")) as GameObject;
						gItem.GetComponent<BaseItemScript>().SetItemName(ItemData.m_szItemName);
						gItem.GetComponent<BaseItemScript>().SetDescription(ItemData.m_szDescription);
						gItem.GetComponent<BaseItemScript>().SetItemType(ItemData.m_nItemType);
						gItem.GetComponent<BaseItemScript>().SetHPMod(ItemData.m_nHPMod);
						gItem.GetComponent<BaseItemScript>().SetPowMod(ItemData.m_nPowMod);
						gItem.GetComponent<BaseItemScript>().SetDefMod(ItemData.m_nDefMod);
						gItem.GetComponent<BaseItemScript>().SetSpdMod(ItemData.m_nSpdMod);

						gItem.GetComponent<ItemSingleHeal>().Initialize();
						gItem.GetComponent<ItemSingleHeal>().m_dFunc(gameObject);
					}
						break;
					case (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL:
					{
						GameObject gItem = Instantiate(Resources.Load("Items/GroupItemHeal")) as GameObject;
						gItem.GetComponent<BaseItemScript>().SetItemName(ItemData.m_szItemName);
						gItem.GetComponent<BaseItemScript>().SetDescription(ItemData.m_szDescription);
						gItem.GetComponent<BaseItemScript>().SetItemType(ItemData.m_nItemType);
						gItem.GetComponent<BaseItemScript>().SetHPMod(ItemData.m_nHPMod);
						gItem.GetComponent<BaseItemScript>().SetPowMod(ItemData.m_nPowMod);
						gItem.GetComponent<BaseItemScript>().SetDefMod(ItemData.m_nDefMod);
						gItem.GetComponent<BaseItemScript>().SetSpdMod(ItemData.m_nSpdMod);

						gItem.GetComponent<ItemGroupHeal>().Initialize();
						gItem.GetComponent<ItemGroupHeal>().m_dFunc(gameObject);
					}
						break;
					case (int)BaseItemScript.ITEM_TYPES.eSINGLE_DAMAGE:
					{
						GameObject gItem = Instantiate(Resources.Load("Items/SingleItemDamage")) as GameObject;
						gItem.GetComponent<BaseItemScript>().SetItemName(ItemData.m_szItemName);
						gItem.GetComponent<BaseItemScript>().SetDescription(ItemData.m_szDescription);
						gItem.GetComponent<BaseItemScript>().SetItemType(ItemData.m_nItemType);
						gItem.GetComponent<BaseItemScript>().SetHPMod(ItemData.m_nHPMod);
						gItem.GetComponent<BaseItemScript>().SetPowMod(ItemData.m_nPowMod);
						gItem.GetComponent<BaseItemScript>().SetDefMod(ItemData.m_nDefMod);
						gItem.GetComponent<BaseItemScript>().SetSpdMod(ItemData.m_nSpdMod);

						gItem.GetComponent<ItemSingleDamage>().Initialize();
						gItem.GetComponent<ItemSingleDamage>().m_dFunc(gameObject);
					}
						break;
					case (int)BaseItemScript.ITEM_TYPES.eGROUP_DAMAGE:
					{
						GameObject gItem = Instantiate(Resources.Load("Items/GroupItemDamage")) as GameObject;
						gItem.GetComponent<BaseItemScript>().SetItemName(ItemData.m_szItemName);
						gItem.GetComponent<BaseItemScript>().SetDescription(ItemData.m_szDescription);
						gItem.GetComponent<BaseItemScript>().SetItemType(ItemData.m_nItemType);
						gItem.GetComponent<BaseItemScript>().SetHPMod(ItemData.m_nHPMod);
						gItem.GetComponent<BaseItemScript>().SetPowMod(ItemData.m_nPowMod);
						gItem.GetComponent<BaseItemScript>().SetDefMod(ItemData.m_nDefMod);
						gItem.GetComponent<BaseItemScript>().SetSpdMod(ItemData.m_nSpdMod);

						gItem.GetComponent<ItemGroupDamage>().Initialize();
						gItem.GetComponent<ItemGroupDamage>().m_dFunc(gameObject);
					}
						break;
					}
					
				}
			}
			else if(m_bMagicChosen == true && m_bChoosingMagic == false)
			{
				m_bChoosingMagic = true;
				DisableAllCursors();
				//Cast depending on what spell is chosen
				switch(m_lSpellList[m_nMagicSelectionIndex])
				{

				case "Kind Rain":
				{
					Vector3 pos = transform.position;
					GameObject KindRain = Instantiate(Resources.Load("Spell Effects/KindRain"), pos, Quaternion.identity) as GameObject;
					KindRain.GetComponent<KindRainScript>().KindRainFunction(gameObject);
				}
					break;
				case "Thunder":
				{
					Vector3 pos = transform.position;
					GameObject Thunder = Instantiate(Resources.Load("Spell Effects/Thunder"), pos, Quaternion.identity) as GameObject;
					Thunder.GetComponent<ThunderScript>().ThunderFunction(gameObject);
				}
					break;
				}
			}
		}
	}


	bool CheckIfHit()
	{
		GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject tar in posTargs)
		{
			if(tar.GetComponent<UnitScript>().m_nPositionOnField == m_nTargetPositionOnField)
			{
				int nChanceToHit = UnityEngine.Random.Range(0,100);
				int nRange = 60 + m_nHit - tar.GetComponent<UnitScript>().GetEVA();
				if(nRange < 5)
					nRange = 5;
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

	void HandleState()
	{
		switch(m_nState)
		{
		case (int)States.eIDLE:
		{
		}
			break;
		case (int)States.eCHARGE:
		{
			GameObject target = GameObject.Find("Enemy_StartPos" + m_nTargetPositionOnField.ToString());
			if(target)
			{
				Vector3 targetPos = target.transform.position;
				Vector3 dir = targetPos - transform.position;
				dir.Normalize();
				Vector3 curPos = transform.position;
				curPos += dir * m_fMovementSpeed * Time.deltaTime;
				transform.position = curPos;

				if(m_fShadowTimer >= m_fShadowTimerBucket)
				{
					
					m_goShadowClone.GetComponent<SpriteRenderer>().sprite = anim.gameObject.GetComponent<SpriteRenderer>().sprite;
					Vector3 cloneTransform = anim.gameObject.transform.localScale;
					m_goShadowClone.transform.localScale = cloneTransform;
					Vector3 pos = transform.position;
					//adjust so the clone is behind the unit
					pos.z += 0.1f;
					
					GameObject shadowClone = Instantiate(m_goShadowClone, pos, Quaternion.identity) as GameObject;
					if(shadowClone)
						Destroy(shadowClone, m_fShadowTimerBucket*3);
					m_fShadowTimer = 0.0f;
				}
				else
					m_fShadowTimer += Time.deltaTime;
			}
		}
			break;
		case (int)States.eRETURN:
		{
			GameObject target = GameObject.Find("Ally_StartPos" + m_nPositionOnField.ToString());
			if(target)
			{
				Vector3 targetPos = target.transform.position;
				Vector3 dir = targetPos - transform.position;
				dir.Normalize();
				Debug.Log(dir);
				Vector3 curPos = transform.position;
				curPos += dir * m_fMovementSpeed * Time.deltaTime;
				transform.position = curPos;
			}
		}
			break;
		case (int)States.eATTACK:
		{
			m_fAttackBucket -= Time.deltaTime;


			if(m_fAttackBucket <= 0.0001f)
			{
				if(m_nUnitType == (int)UnitTypes.ALLY_MELEE)
				{
					m_nState = (int)States.eRETURN;
					anim.SetBool("m_bIsMoving", true);
					if(CheckIfHit())
					{
						//HIT
						int dmgAdjustment = UnityEngine.Random.Range(1, 5) + m_nStr;
						GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Enemy");
						foreach(GameObject tar in posTargs)
						{
							if(tar.GetComponent<UnitScript>().m_nPositionOnField == m_nTargetPositionOnField)
							{
								tar.GetComponent<UnitScript>().AdjustHP(dmgAdjustment);
							}
						}
					}
					else
					{
						//MISS
					}
				}
				else if(m_nUnitType == (int)UnitTypes.ALLY_RANGED)
				{

					m_nState = (int)States.eIDLE;
					GameObject goArrow = Instantiate(Resources.Load<GameObject>("Spell Effects/Arrow")) as GameObject;
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
						if(tar.GetComponent<UnitScript>().m_nPositionOnField == m_nTargetPositionOnField)
						{
							goArrow.GetComponent<ProjectileScript>().m_goTarget = tar;
						}
					}
					goArrow.GetComponent<ProjectileScript>().m_nDamageDealt = dmg;

					TurnOffFlags();
					Invoke("EndMyTurn", 2.0f);
				}
				anim.SetBool("m_bIsAttacking", false);
				m_fAttackBucket = 0.0f;
				GameObject GO = GameObject.Find("PersistantData");
				if(GO != null)
					GetComponent<AudioSource>().PlayOneShot(m_acAttackAudio, 0.5f + GO.GetComponent<DCScript>().m_fSFXVolume);

			
			}
		}
			break;
		case (int) States.eITEM:
		{
		}
			break;
		case (int)States.eMAGIC:
		{



		}
			break;
		case (int) States.eDAMAGED:
		{
			m_fDamagedBucket -= Time.deltaTime;
			if(m_fDamagedBucket <= 0.0f)
			{
				m_nState = (int)States.eIDLE;
				anim.SetBool("m_bIsDamaged", false);
			}
		}
			break;
		case (int)States.eDEAD:
		{
			m_fDeadBucket -= Time.deltaTime;
			if(m_fDeadBucket <= 0.0f)
			{
				//end
				anim.SetBool("m_bIsDead", true);
			}
		}
			break;
		}
	}

	void OnGUI()
	{
		if(m_bIsMyTurn)
		{
			if(m_bMagicChosen == false && m_bItemChosen == false && m_bAttackChosen == false)
			{
				//draw background
				GUI.Box(new Rect(Screen.width * 0.4f,Screen.height * 0.6f, 125, 150), "");
				GUIStyle textStyle = new GUIStyle();
				textStyle.fontSize = 24;
				//draw options
				GUI.Label(new Rect(Screen.width * 0.4f + 10,Screen.height * 0.6f, 10, 5), "Attack", textStyle);
				GUI.Label(new Rect(Screen.width * 0.4f + 10,Screen.height * 0.6f + 24, 10, 5), "Magic", textStyle);
				GUI.Label(new Rect(Screen.width * 0.4f + 10,Screen.height * 0.6f + 48, 10, 5), "Item", textStyle);
				GUI.Label(new Rect(Screen.width * 0.4f + 10,Screen.height * 0.6f + 72, 10, 5), "Defend", textStyle);
			
				//draw selector
				GUIStyle selectorStyle = new GUIStyle(GUI.skin.box);
				selectorStyle.normal.background = m_t2dTexture;
				//draw the selector box for the dialogue choice
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
				GUI.Box((new Rect(Screen.width * 0.4f + 10,Screen.height * 0.6f + (m_nSelectionIndex * 24), 80, 25)), "",selectorStyle);
			}
			else if(m_bAttackChosen == true)
			{
			}
			else if(m_bItemChosen == true)
			{
				float xPos = 0.0f; float yPos = 0.0f;
				float xWidth = 100.0f;  
				float yHeight = m_fItemHeight;
				float widthAdjustment = 0.0f;
				float heightAdjustment = 25.0f;
				List<DCScript.CharactersItems> theInv = GetItemsOfType(0);
				int itemCount = theInv.Count;

				if(itemCount < 5)
				{
					widthAdjustment = 13;
					heightAdjustment = 0;
				}

				Rect theBox = new Rect(Screen.width * 0.5f - xWidth*0.5f, Screen.height * 0.5f - yHeight*0.5f, xWidth* 1.1f - widthAdjustment, yHeight * 4);
				GUI.Box(theBox, "");
				m_vItemScrollPosition = GUI.BeginScrollView(theBox,m_vItemScrollPosition, new Rect(0, 0, theBox.width*0.8f, yHeight * itemCount + heightAdjustment),false, false);
				foreach(DCScript.CharactersItems item in theInv)
				{
					if(item.m_nItemType >= (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL && item.m_nItemType <= (int)BaseItemScript.ITEM_TYPES.eGROUP_DAMAGE)
					{
						GUI.Button(new Rect(xPos, yPos, xWidth, yHeight), item.m_szItemName);
						yPos += yHeight;
					}
				}

				//draw selector
				GUIStyle selectorStyle = new GUIStyle(GUI.skin.box);
				selectorStyle.normal.background = m_t2dTexture;
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
				GUI.Box((new Rect(-2, m_vItemSecondScroll.y+2, xWidth +2, yHeight -2)), "",selectorStyle);

				GUI.EndScrollView();


			}
			else
			{
				//draw background
				GUI.Box(new Rect(Screen.width * 0.4f,Screen.height * 0.6f, 125, 150), "");
				GUIStyle textStyle = new GUIStyle(); 
				textStyle.fontSize = 24;




				int spellCounter = 0;


				foreach(string s in m_lSpellList)
				{
					GUI.Label(new Rect(Screen.width * 0.4f + 10,Screen.height * 0.6f + (24*spellCounter), 10, 5), s, textStyle);
					spellCounter++;
				}


				//draw selector
				GUIStyle selectorStyle = new GUIStyle(GUI.skin.box);
				selectorStyle.normal.background = m_t2dTexture;
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
				GUI.Box((new Rect(Screen.width * 0.4f + 10,Screen.height * 0.6f + (m_nMagicSelectionIndex * 24), 
				                 100, 25)), "",selectorStyle);
			}


		}
	}

	List<DCScript.CharactersItems> GetItemsOfType(int type)
	{
		List<DCScript.CharactersItems> inv = new List<DCScript.CharactersItems>();
		List<DCScript.CharactersItems> fullInv =  dataCan.GetInventory();
		foreach(DCScript.CharactersItems item in fullInv)
		{
			// 0 - useable item, 1- Armor, 2- Trinkets, 3- Junk
			//1-4 : useable item, 5 : weapon, 6: armor, 7: junk
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
			}
		}
		return inv;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.name == "Near_Enemy" + m_nTargetPositionOnField.ToString() || other.name == "Ally_StartPos" + m_nPositionOnField.ToString())
			WaypointTriggered(other);
	}
	public void WaypointTriggered(Collider c)
	{
		if(c.name ==  "Near_Enemy" + m_nTargetPositionOnField.ToString())
		{
			anim.SetBool("m_bIsMoving", false);
			anim.SetBool("m_bIsAttacking", true);
			m_nState = (int)States.eATTACK;
			m_fAttackBucket = m_acAttackAnim.length;
			c.enabled = false;
			GameObject wypnt = GameObject.Find("Ally_StartPos" + m_nPositionOnField.ToString());
			if(wypnt)
			{
				wypnt.GetComponent<BoxCollider>().enabled = true;
			}


		}
		if(c.name ==  "Ally_StartPos" + m_nPositionOnField.ToString() && m_bIsMyTurn == true)
		{
			if(anim)
				anim.SetBool("m_bIsMoving", false);
			c.enabled = false;
			m_nState = (int)States.eIDLE;
			//Set the x/y to the original position to make sure there's no jittering
			transform.position = m_vInitialPos;
			GameObject wypnt = GameObject.Find("Near_Enemy" + m_nTargetPositionOnField.ToString());
			if(wypnt)
			{
				wypnt.GetComponent<BoxCollider>().enabled = true;
			}
			TurnOffFlags();
			EndMyTurn();
		}
	}

	int AdjustDefense(int def)
	{
		if(m_idChestSlot != null)
		{
			def += m_idChestSlot.m_nDefMod;
			GameObject gItem = Instantiate(Resources.Load<GameObject>("Items/Armor")) as GameObject;
			gItem.GetComponent<BaseItemScript>().SetItemName(m_idChestSlot.m_szItemName);
			gItem.GetComponent<BaseItemScript>().SetDescription(m_idChestSlot.m_szDescription);
			gItem.GetComponent<BaseItemScript>().SetItemType(m_idChestSlot.m_nItemType);
			gItem.GetComponent<BaseItemScript>().SetHPMod(m_idChestSlot.m_nHPMod);
			gItem.GetComponent<BaseItemScript>().SetPowMod(m_idChestSlot.m_nPowMod);
			gItem.GetComponent<BaseItemScript>().SetDefMod(m_idChestSlot.m_nDefMod);
			gItem.GetComponent<BaseItemScript>().SetSpdMod(m_idChestSlot.m_nSpdMod);
			gItem.GetComponent<ArmorItemScript>().SetSpecialItemType(m_idChestSlot.m_nSpecialType);
			gItem.GetComponent<ArmorItemScript>().SetSpecialItemModifier(m_idChestSlot.m_nSpecialModifier);
			gItem.GetComponent<ArmorItemScript>().Initialize();
			gItem.GetComponent<ArmorItemScript>().m_dFunc(gameObject);
			
		}


		return def;
	}

	public void TurnOffFlags()
	{
		m_bAttackChosen = false;
		if(m_bChoosingMagic == true)
			m_bChoosingMagic = false;
		else if(m_bMagicChosen == true)
			m_bMagicChosen = false;
		if(m_bChoosingItem == true)
			m_bChoosingItem = false;
		else if(m_bItemChosen == true)
			m_bItemChosen = false;
	}

	new public void AdjustHP(int dmg)
	{
		if(dmg >= 0)
		{
			int totalDefense = AdjustDefense(m_nDef);

			if(m_nState == (int)States.eDEFEND)
				dmg = dmg - totalDefense*2;
			else 
				dmg = dmg - totalDefense;
		
			if(dmg < 0)
				dmg = 0;
		}
		m_nCurHP -= dmg;
		if(m_nCurHP <= 0)
		{
			m_nCurHP = 0;
			anim.SetBool("m_bIsDying", true);
			m_nState = (int)States.eDEAD;
			m_goFadingText.GetComponent<GUI_FadeText>().SetColor(true);
			GameObject tw = GameObject.Find("TurnWatcher");
			if(tw)
			{
				TurnOffFlags();
				tw.GetComponent<TurnWatcherScript>().RemoveMeFromList(gameObject, m_acDyingAnim.length);
			}
		}
		else if(dmg >= 0)
		{
			anim.SetBool("m_bIsDamaged", true);
			if(m_nState == (int)States.eDEFEND)
				m_bWasDefending = true;
			m_nState = (int)States.eDAMAGED;
			m_fDamagedBucket = m_acDamagedAnim.length;
			m_goFadingText.GetComponent<GUI_FadeText>().SetColor(true);
		}
		else
		{
			//Make sure that the cur HP never goes above the max hp
			if(GetCurHP() > GetMaxHP())
				SetCurHP(GetMaxHP());
			m_goFadingText.GetComponent<GUI_FadeText>().SetColor(false);
		}
		
		m_goFadingText.GetComponent<GUI_FadeText>().SetText((Mathf.Abs(dmg)).ToString());
		m_goFadingText.GetComponent<GUI_FadeText>().SetShouldFloat(true);
		Vector3 textPos = transform.GetComponent<Collider>().transform.position;
		textPos.y += (gameObject.GetComponent<BoxCollider>().size.y * 0.75f);
		textPos = Camera.main.WorldToViewportPoint(textPos);
		m_goFadingText.transform.position = textPos;
		Instantiate(m_goFadingText);

	}

	void InitializeTargetReticle()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		GameObject zero = null, one = null, two = null;
		foreach(GameObject enemy in enemies)
		{
			if(enemy.GetComponent<UnitScript>().GetCurHP() > 0)
			switch(enemy.GetComponent<UnitScript>().m_nPositionOnField)
			{
			case 0:
			{
				zero = enemy;
			}
				break;
			case 1:
			{
				one = enemy;
			}
				break;
			case 2:
			{
				two = enemy;
			}
				break;
			}
		}

		if(zero)
			m_nTargetPositionOnField = 0;
		else if(one)
			m_nTargetPositionOnField = 1;
		else if(two)
			m_nTargetPositionOnField = 2;
	}

	void PositionTargetReticleUp()
	{

		#region Acquire Enemies
		//acquire the available enmies on field, set flags for which positions
		//on the field are able to be targetted
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		bool zero = false, one = false, two = false;
		foreach(GameObject enemy in enemies)
		{
			if(enemy.GetComponent<UnitScript>().GetCurHP() > 0)
			switch(enemy.GetComponent<UnitScript>().m_nPositionOnField)
			{
			case 0:
			{
				zero = true;
			}
				break;
			case 1:
			{
				one = true;
			}
				break;
			case 2:
			{
				two = true;
			}
				break;
			}
		}
		#endregion
		
		#region AdjustTargetting
		//Adjust the targetting iter if it's at an unavailable target
		if(m_nTargetPositionOnField == 0)
		{
			//Was at bottom target, trying to go up one.
			if(zero == true)
			{
				//target is available.
			}
			else
			{
				//target unavailable, try moving target to the top position
				if(one == true)
				{
					//top target available
					m_nTargetPositionOnField = 1;
				}
				else
				{
					//no other targets available, remain at the bottom target.
					m_nTargetPositionOnField = 2;
				}
			}
		}
		else if(m_nTargetPositionOnField == 1)
		{
			//Was at middle, trying to go to top target
			if(one == true)
			{
				//Top target available
			}
			else
			{
				//Top target was unavailable, try moving target to the bottom position
				if(two == true)
				{
					//Bottom target is available
					m_nTargetPositionOnField = 2;
				}
				else
				{
					//No other target available, remain at the middle position target
					m_nTargetPositionOnField = 0;
				}
			}
		}
		else
		{
			//Was at the top, trying to wrap around to the bottom target
			if(two == true)
			{
				//bottom target available
			}
			else
			{
				//try to wrap to the middle target
				if(zero == true)
				{
					//middle target available
					m_nTargetPositionOnField = 0;
				}
				else
				{
					//no other target is available, remain at the top target.
					m_nTargetPositionOnField = 1;
				}
			}
		}
		#endregion

		#region Enable/Disable Cursors
		if(m_nTargetPositionOnField == 0)
		{
			//disable 1, 2
			GameObject.Find("Enemy_Cursor1").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find("Enemy_Cursor2").GetComponent<SpriteRenderer>().enabled = false;
			//enable 0
			GameObject.Find("Enemy_Cursor0").GetComponent<SpriteRenderer>().enabled = true;
		}
		else if(m_nTargetPositionOnField == 1)
		{
			//disable 0, 2
			GameObject.Find("Enemy_Cursor0").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find("Enemy_Cursor2").GetComponent<SpriteRenderer>().enabled = false;
			//enable 1
			GameObject.Find("Enemy_Cursor1").GetComponent<SpriteRenderer>().enabled = true;
		}
		else if(m_nTargetPositionOnField == 2)
		{
			//disable 1, 0
			GameObject.Find("Enemy_Cursor1").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find("Enemy_Cursor0").GetComponent<SpriteRenderer>().enabled = false;
			//enable 0
			GameObject.Find("Enemy_Cursor2").GetComponent<SpriteRenderer>().enabled = true;
		}
		#endregion

	}

	void PositionTargetReticleDown()
	{
		#region Acquire Enemies
		//acquire the available enmies on field, set flags for which positions
		//on the field are able to be targetted
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		bool zero = false, one = false, two = false;
		foreach(GameObject enemy in enemies)
		{
			if(enemy.GetComponent<UnitScript>().GetCurHP() > 0)
			switch(enemy.GetComponent<UnitScript>().m_nPositionOnField)
			{
			case 0:
			{
				zero = true;
			}
				break;
			case 1:
			{
				one = true;
			}
				break;
			case 2:
			{
				two = true;
			}
				break;
			}
		}
		#endregion
		
		#region Adjust Targetting
		if(m_nTargetPositionOnField == 0)
		{
			//Player is moving target from top to middle
			if(zero == true)
			{
				//middle target is available
			}
			else
			{
				//target unavailable, try targetting bottom target
				if(two == true)
				{
					//Bottom target available
					m_nTargetPositionOnField = 2;
				}
				else
				{
					//No other target available, retarget the top target
					m_nTargetPositionOnField = 1;
				}
			}
		}
		else if(m_nTargetPositionOnField == 1)
		{
			//Player is moving target from bottom to top
			if(one == true)
			{
				//target available
			}
			else
			{
				//target unavailable, try to wrap to middle target instead
				if(zero == true)
				{
					//Middle target available
					m_nTargetPositionOnField = 0;
				}
				else
				{
					//no other target available, retarget the bottom target
					m_nTargetPositionOnField = 2;
				}
			}
		}
		else
		{
			//Player is moving from middle to bottom target
			if(two == true)
			{
				//Initial target available
			}
			else
			{
				//Try to wrap to the top target
				if(one == true)
				{
					//Top target is available
					m_nTargetPositionOnField = 1;
				}
				else
				{
					//no other target available, retarget the middle target
					m_nTargetPositionOnField = 0;
				}
			}
		}
		
		#endregion
		#region Enable/Disable Cursors
		if(m_nTargetPositionOnField == 0)
		{
			//disable 1, 2
			GameObject.Find("Enemy_Cursor1").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find("Enemy_Cursor2").GetComponent<SpriteRenderer>().enabled = false;
			//enable 0
			GameObject.Find("Enemy_Cursor0").GetComponent<SpriteRenderer>().enabled = true;
		}
		else if(m_nTargetPositionOnField == 1)
		{
			//disable 0, 2
			GameObject.Find("Enemy_Cursor0").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find("Enemy_Cursor2").GetComponent<SpriteRenderer>().enabled = false;
			//enable 1
			GameObject.Find("Enemy_Cursor1").GetComponent<SpriteRenderer>().enabled = true;
		}
		else if(m_nTargetPositionOnField == 2)
		{
			//disable 1, 0
			GameObject.Find("Enemy_Cursor1").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find("Enemy_Cursor0").GetComponent<SpriteRenderer>().enabled = false;
			//enable 0
			GameObject.Find("Enemy_Cursor2").GetComponent<SpriteRenderer>().enabled = true;
		}
		#endregion
	}

	void DisableAllCursors()
	{
		for(int i = 0; i < 3; ++i)
			GameObject.Find("Enemy_Cursor" + i).GetComponent<SpriteRenderer>().enabled = false;
		for(int i = 0; i < 3; ++i)
			GameObject.Find("Ally_Cursor" + i).GetComponent<SpriteRenderer>().enabled = false;
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
		SetCurrentExperience(GetCurrentExperience() + nTotalExp);
		while(GetCurrentExperience() >= m_nExperienceToLevel && GetUnitLevel() < 99)
		{
			LevelUp();
			SetCurrentExperience(GetCurrentExperience() - m_nExperienceToLevel);
		}
		if(GetUnitLevel() == 99 && GetCurrentExperience() > 999)
		{
			SetCurrentExperience(999);
			return 0;
		}
		return nTotalExp;
	}
}
