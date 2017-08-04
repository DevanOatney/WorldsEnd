using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DCScript : MonoBehaviour 
{
	[HideInInspector]
	public float m_fMasterVolume = 0.5f;
	[HideInInspector]
	public float m_fMusicVolume = 0.5f;
	[HideInInspector]
	public float m_fSFXVolume = 0.5f;
	[HideInInspector]
	public float m_fVoiceVolume = 0.5f;
	[HideInInspector]
	public float m_fBrightness = 0.0f;
	[HideInInspector]
	public bool m_bToUseBattleAnimations = true;
	[HideInInspector]
	public int m_nTextSpeed = 2;
	//List of characters in party
	List<CharacterData> m_lPartyMembers = new List<CharacterData>();
	//List of character in the roster
	List<CharacterData> m_lRoster = new List<CharacterData>();
    //List of Generals in army
    List<FightSceneControllerScript.cWarUnit> m_lAllyUnits = new List<FightSceneControllerScript.cWarUnit>();
    
	//flag field that is toggled depending on what the players actions are in the game
	//["Name"] - 1 if activated 								  : Place Created
	public Dictionary<string, int> m_dStoryFlagField = new Dictionary<string, int>();
	//Dictionary for the state of the base.
	//("BlacksmithLevel", 0-3) shows the current progress of the blacksmith shop.

	public Dictionary<string, int> m_dBaseFlagField = new Dictionary<string, int>();

	//-------
	//FIELD INFORMATION

	//for encounter information
	List<EncounterGroupLoaderScript.cEnemyData> m_lEnemies;
	public List<EncounterGroupLoaderScript.cEnemyData> GetEnemyNames() {return m_lEnemies;}
	public void SetEnemyNames(List<EncounterGroupLoaderScript.cEnemyData> e) {m_lEnemies = e;}

    //For war battle encounter
    public string m_szWarBattleDataPath;

	//The name of the scene that the player was on before the encounter started.  Needs to be set at the field just before the battle is started.
	//Also using this for loading/saving to be the current scene that the player is in when they save.
	string m_szPreviousField;
	public string GetPreviousFieldName() {return m_szPreviousField;}
	public void SetPreviousFieldName(string s) {m_szPreviousField = s;}
	//The position of the player on the field before the encounter starts.  Set before battle is loaded, and before a field is loaded.
	Vector3 m_vPreviousPosition = new Vector3();
	public Vector3 GetPreviousPosition() {return m_vPreviousPosition;}
	public void SetPreviousPosition(Vector3 v) {m_vPreviousPosition = v;}
	//The direction in which the player was previously facing
	int m_nFacingDirection = 0;
	public int GetPreviousFacingDirection() {return m_nFacingDirection;}
	public void SetPreviousFacingDirection(int fd) {m_nFacingDirection = fd;}
	//The starting point that the character should go to (used from screen switching)
	string m_szNextStartingPosition;
	public void SetStartingPos(string strtPos) {m_szNextStartingPosition = strtPos;}
	public string GetStartingPos() {return m_szNextStartingPosition;}


	//amount of cash that the player has
	[HideInInspector]
	public int m_nGold;
	[HideInInspector]
	public int m_nMusicIter;

	//The amount of time that the player has played for.
	[HideInInspector]
	public float m_fTimePlayed = 0.0f;

	//the library of items (holds data for every item in the game, and also the inventory for the player)
	public ItemLibrary m_lItemLibrary = new ItemLibrary();

	//the library of spells that exist in the game. (loaded at runtime, does not need to be saved/loaded with player)
	public SpellLibrary m_lSpellLibrary = new SpellLibrary();

	//the library of all of the status effects that exist in the game
	public StatusEffectLibrary m_lStatusEffectLibrary = new StatusEffectLibrary();
	//("BaseUnlocked", 0) - just a flag for if the base has been unlocked (defeated the boss boar at the lowest level), the return value is unimportant for now
	//("BlacksmithLevel", 0-3) - Each level increases the amount of stuff in the blacksmith's area.


	[System.Serializable]
	public class StatusEffect
	{
		public string m_szEffectName;
		public int m_nEffectType; //0-Poison, 1-Paralyze, 2-Stone
		public int m_nStartingTickCount;
		public int m_nHPMod;
		public int m_nMPMod;
		public int m_nPOWMod;
		public int m_nDEFMod;
		public int m_nSPDMod;
		public int m_nHITMod;
		public int m_nEVAMod;
		public List<cEffectedMember> m_lEffectedMembers = new List<cEffectedMember>();
		[System.Serializable]
		public class cEffectedMember
		{
			public cEffectedMember(string _name, int _ticksLeft) {m_szCharacterName = _name;m_nTicksLeft = _ticksLeft;}
			public string m_szCharacterName;
			public int m_nTicksLeft;
		}
		public cEffectedMember GetMember(string _name)
		{
			foreach(cEffectedMember _mem in m_lEffectedMembers)
			{
				if(_mem.m_szCharacterName == _name)
					return _mem;
			}
			return null;
		}
		public bool RemoveMember(string _name)
		{
			for(int i = m_lEffectedMembers.Count - 1; i >= 0; --i)
			{
				if(m_lEffectedMembers[i].m_szCharacterName == _name)
				{
					m_lEffectedMembers.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

	}
	//list of status effects that are inflicting the party
	List<StatusEffect> m_lStatusEffects = new List<StatusEffect>();
	public List<StatusEffect> GetStatusEffects() {return m_lStatusEffects;}
	public StatusEffect GetStatusEffect(string szName) {foreach(StatusEffect se in m_lStatusEffects){if(se.m_szEffectName == szName)return se;}return null;}
	public void SetStatusEffects(List<StatusEffect> l) {m_lStatusEffects.Clear(); m_lStatusEffects = l;}
	public void AddStatusEffect(StatusEffect se)
	{
		int catchIter = IsStatusEffectInList(se.m_nEffectType);
		if(catchIter != -1)
		{
			foreach(DCScript.StatusEffect.cEffectedMember c in se.m_lEffectedMembers)
			{
				bool inList = false;
				//for each effected unit, check all effected units, if you're not in it.. add this unit to the effected units
				foreach(DCScript.StatusEffect.cEffectedMember s in m_lStatusEffects[catchIter].m_lEffectedMembers)
				{
					if(s.m_szCharacterName == c.m_szCharacterName)
						inList = true;
				}
				if(inList == false)
				{
					m_lStatusEffects[catchIter].m_lEffectedMembers.Add(c);
				}
				return;
			}
		}
		else
		{
			m_lStatusEffects.Add (se);
		}
	}

	public void RemoveStatusEffect(string _szName)
	{
		for(int i = m_lStatusEffects.Count - 1; i >= 0; --i)
		{
			if(m_lStatusEffects[i].m_szEffectName == _szName)
			{
				m_lStatusEffects.RemoveAt(i);
				return;
			}
		}
	}

	public int IsStatusEffectInList(int szEffectType)
	{
		int counter = 0;
		foreach(StatusEffect se in m_lStatusEffects)
		{
			if(se.m_nEffectType == szEffectType)
				return counter;
			counter++;
		}
		return -1;
	}

	[System.Serializable]
	public class CharacterData
	{
		public string m_szCharacterName;
		public int m_nMaxHP, m_nCurHP, m_nMaxMP, m_nCurMP, m_nSTR, m_nDEF, m_nSPD, m_nEVA, m_nHIT, m_nLevel, m_nCurrentEXP;

		public string m_szWeaponName;
		public int m_nWeaponDamageModifier;
		public int m_nWeaponLevel;
		public string m_szWeaponModifierName;

		public ItemLibrary.ArmorData m_idChestSlot = null;
		public ItemLibrary.ArmorData m_idLegSlot = null;
		public ItemLibrary.ArmorData m_idBeltSlot = null;
		public ItemLibrary.ArmorData m_idShoulderSlot = null;
		public ItemLibrary.ArmorData m_idHelmSlot = null;
		public ItemLibrary.ArmorData m_idGloveSlot = null;

		public ItemLibrary.ArmorData m_idTrinket1 = null;
		public ItemLibrary.ArmorData m_idTrinket2 = null;

		public List<string> m_lSpellsKnown;

		public string m_szCharacterRace;
		public string m_szCharacterClassType;
		public string m_szCharacterBio;
		public int m_nFormationIter = 5;

		//if this unit is currently in the party
		public bool m_bIsInParty = false;
		//if this unit has been recruited to the roster
		public bool m_bHasBeenRecruited = false;

		public ItemLibrary.ArmorData EquipArmor(ItemLibrary.ArmorData _iArmor, int _nTrinketSlotNum)
		{
			ItemLibrary.ArmorData _armor = null;
			switch (_iArmor.m_nItemType) {
			case (int)BaseItemScript.ITEM_TYPES.eCHESTARMOR:
				{
					_armor = m_idChestSlot;
					m_idChestSlot = _iArmor;
					return _armor;
				}
			case (int)BaseItemScript.ITEM_TYPES.eLEGARMOR:
				{
					_armor = m_idLegSlot;
					m_idLegSlot = _iArmor;
					return _armor;
				}
			case (int)BaseItemScript.ITEM_TYPES.eBELTARMOR:
				{
					_armor = m_idBeltSlot;
					m_idBeltSlot = _iArmor;
					return _armor;
				}
			case (int)BaseItemScript.ITEM_TYPES.eSHOULDERARMOR:
				{
					_armor = m_idShoulderSlot;
					m_idShoulderSlot = _iArmor;
					return _armor;
				}
			case (int)BaseItemScript.ITEM_TYPES.eHELMARMOR:
				{
					_armor = m_idHelmSlot;
					m_idHelmSlot = _iArmor;
					return _armor;
				}
			case (int)BaseItemScript.ITEM_TYPES.eGLOVEARMOR:
				{
					_armor = m_idGloveSlot;
					m_idGloveSlot = _iArmor;
					return _armor;
				}
			case (int)BaseItemScript.ITEM_TYPES.eTRINKET:
				{
					if (_nTrinketSlotNum == 1) {
						_armor = m_idTrinket1;
						m_idTrinket1 = _iArmor;
						return _armor;
					}
					else {
						_armor = m_idTrinket2;
						m_idTrinket2 = _iArmor;
						return _armor;
					}
				}
			}
			return _armor;
		}

		//1 - HP, 2 - MP, 3 - POW, 4 - DEF, 5 - SPD, 6 - EVA, 7 - HIT
		public int GetStatWithGearInfluencing(int _statIter)
		{
			switch (_statIter) {
			case 1:
				{
					//HP
					int _returnHP = m_nMaxHP;
					if(m_idChestSlot != null)
						_returnHP += m_idChestSlot.m_nHPMod;
					if (m_idLegSlot != null)
						_returnHP += m_idLegSlot.m_nHPMod;
					if (m_idBeltSlot != null)
						_returnHP += m_idBeltSlot.m_nHPMod;
					if (m_idShoulderSlot != null)
						_returnHP += m_idShoulderSlot.m_nHPMod;
					if (m_idHelmSlot != null)
						_returnHP += m_idHelmSlot.m_nHPMod;
					if (m_idGloveSlot != null)
						_returnHP += m_idGloveSlot.m_nHPMod;
					if (m_idTrinket1 != null)
						_returnHP += m_idTrinket1.m_nHPMod;
					if (m_idTrinket2 != null)
						_returnHP += m_idTrinket2.m_nHPMod;
					return _returnHP;
				}
			case 2:
				{
					//MP
					int _returnMP = m_nMaxMP;
					if(m_idChestSlot != null)
						_returnMP += m_idChestSlot.m_nMPMod;
					if (m_idLegSlot != null)
						_returnMP += m_idLegSlot.m_nMPMod;
					if (m_idBeltSlot != null)
						_returnMP += m_idBeltSlot.m_nMPMod;
					if (m_idShoulderSlot != null)
						_returnMP += m_idShoulderSlot.m_nMPMod;
					if (m_idHelmSlot != null)
						_returnMP += m_idHelmSlot.m_nMPMod;
					if (m_idGloveSlot != null)
						_returnMP += m_idGloveSlot.m_nMPMod;
					if (m_idTrinket1 != null)
						_returnMP += m_idTrinket1.m_nMPMod;
					if (m_idTrinket2 != null)
						_returnMP += m_idTrinket2.m_nMPMod;
					return _returnMP;
				}
			case 3:
				{
					//POW
					int _returnPOW = m_nSTR;
					if(m_idChestSlot != null)
						_returnPOW += m_idChestSlot.m_nPowMod;
					if (m_idLegSlot != null)
						_returnPOW += m_idLegSlot.m_nPowMod;
					if (m_idBeltSlot != null)
						_returnPOW += m_idBeltSlot.m_nPowMod;
					if (m_idShoulderSlot != null)
						_returnPOW += m_idShoulderSlot.m_nPowMod;
					if (m_idHelmSlot != null)
						_returnPOW += m_idHelmSlot.m_nPowMod;
					if (m_idGloveSlot != null)
						_returnPOW += m_idGloveSlot.m_nPowMod;
					if (m_idTrinket1 != null)
						_returnPOW += m_idTrinket1.m_nPowMod;
					if (m_idTrinket2 != null)
						_returnPOW += m_idTrinket2.m_nPowMod;
					return _returnPOW;
				}
			case 4:
				{
					//DEF
					int _returnDEF = m_nDEF;
					if(m_idChestSlot != null)
						_returnDEF += m_idChestSlot.m_nDefMod;
					if (m_idLegSlot != null)
						_returnDEF += m_idLegSlot.m_nDefMod;
					if (m_idBeltSlot != null)
						_returnDEF += m_idBeltSlot.m_nDefMod;
					if (m_idShoulderSlot != null)
						_returnDEF += m_idShoulderSlot.m_nDefMod;
					if (m_idHelmSlot != null)
						_returnDEF += m_idHelmSlot.m_nDefMod;
					if (m_idGloveSlot != null)
						_returnDEF += m_idGloveSlot.m_nDefMod;
					if (m_idTrinket1 != null)
						_returnDEF += m_idTrinket1.m_nDefMod;
					if (m_idTrinket2 != null)
						_returnDEF += m_idTrinket2.m_nDefMod;
					return _returnDEF;
				}
			case 5:
				{
					//SPD
					int _returnSPD = m_nSPD;
					if(m_idChestSlot != null)
						_returnSPD += m_idChestSlot.m_nSpdMod;
					if (m_idLegSlot != null)
						_returnSPD += m_idLegSlot.m_nSpdMod;
					if (m_idBeltSlot != null)
						_returnSPD += m_idBeltSlot.m_nSpdMod;
					if (m_idShoulderSlot != null)
						_returnSPD += m_idShoulderSlot.m_nSpdMod;
					if (m_idHelmSlot != null)
						_returnSPD += m_idHelmSlot.m_nSpdMod;
					if (m_idGloveSlot != null)
						_returnSPD += m_idGloveSlot.m_nSpdMod;
					if (m_idTrinket1 != null)
						_returnSPD += m_idTrinket1.m_nSpdMod;
					if (m_idTrinket2 != null)
						_returnSPD += m_idTrinket2.m_nSpdMod;
					return _returnSPD;
				}
			case 6:
				{
					//EVA
					int _returnEVA = m_nSPD;
					if(m_idChestSlot != null)
						_returnEVA += m_idChestSlot.m_nEvaMod;
					if (m_idLegSlot != null)
						_returnEVA += m_idLegSlot.m_nEvaMod;
					if (m_idBeltSlot != null)
						_returnEVA += m_idBeltSlot.m_nEvaMod;
					if (m_idShoulderSlot != null)
						_returnEVA += m_idShoulderSlot.m_nEvaMod;
					if (m_idHelmSlot != null)
						_returnEVA += m_idHelmSlot.m_nEvaMod;
					if (m_idGloveSlot != null)
						_returnEVA += m_idGloveSlot.m_nEvaMod;
					if (m_idTrinket1 != null)
						_returnEVA += m_idTrinket1.m_nEvaMod;
					if (m_idTrinket2 != null)
						_returnEVA += m_idTrinket2.m_nEvaMod;
					return _returnEVA;
				}
			case 7:
				{
					//HIT
					int _returnHIT = m_nHIT;
					if(m_idChestSlot != null)
						_returnHIT += m_idChestSlot.m_nHitMod;
					if (m_idLegSlot != null)
						_returnHIT += m_idLegSlot.m_nHitMod;
					if (m_idBeltSlot != null)
						_returnHIT += m_idBeltSlot.m_nHitMod;
					if (m_idShoulderSlot != null)
						_returnHIT += m_idShoulderSlot.m_nHitMod;
					if (m_idHelmSlot != null)
						_returnHIT += m_idHelmSlot.m_nHitMod;
					if (m_idGloveSlot != null)
						_returnHIT += m_idGloveSlot.m_nHitMod;
					if (m_idTrinket1 != null)
						_returnHIT += m_idTrinket1.m_nHitMod;
					if (m_idTrinket2 != null)
						_returnHIT += m_idTrinket2.m_nHitMod;
					return _returnHIT;
				}
			default:
				{
					return -1;
				}
			}
		}


		public void UpdateCharacterData(CharacterData newData)
		{
			m_szCharacterName = newData.m_szCharacterName;
			m_nMaxHP = newData.m_nMaxHP;
			m_nCurHP = newData.m_nCurHP;
			m_nMaxMP = newData.m_nMaxMP;
			m_nCurMP = newData.m_nCurMP;
			m_nSTR = newData.m_nSTR;
			m_nDEF = newData.m_nDEF;
			m_nSPD = newData.m_nSPD;
			m_nEVA = newData.m_nEVA;
			m_nHIT = newData.m_nHIT;
			m_nLevel = newData.m_nLevel;
			m_nCurrentEXP = newData.m_nCurrentEXP;

			m_szWeaponName = newData.m_szWeaponName;
			m_nWeaponDamageModifier = newData.m_nWeaponDamageModifier;
			m_nWeaponLevel = newData.m_nWeaponLevel;
			m_szWeaponModifierName = newData.m_szWeaponModifierName;

			m_idChestSlot = newData.m_idChestSlot;
			m_idLegSlot = newData.m_idLegSlot;
			m_idBeltSlot = newData.m_idBeltSlot;
			m_idShoulderSlot = newData.m_idShoulderSlot;
			m_idHelmSlot = newData.m_idHelmSlot;
			m_idGloveSlot = newData.m_idGloveSlot;

			m_idTrinket1 = newData.m_idTrinket1;
			m_idTrinket2 = newData.m_idTrinket2;

			m_lSpellsKnown = newData.m_lSpellsKnown;

			m_szCharacterRace = newData.m_szCharacterRace;
			m_szCharacterClassType = newData.m_szCharacterClassType;
			m_szCharacterBio = newData.m_szCharacterBio;
			m_nFormationIter = newData.m_nFormationIter;


			m_bIsInParty = newData.m_bIsInParty;
			m_bHasBeenRecruited = newData.m_bHasBeenRecruited;
		}
	}


	//Return the character data of each unit in the party
	public List<CharacterData> GetParty() 
	{
		return m_lPartyMembers;
	}
	public List<CharacterData> GetRoster()
	{
		return m_lRoster;
	}
    public List<FightSceneControllerScript.cWarUnit> GetWarUnits()
    {
        return m_lAllyUnits;
    }

    public void SetWarUnits(List<FightSceneControllerScript.cWarUnit> a) { m_lAllyUnits = a; }
	public void SetRoster(List<CharacterData> c) {m_lRoster = c;}
	//So this call is also updating character stats, so iterate through each character and update their stats on the roster
	public void SetParty(List<CharacterData> p) 
	{
		Debug.Log("New party count " + p.Count);
		m_lPartyMembers.Clear();
		foreach(CharacterData character in p)
		{
			m_lPartyMembers.Add(character);
			foreach(CharacterData rosterCharacter in m_lRoster)
			{
				
				if(character.m_szCharacterName == rosterCharacter.m_szCharacterName)
				{
					Debug.Log(rosterCharacter.m_szCharacterName + " to true");
					rosterCharacter.UpdateCharacterData(character);
					rosterCharacter.m_bIsInParty = true;
					break;
				}
				else
				{
					Debug.Log(rosterCharacter.m_szCharacterName + " to false");
					rosterCharacter.m_bIsInParty = false;
				}
			}
		}
		Debug.Log("Count is " + m_lPartyMembers.Count);
	}

	public void AddPartyMember(CharacterData character) 
	{
		List<int> lAvailableSpots = new List<int>();
		for(int i = 0; i < 6; ++i){lAvailableSpots.Add(i);}
		int nLowestSpot = 6;
		foreach(CharacterData c in m_lPartyMembers)
		{
			if(lAvailableSpots.Contains(c.m_nFormationIter))
				lAvailableSpots.Remove(c.m_nFormationIter);
		}
		foreach(int n in lAvailableSpots)
		{
			if(nLowestSpot > n)
				nLowestSpot = n;
		}
		character.m_nFormationIter = nLowestSpot;
		character.m_bIsInParty = true;
		character.m_bHasBeenRecruited = true;
		foreach(CharacterData c in m_lPartyMembers)
		{
			if(c.m_szCharacterName == character.m_szCharacterName)
			{
				c.UpdateCharacterData(character);
				return;
			}
		}
		m_lPartyMembers.Add(character);
		SetRosteredCharacterData(character);
		UpdateRostersPartiedCharacters();
	}
	//add a character from the roster into the party
	public void AddPartyMember(string szCharacterName)
	{
		DCScript.CharacterData character = GetRosteredCharacterData(szCharacterName);
		AddPartyMember(character);
	}
	//remove a character from the party
	public void RemovePartyMember(CharacterData character) 
	{
		character.m_bIsInParty = false;
		GetRosteredCharacterData(character.m_szCharacterName).UpdateCharacterData(character);
		m_lPartyMembers.Remove(character);
	}
	//returns the character if they're in the party.
	public CharacterData GetCharacter(string szName)
	{
		foreach(CharacterData c in m_lPartyMembers)
		{
			if(szName == c.m_szCharacterName)
				return c;
		}
		return null;
	}
	//returns a character if they're in the roster
	public CharacterData GetRosteredCharacterData(string characterName)
	{
		foreach(CharacterData cd in m_lRoster)
		{
			if(cd.m_szCharacterName == characterName)
			{
				return cd;
			}
		}
		return null;
	}
	//Updates the data of a character in the roster
	public void SetRosteredCharacterData(DCScript.CharacterData character)
	{
		foreach(CharacterData cd in m_lRoster)
		{
			if(cd.m_szCharacterName == character.m_szCharacterName)
			{
				cd.UpdateCharacterData(character);
				return;
			}
		}
		m_lRoster.Add(character);
	}

	//Sets any character that is in the party 
	public void UpdateRostersPartiedCharacters()
	{
		foreach(DCScript.CharacterData rosterCharacter in m_lRoster)
		{
			foreach(DCScript.CharacterData character in m_lPartyMembers)
			{
				if(character.m_szCharacterName == rosterCharacter.m_szCharacterName)
				{
					rosterCharacter.UpdateCharacterData(character);
					rosterCharacter.m_bIsInParty = true;
					break;
				}
				else
				{
					rosterCharacter.m_bIsInParty = false;
				}
			}
		}
	}

	//iter to which background to load during battle (DOES NOT NEED TO BE SAVED/LOADED)
	int m_nBattleFieldBackgroundIter = 0;
	public int GetBattleFieldBackgroundIter() {return m_nBattleFieldBackgroundIter;}
	public void SetBattleFieldBackgroundIter(int iter) {m_nBattleFieldBackgroundIter = iter;}



	public class cModifier
	{
		public string m_szModifierName;
		public string m_szModifierDesc;
		public int m_nModCost;
	}
	List<cModifier> m_lModifiers = new List<cModifier>();
	public List<cModifier> GetModifierList() {return m_lModifiers;}
	public void SetModifierList(List<cModifier> lst) {m_lModifiers = lst;}



	public class LevelingWeapon
	{
		public string m_szWeaponName;
		public int m_nLevel;
		public int m_nDamage;
	}
	public class WeaponData
	{
		public string m_szOwnerName;
		public List<LevelingWeapon> m_lLevels = new List<LevelingWeapon>();
	}
	List<WeaponData> m_lWeapons = new List<WeaponData>();
	public List<WeaponData> GetWeaponList() {return m_lWeapons;}
	public void SetWeaponList(List<WeaponData> wpns) {m_lWeapons = wpns;}


	public TextAsset m_taStatProgression;
	public List<ClassStats> m_lClassStatProgressions = new List<ClassStats>();
	public ClassStats GetClassType(string szName)
	{
		foreach(ClassStats c in m_lClassStatProgressions)
		{
			if(c.m_szClassName == szName)
				return c;
		}
		return null;
	}
	public class ClassStats
	{
		public string m_szClassName;
		public int m_nHPProg, m_nStrProg, m_nDefProg, m_nSpdProg, m_nEvaProg, m_nHitProg;
	}

	void Awake () 
	{
        DontDestroyOnLoad (gameObject);
    }
	// Use this for initialization
	void Start () 
	{
		m_fTimePlayed = PlayerPrefs.GetFloat("Time_Played", 0.0f);
		StartCoroutine(IncrementTimePlayed());
		AdjustValues(); 
		m_nGold = 100;
		LoadStatProgressions();
	}

	public IEnumerator  IncrementTimePlayed () 
	{
		while(true)
		{
			m_fTimePlayed +=1;
			yield return new WaitForSeconds(1);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			AddPartyMember("Briol");
		}
	}

	void LoadStatProgressions()
	{
		string[] classes = m_taStatProgression.ToString().Split('\n');
		foreach(string szClass in classes)
		{
			ClassStats cClass = new ClassStats();
			string[] szpieces = szClass.Split(',');
			cClass.m_szClassName = szpieces[0].Trim();
			cClass.m_nHPProg = int.Parse(szpieces[1].Trim());
			cClass.m_nStrProg = int.Parse(szpieces[2].Trim());
			cClass.m_nDefProg = int.Parse(szpieces[3].Trim());
			cClass.m_nSpdProg = int.Parse(szpieces[4].Trim());
			cClass.m_nEvaProg = int.Parse(szpieces[5].Trim());
			cClass.m_nHitProg = int.Parse(szpieces[6].Trim());
			m_lClassStatProgressions.Add(cClass);
		}
	}

	public void RestoreParty()
	{
		foreach(CharacterData character in m_lPartyMembers)
		{
			character.m_nCurHP = character.m_nMaxHP;
		}
		foreach(StatusEffect se in m_lStatusEffects)
		{
			GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().RemoveStatusEffect(se.m_szEffectName);
		}
		m_lStatusEffects.Clear();
	}

	public void AdjustValues()
	{
		if(m_fMasterVolume < 0.0f)
			m_fMasterVolume = 0.0f;
		else if(m_fMasterVolume > 1.0f)
			m_fMasterVolume = 1.0f;
		if(m_fBrightness < -2.0f)
			m_fBrightness = -2.0f;
		else if(m_fBrightness > 2.0f)
			m_fBrightness = 2.0f;
		if(m_fSFXVolume < -0.5f)
			m_fSFXVolume = -0.5f;
		else if(m_fSFXVolume > 0.5f)
			m_fSFXVolume = 0.5f;
		if(m_fMusicVolume < -0.5f)
			m_fMusicVolume = -0.5f;
		else if(m_fMusicVolume > 0.5f)
			m_fMusicVolume = 0.5f;
		if(m_fVoiceVolume < -0.5f)
			m_fVoiceVolume = -0.5f;
		else if(m_fVoiceVolume > 0.5f)
			m_fVoiceVolume = 0.5f;
		if(m_nTextSpeed > 3)
			m_nTextSpeed = 3;
		else if(m_nTextSpeed < 1)
			m_nTextSpeed = 1;
	}
	
	public void SetMasterVolume()
	{
		AudioListener.volume = m_fMasterVolume;
	}
}
