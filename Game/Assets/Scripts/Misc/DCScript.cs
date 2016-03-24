using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DCScript : MonoBehaviour 
{
	public float m_fMasterVolume = 0.5f;
	public float m_fMusicVolume = 0.5f;
	public float m_fSFXVolume = 0.5f;
	public float m_fVoiceVolume = 0.5f;
	public float m_fBrightness = 0.0f;
	public bool m_bToUseBattleAnimations = true;
	public int m_nTextSpeed = 2;

	//flag field that is toggled depending on what the players actions are in the game
	//["Name"] - 1 if activated 								  : Place Created
	public Dictionary<string, int> m_dStoryFlagField = new Dictionary<string, int>();
	//-------
	//FIELD INFORMATION

	//for encounter information
	List<EncounterGroupLoaderScript.cEnemyData> m_lEnemies;
	public List<EncounterGroupLoaderScript.cEnemyData> GetEnemyNames() {return m_lEnemies;}
	public void SetEnemyNames(List<EncounterGroupLoaderScript.cEnemyData> e) {m_lEnemies = e;}

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
	public int m_nGold;

	public int m_nMusicIter;

	//The amount of time that the player has played for.
	public float m_fTimePlayed = 0.0f;

	//the library of items (holds data for every item in the game, and also the inventory for the player)
	public ItemLibrary m_lItemLibrary = new ItemLibrary();

	//the library of spells that exist in the game. (loaded at runtime, does not need to be saved/loaded with player)
	public SpellLibrary m_lSpellLibrary = new SpellLibrary();

	//the library of all of the status effects that exist in the game
	public StatusEffectLibrary m_lStatusEffectLibrary = new StatusEffectLibrary();


	public class StatusEffect
	{
		public string m_szEffectName;
		public int m_nEffectType; //0-Poison, 1-Paralyze, 2-Stone
		public int m_nAmountOfTicks;
		public int m_nHPMod;
		public int m_nMPMod;
		public int m_nPOWMod;
		public int m_nDEFMod;
		public int m_nSPDMod;
		public int m_nHITMod;
		public int m_nEVAMod;
		public List<string> m_lEffectedMembers = new List<string>();
	}
	//list of status effects that are inflicting the party
	List<StatusEffect> m_lStatusEffects = new List<StatusEffect>();
	public List<StatusEffect> GetStatusEffects() {return m_lStatusEffects;}
	public void SetStatusEffects(List<StatusEffect> l) {m_lStatusEffects.Clear(); m_lStatusEffects = l;}
	public void AddStatusEffect(StatusEffect se)
	{
		int catchIter = IsStatusEffectInList(se.m_nEffectType);
		if(catchIter != -1)
		{
			foreach(string c in se.m_lEffectedMembers)
			{
				bool inList = false;
				//for each effected unit, check all effected units, if you're not in it.. add this unit to the effected units
				foreach(string s in m_lStatusEffects[catchIter].m_lEffectedMembers)
				{
					if(s == c)
						inList = true;
				}
				if(inList == false)
				{
					m_lStatusEffects[catchIter].m_lEffectedMembers.Add(c);
				}
				if(m_lStatusEffects[catchIter].m_nAmountOfTicks < se.m_nAmountOfTicks)
					m_lStatusEffects[catchIter].m_nAmountOfTicks = se.m_nAmountOfTicks;
				return;
			}
		}
		else
		{
			m_lStatusEffects.Add (se);
		}
	}
	public void ReduceStatusEffectCount(string szName)
	{
		Debug.Log("Reduce SE count");
		foreach(StatusEffect se in m_lStatusEffects)
		{
			se.m_nAmountOfTicks = se.m_nAmountOfTicks - 1;
		}
	}
	public void RemoveMeFromStatus(string szNameOfCharacter, int iter)
	{
		m_lStatusEffects[iter].m_lEffectedMembers.Remove(szNameOfCharacter);
		if(m_lStatusEffects[iter].m_lEffectedMembers.Count <= 0)
			m_lStatusEffects.RemoveAt(iter);
	}
	public void RemoveMeFromStatus(string szNameOfCharacter, string szNameOfStatusEffect)
	{
		int counter = 0;
		foreach(StatusEffect se in m_lStatusEffects)
		{
			if(se.m_szEffectName == szNameOfStatusEffect)
			{
				RemoveMeFromStatus(szNameOfCharacter, counter);
				return;
			}
			counter++;
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

	public class CharacterData
	{
		public string m_szCharacterName;
		public int m_nMaxHP, m_nCurHP, m_nMaxMP, m_nCurMP, m_nSTR, m_nDEF, m_nSPD, m_nEVA, m_nHIT, m_nLevel, m_nCurrentEXP;

		public string m_szWeaponName;
		public int m_nWeaponDamageModifier;
		public int m_nWeaponLevel;
		public string m_szWeaponModifierName;

		public ItemLibrary.ArmorData m_idChestSlot;
		public ItemLibrary.ArmorData m_idLegSlot;
		public ItemLibrary.ArmorData m_idBeltSlot;
		public ItemLibrary.ArmorData m_idShoulderSlot;
		public ItemLibrary.ArmorData m_idHelmSlot;
		public ItemLibrary.ArmorData m_idGloveSlot;

		public ItemLibrary.ItemData m_idTrinket1;
		public ItemLibrary.ItemData m_idTrinket2;

		public List<string> m_lSpellsKnown;

		public string m_szCharacterRace;
		public string m_szCharacterClassType;
		public string m_szCharacterBio;
		public int m_nFormationIter = 5;

		//if this unit is currently in the party
		public bool m_bIsInParty = false;
		//if this unit has been recruited to the roster
		public bool m_bHasBeenRecruited = false;

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

	//List of characters in party
	List<CharacterData> m_lPartyMembers = new List<CharacterData>();
	//List of character in the roster
	List<CharacterData> m_lRoster = new List<CharacterData>();
	//Return the character data of each unit in the party
	public List<CharacterData> GetParty() 
	{
		return m_lPartyMembers;
	}
	public List<CharacterData> GetRoster()
	{
		return m_lRoster;
	}
	//So this call is also updating character stats, so iterate through each character and update their stats on the roster
	public void SetParty(List<CharacterData> p) 
	{
		m_lPartyMembers.Clear();
		foreach(CharacterData character in p)
		{
			m_lPartyMembers.Add(character);
			foreach(CharacterData rosterCharacter in m_lRoster)
			{
				
				if(character.m_szCharacterName == rosterCharacter.m_szCharacterName)
				{
					rosterCharacter.UpdateCharacterData(character);
					rosterCharacter.m_bIsInParty = true;
				}
				else
					rosterCharacter.m_bIsInParty = false;
			}
		}
		Debug.Log("Count is " + m_lPartyMembers.Count);
	}

	public void AddPartyMember(CharacterData character) 
	{
		//update the characters stats in the roster (or add it to the roster if it's the first time.
		SetRosteredCharacterData(character);

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
	}

	public void AddPartyMember(string szCharacterName)
	{
		AddPartyMember(GetRosteredCharacterData(szCharacterName));
	}

	public void RemovePartyMember(CharacterData character) 
	{
		character.m_bIsInParty = false;
		GetRosteredCharacterData(character.m_szCharacterName).UpdateCharacterData(character);
		m_lPartyMembers.Remove(character);
	}
	public CharacterData GetCharacter(string szName)
	{
		foreach(CharacterData c in m_lPartyMembers)
		{
			if(szName == c.m_szCharacterName)
				return c;
		}
		return null;
	}



	public CharacterData GetRosteredCharacterData(string characterName)
	{
		foreach(CharacterData cd in m_lRoster)
		{
			if(cd.m_szCharacterName == characterName)
				return cd;
		}
		return null;
	}
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
		Debug.Log("Wasn't able to find character, adding it to the list");
		m_lRoster.Add(character);
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
			foreach(CharacterData character in m_lRoster)
			{
				Debug.Log(character.m_szCharacterName + " is in roster");
			}
			foreach(CharacterData character in m_lPartyMembers)
			{
				Debug.Log(character.m_szCharacterName + " is in party");
			}
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
