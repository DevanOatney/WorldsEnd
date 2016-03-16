using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScreenScript : MonoBehaviour 
{

	public enum MENU_STATES{eINNACTIVE, eTOPTAB_SELECTION, ePARTYTAB, eSTATUS_SUBTAB, eVIEWSTATUSSCREEN, eFORMATION_SUBTAB, eROSTER_SUBTAB, eITEMTAB, eMAGICTAB, eSKILLSTAB, eLOGTAB, eSYSTEMTAB}
	public int m_nMenuState = (int)MENU_STATES.eINNACTIVE;
	//iter for which tab on the top of the menu is selected
	[HideInInspector]
	public int m_nTopTabMenuSelectionIndex = 0;
	//iter for which sub-tab on the left most of the menu is selected
	[HideInInspector]
	public int m_nSubTabMenuSelectionIndex = 0;
	//iter for which character panel is selected.  DOES NOT reflect which character is selected, need to call RetrieveCharacter(int panelIndex) to get which character is being selected;
	[HideInInspector]
	public int m_nCharacterPanelSelectionIndex = 0;
	//counter for how many panels are done sliding. (reset back to zero at the end of each event)
	int m_nPanelsThatHaveFinishedSliding = 0;
	//Hook to the canvas
	public GameObject m_goMainMenu;
	//Hooks to the top tab options
	public GameObject[] m_goTopTabs;
	//Hooks to the Party Sub Tab Options
	public GameObject[] m_goPartySubTabs;
	//Hooks to the character panels
	public GameObject[] m_goCharacterPanels;
	//Hook to the top tabs of the character panels
	public GameObject m_goTopCharacterTabs;
	//Hook to the spider graph
	public GameObject m_goRadarChart;
	//Flag to stop ALL input until some event is over
	bool m_bWaiting = false;
	//Flag for first time back after waiting for events
	bool m_bFirstTimeFlag = false;
	List<DCScript.CharacterData> m_lParty;

	DCScript dc;

	public GameObject m_goCharacterSelector;


	public GameObject m_goInventory;
	public GameObject m_goStatus;
	public GameObject m_goEquipment;

	public GameObject m_goItemPrefab;
	public GameObject m_goItemSelected = null;
	int m_nCharacterSelectedIndexForItemUse = 0;

	// Use this for initialization
	void Start () 
	{
		dc = GameObject.Find("PersistantData").GetComponent<DCScript>();
		foreach(Transform child in transform)
		{
			child.gameObject.SetActive(false);
		}
		PopulatePartyMembers();
		m_goMainMenu.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		HandleState();
	}

	void HandleState()
	{
		switch(m_nMenuState)
		{
		case (int)MENU_STATES.eINNACTIVE:
			{
				//Escape opens up the menu
				if(Input.GetKeyDown(KeyCode.Escape) && GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().GetAllowInput() == true)
				{
					m_nTopTabMenuSelectionIndex = 0;
					m_nMenuState = (int)MENU_STATES.eTOPTAB_SELECTION;
					GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
					Camera.main.GetComponent<GreyScaleScript>().SendMessage("StartGreyScale");
					GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
					m_goMainMenu.SetActive(true);
					DisplayCharacterPanels(false);
				}
			}	
				
			break;
		case (int)MENU_STATES.eTOPTAB_SELECTION:
			{
				//arrow keys and mouse can select one of the top options, escape closes the menu.
				AdjustTopTabLayers();
				if(m_bWaiting == false)
					TopTabMenu_Input();

			}
			break;
		case (int)MENU_STATES.ePARTYTAB:
			{
				//arrow keys and mouse can select one of the sub tab options in menu, escape returns to top tab selection
				AdjustPartyTabSelection();
				if(m_bWaiting == false)
					PartyTabMenu_Input();

			}
			break;
		case (int)MENU_STATES.eSTATUS_SUBTAB:
			{
				AdjustPartyPanels();
				if(m_bWaiting == false)
					StatusTabMenu_Input();
			}
			break;
		case (int)MENU_STATES.eVIEWSTATUSSCREEN:
			{
				//This is really just viewing the status screen, I believe the only input is to go BACK to the previous state. First, don't do anything till the slide event is over...
				if(m_bWaiting == false)
				{
					//If this is the first frame after waiting, set the status screen panel to active
					if(m_bFirstTimeFlag == true)
					{
						m_bFirstTimeFlag = false;
						m_goStatus.SetActive(true);
						AdjustStatusScreen(RetrieveCharacter(m_nCharacterPanelSelectionIndex));
					}
					//mmk, if the user hits back...
					if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetMouseButtonDown(1))
					{
						m_bWaiting = true;
						m_bFirstTimeFlag = true;
						m_nMenuState = (int)MENU_STATES.eSTATUS_SUBTAB;
						m_goStatus.SetActive(false);
						foreach(GameObject go in m_goCharacterPanels)
						{
							go.GetComponent<CharacterPanelScript>().ReturnToPosition(gameObject);
						}
						m_goTopCharacterTabs.GetComponent<CharacterPanelScript>().ReturnToPosition(gameObject);
					}
				}
			}
			break;
		}
	}

	#region TopMenu
	void TopTabMenu_Input()
	{
		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
		{
			m_goMainMenu.SetActive(false);
			m_nMenuState = (int)MENU_STATES.eINNACTIVE;
			GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			Camera.main.GetComponent<GreyScaleScript>().SendMessage("EndGreyScale");
		}
		else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Return))
		{
			TopTabSelectionSelected(m_nTopTabMenuSelectionIndex);
		}
		else if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			m_nTopTabMenuSelectionIndex = m_nTopTabMenuSelectionIndex - 1;
			if(m_nTopTabMenuSelectionIndex < 0)
				m_nTopTabMenuSelectionIndex = 5;
		}
		else if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			m_nTopTabMenuSelectionIndex += 1;
			if(m_nTopTabMenuSelectionIndex > 5)
				m_nTopTabMenuSelectionIndex = 0;
		}
	}
	public void TopTabHighlighted(int nIndex)
	{
		if(m_nMenuState != (int)MENU_STATES.eTOPTAB_SELECTION)
			return;
		m_nTopTabMenuSelectionIndex = nIndex;
	}
	public void TopTabSelectionSelected(int nIndex)
	{
		if(m_nMenuState != (int)MENU_STATES.eTOPTAB_SELECTION)
			return;
		m_nTopTabMenuSelectionIndex = nIndex;
		switch(m_nTopTabMenuSelectionIndex)
		{
		case 0:
			{
				//Party
				m_nMenuState = (int)MENU_STATES.ePARTYTAB;
				PopulatePartyMembers();
				DisplayCharacterPanels(true);
			}
			break;
		case 1:
			{
				//Item
				m_nMenuState = (int)MENU_STATES.eITEMTAB;
			}
			break;
		case 2:
			{
				//Magic
				m_nMenuState = (int)MENU_STATES.eMAGICTAB;
			}
			break;
		case 3:
			{
				//Skills
				m_nMenuState = (int)MENU_STATES.eSKILLSTAB;
			}
			break;
		case 4:
			{
				//Log
				m_nMenuState = (int)MENU_STATES.eLOGTAB;
			}
			break;
		case 5:
			{
				//system
				m_nMenuState = (int)MENU_STATES.eSYSTEMTAB;
			}
			break;
		}
	}
	void AdjustTopTabLayers()
	{
		int counter = 0;
		foreach(GameObject go in m_goTopTabs)
		{
			if(counter == m_nTopTabMenuSelectionIndex)
			{
				go.GetComponent<Canvas>().sortingOrder = 1;
				foreach(Transform child in go.transform)
				{
					if(child.name != "Text")
						child.gameObject.SetActive(true);
				}
				Image_Darken(go);
			}
			else
			{
				go.GetComponent<Canvas>().sortingOrder = -1;
				foreach(Transform child in go.transform)
				{
					if(child.name != "Text")
						child.gameObject.SetActive(false);
				}
				Image_Brighten(go);
			}
			counter++;
		}
	}
	#endregion
	#region PartyMenu
	void PartyTabMenu_Input()
	{
		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
		{
			m_nSubTabMenuSelectionIndex = 0;
			m_nMenuState = (int)MENU_STATES.eTOPTAB_SELECTION;
			DisplayCharacterPanels(false);
			foreach(GameObject go in m_goPartySubTabs)
				Image_Brighten(go);
		}
		else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Return))
		{
			
			PartyTabSelectionSelected(m_nSubTabMenuSelectionIndex);
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			m_nSubTabMenuSelectionIndex = m_nSubTabMenuSelectionIndex - 1;
			if(m_nSubTabMenuSelectionIndex < 0)
			{
				m_nSubTabMenuSelectionIndex = 0;
				m_nMenuState = (int)MENU_STATES.eTOPTAB_SELECTION;
				DisplayCharacterPanels(false);
				foreach(GameObject go in m_goPartySubTabs)
					Image_Brighten(go);
			}
		}
		else if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			m_nSubTabMenuSelectionIndex += 1;
			if(m_nSubTabMenuSelectionIndex > 2)
				m_nSubTabMenuSelectionIndex = 2;
		}
	}
	void AdjustPartyTabSelection()
	{
		int counter = 0;
		foreach(GameObject go in m_goPartySubTabs)
		{
			if(counter == m_nSubTabMenuSelectionIndex)
			{
				Image_Darken(go);
			}
			else
			{
				Image_Brighten(go);
			}
			counter++;
		}
	}
	public void PartyTabHighlighted(int nIndex)
	{
		if(m_nMenuState != (int)MENU_STATES.ePARTYTAB)
			return;
		m_nSubTabMenuSelectionIndex = nIndex;
	}
	public void PartyTabSelectionSelected(int nIndex)
	{
		if(m_nMenuState != (int)MENU_STATES.ePARTYTAB)
			return;
		m_nSubTabMenuSelectionIndex = nIndex;
		switch(m_nSubTabMenuSelectionIndex)
		{
		case 0:
			{
				//STATUS
				if(RecursivePanelShiftRight(m_nCharacterPanelSelectionIndex) == true)
				{
					PopulatePartyMembers();
					m_nMenuState = (int)MENU_STATES.eSTATUS_SUBTAB;
				}
			}
			break;
		case 1:
			{
				//FORMATION
			}
			break;
		case 2:
			{
				//ROSTER
			}
			break;
		}
	}
	#endregion
	#region StatusMenu
	void StatusTabMenu_Input()
	{
		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
		{
			m_nMenuState = (int)MENU_STATES.ePARTYTAB;
			foreach(GameObject go in m_goCharacterPanels)
			{
				m_bWaiting = true;
				m_bFirstTimeFlag = true;
				foreach(GameObject panel in m_goCharacterPanels)
					Image_Brighten(panel);
				go.GetComponent<CharacterPanelScript>().ReturnToPosition(gameObject);
			}
			m_goTopCharacterTabs.GetComponent<CharacterPanelScript>().ReturnToPosition(gameObject);
		}
		else if(Input.GetKeyDown(KeyCode.Return))
		{
			CharacterSelected(m_nCharacterPanelSelectionIndex);
		}
		else if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			//For this you need to check if this panel actually has an active character first, else, move to the next, if there are no more to the left, go back to the previous selection.
			if(RecursivePanelShiftLeft(m_nCharacterPanelSelectionIndex - 1) == true)
			{
				//was able to shift left
			}
			else
			{
				//wasn't able to shift left
				m_bWaiting = true;
				m_bFirstTimeFlag = true;
				m_nMenuState = (int)MENU_STATES.ePARTYTAB;
				m_nCharacterPanelSelectionIndex = 0;
				foreach(GameObject panel in m_goCharacterPanels)
					Image_Brighten(panel);
				foreach(GameObject go in m_goCharacterPanels)
				{
					go.GetComponent<CharacterPanelScript>().ReturnToPosition(gameObject);
				}
				m_goTopCharacterTabs.GetComponent<CharacterPanelScript>().ReturnToPosition(gameObject);
			}
		}
		else if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			//For this you need to check if this panel actually has an active character first, else, move to the next, if there are no more to the right, don't do anything.
			if(RecursivePanelShiftRight(m_nCharacterPanelSelectionIndex + 1) == true)
			{
				//was able to shift right
			}
			else
			{
				//wasn't able to shift right
			}
		}
	}

	public void CharacterHighlighted(int nIndex)
	{
		if(m_nMenuState != (int)MENU_STATES.eSTATUS_SUBTAB || m_bWaiting == true)
			return;
		m_nCharacterPanelSelectionIndex = nIndex;
		RecursivePanelShiftLeft(m_nCharacterPanelSelectionIndex);
	}

	public void CharacterSelected(int nIndex)
	{
		if(m_nMenuState != (int)MENU_STATES.eSTATUS_SUBTAB || m_bWaiting == true)
			return;
		//first check to see if this character is even a valid selection...
		DCScript.CharacterData character = RetrieveCharacter(nIndex);
		if(character == null)
			return;
		else
		{
			//So we've selected a character...  let's see what we should be doing next!
			switch(m_nMenuState)
			{
			case (int)MENU_STATES.eSTATUS_SUBTAB:
				{
					//Pause input so that the panels can do their slide event
					m_bWaiting = true;
					m_bFirstTimeFlag = true;
					//Move the state to viewing the status screen panel
					m_nMenuState = (int)MENU_STATES.eVIEWSTATUSSCREEN;
					m_nCharacterPanelSelectionIndex = nIndex;
					//Tell each of the panels to begin sliding
					foreach(GameObject go in m_goCharacterPanels)
					{
						go.GetComponent<CharacterPanelScript>().BeginSlide(gameObject, m_goCharacterPanels[0].GetComponent<RectTransform>().localPosition);
					}
					m_goTopCharacterTabs.GetComponent<CharacterPanelScript>().BeginSlide(gameObject, Vector3.zero);
				}
				break;
			}
		}
	}
	#endregion



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

	#region Inventory Functions
	public void PopulateInventory(int type)
	{
		Transform contents = gameObject.transform.FindChild("Inventory").transform.FindChild("Inventory Space").FindChild("Inventory Contents").transform;
		foreach(Transform child in contents)
		{
			Destroy(child.gameObject);
		}
		List<ItemLibrary.CharactersItems> m_lInv = dc.m_lItemLibrary.GetItemsOfType(type);
		int i = 0;
		float xOffset = -240.0f; float xAdj = 240.0f;
		float yOffset = 380.0f; float yAdj = -40.0f;
		foreach(ItemLibrary.CharactersItems item in m_lInv)
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
		Transform tChoice = transform.FindChild("Inventory").FindChild("Item Choice");
		if(tChoice.gameObject.activeSelf == false)
		{
			m_goItemSelected = pItem;
			ItemLibrary.CharactersItems item = dc.m_lItemLibrary.GetItemFromInventory(pItem.transform.FindChild("Item Name").GetComponent<Text>().text);
			
			tChoice.gameObject.SetActive(true);
			if(item.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL)
			{
				//Single heal item, draw one selector and allow the player to move up/down.
				tChoice.FindChild("Use").GetComponent<Image>().color = Color.white;
				
			}
			else if(item.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL)
			{
				//Group heal, draw selector over all units, heal if player presses confirm.
				tChoice.FindChild("Use").GetComponent<Image>().color = Color.white;
			}
			else
			{
				//Just show discard option, grey out use option.
				tChoice.FindChild("Use").GetComponent<Image>().color = Color.grey;
			}
		}
  	}
	//"Use" has been chosen
	public void ItemChoice_USE()
	{
		ItemLibrary.CharactersItems item = dc.m_lItemLibrary.GetItemFromInventory(m_goItemSelected.transform.FindChild("Item Name").GetComponent<Text>().text);
		if(item.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL ||
		   item.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL)
		{
			transform.FindChild("Inventory").FindChild("Item Choice").gameObject.SetActive(false);
			transform.FindChild("Inventory").FindChild("Character Selector").gameObject.SetActive(true);
		}
  	}
	//"Discard" has been selected
	public void ItemChoice_DISCARD()
	{
		ItemLibrary.CharactersItems item = dc.m_lItemLibrary.GetItemFromInventory(m_goItemSelected.transform.FindChild("Item Name").GetComponent<Text>().text);
		transform.FindChild("Inventory").FindChild("Item Choice").gameObject.SetActive(false);
		m_goCharacterSelector.SetActive(false);
		dc.m_lItemLibrary.RemoveItem(item);
  	}
	//"Cancel" has been selected
	public void ItemChoice_CANCEL()
	{
		m_goItemSelected = null;
		transform.FindChild("Inventory").FindChild("Item Choice").gameObject.SetActive(false);
  	}
	//A character has been chosen to use an item on
	public void UseItemOnSelectedCharacter(int characterIndex)
	{
		//m_gPartyMembers[characterIndex];
		ItemLibrary.CharactersItems item = dc.m_lItemLibrary.GetItemFromInventory(m_goItemSelected.transform.FindChild("Item Name").GetComponent<Text>().text);
		ItemLibrary.ItemData dcItemData = dc.m_lItemLibrary.GetItemFromDictionary(item.m_szItemName);
		m_nCharacterSelectedIndexForItemUse = characterIndex;
		switch(dcItemData.m_szDescription)
		{
			case "Cures Poison.":
			{
				//check to see if it's healing all targets or just one.
				if(dcItemData.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL)
				{
					//check to see if anyone is effected by poison
					int removeIter = -1;
					int counter = 0;
					foreach(DCScript.StatusEffect se in dc.GetStatusEffects())
					{
						if(se.m_szEffectName == "Poison")
						{
							removeIter = counter;
							se.m_lEffectedMembers.Clear();
						}
						counter++;
					}
					if(removeIter != -1)
					{
						//some people were effected by poison, decrement the item count by one, remove the status effect.
						
						//is this the last of this item?
						if(item.m_nItemCount == 1)
						{
							m_goCharacterSelector.SetActive(false);
							dc.m_lItemLibrary.RemoveItem(item);
						}
						//this isn't the last item? just remove one of it then.
						else
							dc.m_lItemLibrary.RemoveItem(item);
						              //remove the status effect from the party
						dc.GetStatusEffects().RemoveAt(removeIter);
						GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().RemoveStatusEffect("Poison");
					}
					else
					{
						//no unit was effected by this status, do nothing
					}
					
				}
				else
				{
					//only trying to remove poison from a single target
					//check to see if anyone is effected by poison
					int removeIter = -1;
					int counter = 0;
					bool effectFound = false;
					foreach(DCScript.StatusEffect se in dc.GetStatusEffects())
					{
						if(se.m_szEffectName == "Poison")
						{
							removeIter = counter;
							if(se.m_lEffectedMembers.Remove(dc.GetParty()[characterIndex].m_szCharacterName) == true)
							{
								//this unit WAS effected by the status
								effectFound = true;
							}
						}
						counter++;
					}
					if(removeIter != -1 && effectFound == true)
					{
						//is this the last of this item?
						if(item.m_nItemCount == 1)
						{
							m_goCharacterSelector.SetActive(false);
							dc.m_lItemLibrary.RemoveItem(item);
						}
						//this isn't the last item? just remove one of it then.
						else
							dc.m_lItemLibrary.RemoveItem(item);
						//if there are no more effect members, remove the status effect from the party.
						if(dc.GetStatusEffects()[removeIter].m_lEffectedMembers.Count == 0)
						{
							dc.GetStatusEffects().RemoveAt(removeIter);
							GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().RemoveStatusEffect("Poison");
						}
					}
					
				}
				
			}
				break;
			case "Cures Stone.":
			{
			}
				break;
			case "Cures Paralyze":
			{
			}
				break;
			case "Cures Ailments.":
			{
			}
				break;
			default:
			{
				//check to see if it's healing all targets or just one.
				if(dcItemData.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL)
				{

					//play the heal animation for each portrait
					for(int i =0 ; i < m_lParty.Count; ++i)
					{
						GameObject pMem = m_goCharacterSelector.transform.FindChild("Scaled_PartyMember" +(i+1).ToString()).gameObject;
						pMem.transform.FindChild("Animated Effect").GetComponent<Image>().enabled = true;
						pMem.transform.FindChild("Animated Effect").GetComponent<Animator>().Play("HealPortrait");
					}
				}
				else
				{
					//heal whichever unit is selected
					GameObject pMem = m_goCharacterSelector.transform.FindChild("Scaled_PartyMember" +(characterIndex+1).ToString()).gameObject;
					pMem.transform.FindChild("Animated Effect").GetComponent<Image>().enabled = true;
					pMem.transform.FindChild("Animated Effect").GetComponent<Animator>().Play("HealPortrait");
				}
				
			}
				break;
		}

	}
	#endregion

	#region Status Screen Functions

	#endregion



	#region Random Helper Functions
	//Make sure to call this anytime we're going to be looking at the character panels, as their stats may have changed, or the formation may have changed.
	void PopulatePartyMembers()
	{
		int counter = 0;
		foreach(GameObject panel in m_goCharacterPanels)
		{
			DCScript.CharacterData character = RetrieveCharacter(counter);
			if(character != null)
			{
				//populate this panel with this character's information
				Transform cName = panel.transform.FindChild("CharacterName");
				cName.GetComponent<Text>().text = character.m_szCharacterName;
				Transform cLVL = panel.transform.FindChild("CharacterLVL");
				cLVL.GetComponent<Text>().text = "Lvl : " + character.m_nLevel.ToString();
				Transform cHP = panel.transform.FindChild("CharacterHP");
				float fPercentHPLeft = (float)((float)character.m_nCurHP / (float)character.m_nMaxHP);
				if(fPercentHPLeft > 0.8f)
					cHP.GetComponent<Text>().color = Color.green;
				else if(fPercentHPLeft > 0.3f)
					cHP.GetComponent<Text>().color = Color.yellow;
				else if(fPercentHPLeft > 0.001f)
					cHP.GetComponent<Text>().color = Color.red;
				else
					cHP.GetComponent<Text>().color = Color.black;
				cHP.GetComponent<Text>().text =  "HP : " + character.m_nCurHP.ToString();
				Transform cMP = panel.transform.FindChild("CharacterMP");
				fPercentHPLeft = (float)((float)character.m_nCurMP / (float)character.m_nMaxMP);
				if(fPercentHPLeft > 0.8f)
					cMP.GetComponent<Text>().color = Color.green;
				else if(fPercentHPLeft > 0.3f)
					cMP.GetComponent<Text>().color = Color.yellow;
				else if(fPercentHPLeft > 0.001f)
					cMP.GetComponent<Text>().color = Color.red;
				else
					cMP.GetComponent<Text>().color = Color.black;
				cMP.GetComponent<Text>().text = "MP : " + character.m_nCurMP.ToString();
				Transform cEXP = panel.transform.FindChild("CharacterEXP");
				cEXP.GetComponent<Text>().text = "EXP : " + character.m_nCurrentEXP.ToString();
				Transform cPort = panel.transform.FindChild("CharacterImage");
				cPort.GetComponent<Image>().color = Color.white;
				GameObject pCont = GameObject.Find("Portraits Container");
				Texture2D texture;
				if(pCont.GetComponent<PortraitContainerScript>().m_dPortraits.TryGetValue(character.m_szCharacterName + "1", out texture))
				{
					cPort.GetComponent<Image>().sprite = Sprite.Create(texture, 
						new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
				}
				else
				{
					GameObject unit = Resources.Load<GameObject>("Units/Ally/" + character.m_szCharacterName + "/" + character.m_szCharacterName);
					cPort.GetComponent<Image>().sprite = Sprite.Create(unit.GetComponent<CAllyBattleScript>().m_tLargeBust, 
						new Rect(0, 0, unit.GetComponent<CAllyBattleScript>().m_tLargeBust.width,
						unit.GetComponent<CAllyBattleScript>().m_tLargeBust.height), new Vector2(0.5f, 0.5f));
				}
			}
			else
			{
				//no character in this slot, de-activate the panel.
				Transform cName = panel.transform.FindChild("CharacterName");
				cName.GetComponent<Text>().text = "";
				Transform cLVL = panel.transform.FindChild("CharacterLVL");
				cLVL.GetComponent<Text>().text = "";
				Transform cHP = panel.transform.FindChild("CharacterHP");
				cHP.GetComponent<Text>().text =  "";
				Transform cMP = panel.transform.FindChild("CharacterMP");
				cMP.GetComponent<Text>().text = "";
				Transform cEXP = panel.transform.FindChild("CharacterEXP");
				cEXP.GetComponent<Text>().text = "";
				Transform cPort = panel.transform.FindChild("CharacterImage");
				cPort.GetComponent<Image>().sprite = null;
				cPort.GetComponent<Image>().color = Color.clear;
			}
			counter++;
		}
	}
	void AdjustPartyPanels()
	{
		int counter = 0;
		foreach(GameObject panel in m_goCharacterPanels)
		{
			if(counter == m_nCharacterPanelSelectionIndex)
			{
				panel.GetComponent<Canvas>().sortingOrder = 1;
				Image_Darken(panel);
			}
			else
			{
				panel.GetComponent<Canvas>().sortingOrder = -1;
				Image_Brighten(panel);
			}
			counter++;
		}
	}
	void AdjustStatusScreen(DCScript.CharacterData character)
	{
		Transform characterName = m_goStatus.transform.FindChild("CharacterName");
		characterName.GetComponent<Text>().text = character.m_szCharacterName;
		Transform characterDesc = m_goStatus.transform.FindChild("CharacterDescription");
		characterDesc.GetComponent<Text>().text = character.m_szCharacterBio;
		Transform characterBody = m_goStatus.transform.FindChild("CharacterBody");
		Color fadedWhite = Color.white;
		fadedWhite.a = 0.3f;
		characterBody.GetComponent<Image>().color = fadedWhite;
		GameObject pCont = GameObject.Find("Portraits Container");
		Texture2D texture;
		if(pCont.GetComponent<PortraitContainerScript>().m_dPortraits.TryGetValue(character.m_szCharacterName + "1", out texture))
		{
			characterBody.GetComponent<Image>().sprite = Sprite.Create(texture, 
				new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
		}
		else
		{
			GameObject unit = Resources.Load<GameObject>("Units/Ally/" + character.m_szCharacterName + "/" + character.m_szCharacterName);
			characterBody.GetComponent<Image>().sprite = Sprite.Create(unit.GetComponent<CAllyBattleScript>().m_tLargeBust, 
				new Rect(0, 0, unit.GetComponent<CAllyBattleScript>().m_tLargeBust.width,
					unit.GetComponent<CAllyBattleScript>().m_tLargeBust.height), new Vector2(0.5f, 0.5f));
		}

		Transform weaponPanel = m_goStatus.transform.FindChild("WeaponPanel");
		Transform weaponName = weaponPanel.FindChild("WeaponName");
		weaponName.GetComponent<Text>().text = character.m_szWeaponName;
		Transform weaponLevel = weaponPanel.FindChild("WeaponLevel");
		weaponLevel.GetComponent<Text>().text = "Weapon Level: " + character.m_nWeaponLevel.ToString();
		Transform weaponMod = weaponPanel.FindChild("WeaponMod");
		if(character.m_szWeaponModifierName != "")
			weaponMod.GetComponent<Text>().text = "Weapon Mod : " + character.m_szWeaponModifierName;
		else
			weaponMod.GetComponent<Text>().text = "Weapon Mod : None";


		//So we need to create a list of 0-1 floats that represent the stat fill radar chart.
		List<float> lStatDistances = new List<float>();
		List<int> lStats = new List<int>();
		//SPD, DEF, HP, POW, HIT, MP, EVA (I think this is the order, more testing.
		lStats.Add(character.m_nSPD);
		m_goRadarChart.transform.FindChild("SPD").FindChild("Stat").GetComponent<Text>().text = character.m_nSPD.ToString();
		lStats.Add(character.m_nEVA);
		m_goRadarChart.transform.FindChild("EVA").FindChild("Stat").GetComponent<Text>().text = character.m_nEVA.ToString();
		lStats.Add(character.m_nMaxMP);
		m_goRadarChart.transform.FindChild("MP").FindChild("Stat").GetComponent<Text>().text = character.m_nMaxMP.ToString();
		lStats.Add(character.m_nHIT);
		m_goRadarChart.transform.FindChild("HIT").FindChild("Stat").GetComponent<Text>().text = character.m_nHIT.ToString();
		lStats.Add(character.m_nSTR);
		m_goRadarChart.transform.FindChild("POW").FindChild("Stat").GetComponent<Text>().text = character.m_nSTR.ToString();
		lStats.Add(character.m_nMaxHP);
		m_goRadarChart.transform.FindChild("HP").FindChild("Stat").GetComponent<Text>().text = character.m_nMaxHP.ToString();
		lStats.Add(character.m_nDEF);
		m_goRadarChart.transform.FindChild("DEF").FindChild("Stat").GetComponent<Text>().text = character.m_nDEF.ToString();

		int highestStat = 0;
		foreach(int n in lStats)
			if(highestStat < n)
				highestStat = n;
		foreach(int n in lStats)
		{
			float distance = (float)((float)n / (float)highestStat);
			lStatDistances.Add(distance);
		}
		m_goRadarChart.GetComponent<RadarGraphScript>().AdjustFill(lStatDistances);

		//Now let's populate the equipment.
		if(character.m_idHelmSlot != null)
		{
			m_goEquipment.transform.FindChild("Head").GetComponent<Text>().text = "Head Slot : " + character.m_idHelmSlot.m_szItemName;
		}
		else
		{
			m_goEquipment.transform.FindChild("Head").GetComponent<Text>().text = "Head Slot : None";
		}
		if(character.m_idShoulderSlot != null)
		{
			m_goEquipment.transform.FindChild("Shoulder").GetComponent<Text>().text = "Shoulder Slot : " + character.m_idShoulderSlot.m_szItemName;
		}
		else
		{
			m_goEquipment.transform.FindChild("Shoulder").GetComponent<Text>().text = "Shoulder Slot : None";
		}
		if(character.m_idChestSlot != null)
		{
			m_goEquipment.transform.FindChild("Chest").GetComponent<Text>().text = "Chest Slot : " + character.m_idChestSlot.m_szItemName;
		}
		else
		{
			m_goEquipment.transform.FindChild("Chest").GetComponent<Text>().text = "Chest Slot : None";
		}
		if(character.m_idGloveSlot != null)
		{
			m_goEquipment.transform.FindChild("Arms").GetComponent<Text>().text = "Glove Slot : " + character.m_idGloveSlot.m_szItemName;
		}
		else
		{
			m_goEquipment.transform.FindChild("Arms").GetComponent<Text>().text = "Glove Slot : None";
		}
		if(character.m_idBeltSlot != null)
		{
			m_goEquipment.transform.FindChild("Waist").GetComponent<Text>().text = "Belt Slot : " + character.m_idBeltSlot.m_szItemName;
		}
		else
		{
			m_goEquipment.transform.FindChild("Waist").GetComponent<Text>().text = "Waist Slot : None";
		}
		if(character.m_idLegSlot != null)
		{
			m_goEquipment.transform.FindChild("Legs").GetComponent<Text>().text = "Leg Slot : " + character.m_idLegSlot.m_szItemName;
		}
		else
		{
			m_goEquipment.transform.FindChild("Legs").GetComponent<Text>().text = "Leg Slot : None";
		}
		if(character.m_idTrinket1 != null)
		{
			m_goEquipment.transform.FindChild("Trinket1").GetComponent<Text>().text = "Trinket Slot : " + character.m_idTrinket1.m_szItemName;
		}
		else
		{
			m_goEquipment.transform.FindChild("Trinket1").GetComponent<Text>().text = "Trinket Slot : None";
		}
		if(character.m_idTrinket2 != null)
		{
			m_goEquipment.transform.FindChild("Trinket2").GetComponent<Text>().text = "Trinket Slot : " + character.m_idTrinket2.m_szItemName;
		}
		else
		{
			m_goEquipment.transform.FindChild("Trinket1").GetComponent<Text>().text = "Trinket Slot : None";
		}

	}
	bool RecursivePanelShiftRight(int nIndex)
	{
		if(nIndex > 6)
			return false;
		if(RetrieveCharacter(nIndex) != null)
		{
			m_nCharacterPanelSelectionIndex = nIndex;
			return true;
		}
		else
			return RecursivePanelShiftRight(nIndex+1);
	}
	bool RecursivePanelShiftLeft(int nIndex)
	{
		if(nIndex < 0)
			return false;
		if(RetrieveCharacter(nIndex) != null)
		{
			m_nCharacterPanelSelectionIndex = nIndex;
			return true;
		}
		else
			return RecursivePanelShiftLeft(nIndex - 1);
	}
	DCScript.CharacterData RetrieveCharacter(int nPanelIndex)
	{
		int nFormationOfCharacter = 0;
		//convert the index of panel selected, to the formation of that party member.
		switch(nPanelIndex)
		{
		case 0:
			{
				//top left
				nFormationOfCharacter = 3;
			}
			break;
		case 1:
			{
				//mid left
				nFormationOfCharacter = 4;
			}
			break;
		case 2:
			{
				//bottom left
				nFormationOfCharacter = 5;
			}
			break;
		case 3:
			{
				//top right
				nFormationOfCharacter = 0;
			}
			break;
		case 4:
			{
				//mid right
				nFormationOfCharacter = 1;
			}
			break;
		case 5:
			{
				//bottom right
				nFormationOfCharacter = 2;
			}
			break;
		case 6:
			{
				//The support character
				nFormationOfCharacter = -1;
			}
			break;
		}
		if(nFormationOfCharacter != -1)
		{
			List<DCScript.CharacterData> lParty = dc.GetParty();
			foreach(DCScript.CharacterData character in lParty)
			{
				if(character.m_nFormationIter == nFormationOfCharacter)
				{
					return character;
				}
			}
		}
		else
		{
			//this is the support character.. not sure what to do here yet since support characters don't actually exist yet
		}
		return null;
	}

	public void Image_Brighten(GameObject image)
	{
		image.GetComponent<Image>().color = new Color(255, 255, 255);
	}
	public void Image_Darken(GameObject image)
	{
		image.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
	}
	public void DisplayCharacterPanels(bool bFlag)
	{
		foreach(GameObject panel in m_goCharacterPanels)
			panel.SetActive(bFlag);
		m_goTopCharacterTabs.SetActive(bFlag);
	}
	void PanelReachedSlot()
	{
		m_nPanelsThatHaveFinishedSliding += 1;
		if(m_nPanelsThatHaveFinishedSliding == 8)
		{
			m_nPanelsThatHaveFinishedSliding = 0;
			m_bWaiting = false;
		}
	}
	#endregion
}
