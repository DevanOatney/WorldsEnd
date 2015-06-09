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
	List<string> m_lEnemies;
	public List<string> GetEnemyNames() {return m_lEnemies;}
	public void SetEnemyNames(List<string> e) {m_lEnemies = e;}

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


	public class ItemData
	{
		public string m_szItemName;
		public int m_nItemType;
		public int m_nHPMod;
		public int m_nPowMod;
		public int m_nDefMod;
		public int m_nSpdMod;
		public int m_nBaseValue;
		public string m_szDescription;
	}
	public class ArmorData : ItemData
	{
		//m_nModifier effects the reduction of damage, this is for like if it heals or damages units or increases/decreases other stats
		public int m_nSpecialModifier;
		//type value - see armorscript for what iter means what
		public int m_nSpecialType;
	}
	//Dictionary of all of the items in the game, key is the items name
	Dictionary<string, ItemData> m_dItemDictionary = new Dictionary<string, ItemData>();
	public ItemData GetItemFromDictionary(string key) 
	{
		ItemData outPut;
		if(m_dItemDictionary.TryGetValue(key, out outPut))
			return outPut;

		return outPut;
	}

	public Dictionary<string, ItemData> GetItemDictionary() {return m_dItemDictionary;}
	public void SetItemDictionary(Dictionary<string, ItemData> itemdictionary) {m_dItemDictionary = itemdictionary;}

	//Struct for Items, adding in an integer for the amount that the player has
	public class CharactersItems
	{
		public string m_szItemName;
		public int m_nItemType;
		public int m_nItemCount;
	}
	//List of Items held by the player
	List<CharactersItems> m_lInventory = new List<CharactersItems>();
	public List<CharactersItems> GetInventory() {return m_lInventory;}
	public void SetInventory(List<CharactersItems> inv) {m_lInventory = inv;}
	//returns null if item not found... shouldn't theoretically happen but.. meh
	public CharactersItems GetItemFromInventory(string itemName)
	{
		foreach(CharactersItems i in m_lInventory)
		{
			if(i.m_szItemName == itemName)
				return i;
		}
		return null;
	}
	public void AddItem(CharactersItems item)
	{
		int iter = 0;
		foreach(CharactersItems i in m_lInventory)
		{
			if(item.m_szItemName == i.m_szItemName)
			{
				m_lInventory[iter].m_nItemCount += item.m_nItemCount;
				if(m_lInventory[iter].m_nItemCount >= 45)
					m_lInventory[iter].m_nItemCount = 45;
				return;
			}
			iter++;
		}
		m_lInventory.Add(item);
	}
	public void RemoveItem(CharactersItems item)
	{
		foreach(CharactersItems i in m_lInventory.ToArray())
		{
			if(item.m_szItemName == i.m_szItemName)
			{
				i.m_nItemCount -= item.m_nItemCount;
				if(i.m_nItemCount <= 0)
				{
					m_lInventory.Remove(i);
				}
			}
		}
	}
	public void RemoveItem(CharactersItems item, int count)
	{
		foreach(CharactersItems i in m_lInventory.ToArray())
		{
			if(item.m_szItemName == i.m_szItemName)
			{
				i.m_nItemCount -= count;
				if(i.m_nItemCount <= 0)
				{
					m_lInventory.Remove(i);
				}
			}
		}
	}
	public void RemoveItemAll(CharactersItems item)
	{
		foreach(CharactersItems i in m_lInventory.ToArray())
		{
			if(item.m_szItemName == i.m_szItemName)
			{
				m_lInventory.Remove(i);
			}
		}
	}
	public class StatusEffect
	{
		public string m_szName;
		public int m_nCount;
		public List<string> m_lEffectedMembers;
	}
	//list of status effects that are inflicting the party
	public List<StatusEffect> m_lStatusEffects = new List<StatusEffect>();

	public class CharacterData
	{
		public string m_szCharacterName;
		public int m_nMaxHP, m_nCurHP, m_nSTR, m_nDEF, m_nSPD, m_nLevel, m_nCurrentEXP;

		public string m_szWeaponName;
		public int m_nWeaponDamageModifier;
		public int m_nWeaponLevel;
		public string m_szWeaponModifierName;

		public ArmorData m_idChestSlot;
		public ArmorData m_idLegSlot;
		public ArmorData m_idBeltSlot;
		public ArmorData m_idShoulderSlot;
		public ArmorData m_idHelmSlot;
		public ArmorData m_idGloveSlot;

		public ItemData m_idTrinket1;
		public ItemData m_idTrinket2;

		public List<string> m_lSpellsKnown;

		public string m_szCharacterRace;
		public string m_szCharacterClassType;
		public string m_szCharacterBio;


	}

	//List of characters in party
	List<CharacterData> m_lPartyMembers = new List<CharacterData>();
	public List<CharacterData> GetParty() {return m_lPartyMembers;}
	public void SetParty(List<CharacterData> p) {m_lPartyMembers = p;}
	public void AddPartyMember(CharacterData character) {m_lPartyMembers.Add(character);}
	public void RemovePartyMember(CharacterData character) {m_lPartyMembers.Remove(character);}

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


	void Awake () 
	{
        DontDestroyOnLoad (gameObject);

		//Temp, instead load from file, or.. if it's a new game, just add the player.
		/*
		GameObject Matt = Resources.Load<GameObject>("Units/Ally/Matt/Matt");
		Matt.GetComponent<PlayerBattleScript>().SetUnitStats();
		Matt.GetComponent<UnitScript>().m_nPositionOnField = 0;
		GameObject Devan = Resources.Load<GameObject>("Units/Ally/Devan/Devan");
		Devan.GetComponent<PlayerBattleScript>().SetUnitStats();
		Devan.GetComponent<UnitScript>().m_nPositionOnField = 1;
		m_lPartyMembers.Add(Matt);
		m_lPartyMembers.Add(Devan);
		*/
    }
	// Use this for initialization
	void Start () 
	{
		AdjustValues(); 
		m_nGold = 10000;
	}
	
	// Update is called once per frame
	void Update () 
	{
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
