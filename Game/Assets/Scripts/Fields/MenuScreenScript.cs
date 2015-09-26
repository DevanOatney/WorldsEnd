using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MenuScreenScript : MonoBehaviour 
{
	bool m_bShouldPause = false;
	//iter for which menu option is highlighted
	int m_nMenuSelectionIndex = 0;
	//iter for which character is highlighted
	int m_nCharacterSelectionIndex = 0;
	//flag for if the highlighter should be on the characters
	bool m_bCharacterBeingSelected = false;
	//flag for showing a different screen other than the main party meny
	bool m_bShowDifferentMenuScreen = false;
	//delegate for handling the different party menu renderings
	delegate void m_delegate(DCScript.CharacterData g);
	m_delegate m_dFunc;
	//iter for the options "Change, Optimize, Unequipped All"
	int m_nEquipmentChangeIndex = 0;
	//flags for if the character chooses chng/opt/uneq
	bool m_bChangeSelected = false;
	bool m_bOptimizeSelected = false;
	bool m_bClearSelected = false;
	//iter for selecting which slot of gear to change
	int m_nEquipmentSlotChangeIndex = 0;
	//flag for if a slot to change has been chosen
	bool m_bSlotChangeChosen = false;
	//iter for which item in the list is being highlighted
	int m_nItemSlotIndex = 0;
	List<DCScript.CharacterData> m_lParty;

	DCScript dc;

	GameObject[] m_gPartyMembers = new GameObject[3];
	public GameObject partyMemberPrefab;
	public GameObject m_goCharacterSelector;


	public GameObject m_goMenu;
	public GameObject m_goInventory;
	public GameObject m_goStatus;
	public GameObject m_goEquipment;

	public GameObject m_goItemPrefab;
	GameObject m_goItemSelected = null;


	// Use this for initialization
	void Start () 
	{
		dc = GameObject.Find("PersistantData").GetComponent<DCScript>();
		foreach(Transform child in transform)
		{
			child.gameObject.SetActive(false);
		}
		PopulatePartyMembers();

	}
	
	// Update is called once per frame
	void Update () 
	{
		#region Input For Menu
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			//reset all of the iters and flags
			if(m_bCharacterBeingSelected == true)
			{
				m_bCharacterBeingSelected = false;
				m_nCharacterSelectionIndex = 0;
			}
			else
			{
				m_nMenuSelectionIndex = 0;
				m_bShowDifferentMenuScreen = false;
				
				m_bShouldPause = !m_bShouldPause;
				if(m_bShouldPause == false)
				{
					foreach(Transform child in transform)
					{
						child.gameObject.SetActive(false);
					}
					GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
					Camera.main.GetComponent<GreyScaleScript>().SendMessage("EndGreyScale");
					m_dFunc = null;
				}
				else
				{
					if(GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().GetAllowInput() == true)
					{
						GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
						Camera.main.GetComponent<GreyScaleScript>().SendMessage("StartGreyScale");
						foreach(Transform child in transform)
						{
							child.gameObject.SetActive(true);
						}
						foreach(Transform child in transform.FindChild("Menu").transform)
						{
							child.gameObject.SetActive(true);
						}
						m_goMenu.GetComponent<Animator>().Play("OpeningMenu");
						transform.FindChild("Inventory").FindChild("Item Choice").gameObject.SetActive(false);
						transform.FindChild("Inventory").FindChild("Character Selector").gameObject.SetActive(false);
						
					}
					else
					{
						m_bShouldPause = false;

					}
				}
				GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
			}
		}
		if(m_bShouldPause == true)
		{
			if(Input.GetKeyDown(KeyCode.DownArrow))
			{
				if(m_bCharacterBeingSelected == true && m_bShowDifferentMenuScreen == false)
				{
					//If we're selecting a character
					m_nCharacterSelectionIndex++;
					if(m_nCharacterSelectionIndex >= dc.GetParty().Count)
					{
						m_nCharacterSelectionIndex = 0;
					}
				}
				else if(m_bCharacterBeingSelected == false && m_bShowDifferentMenuScreen == false)
				{
					//If we're selecting a menu option
					m_nMenuSelectionIndex++;
					if(m_nMenuSelectionIndex >= 6)
					{
						m_nMenuSelectionIndex = 0;
					}
				}
				else if(m_bShowDifferentMenuScreen == true && m_dFunc == EquipmentScreen)
				{
					//we're in the equipment screen.. 
					if(m_bChangeSelected == false && m_bOptimizeSelected == false && m_bClearSelected == false)
					{
						//nothing in the equipment options has been selected yet, cycle through the options
						m_nEquipmentChangeIndex++;
						if(m_nEquipmentChangeIndex >= 3)
							m_nEquipmentChangeIndex = 0;
					}
					else if(m_bChangeSelected == true)
					{
						if(m_bSlotChangeChosen == false && m_bSlotChangeChosen == false)
						{
							m_nEquipmentSlotChangeIndex++;
							if(m_nEquipmentSlotChangeIndex > 7)
								m_nEquipmentSlotChangeIndex = 0;
						}
						else if(m_bSlotChangeChosen == true)
						{
							//cycle through the different items available for the slot that was selected
							int nItemType = m_nEquipmentSlotChangeIndex + (int)BaseItemScript.ITEM_TYPES.eHELMARMOR;
							if(nItemType > (int)BaseItemScript.ITEM_TYPES.eTRINKET)
								nItemType = (int)BaseItemScript.ITEM_TYPES.eTRINKET;
							List<DCScript.CharactersItems> inv = new List<DCScript.CharactersItems>();
							foreach(DCScript.CharactersItems item in dc.GetInventory())
							{
								if(nItemType == item.m_nItemType)
									inv.Add(item);
							}
							if(inv.Count > 0)
							{
								m_nItemSlotIndex++;
								if(m_nItemSlotIndex >= inv.Count)
									m_nItemSlotIndex = 0;
							}
							
						}
					}
				}
			}
			else if(Input.GetKeyDown(KeyCode.UpArrow))
			{
				if(m_bCharacterBeingSelected == true && m_bShowDifferentMenuScreen == false)
				{
					//If we're selecting a character
					m_nCharacterSelectionIndex--;
					if(m_nCharacterSelectionIndex < 0)
					{
						m_nCharacterSelectionIndex = dc.GetParty().Count - 1;
					}
				}
				else if(m_bCharacterBeingSelected == false && m_bShowDifferentMenuScreen == false)
				{
					//If we're selecting a menu option
					m_nMenuSelectionIndex--;
					if(m_nMenuSelectionIndex < 0)
					{
						m_nMenuSelectionIndex = 5;
					}
				}
				else if(m_bShowDifferentMenuScreen == true && m_dFunc == EquipmentScreen)
				{
					//we're in the equipment screen.. 
					if(m_bChangeSelected == false && m_bOptimizeSelected == false && m_bClearSelected == false)
					{
						//nothing in the equipment options has been selected yet, cycle through the options
						m_nEquipmentChangeIndex--;
						if(m_nEquipmentChangeIndex < 0)
							m_nEquipmentChangeIndex = 2;
					}
					else if(m_bChangeSelected == true && m_bSlotChangeChosen == false)
					{
						//cycle through the different slots that the character can wear
						m_nEquipmentSlotChangeIndex--;
						if(m_nEquipmentSlotChangeIndex < 0)
							m_nEquipmentSlotChangeIndex = 7;
					}
					else if(m_bSlotChangeChosen == true)
					{
						//cycle through the different items available for the slot that was selected
						int nItemType = m_nEquipmentSlotChangeIndex + (int)BaseItemScript.ITEM_TYPES.eHELMARMOR;
						if(nItemType > (int)BaseItemScript.ITEM_TYPES.eTRINKET)
							nItemType = (int)BaseItemScript.ITEM_TYPES.eTRINKET;
						List<DCScript.CharactersItems> inv = new List<DCScript.CharactersItems>();
						foreach(DCScript.CharactersItems item in dc.GetInventory())
						{
							if(nItemType == item.m_nItemType)
								inv.Add(item);
						}
						if(inv.Count > 0)
						{
							m_nItemSlotIndex--;
							if(m_nItemSlotIndex < 0 )
								m_nItemSlotIndex = inv.Count - 1;
						}
						
					}
				}
			}
			else if(Input.GetKeyDown(KeyCode.Return))
			{
				//if you're selecting a character
				if(m_bCharacterBeingSelected == true && m_bShowDifferentMenuScreen == false)
				{
					m_bShowDifferentMenuScreen = true;
				}
				//else, you're selecting a menu option? (might need to change this as the menu becomes more robust)
				else if(m_bCharacterBeingSelected == false && m_bShowDifferentMenuScreen == false)
				{
					switch(m_nMenuSelectionIndex)
					{
					case 0:
					{
						//Inventory
						m_bShowDifferentMenuScreen = true;
						m_dFunc = InventoryScreen;
					}
						break;
					case 1:
					{
						//Status
						m_bCharacterBeingSelected = true;
						m_dFunc = StatusScreen;
					}
						break;
					case 2:
					{
						//Equipment
						m_bCharacterBeingSelected = true;
						m_dFunc = EquipmentScreen;
					}
						break;
					case 3:
					{
						//Save Game
						if(Application.loadedLevelName == "Regilance")
							m_dFunc = SaveGame;
					}
						break;
					case 4:
					{
						//Quit to the main menu
						GameObject data = GameObject.Find("PersistantData");
						if(data)
						{
							Destroy(data);
						}
						Application.LoadLevel("Intro_Scene");
					}
						break;
					case 5:
					{
						//Exit the menu
						m_nMenuSelectionIndex = 0;
						m_bShowDifferentMenuScreen = false;
						
						m_bShouldPause = !m_bShouldPause;
						if(m_bShouldPause == false)
						{
							GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
							Camera.main.GetComponent<GreyScaleScript>().SendMessage("EndGreyScale");
							m_dFunc = null;
						}
						else
						{
							if(GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().GetAllowInput() == true)
							{
								GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
								Camera.main.GetComponent<GreyScaleScript>().SendMessage("StartGreyScale");
							}
							else
								m_bShouldPause = false;
						}
						GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
					}
						break;
					}
				}
			}
		}
		
		
		#endregion
	}

	void EquipmentScreen(DCScript.CharacterData character)
	{
	}

	void InventoryScreen(DCScript.CharacterData character)
	{
	}

	void StatusScreen(DCScript.CharacterData character)
	{
	}

	void SaveGame(DCScript.CharacterData character)
	{
	}

	//This function is called by the GUI, parameter is the index to which option was picked.
	public void MenuSelected(int index)
	{
		switch(index)
		{
		case 0:
		{
			//opening up the inventory
			m_goMenu.GetComponent<Animator>().Play("ClosingMenu");
			m_goInventory.GetComponent<Animator>().Play("OpeningInventory");
		}
			break;
		}
	}

	public void PopulateInventory(int type)
	{
		Transform contents = gameObject.transform.FindChild("Inventory").transform.FindChild("Inventory Space").FindChild("Inventory Contents").transform;
		foreach(Transform child in contents)
		{
			Destroy(child.gameObject);
		}
		List<DCScript.CharactersItems> m_lInv = GetItemsOfType(type);
		Debug.Log(m_lInv.Count);
		int i = 0;
		float xOffset = -240.0f; float xAdj = 240.0f;
		float yOffset = 380.0f; float yAdj = -40.0f;
		foreach(DCScript.CharactersItems item in m_lInv)
		{
			GameObject pMem = Instantiate(m_goItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			RectTransform myRect = pMem.GetComponent<RectTransform>();
			myRect.SetParent(contents);
			myRect.anchoredPosition = new Vector3(xOffset, yOffset, 0);
			pMem.transform.FindChild("Item Name").GetComponent<Text>().text = item.m_szItemName;
			pMem.transform.FindChild("Item Count").GetComponent<Text>().text = item.m_nItemCount.ToString();

			if(i == 2)
			{
				xOffset = -240.0f;
				yOffset += yAdj;
				i = 0;
			}
			else
			{
				xOffset += xAdj;
				i++;
			}
		}
	}

	public void InventoryItemSelected(GameObject pItem)
	{
		m_goItemSelected = pItem;
		DCScript.CharactersItems item = dc.GetItemFromInventory(pItem.transform.FindChild("Item Name").GetComponent<Text>().text);
		Transform tChoice = transform.FindChild("Inventory").FindChild("Item Choice");
		tChoice.gameObject.SetActive(true);
		if(item.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL)
		{
			//Single heal item, draw one selector and allow the player to move up/down.
			
		}
		else if(item.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL)
		{
			//Group heal, draw selector over all units, heal if player presses confirm.
		}
		else
		{
			//Just show discard option, grey out use option.
			tChoice.FindChild("Use").GetComponent<Image>().color = Color.grey;
		}
	}

	public void ItemChoice_USE()
	{
		DCScript.CharactersItems item = dc.GetItemFromInventory(m_goItemSelected.transform.FindChild("Item Name").GetComponent<Text>().text);
		if(item.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL ||
		   item.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL)
		{
			transform.FindChild("Inventory").FindChild("Character Selector").gameObject.SetActive(true);
		}
	}

	List<DCScript.CharactersItems> GetItemsOfType(int type)
	{
		List<DCScript.CharactersItems> inv = new List<DCScript.CharactersItems>();
		List<DCScript.CharactersItems> fullInv =  dc.GetInventory();
		foreach(DCScript.CharactersItems item in fullInv)
		{
			// 0 - useable item, 1- Armor, 2- Trinkets, 3- Junk, 4: key
			//1-4 : useable item, 5 : weapon, 6: armor, 7: junk, 8: key
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
			case 4:
			{
				if(item.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eKEYITEM)
					inv.Add(item);
			}
				break;
			}
		}
		return inv;
	}

	public void PopulatePartyMembers()
	{
		m_lParty = dc.GetParty();
		int i = 0;
		foreach(DCScript.CharacterData character in m_lParty)
		{
			//Create the icons for the menu screen
			GameObject pMem = Instantiate(partyMemberPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			pMem.GetComponent<RectTransform>().SetParent(gameObject.transform.FindChild("Menu").transform);
			m_gPartyMembers[i] = pMem;
			m_gPartyMembers[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(115, 130 - i * 140, 0);
			m_gPartyMembers[i].transform.FindChild("Character Name").GetComponent<Text>().text = character.m_szCharacterName;
			m_gPartyMembers[i].transform.FindChild("Character Level").GetComponent<Text>().text = "LV: \t" + character.m_nLevel.ToString();
			m_gPartyMembers[i].transform.FindChild("Character HP").GetComponent<Text>().text = "HP: \t" + character.m_nCurHP.ToString() + " \t / \t" + character.m_nMaxHP.ToString();
			m_gPartyMembers[i].transform.FindChild("Character Experience").GetComponent<Text>().text = "EXP: \t" + character.m_nCurrentEXP.ToString() + " \t / \t" + "1000";
			m_gPartyMembers[i].transform.FindChild("Character Portrait").GetComponent<Image>().sprite = Resources.Load<Sprite>("Units/Ally/" + character.m_szCharacterName + "/Portraits/" + character.m_szCharacterName + "1");

			//Create the icons for the item use screen
			pMem = m_goCharacterSelector.transform.FindChild("Scaled_PartyMember" +(i+1).ToString()).gameObject;
			pMem.SetActive(true);
			pMem.transform.FindChild("Character Name").GetComponent<Text>().text = character.m_szCharacterName;
			pMem.transform.FindChild("Character HP").GetComponent<Text>().text = "HP: " + character.m_nCurHP.ToString() + " / " + character.m_nMaxHP.ToString();

			i++;
		}
		for(;i<3; ++i)
		{
			m_goCharacterSelector.transform.FindChild("Scaled_PartyMember" +(i+1).ToString()).gameObject.SetActive(false);
		}
	}
}
