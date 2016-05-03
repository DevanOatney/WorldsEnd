using UnityEngine;
using System.Collections;

public class CraftedItemListLoaderScript : MonoBehaviour 
{
	public TextAsset m_taCraftableItemsList;
	// Use this for initialization
	void Start () 
	{
		LoadInList();
	}
	
	void LoadInList()
	{
		DCScript dc = GameObject.Find("PersistantData").GetComponent<DCScript>();
		ItemLibrary IL = dc.m_lItemLibrary.GetInstance();
		IL.m_lCraftableItems.Clear();
		string[] szLines = m_taCraftableItemsList.ToString().Split('\n');
		foreach(string szLine in szLines)
		{
			//command for a commented line, ignores this line and moves to the next.
			if(szLine[0] == '/')
			{
				continue;
			}
			string[] szPieces = szLine.Split(',');
			ItemLibrary.CraftingItemData newItem = new ItemLibrary.CraftingItemData();
			newItem.m_szItemName = szPieces[0].Split(':')[1].Trim();
			newItem.m_bIsUnlocked = false;
			newItem.m_nCraftingLevelRequired = int.Parse(szPieces[1].Split(':')[1].Trim());
			if(int.Parse(szPieces[2].Split(':')[1].Trim()) == 0)
				newItem.m_bSpecialCaseUnlock = false;
			else
				newItem.m_bSpecialCaseUnlock = true;
			for(int i = 3; i < szPieces.Length; ++i)
			{
				string[] szReqItem = szPieces[i].Split(':');
				ItemLibrary.CraftingItemData.cRequiredItem newReqItem = new ItemLibrary.CraftingItemData.cRequiredItem(szReqItem[1].Trim(), int.Parse(szReqItem[2].Trim()));
				newItem.m_lRequiredItems.Add(newReqItem);
			}
			IL.m_lCraftableItems.Add(newItem);
		}
	}
}
