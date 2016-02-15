using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemsAndSpellsContainer : MonoBehaviour 
{
	
	public List<cData> m_lContainer = new List<cData>();

	public class cData{public string m_szName;public int m_nAmount;public int m_nIconType;}
	//0 - Items, 1 - Magic, ----(could be more, like abilities/skills/etc)
	int m_nKey = 0;
	//Character data of current character (for spells lists)
	DCScript.CharacterData m_cCurrentCharacter = null;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void SwitchContainerTypeTo(int nKey)
	{
		m_nKey = nKey;
		UpdateContainer();
	}
	public void SwitchContainerTypeTo(DCScript.CharacterData cCharacter, int nKey)
	{
		m_cCurrentCharacter = cCharacter;
		SwitchContainerTypeTo(nKey);
	}
	void UpdateContainer()
	{
		m_lContainer.Clear();
		switch(m_nKey)
		{
		case 0:
			{
				//Items - Grab all of the useable items and how many there are of them and set those values.
				List<ItemLibrary.CharactersItems> lInventory = GameObject.Find("PersistantData").GetComponent<DCScript>().m_lItemLibrary.m_lInventory;
				foreach(ItemLibrary.CharactersItems item in lInventory)
				{
					cData newData = new cData();
					newData.m_szName = item.m_szItemName;
					newData.m_nAmount = item.m_nItemCount;
					newData.m_nIconType = item.m_nItemType;
					m_lContainer.Add(newData);
				}
			}
			break;
		case 1:
			{
				//Spells - Grab all of the spells this character knows and their mp costs.
			}
			break;
		}
	}
}
