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
	DCScript dcs;
	// Use this for initialization
	void Start () 
	{
		dcs = GameObject.Find("PersistantData").GetComponent<DCScript>();
		LoadUseableItems();
		LoadArmorItems();


		foreach(KeyValuePair<string, DCScript.ItemData> kvp in dcs.GetItemDictionary())
		{
			DCScript.CharactersItems m_ciItemHeld = new DCScript.CharactersItems();
			m_ciItemHeld.m_szItemName = kvp.Key;
			m_ciItemHeld.m_nItemCount = 1;
			m_ciItemHeld.m_nItemType = kvp.Value.m_nItemType;
			dcs.AddItem(m_ciItemHeld);
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
			item.m_nModifier = int.Parse(szPieces[2].Trim());
			item.m_nBaseValue = int.Parse(szPieces[3].Trim());
			item.m_szDescription = szPieces[4].Trim();
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
			armor.m_nBaseValue = int.Parse(szPieces[6].Trim());
			armor.m_nSpecialType = int.Parse(szPieces[7].Trim());
			armor.m_nSpecialModifier = int.Parse(szPieces[8].Trim());
			armor.m_szDescription = szPieces[9].Trim();
			dcs.GetItemDictionary().Add(armor.m_szItemName, armor);
			
		}
	}
}
