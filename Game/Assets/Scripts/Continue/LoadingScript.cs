﻿using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
public class LoadingScript : MonoBehaviour 
{
	string m_szFileName;
	StreamReader sr;
	// Use this for initialization
	void Awake()
	{
		m_szFileName = Application.dataPath + "/Resources/Save Files/";
	}
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	//check to see if file exists
	public bool CheckFile(int iter)
	{
		try
		{
			sr = new StreamReader(m_szFileName + iter.ToString() + ".txt");
			sr.Close();
			return true;
		}
		catch(Exception e)
		{
			Debug.Log(e.Message);
			return false;
		}
	}

	public ContinueHighlightInputScript.SaveDataInformation GetSaveData(int iter)
	{
		if(File.Exists(m_szFileName + iter.ToString() + ".txt") == false)
			return null;
		ContinueHighlightInputScript.SaveDataInformation saveData = new ContinueHighlightInputScript.SaveDataInformation();

		string szLine = "";
		sr = new StreamReader(m_szFileName + iter.ToString() + ".txt");
		szLine = sr.ReadLine().Trim();

		//Music Volume
		szLine = sr.ReadLine().Trim();
		//Sound Effect Volume
		szLine = sr.ReadLine().Trim();
		//Voice Volume
		szLine = sr.ReadLine().Trim();
		//Brightness
		szLine = sr.ReadLine().Trim();
		//Battle animation flag
		szLine = sr.ReadLine().Trim();
		//Text Speed
		szLine = sr.ReadLine().Trim();
		//The amount of flags in the flag field
		szLine = sr.ReadLine().Trim();
		int flagCount = int.Parse(szLine);
		for(int i = 0; i < flagCount; ++i)
		{
			szLine = sr.ReadLine().Trim();
			szLine = sr.ReadLine().Trim();
		}
		//Amount of gold the player has
		saveData.m_nGold = int.Parse(sr.ReadLine().Trim());
		
		//Amount of characters in the party
		szLine = sr.ReadLine().Trim();
		int partyCount = int.Parse(szLine);
		for(int i = 0; i < partyCount; ++i)
		{
			//name of the character
			szLine = sr.ReadLine();
			saveData.m_szName = szLine.Trim();

			//Race of character
			szLine = sr.ReadLine().Trim();

			//Class type
			szLine = sr.ReadLine().Trim();

			//bio
			szLine = sr.ReadLine().Trim();
			
			//Max HP
			szLine = sr.ReadLine().Trim();
			
			//Cur HP
			szLine = sr.ReadLine().Trim();

			//MaxMP
			szLine = sr.ReadLine().Trim();

			//Cur MP
			szLine = sr.ReadLine().Trim();
			
			//STR
			szLine = sr.ReadLine().Trim();
			
			//DEF
			szLine = sr.ReadLine().Trim();
			
			//SPD
			szLine = sr.ReadLine().Trim();

			//EVA
			szLine = sr.ReadLine().Trim();

			//HIT
			szLine = sr.ReadLine().Trim();

			//Level
			szLine = sr.ReadLine();
			int LVL = int.Parse(szLine.Trim());
			saveData.m_nLevel = LVL;
			
			//Current exp
			szLine = sr.ReadLine().Trim();

			//Weapon Name
			szLine = sr.ReadLine().Trim();

			//Weapon Level
			szLine = sr.ReadLine().Trim();

			//Weapon Damage Mod
			szLine = sr.ReadLine().Trim();

			//Weapon Mod name
			szLine = sr.ReadLine().Trim();

			//Helm
			szLine = sr.ReadLine().Trim();

			//Shoulder
			szLine = sr.ReadLine().Trim();

			//Chest 
			szLine = sr.ReadLine().Trim();

			//Gloves
			szLine = sr.ReadLine().Trim();

			//Belt
			szLine = sr.ReadLine().Trim();

			//Legs
			szLine = sr.ReadLine().Trim();

			//Trinket 1
			szLine = sr.ReadLine().Trim();

			//Trinket 2
			szLine = sr.ReadLine().Trim();

			//Amount of spells this character knows
			szLine = sr.ReadLine().Trim();
			int spellCount = int.Parse(szLine);
			for(int j = 0; j < spellCount; ++j)
			{
				//Read the spell name
				sr.ReadLine().Trim();
			}
			
			
		}

		//amount of status effects effecting character
		szLine = sr.ReadLine().Trim();
		int effectCount = int.Parse(szLine.Trim());
		for(int j = 0; j < effectCount; ++j)
		{
			//name of effect
			sr.ReadLine().Trim();
			//effect type
			sr.ReadLine().Trim();
			//amount of ticks left on the effect
			sr.ReadLine().Trim();
			//hp mod
			sr.ReadLine().Trim();
			//mp mod
			sr.ReadLine().Trim();
			//pow mod
			sr.ReadLine().Trim();
			//def mod
			sr.ReadLine().Trim();
			//spd mod
			sr.ReadLine().Trim();
			//hit mod
			sr.ReadLine().Trim();
			//eva mod
			sr.ReadLine().Trim();
			//amount of units effected
			int unitsEffected = int.Parse(sr.ReadLine().Trim());
			for(int x = 0; x < unitsEffected; ++x)
				sr.ReadLine().Trim();
		}

		
		//Amount of items in inventory
		szLine = sr.ReadLine().Trim();
		int inventoryCount = int.Parse(szLine);
		
		//Inventory
		for(int i = 0; i < inventoryCount; ++i)
		{
			szLine = sr.ReadLine().Trim();
			szLine = sr.ReadLine().Trim();
			szLine = sr.ReadLine().Trim();
		}
		
		
		//The scene to load
		szLine = sr.ReadLine().Trim();
		saveData.m_szFieldName = szLine;

		sr.Close();

		return saveData;
	}


	//iter is for 1 of the 3 save files that can be written
	public void Load(int iter)
	{
		if(File.Exists(m_szFileName + iter.ToString() + ".txt") == false)
			return;
		DCScript NewData = GameObject.Find("PersistantData").GetComponent<DCScript>();
		
		sr = new StreamReader(m_szFileName + iter.ToString() + ".txt");

		string szLine;
		//User settings
		
		//Master Volume
		szLine = sr.ReadLine().Trim();
		NewData.m_fMasterVolume = float.Parse(szLine);
		NewData.SetMasterVolume();
		//Music Volume
		szLine = sr.ReadLine().Trim();
		NewData.m_fMusicVolume = float.Parse(szLine);
		//Sound Effect Volume
		szLine = sr.ReadLine().Trim();
		NewData.m_fSFXVolume = float.Parse(szLine);
		//Voice Volume
		szLine = sr.ReadLine().Trim();
		NewData.m_fVoiceVolume = float.Parse(szLine);
		//Brightness
		szLine = sr.ReadLine().Trim();
		NewData.m_fBrightness = float.Parse(szLine);
		//Battle animation flag
		szLine = sr.ReadLine().Trim();
		NewData.m_bToUseBattleAnimations = bool.Parse(szLine);
		//Text Speed
		szLine = sr.ReadLine().Trim();
		NewData.m_nTextSpeed = int.Parse(szLine);
		//The amount of flags in the flag field
		szLine = sr.ReadLine().Trim();
		int flagCount = int.Parse(szLine);
		Dictionary<string, int> dStoryFlagField = new Dictionary<string, int>();
		for(int i = 0; i < flagCount; ++i)
		{
			string key;
			int value;
			szLine = sr.ReadLine().Trim();
			key = szLine;
			szLine = sr.ReadLine().Trim();
			value = int.Parse(szLine);
			dStoryFlagField.Add(key, value);
		}
		NewData.m_dStoryFlagField = dStoryFlagField;

		//Amount of gold the player has.
		NewData.m_nGold = int.Parse(sr.ReadLine().Trim());
		
		//Amount of characters in the party
		szLine = sr.ReadLine().Trim();
		int partyCount = int.Parse(szLine);
		for(int i = 0; i < partyCount; ++i)
		{

			//"Resources/Units/Ally/Name/Name.prefab
			DCScript.CharacterData character = new DCScript.CharacterData();
			//name of the character
			szLine = sr.ReadLine().Trim();
			character.m_szCharacterName = szLine.Trim();

			//Race
			szLine = sr.ReadLine().Trim();
			character.m_szCharacterRace = szLine.Trim();

			//Class
			szLine = sr.ReadLine().Trim();
			character.m_szCharacterClassType = szLine.Trim();

			//Bio
			szLine = sr.ReadLine().Trim();
			character.m_szCharacterBio = szLine.Trim();

			//Max HP
			szLine = sr.ReadLine().Trim();
			int MaxHP = int.Parse(szLine);
			character.m_nMaxHP = MaxHP;
		
			//Cur HP
			szLine = sr.ReadLine().Trim();
			int CurHP = int.Parse(szLine);
			character.m_nCurHP = CurHP;

			//Max MP
			szLine = sr.ReadLine().Trim();
			int MaxMP = int.Parse(szLine);
			character.m_nMaxMP = MaxMP;

			//Cur MP
			szLine = sr.ReadLine().Trim();
			int CurMP = int.Parse(szLine);
			character.m_nCurMP = int.Parse(szLine);
		
			//STR
			szLine = sr.ReadLine().Trim();
			int STR = int.Parse(szLine);
			character.m_nSTR = STR;
		
			//DEF
			szLine = sr.ReadLine().Trim();
			int DEF = int.Parse(szLine);
			character.m_nDEF = DEF;
		
			//SPD
			szLine = sr.ReadLine().Trim();
			int SPD = int.Parse(szLine);
			character.m_nSPD = SPD;

			//EVA
			szLine = sr.ReadLine().Trim();
			int EVA = int.Parse(szLine);
			character.m_nEVA = EVA;

			//HIT
			szLine = sr.ReadLine().Trim();
			int HIT = int.Parse(szLine);
			character.m_nHIT = HIT;
		
			//Level
			szLine = sr.ReadLine().Trim();
			int LVL = int.Parse(szLine);
			character.m_nLevel = LVL;
		
			//Current exp
			szLine = sr.ReadLine().Trim();
			int exp = int.Parse(szLine);
			character.m_nCurrentEXP = exp;

			//Weapon Name
			szLine = sr.ReadLine();
			character.m_szWeaponName = szLine.Trim();

			//Weapon Level
			character.m_nWeaponLevel = int.Parse(sr.ReadLine().Trim());

			//Weapon Damage Mod
			character.m_nWeaponDamageModifier = int.Parse(sr.ReadLine().Trim());

			//Weapon Mod Name
			character.m_szWeaponModifierName = sr.ReadLine().Trim();

			//Helm name
			szLine = sr.ReadLine();
			if(szLine.Trim() != "NULL")
			{
				ItemLibrary.ArmorData helm = (ItemLibrary.ArmorData)NewData.m_lItemLibrary.GetItemFromDictionary(szLine.Trim());
				character.m_idHelmSlot = helm;
			}
			//Shoulder name
			szLine = sr.ReadLine();
			if(szLine.Trim() != "NULL")
			{
				ItemLibrary.ArmorData shoulder = (ItemLibrary.ArmorData)NewData.m_lItemLibrary.GetItemFromDictionary(szLine.Trim());
				character.m_idShoulderSlot = shoulder;
			}
			//Armor name
			szLine = sr.ReadLine();
			if(szLine.Trim() != "NULL")
			{
				ItemLibrary.ArmorData armor = (ItemLibrary.ArmorData)NewData.m_lItemLibrary.GetItemFromDictionary(szLine.Trim());
				character.m_idChestSlot = armor;
			}

			//Glove name
			szLine = sr.ReadLine();
			if(szLine.Trim() != "NULL")
			{
				ItemLibrary.ArmorData glove = (ItemLibrary.ArmorData)NewData.m_lItemLibrary.GetItemFromDictionary(szLine.Trim());
				character.m_idGloveSlot = glove;
			}
			//Belt name
			szLine = sr.ReadLine();
			if(szLine.Trim() != "NULL")
			{
				ItemLibrary.ArmorData belt = (ItemLibrary.ArmorData)NewData.m_lItemLibrary.GetItemFromDictionary(szLine.Trim());
				character.m_idBeltSlot = belt;
			}
			//leg name
			szLine = sr.ReadLine();
			if(szLine.Trim() != "NULL")
			{
				ItemLibrary.ArmorData leg = (ItemLibrary.ArmorData)NewData.m_lItemLibrary.GetItemFromDictionary(szLine.Trim());
				character.m_idLegSlot = leg;
			}
			//Trinket 1 name
			szLine = sr.ReadLine();
			if(szLine.Trim() != "NULL")
			{
				ItemLibrary.ItemData trinket1 = NewData.m_lItemLibrary.GetItemFromDictionary(szLine.Trim());
				character.m_idTrinket1 = trinket1;
			}

			//Trinket 2 name
			szLine = sr.ReadLine();
			if(szLine.Trim() != "NULL")
			{
				ItemLibrary.ItemData trinket2 = NewData.m_lItemLibrary.GetItemFromDictionary(szLine.Trim());
				character.m_idTrinket2 = trinket2;
			}


			//Amount of spells this character knows
			szLine = sr.ReadLine();
			int spellCount = int.Parse(szLine);
			List<string> characterSpells = new List<string>();
			for(int j = 0; j < spellCount; ++j)
			{
				//Read the spell name
				characterSpells.Add(sr.ReadLine());
			}
			character.m_lSpellsKnown = characterSpells;
			NewData.AddPartyMember(character);
		
		}

		//Amount of effects the character has
		szLine = sr.ReadLine();
		int effectCount = int.Parse(szLine.Trim());
		Debug.Log(effectCount);
		List<DCScript.StatusEffect> lStatusEffects = new List<DCScript.StatusEffect>();
		for(int j = 0; j < effectCount; ++j)
		{
			DCScript.StatusEffect se = new DCScript.StatusEffect();
			se.m_szEffectName = sr.ReadLine().Trim();
			se.m_nEffectType = int.Parse(sr.ReadLine().Trim());
			se.m_nAmountOfTicks = int.Parse(sr.ReadLine().Trim());
			se.m_nHPMod = int.Parse(sr.ReadLine().Trim());
			se.m_nMPMod = int.Parse(sr.ReadLine().Trim());
			se.m_nPOWMod = int.Parse(sr.ReadLine().Trim());
			se.m_nDEFMod = int.Parse(sr.ReadLine().Trim());
			se.m_nSPDMod = int.Parse(sr.ReadLine().Trim());
			se.m_nHITMod = int.Parse(sr.ReadLine().Trim());
			se.m_nEVAMod = int.Parse(sr.ReadLine().Trim());
			int unitCount = int.Parse(sr.ReadLine().Trim());
			List<string> lEffectUnits = new List<string>();
			for(int x = 0; x < unitCount; ++x)
			{
				lEffectUnits.Add(sr.ReadLine().Trim());
			}
			
			
			lStatusEffects.Add(se);
		}
		NewData.SetStatusEffects(lStatusEffects);
		
		//Amount of items in inventory
		szLine = sr.ReadLine();
		int inventoryCount = int.Parse(szLine);
		
		//Inventory
		List<ItemLibrary.CharactersItems> lInventory = new List<ItemLibrary.CharactersItems>();
		for(int i = 0; i < inventoryCount; ++i)
		{
			ItemLibrary.CharactersItems item = new ItemLibrary.CharactersItems();
			szLine = sr.ReadLine();
			item.m_szItemName = szLine.Trim();
			szLine = sr.ReadLine();
			item.m_nItemCount = int.Parse(szLine.Trim());
			szLine = sr.ReadLine();
			item.m_nItemType = int.Parse(szLine.Trim());
			lInventory.Add(item);
		}
		NewData.m_lItemLibrary.m_lInventory = lInventory;
		
		
		//The scene to load
		szLine = sr.ReadLine();
		NewData.SetPreviousFieldName(szLine);
		
		//The position on that scene of the player
		szLine = sr.ReadLine();

		szLine = szLine.Substring(1, szLine.Length - 2);
		string[] szAxis = szLine.Split(',');
		Vector3 pos = new Vector3(float.Parse(szAxis[0]), float.Parse(szAxis[1]), float.Parse(szAxis[2]));
		NewData.SetPreviousPosition(pos);
		
		//The direction to face the player
		szLine = sr.ReadLine();
		NewData.SetPreviousFacingDirection(int.Parse(szLine));
		
		
		//Finished writing out, close
		sr.Close();
	}
}
