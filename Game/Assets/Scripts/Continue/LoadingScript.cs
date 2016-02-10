using UnityEngine;
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
		szLine = sr.ReadLine();

		//Music Volume
		szLine = sr.ReadLine();
		//Sound Effect Volume
		szLine = sr.ReadLine();
		//Voice Volume
		szLine = sr.ReadLine();
		//Brightness
		szLine = sr.ReadLine();
		//Battle animation flag
		szLine = sr.ReadLine();
		//Text Speed
		szLine = sr.ReadLine();
		//The amount of flags in the flag field
		szLine = sr.ReadLine();
		int flagCount = int.Parse(szLine);
		for(int i = 0; i < flagCount; ++i)
		{
			szLine = sr.ReadLine();
			szLine = sr.ReadLine();
		}
		//Amount of gold the player has
		saveData.m_nGold = int.Parse(sr.ReadLine().Trim());
		
		//Amount of characters in the party
		szLine = sr.ReadLine();
		int partyCount = int.Parse(szLine);
		for(int i = 0; i < partyCount; ++i)
		{
			//name of the character
			szLine = sr.ReadLine();
			saveData.m_szName = szLine.Trim();

			//Race of character
			szLine = sr.ReadLine();

			//Class type
			szLine = sr.ReadLine();

			//bio
			szLine = sr.ReadLine();
			
			//Max HP
			szLine = sr.ReadLine();
			
			//Cur HP
			szLine = sr.ReadLine();
			
			//STR
			szLine = sr.ReadLine();
			
			//DEF
			szLine = sr.ReadLine();
			
			//SPD
			szLine = sr.ReadLine();

			//EVA
			szLine = sr.ReadLine();

			//HIT
			szLine = sr.ReadLine();

			//Level
			szLine = sr.ReadLine();
			int LVL = int.Parse(szLine.Trim());
			saveData.m_nLevel = LVL;
			
			//Current exp
			szLine = sr.ReadLine();

			//Weapon Name
			szLine = sr.ReadLine();

			//Weapon Level
			szLine = sr.ReadLine();

			//Weapon Damage Mod
			szLine = sr.ReadLine();

			//Weapon Mod name
			szLine = sr.ReadLine();

			//Helm
			szLine = sr.ReadLine();

			//Shoulder
			szLine = sr.ReadLine();

			//Chest 
			szLine = sr.ReadLine();

			//Gloves
			szLine = sr.ReadLine();

			//Belt
			szLine = sr.ReadLine();

			//Legs
			szLine = sr.ReadLine();

			//Trinket 1
			szLine = sr.ReadLine();

			//Trinket 2
			szLine = sr.ReadLine();

			//Amount of spells this character knows
			szLine = sr.ReadLine();
			int spellCount = int.Parse(szLine);
			for(int j = 0; j < spellCount; ++j)
			{
				//Read the spell name
				sr.ReadLine();
			}
			
			
		}

		//amount of status effects effecting character
		szLine = sr.ReadLine();
		int effectCount = int.Parse(szLine.Trim());
		for(int j = 0; j < effectCount; ++j)
		{
			//name of effect
			sr.ReadLine();
			//amount of ticks left on the effect
			sr.ReadLine();
			//mod of effect
			sr.ReadLine();
			//amount of units effected
			int unitsEffected = int.Parse(sr.ReadLine().Trim());
			for(int x = 0; x < unitsEffected; ++x)
				sr.ReadLine();
		}

		
		//Amount of items in inventory
		szLine = sr.ReadLine();
		int inventoryCount = int.Parse(szLine);
		
		//Inventory
		for(int i = 0; i < inventoryCount; ++i)
		{
			szLine = sr.ReadLine();
			szLine = sr.ReadLine();
			szLine = sr.ReadLine();
		}
		
		
		//The scene to load
		szLine = sr.ReadLine();
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
		szLine = sr.ReadLine();
		NewData.m_fMasterVolume = float.Parse(szLine);
		NewData.SetMasterVolume();
		//Music Volume
		szLine = sr.ReadLine();
		NewData.m_fMusicVolume = float.Parse(szLine);
		//Sound Effect Volume
		szLine = sr.ReadLine();
		NewData.m_fSFXVolume = float.Parse(szLine);
		//Voice Volume
		szLine = sr.ReadLine();
		NewData.m_fVoiceVolume = float.Parse(szLine);
		//Brightness
		szLine = sr.ReadLine();
		NewData.m_fBrightness = float.Parse(szLine);
		//Battle animation flag
		szLine = sr.ReadLine();
		NewData.m_bToUseBattleAnimations = bool.Parse(szLine);
		//Text Speed
		szLine = sr.ReadLine();
		NewData.m_nTextSpeed = int.Parse(szLine);
		//The amount of flags in the flag field
		szLine = sr.ReadLine();
		int flagCount = int.Parse(szLine);
		Dictionary<string, int> dStoryFlagField = new Dictionary<string, int>();
		for(int i = 0; i < flagCount; ++i)
		{
			string key;
			int value;
			szLine = sr.ReadLine();
			key = szLine;
			szLine = sr.ReadLine();
			value = int.Parse(szLine);
			dStoryFlagField.Add(key, value);
		}
		NewData.m_dStoryFlagField = dStoryFlagField;

		//Amount of gold the player has.
		NewData.m_nGold = int.Parse(sr.ReadLine().Trim());
		
		//Amount of characters in the party
		szLine = sr.ReadLine();
		int partyCount = int.Parse(szLine);
		for(int i = 0; i < partyCount; ++i)
		{

			//"Resources/Units/Ally/Name/Name.prefab
			DCScript.CharacterData character = new DCScript.CharacterData();
			//name of the character
			szLine = sr.ReadLine();
			character.m_szCharacterName = szLine.Trim();

			//Race
			szLine = sr.ReadLine();
			character.m_szCharacterRace = szLine.Trim();

			//Class
			szLine = sr.ReadLine();
			character.m_szCharacterClassType = szLine.Trim();

			//Bio
			szLine = sr.ReadLine();
			character.m_szCharacterBio = szLine.Trim();

			//Max HP
			szLine = sr.ReadLine();
			int MaxHP = int.Parse(szLine);
			character.m_nMaxHP = MaxHP;
		
			//Cur HP
			szLine = sr.ReadLine();
			int CurHP = int.Parse(szLine);
			character.m_nCurHP = CurHP;
		
			//STR
			szLine = sr.ReadLine();
			int STR = int.Parse(szLine);
			character.m_nSTR = STR;
		
			//DEF
			szLine = sr.ReadLine();
			int DEF = int.Parse(szLine);
			character.m_nDEF = DEF;
		
			//SPD
			szLine = sr.ReadLine();
			int SPD = int.Parse(szLine);
			character.m_nSPD = SPD;

			//EVA
			szLine = sr.ReadLine();
			int EVA = int.Parse(szLine);
			character.m_nEVA = EVA;

			//HIT
			szLine = sr.ReadLine();
			int HIT = int.Parse(szLine);
			character.m_nHIT = HIT;
		
			//Level
			szLine = sr.ReadLine();
			int LVL = int.Parse(szLine);
			character.m_nLevel = LVL;
		
			//Current exp
			szLine = sr.ReadLine();
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
			DCScript.StatusEffect se = new DCScript.StatusEffect(sr.ReadLine().Trim(), int.Parse(sr.ReadLine().Trim()), int.Parse(sr.ReadLine().Trim()));
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
