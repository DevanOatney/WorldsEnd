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
		// 0 - useable item, 1- Armor, 2- Trinkets, 3- Junk, 4- Key Items
		//1-4 : useable item, 5: armor, 6: junk, 7- Key Items
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

	public List<CraftingItemData> m_lCraftableItems = new List<CraftingItemData>();
	[System.Serializable]
	public class CraftingItemData
	{
		public string m_szItemName;
		//flag for if this recipe is unlocked, if it is.. well.. it is and the player can craft it
		public bool m_bIsUnlocked = true;
		//turn on if you don't want this to be a level restricted crafting recipe but instead something that's unlocked by something else.
		public bool m_bSpecialCaseUnlock = false;
		//Only necessary if you want this recipe unlocked purely by crafting level.  Let's have -1 be the identifier for if this is special (incase we want to display crafting level req for other things.
		public int m_nCraftingLevelRequired;
		public class cRequiredItem
		{
			public cRequiredItem(string _name, int _count) {m_szItemName = _name; m_nItemCount = _count;}
			public string m_szItemName;
			public int m_nItemCount;
		}
		public List<cRequiredItem> m_lRequiredItems = new List<cRequiredItem>();
	}

	//Struct for Items, adding in an integer for the amount that the player has
	[System.Serializable]
	public class CharactersItems
	{
		public CharactersItems() {m_szItemName = ""; m_nItemType = -1; m_nItemCount = -1;}
		public CharactersItems(string _name, int _type, int _count) { m_szItemName = _name; m_nItemType = _type; m_nItemCount = _count;}
		public string m_szItemName;
		public int m_nItemType;
		public int m_nItemCount;
		public string m_szItemDesc;

		public string GetItemTypeName()
		{
			switch (m_nItemType) {
			case (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL:
				{
					return "Single Heal";
				}
			case (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL:
				{
					return "Group Heal";
				}
			case (int)BaseItemScript.ITEM_TYPES.eSINGLE_DAMAGE:
				{
					return "Single Damage";
				}
			case (int)BaseItemScript.ITEM_TYPES.eGROUP_DAMAGE:
				{
					return "Group Damage";
				}
			case (int)BaseItemScript.ITEM_TYPES.eBELTARMOR:
				{
					return "Belt Armor";
				}
			case (int)BaseItemScript.ITEM_TYPES.eHELMARMOR:
				{
					return "Helmet Armor";
				}
			case (int)BaseItemScript.ITEM_TYPES.eGLOVEARMOR:
				{
					return "Glove Armor";
				}
			case (int)BaseItemScript.ITEM_TYPES.eCHESTARMOR:
				{
					return "Chest Amor";
				}
			case (int)BaseItemScript.ITEM_TYPES.eJUNK:
				{
					return "Junk";
				}
			case (int)BaseItemScript.ITEM_TYPES.eKEYITEM:
				{
					return "Key Item";
				}
			case (int)BaseItemScript.ITEM_TYPES.eLEGARMOR:
				{
					return "Leg Armor";
				}
			case (int)BaseItemScript.ITEM_TYPES.eSHOULDERARMOR:
				{
					return "Shoulder Armor";
				}
			case (int)BaseItemScript.ITEM_TYPES.eTRINKET:
				{
					return "Trinket";
				}
			}
			return "";
		}
	}


	//returns list of items of a specific type, -1 returns all
	public List<CharactersItems> GetItemsOfType(int type)
	{
		List<CharactersItems> inv = new List<CharactersItems>();
		foreach(CharactersItems item in m_lInventory)
		{
			// 0 - useable item, 1- Armor, 2- Trinkets, 3- Junk, 4 - Key Items
			//1-4 : useable item, 5: armor, 6: junk, 7: Key Item
			switch(type)
			{
			case -1:
				{
					inv.Add (item);
				}
				break;
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
			case 4:
				{
					if (item.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eKEYITEM)
						inv.Add (item);
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
