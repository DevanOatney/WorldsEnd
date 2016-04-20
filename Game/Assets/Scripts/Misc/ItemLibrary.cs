using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemLibrary  
{
	static ItemLibrary m_sInstance = null;
	public ItemLibrary GetInstance()
	{
		if(m_sInstance == null)
			m_sInstance = this;
		return m_sInstance;
	}
	//Dictionary of all of the items in the game, key is the items name
	public Dictionary<string, ItemLibrary.ItemData> m_dItemDictionary = new Dictionary<string, ItemData>();
	//List of all items currently in inventory
	public List<CharactersItems> m_lInventory = new List<CharactersItems>();
	[System.Serializable]
	public class ItemData
	{
		public string m_szItemName;
		// 0 - useable item, 1- Armor, 2- Trinkets, 3- Junk
		//1-4 : useable item, 5 : weapon, 6: armor, 7: junk
		public int m_nItemType;
		public int m_nHPMod;
		public int m_nMPMod;
		public int m_nPowMod;
		public int m_nDefMod;
		public int m_nSpdMod;
		public int m_nEvaMod;
		public int m_nHitMod;

		public int m_nBaseValue;
		public string m_szDescription;
	}

	//Struct for Items, adding in an integer for the amount that the player has
	[System.Serializable]
	public class CharactersItems
	{
		public string m_szItemName;
		public int m_nItemType;
		public int m_nItemCount;
	}


	//returns list of items of a specific type
	public List<CharactersItems> GetItemsOfType(int type)
	{
		List<CharactersItems> inv = new List<CharactersItems>();
		foreach(CharactersItems item in m_lInventory)
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


	public ItemData GetItemFromDictionary(string key) 
	{
		ItemData outPut;
		if(m_dItemDictionary.TryGetValue(key, out outPut))
			return outPut;

		return outPut;
	}

	public Dictionary<string, ItemData> GetItemDictionary() {return m_dItemDictionary;}
	public void SetItemDictionary(Dictionary<string, ItemData> itemdictionary) {m_dItemDictionary = itemdictionary;}

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

	public void AddItem(string szItemName)
	{
		int iter = 0;
		foreach(CharactersItems i in m_lInventory)
		{
			if(szItemName == i.m_szItemName)
			{
				m_lInventory[iter].m_nItemCount += 1;
				if(m_lInventory[iter].m_nItemCount >= 45)
					m_lInventory[iter].m_nItemCount = 45;
				return;
			}
			iter++;
		}
		CharactersItems item = new CharactersItems();
		item.m_szItemName = szItemName;
		item.m_nItemType = GetItemFromDictionary(szItemName).m_nItemType;
		item.m_nItemCount = 1;
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
	[System.Serializable]
	public class ArmorData : ItemData
	{
		//m_nModifier effects the reduction of damage, this is for like if it heals or damages units or increases/decreases other stats
		public int m_nSpecialModifier;
		//type value - see armorscript for what iter means what
		public int m_nSpecialType;
	}

}
