using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System;

public class ItemLoaderScript : MonoBehaviour 
{
	public TextAsset m_szUseableItems;
	public TextAsset m_szArmorItems;
	public TextAsset m_taModifierListAsset;
	public TextAsset m_taWeaponDataAsset;
	public TextAsset m_taQuestItems;

	DCScript dcs;
	// Use this for initialization
	void Start () 
	{
		dcs = GameObject.Find("PersistantData").GetComponent<DCScript>();
		LoadUseableItems();
		LoadArmorItems();
		LoadModifiers();
		LoadWeaponData();
		LoadQuestItems();


		foreach(KeyValuePair<string, DCScript.ItemData> kvp in dcs.GetItemDictionary())
		{
			DCScript.CharactersItems m_ciItemHeld = new DCScript.CharactersItems();
			m_ciItemHeld.m_szItemName = kvp.Key;
			m_ciItemHeld.m_nItemCount = 1;
			m_ciItemHeld.m_nItemType = kvp.Value.m_nItemType;
			dcs.AddItem(m_ciItemHeld);
		}
	}

	void LoadModifiers()
	{
		string[] lines = m_taModifierListAsset.text.Split('\n');
		int counter = 0;
		foreach(string sz in lines)
		{
			counter++;
			if(counter == lines.Length)
				break;
			string[] szPieces = sz.Split(',');
			DCScript.cModifier newMod = new DCScript.cModifier();
			newMod.m_szModifierName = szPieces[0].Trim();
			newMod.m_szModifierDesc = szPieces[1].Trim();
			newMod.m_nModCost = int.Parse(szPieces[2].Trim());
			dcs.GetModifierList().Add(newMod);

		}
	}

	void LoadWeaponData()
	{
		string[] lines = m_taWeaponDataAsset.text.Split('\n');
		int counter = 0;
		foreach(string sz in lines)
		{
			counter++;
			if(counter == lines.Length)
				break;
			string[] szPieces = sz.Split(',');
			DCScript.WeaponData newWeapon = new DCScript.WeaponData();
			newWeapon.m_szOwnerName = szPieces[0].Trim();
			for(int i = 1; i <= 20; i += 2)
			{
				DCScript.LevelingWeapon wpn = new DCScript.LevelingWeapon();
				wpn.m_szWeaponName = szPieces[i].Trim();
				wpn.m_nDamage = int.Parse(szPieces[i+1].Trim());
				wpn.m_nLevel = (int)((i+1)/2);
				newWeapon.m_lLevels.Add(wpn);
			}
			dcs.GetWeaponList().Add(newWeapon);
		}
	}

	void LoadUseableItems()
	{
		Dictionary<string, DCScript.ItemData> useableItemDatabase = new Dictionary<string, DCScript.ItemData>();
		string[] lines = m_szUseableItems.text.Split('\n');
		int counter = 0;
		foreach(string line in lines)
		{
			counter++;
			//apparently there is a blank line added at the end of the csv.. and I like having items in excel soooo putting in a break condition!
			if(counter == lines.Length)
				break;

			string[] szPieces = line.Split(',');
			DCScript.ItemData item = new DCScript.ItemData();
			item.m_szItemName = szPieces[0].Trim();
			item.m_nItemType = int.Parse(szPieces[1].Trim());
			item.m_nHPMod = int.Parse(szPieces[2].Trim());
			item.m_nPowMod = int.Parse(szPieces[3].Trim());
			item.m_nDefMod = int.Parse(szPieces[4].Trim());
			item.m_nSpdMod = int.Parse(szPieces[5].Trim());
			item.m_nEvaMod = int.Parse(szPieces[6].Trim());
			item.m_nHitMod = int.Parse(szPieces[7].Trim());
			item.m_nBaseValue = int.Parse(szPieces[8].Trim());
			item.m_szDescription = szPieces[9].Trim();
			useableItemDatabase.Add(item.m_szItemName,  item);

		}
		dcs.SetItemDictionary(useableItemDatabase);
	}

	void LoadArmorItems()
	{
		string[] lines = m_szArmorItems.text.Split('\n');
		int counter = 0;
		foreach(string line in lines)
		{
			counter++;
			//apparently there is a blank line added at the end of the csv.. and I like having items in excel soooo putting in a break condition!
			if(counter == lines.Length)
				break;
			////name, id, hp, pow, def, spd, cost, spc id, spc mod, description
			string[] szPieces = line.Split(',');
			DCScript.ArmorData armor = new DCScript.ArmorData();
			armor.m_szItemName = szPieces[0].Trim();
			armor.m_nItemType = int.Parse(szPieces[1].Trim());
			armor.m_nHPMod = int.Parse(szPieces[2].Trim());
			armor.m_nPowMod = int.Parse(szPieces[3].Trim());
			armor.m_nDefMod = int.Parse(szPieces[4].Trim());
			armor.m_nSpdMod = int.Parse(szPieces[5].Trim());
			armor.m_nEvaMod = int.Parse(szPieces[6].Trim());
			armor.m_nHitMod = int.Parse(szPieces[7].Trim());
			armor.m_nBaseValue = int.Parse(szPieces[8].Trim());
			armor.m_nSpecialType = int.Parse(szPieces[9].Trim());
			armor.m_nSpecialModifier = int.Parse(szPieces[10].Trim());
			armor.m_szDescription = szPieces[11].Trim();
			dcs.GetItemDictionary().Add(armor.m_szItemName, armor);
			
		}
	}

	void LoadQuestItems()
	{
		string[] lines = m_taQuestItems.text.Split('\n');
		int counter = 0;
		foreach(string line in lines)
		{
			counter++;
			//apparently there is a blank line added at the end of the csv.. and I like having items in excel soooo putting in a break condition!
			if(counter == lines.Length)
				break;


		}
	}
}
