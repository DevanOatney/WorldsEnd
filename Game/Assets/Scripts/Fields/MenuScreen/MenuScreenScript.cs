using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScreenScript : MonoBehaviour 
{

	public enum MENU_STATES{eINNACTIVE, eTOPTAB_SELECTION, ePARTYTAB, eSTATUS_SUBTAB, eVIEWSTATUSSCREEN, eFORMATION_SUBTAB, eROSTER_SUBTAB, eITEMTAB, eCRAFTING_SUBTAB, eINVENTORY_SUBTAB, eEQUIPMENT_SUBTAB, eMAGICTAB, eSKILLSTAB, eLOGTAB, eSYSTEMTAB}
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
	//Hooks to the Item Sub Tab Options
	public GameObject[] m_goItemSubTabs;
	//Hooks to the character panels
	public GameObject[] m_goCharacterPanels;
	//Hooks to the Cells of the roster screen (for formation purposes)
	public GameObject[] m_goRosterCells;
	//Hooks to the units for the inventory screen for what to use items on.
	public GameObject[] m_goUnitInventoryCells;
	//Hook to the top tabs of the character panels
	public GameObject m_goTopCharacterTabs;
	//Hook to the spider graph
	public GameObject m_goRadarChart;
	//Flag to stop ALL input until some event is over
	bool m_bWaiting = false;
	//Flag for first time back after waiting for events
	bool m_bFirstTimeFlag = false;
	List<DCScript.CharacterData> m_lParty;

	public DCScript dc;

	public GameObject m_goCharacterSelector;

	public GameObject m_goRoster;
	public GameObject m_goInventory;
	public GameObject m_goStatus;
	public GameObject m_goEquipment;
	public GameObject m_goCharacterRoot;
	public GameObject m_goCharacterPrefab;
	public GameObject m_goCraftingPanel;
	public GameObject m_goItemCraftedRoot;
	public GameObject m_goItemCraftedPrefab;
	public GameObject m_goInventoryRoot;
	public GameObject m_goInventoryItemPrefab;
	public GameObject m_goItemPrefab;
	public GameObject m_goItemSelected = null;
	public GameObject m_goItemSelectWindow;
	public GameObject m_goUnitSelectWindow;

    //Sound byte for when you traverse the menu selections
    public AudioClip m_acMenuTraversal;
    //Sound byte for when you select a menu option
    public AudioClip m_acMenuSelection;

	//If a unit is selected for a formation swap
	int m_nCharacterSelectedForFormationSwap = -1;
	//Number of panels the script is waitng for to slide (used in the status screen and the formation screen.
	int m_nNumberOfPanelsToWaitFor = 0;

	//used for the inventory screen  : 0 - ALL, 1 - Equipment, 2 - Consumables, 3 - Key Items
	int m_nIterForItemType = 0;
	[HideInInspector]
	public ItemLibrary.CharactersItems m_iSelectedItem;

	// Use this for initialization
	void Start () 
	{
		dc = GameObject.Find("PersistantData").GetComponent<DCScript>();
		m_lParty = dc.GetParty();
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
				//Input while viewing the status screen
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
					if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
					{
						m_bWaiting = true;
						m_bFirstTimeFlag = true;
						m_nMenuState = (int)MENU_STATES.eSTATUS_SUBTAB;
						m_goStatus.SetActive(false);
						foreach(GameObject go in m_goCharacterPanels)
						{
							m_nNumberOfPanelsToWaitFor++;
							go.GetComponent<CharacterPanelScript>().ReturnToPosition(gameObject);
						}
					}
				}
			}
			break;
		case (int)MENU_STATES.eFORMATION_SUBTAB:
			{
				AdjustPartyPanels();
				if(m_bWaiting == false)
					FormationTabMenuInput();
			}
			break;
		case (int)MENU_STATES.eROSTER_SUBTAB:
			{
				if(m_bWaiting == false)
					RosterTabMenuInput();
			}
			break;
		case (int)MENU_STATES.eITEMTAB:
			{
				//arrow keys and mouse can select one of the sub tab options in menu, escape returns to top tab selection
				AdjustItemTab();
				if(m_bWaiting == false)
					ItemTabMenu_Input();
			}
			break;
		case (int)MENU_STATES.eCRAFTING_SUBTAB:
			{
				if(m_bWaiting == false)
					CraftingTabInput();
			}
			break;
		case (int)MENU_STATES.eINVENTORY_SUBTAB:
			{
				if (m_bWaiting == false) {
					if (Input.GetKeyDown (KeyCode.Escape) || Input.GetMouseButtonDown (1)) 
					{
						if (m_goUnitSelectWindow.activeSelf == true) {
							m_goItemSelectWindow.SetActive (true);
							m_goUnitSelectWindow.SetActive (false);
							return;
						}
						if (m_goItemSelectWindow.activeSelf == true) {
							m_goItemSelectWindow.SetActive (false);
							return;
						}
						m_goInventory.SetActive (false);
						m_nMenuState = (int)MENU_STATES.eITEMTAB;
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
		else if(Input.GetKeyDown(KeyCode.Return))
		{
			TopTabSelectionSelected(m_nTopTabMenuSelectionIndex);
		}
	}
	public void TopTabHighlighted(int nIndex)
	{
		if(m_nMenuState != (int)MENU_STATES.eTOPTAB_SELECTION)
			return;
        //If the selector has been changed, play a sound
        if(m_nTopTabMenuSelectionIndex != nIndex)
            GetComponent<AudioSource>().PlayOneShot(m_acMenuTraversal);
		m_nTopTabMenuSelectionIndex = nIndex;
	}
	public void TopTabSelectionSelected(int nIndex)
	{
		if(m_nMenuState != (int)MENU_STATES.eTOPTAB_SELECTION)
			return;
        GetComponent<AudioSource>().PlayOneShot(m_acMenuSelection);
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
		else if(Input.GetKeyDown(KeyCode.Return))
		{
            PartyTabSelectionSelected(m_nSubTabMenuSelectionIndex);
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
        //If we're changing the highlighted option
        if(m_nSubTabMenuSelectionIndex != nIndex)
            GetComponent<AudioSource>().PlayOneShot(m_acMenuTraversal);
        m_nSubTabMenuSelectionIndex = nIndex;
	}
	public void PartyTabSelectionSelected(int nIndex)
	{
		if(m_nMenuState != (int)MENU_STATES.ePARTYTAB)
			return;
        GetComponent<AudioSource>().PlayOneShot(m_acMenuSelection);
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
				if(RecursivePanelShiftRight(m_nCharacterPanelSelectionIndex) == true)
				{
					m_nCharacterPanelSelectionIndex = 0;
					PopulatePartyMembers();
					m_nMenuState = (int)MENU_STATES.eFORMATION_SUBTAB;
				}
			}
			break;
		case 2:
			{
				//ROSTER
				ClearRosterScreen();
				AdjustRosterScreen();
				m_goRoster.SetActive(true);
				m_nMenuState = (int)MENU_STATES.eROSTER_SUBTAB;
				m_goTopCharacterTabs.SetActive(false);
				foreach(GameObject go in m_goCharacterPanels)
					go.SetActive(false);
				//Remove all previous characters from the roster
				GameObject[] previousCharacters = GameObject.FindGameObjectsWithTag("UI_Battle_Selection");
				foreach(GameObject character in previousCharacters)
					Destroy(character);
				//TODO : Change this so that it goes of an entire roster, and not just the party
				foreach(DCScript.CharacterData character in dc.GetRoster())
				{
					if(character.m_bHasBeenRecruited == true)
					{
						GameObject characterInList = Instantiate(m_goCharacterPrefab);
						characterInList.transform.FindChild("CharacterName").GetComponent<Text>().text = character.m_szCharacterName;
						characterInList.transform.FindChild("CharacterLVL").GetComponent<Text>().text = character.m_nLevel.ToString();
						characterInList.transform.SetParent(m_goCharacterRoot.transform);
						characterInList.transform.localScale = new Vector3(1, 1, 1);
					}
				}
				
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
				m_nNumberOfPanelsToWaitFor++;
				go.GetComponent<CharacterPanelScript>().ReturnToPosition(gameObject);
			}
		}
		else if(Input.GetKeyDown(KeyCode.Return))
		{
			CharacterSelected(m_nCharacterPanelSelectionIndex);
		}
	}

	public void CharacterHighlighted(int nIndex)
	{
		if((m_nMenuState != (int)MENU_STATES.eSTATUS_SUBTAB && m_nMenuState != (int)MENU_STATES.eFORMATION_SUBTAB)|| m_bWaiting == true)
			return;
		m_nCharacterPanelSelectionIndex = nIndex;
		if(m_nMenuState != (int)MENU_STATES.eSTATUS_SUBTAB)
			if(RecursivePanelShiftLeft(nIndex) == true)
				m_nCharacterPanelSelectionIndex = nIndex;
		else if(m_nMenuState != (int)MENU_STATES.eFORMATION_SUBTAB)
			m_nCharacterPanelSelectionIndex = nIndex;
	}

	public void CharacterUnhighlighted(int nIndex)
	{
		
	}

	public void CharacterSelected(int nIndex)
	{
		if(m_bWaiting == true)
			return;
			//So we've selected a character...  let's see what we should be doing next!
		switch(m_nMenuState)
		{
		case (int)MENU_STATES.eSTATUS_SUBTAB:
			{
				DCScript.CharacterData character = RetrieveCharacter(nIndex);
				if(character == null)
					return;
				//Pause input so that the panels can do their slide event
				m_bWaiting = true;
				m_bFirstTimeFlag = true;
				//Move the state to viewing the status screen panel
				m_nMenuState = (int)MENU_STATES.eVIEWSTATUSSCREEN;
				m_nCharacterPanelSelectionIndex = nIndex;
				//Tell each of the panels to begin sliding
				foreach(GameObject go in m_goCharacterPanels)
				{
					m_nNumberOfPanelsToWaitFor++;
					go.GetComponent<CharacterPanelScript>().BeginSlide(gameObject, m_goCharacterPanels[0].GetComponent<RectTransform>().localPosition);
				}
			}
			break;
		case (int)MENU_STATES.eFORMATION_SUBTAB:
			{
				if(nIndex > 5)
					return;
				//if this is the first character to be selected, shade them and slightly slide them upward a little
				if(m_nCharacterSelectedForFormationSwap == -1 && m_nCharacterSelectedForFormationSwap != m_nCharacterPanelSelectionIndex)
				{
					m_nCharacterSelectedForFormationSwap = m_nCharacterPanelSelectionIndex;
				}
				//Special case to handle right mouse input cancelling the panel selection since Unity's POINTERCLICK event doesn't check left/right input differences
				else if(m_nCharacterSelectedForFormationSwap == -2)
					m_nCharacterSelectedForFormationSwap = -1;
				//if this is the second character to be selected, unshade the previous one, slide them both to each others previous positions, and make sure to change the formation of that character in dc
				else
				{
					//If you selected the same character, just unhook the first panel selected)
					if(m_nCharacterSelectedForFormationSwap == m_nCharacterPanelSelectionIndex)
						m_nCharacterSelectedForFormationSwap = -1;
					//Otherwise, it's time to do a formation swap!
					else
					{
						//Check to make sure that at least one of the panels has a character to swap to (no sense swapping two empty panels)
						bool timeToSwap = true;
						if(RetrieveCharacter(m_nCharacterPanelSelectionIndex) == null && RetrieveCharacter(m_nCharacterSelectedForFormationSwap) == null)
							timeToSwap = false;
						//If this is true, then we're still good to swap
						if(timeToSwap == true)
						{
							//Commands for sliding the panels to each others positions.
							m_goCharacterPanels[m_nCharacterPanelSelectionIndex].GetComponent<CharacterPanelScript>().BeginSlide(gameObject, m_goCharacterPanels[m_nCharacterSelectedForFormationSwap].GetComponent<CharacterPanelScript>().m_vOriginalPosition, true);
							m_goCharacterPanels[m_nCharacterSelectedForFormationSwap].GetComponent<CharacterPanelScript>().BeginSlide(gameObject, m_goCharacterPanels[m_nCharacterPanelSelectionIndex].GetComponent<CharacterPanelScript>().m_vOriginalPosition, true);
							m_nNumberOfPanelsToWaitFor = 2;
							m_bWaiting = true;
							m_bFirstTimeFlag = true;
							//Commands for changing the formation of the character's being swapped.
							DCScript.CharacterData firstChar = RetrieveCharacter(m_nCharacterPanelSelectionIndex);
							DCScript.CharacterData secChar = RetrieveCharacter(m_nCharacterSelectedForFormationSwap);
							if(firstChar != null)
								firstChar.m_nFormationIter = ConvertPanelIterToFormationNumber(m_nCharacterSelectedForFormationSwap);
							if(secChar != null)
								secChar.m_nFormationIter = ConvertPanelIterToFormationNumber(m_nCharacterPanelSelectionIndex);
							//finally, reset the formation swap iter since the swap has been made
							m_nCharacterSelectedForFormationSwap = -1;
						}
					}

				}
			}
			break;
		}
	}
	#endregion
	#region FormationMenu
	void FormationTabMenuInput()
	{
		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
		{
			//If we've selected a unit to formation swap with, just unhook that unit from being selected
			if(m_nCharacterSelectedForFormationSwap != -1)
			{
				m_nCharacterSelectedForFormationSwap = -2;
			}
			//Otherwise we're backing out of the formation menu
			else
			{
				//Move the state back up one tier
				m_nMenuState = (int)MENU_STATES.ePARTYTAB;
				//We're going to be doing a panel slide wait, incase there are any moving pieces.
				m_bWaiting = true;
				//Turn on the first time flag so that it handles the initial update loop after this call
				m_bFirstTimeFlag = true;
				//Reset the formation iter (slightly redundant as this SHOULD already be called earlier, but just incase)
				m_nCharacterSelectedForFormationSwap = -1;
				foreach(GameObject go in m_goCharacterPanels)
				{
					foreach(GameObject panel in m_goCharacterPanels)
					{
						//Brighten ALL of the panels because none of them should appear "Selected" if they come back to this screen.
						Image_Brighten(panel);
					}
					//Slide the panels back to their original positions incase there are any positional movements previously.
					m_nNumberOfPanelsToWaitFor++;
					go.GetComponent<CharacterPanelScript>().ReturnToPosition(gameObject);
				}
			}
		}
		else if(Input.GetKeyDown(KeyCode.Return))
		{
			CharacterSelected(m_nCharacterPanelSelectionIndex);
		}
	}
	#endregion
	#region RosterMenu
	void RosterTabMenuInput()
	{
		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(1))
		{
			AdjustRosterScreen();
			ClearRosterScreen();
			PopulatePartyMembers();
			m_goRoster.SetActive(false);
			m_goTopCharacterTabs.SetActive(true);
			foreach(GameObject go in m_goCharacterPanels)
				go.SetActive(true);
			m_nMenuState = (int)MENU_STATES.ePARTYTAB;
		}
	}
	#endregion

	#region ItemMenu
	void ItemTabMenu_Input()
	{
		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
		{
			m_nSubTabMenuSelectionIndex = 0;
			m_nMenuState = (int)MENU_STATES.eTOPTAB_SELECTION;
			foreach(GameObject go in m_goItemSubTabs)
				Image_Brighten(go);
		}
		else if(Input.GetKeyDown(KeyCode.Return))
		{
			
			ItemTabMenuSelected(m_nSubTabMenuSelectionIndex);
		}

	}
	public void ItemTabMenuSelected(int _nIndex)
	{
		switch(m_nSubTabMenuSelectionIndex)
		{
		//Crafting(for now)
		case 0:
			{
				AdjustCraftingScreen();
				DisplayCraftingPanels(true);
				m_nMenuState = (int) MENU_STATES.eCRAFTING_SUBTAB;
			}
			break;
			//Inventory
		case 1:
			{
				AdjustInventoryList ();
				DisplayInventoryPanels (true);
				m_nMenuState = (int)MENU_STATES.eINVENTORY_SUBTAB;

			}
			break;
		}
	}
	public void ItemTabHighlighted(int nIndex)
	{
		//if(m_nMenuState != (int)MENU_STATES.eITEMTAB)
			//return;
		m_nSubTabMenuSelectionIndex = nIndex;
	}
	void AdjustItemTab()
	{
		int counter = 0;
		foreach(GameObject go in m_goItemSubTabs)
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
	#endregion

	#region CraftingMenu
	void CraftingTabInput()
	{
		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
		{
			m_nMenuState = (int)MENU_STATES.eITEMTAB;
			DisplayCraftingPanels(false);
		}
	}
	#endregion

	#region InventoryMenu
	void InventoryTabInput()
	{
		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
		{
			m_nMenuState = (int)MENU_STATES.eITEMTAB;
			DisplayCraftingPanels(false);
		}
	}

	public void FilterInventory(int _nType)
	{
		m_nIterForItemType = _nType;
		ClearInventoryScreen ();
		AdjustInventoryList ();
	}

	public void UseItemOnCharacter(int _nFormationIter)
	{
		
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
				Sprite texture;
				if(pCont.GetComponent<PortraitContainerScript>().m_dPortraits.TryGetValue(character.m_szCharacterName + "1", out texture))
				{
					Texture2D _t2dTexture = TextureFromSprite(texture);
					cPort.GetComponent<Image>().sprite = Sprite.Create(_t2dTexture, 
						new Rect(0, 0, _t2dTexture.width, _t2dTexture.height), new Vector2(0.5f, 0.5f));
				}
				else
				{
					GameObject unit = Resources.Load<GameObject>("Units/Ally/" + character.m_szCharacterName + "/" + character.m_szCharacterName);
					Texture2D sprTex = unit.GetComponent<CAllyBattleScript>().TextureFromSprite(unit.GetComponent<CAllyBattleScript>().m_tLargeBust);
					cPort.GetComponent<Image>().sprite = Sprite.Create(sprTex, 
						new Rect(0, 0, sprTex.width, sprTex.height), new Vector2(0.5f, 0.5f));
				}
				//Check to see if this unit is afflicted with poison, if it is, show the icon for it.
				DCScript.StatusEffect _sePoison = dc.GetStatusEffect("Poison");
				bool _isEffected = false;
				if(_sePoison != null)
				{
					foreach(DCScript.StatusEffect.cEffectedMember _member in _sePoison.m_lEffectedMembers)
					{
						if(_member.m_szCharacterName == character.m_szCharacterName)
						{
							//This character is effected with poison, display the icon.
							_isEffected = true;
							Transform cPoison = panel.transform.FindChild("Poison_Icon");
							cPoison.GetComponent<Image>().enabled = true;
						}
					}
				}
				if(_isEffected == false)
				{
					Transform cPoison = panel.transform.FindChild("Poison_Icon");
					cPoison.GetComponent<Image>().enabled = false;
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
				Transform cPoison = panel.transform.FindChild("Poison_Icon");
				cPoison.GetComponent<Image>().enabled = false;
			}
			counter++;
		}
	}
	void AdjustPartyPanels()
	{
		if(m_bWaiting == false)
		{
			int counter = 0;
			foreach(GameObject panel in m_goCharacterPanels)
			{
				if(counter == m_nCharacterPanelSelectionIndex || counter == m_nCharacterSelectedForFormationSwap)
				{
					panel.GetComponent<Canvas>().sortingOrder = 1;
					Vector3 newPos = panel.GetComponent<CharacterPanelScript>().m_vOriginalPosition;;
					newPos.y += 20.0f;
					if(counter == m_nCharacterPanelSelectionIndex)
						panel.transform.localPosition = newPos;
					Image_Darken(panel);
				}
				else
				{
					panel.GetComponent<Canvas>().sortingOrder = -1;
					panel.transform.localPosition = panel.GetComponent<CharacterPanelScript>().m_vOriginalPosition;
					Image_Brighten(panel);
				}
				counter++;
			}
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
		Sprite texture;
		if(pCont.GetComponent<PortraitContainerScript>().m_dPortraits.TryGetValue(character.m_szCharacterName + "0", out texture))
		{
			Texture2D _t2dTexture = TextureFromSprite(texture);
			characterBody.GetComponent<Image>().sprite = Sprite.Create(_t2dTexture, 
				new Rect(0, 0, _t2dTexture.width, _t2dTexture.height), new Vector2(0.5f, 0.5f));
		}
		else
		{
			GameObject unit = Resources.Load<GameObject>("Units/Ally/" + character.m_szCharacterName + "/" + character.m_szCharacterName);
			Texture2D sprTex = unit.GetComponent<CAllyBattleScript>().TextureFromSprite(unit.GetComponent<CAllyBattleScript>().m_tLargeBust);
			characterBody.GetComponent<Image>().sprite = Sprite.Create(sprTex, 
				new Rect(0, 0, sprTex.width, sprTex.height), new Vector2(0.5f, 0.5f));
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
	void AdjustRosterScreen()
	{
		foreach(DCScript.CharacterData character in dc.GetParty())
		{
			m_goRosterCells[character.m_nFormationIter].GetComponent<RosterScreenCellScript>().InstantiateCharacter(character);
		}
	}
	void AdjustCraftingScreen()
	{
		List<ItemLibrary.CraftingItemData> _lCraftables = dc.m_lItemLibrary.m_lCraftableItems;
		CraftingItemScript[] oldCraftables = m_goItemCraftedRoot.GetComponentsInChildren<CraftingItemScript>();
		foreach(CraftingItemScript go in oldCraftables)
			Destroy(go.gameObject);
		foreach(ItemLibrary.CraftingItemData _item in _lCraftables)
		{
			GameObject craftedItem = Instantiate(m_goItemCraftedPrefab) as GameObject;
			craftedItem.GetComponent<RectTransform>().SetParent(m_goItemCraftedRoot.GetComponent<RectTransform>());
			craftedItem.GetComponent<RectTransform>().rotation = Quaternion.identity;
			craftedItem.GetComponent<CraftingItemScript>().Initialize(_item);

		}
	}

	void AdjustInventoryList ()
	{
		ClearInventoryScreen ();
		List<ItemLibrary.CharactersItems> _items = null;
		switch (m_nIterForItemType) {
		case 0:
			{
				//All
				_items = dc.m_lItemLibrary.GetItemsOfType (-1);
			}
			break;
		case 1:
			{
				//Equipment
				_items = dc.m_lItemLibrary.GetItemsOfType (1);
				_items.AddRange(dc.m_lItemLibrary.GetItemsOfType(2));
			}
			break;
		case 2:
			{
				//Consumables
				_items = dc.m_lItemLibrary.GetItemsOfType (0);
			}
			break;
		case 3:
			{
				//Junk
				_items = dc.m_lItemLibrary.GetItemsOfType (3);
			}
			break;
		case 4:
			{
				//Key Items
				_items = dc.m_lItemLibrary.GetItemsOfType (4);
			}
			break;
		}
		foreach (ItemLibrary.CharactersItems item in _items) {
			GameObject invItem = Instantiate (m_goInventoryItemPrefab) as GameObject;
			invItem.GetComponent<RectTransform> ().SetParent (m_goInventoryRoot.GetComponent<RectTransform> ());
			invItem.GetComponent<RectTransform> ().rotation = Quaternion.identity;
			invItem.GetComponent<ItemInInventoryScript> ().Initialize (item, gameObject);

		}

	}

	public void UpdateUnitsOnInventory()
	{
		foreach (GameObject go in m_goUnitInventoryCells)
			go.SetActive (false);
		foreach (DCScript.CharacterData character in dc.GetParty()) 
		{
			int _iter = ConvertPanelIterToFormationNumber(character.m_nFormationIter);
			m_goUnitInventoryCells [_iter].SetActive (true);
			GameObject _unit = m_goUnitInventoryCells [_iter];
		 	GameObject unit = Resources.Load<GameObject>("Units/Ally/" + character.m_szCharacterName + "/" + character.m_szCharacterName);
		 	Texture2D sprTex = unit.GetComponent<CAllyBattleScript>().TextureFromSprite(unit.GetComponent<CAllyBattleScript>().m_tLargeBust);
			Transform _root = _unit.transform.FindChild ("Background");
			_root.FindChild("Icon").GetComponent<Image>().sprite = Sprite.Create(sprTex, 
						new Rect(0, 0, sprTex.width, sprTex.height), new Vector2(0.5f, 0.5f));
			Color _col = _root.FindChild("Icon").GetComponent<Image> ().color;
			_col.a = 0.5f;
			_root.FindChild("Icon").GetComponent<Image> ().color = _col;
			_root.FindChild ("CharacterName").GetComponentInChildren<Text> ().text = character.m_szCharacterName;
			_root.FindChild ("HP").GetComponentInChildren<Text> ().text = character.m_nCurHP + " / " + character.m_nMaxHP;
			_root.FindChild ("MP").GetComponentInChildren<Text> ().text = character.m_nCurMP + " / " + character.m_nMaxMP;
		}
	}

	public void DiscardItem()
	{
		m_goItemSelectWindow.SetActive (false);
		dc.m_lItemLibrary.RemoveItemAll (m_iSelectedItem);
		ClearInventoryScreen ();
		AdjustInventoryList ();
	}


	void ClearInventoryScreen()
	{
		foreach (Transform child in m_goInventoryRoot.transform) {
			Destroy (child.gameObject);
		}

	}

	void ClearRosterScreen()
	{
		foreach(GameObject go in m_goRosterCells)
		{
			if(go.transform.childCount > 0)
			{
				go.GetComponent<RosterScreenCellScript>().Remove();
			}
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
		int nFormationOfCharacter = ConvertPanelIterToFormationNumber(nPanelIndex);

		if(nFormationOfCharacter != -1)
		{
			foreach(DCScript.CharacterData character in m_lParty)
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
	//Used for call from PointerOnEnter override in the tab script
	public void TabHighlighted(MenuScreenScript.MENU_STATES _state)
	{
		switch(_state)
		{
		case MENU_STATES.ePARTYTAB:
			{
				TopTabHighlighted(0);
			}
			break;
		case MENU_STATES.eSTATUS_SUBTAB:
			{
				PartyTabHighlighted(0);
			}
			break;
		case MENU_STATES.eFORMATION_SUBTAB:
			{
				PartyTabHighlighted(1);
			}
			break;
		case MENU_STATES.eROSTER_SUBTAB:
			{
				PartyTabHighlighted(2);
			}
			break;
		case MENU_STATES.eITEMTAB:
			{
				TopTabHighlighted(1);
			}
			break;
		case MENU_STATES.eCRAFTING_SUBTAB:
			{
				ItemTabHighlighted(0);
			}
			break;
		case MENU_STATES.eINVENTORY_SUBTAB:
			{
				ItemTabHighlighted (1);
			}
			break;
		case MENU_STATES.eEQUIPMENT_SUBTAB:
			{
				ItemTabHighlighted (2);
			}
			break;

		}
	}
	//Used for call from PointerOnClick override in the tab script
	public void TabSelected(MenuScreenScript.MENU_STATES _state)
	{
		switch(_state)
		{
		case MENU_STATES.ePARTYTAB:
			{
				TopTabSelectionSelected(0);
			}
			break;
		case MENU_STATES.eSTATUS_SUBTAB:
			{
				PartyTabSelectionSelected(0);
			}
			break;
		case MENU_STATES.eFORMATION_SUBTAB:
			{
				PartyTabSelectionSelected(1);
			}
			break;
		case MENU_STATES.eROSTER_SUBTAB:
			{
				PartyTabSelectionSelected(2);
			}
			break;
		case MENU_STATES.eITEMTAB:
			{
				//TopTabHighlighted(1);
                TopTabSelectionSelected(1);
			}
			break;
		case MENU_STATES.eCRAFTING_SUBTAB:
			{
				ItemTabMenuSelected(0);
			}
			break;
		case MENU_STATES.eINVENTORY_SUBTAB:
			{
				ItemTabMenuSelected (1);
			}
			break;
		case MENU_STATES.eEQUIPMENT_SUBTAB:
			{
				ItemTabMenuSelected (2);
			}
			break;

		}
	}



	int ConvertPanelIterToFormationNumber(int nPanelIndex)
	{
		//convert the index of panel selected, to the formation of that party member.
		switch(nPanelIndex)
		{
		case 0:
			{
				//top left
				return 3;
			}
		case 1:
			{
				//mid left
				return 4;
			}
		case 2:
			{
				//bottom left
				return 5;
			}
		case 3:
			{
				//top right
				return 0;
			}
		case 4:
			{
				//mid right
				return 1;
			}
		case 5:
			{
				//bottom right
				return 2;
			}
		case 6:
			{
				//The support character
				return -1;
			}
			default:
			{
				//error catching
				return -2;
			}
		}
	}

	public void Image_Brighten(GameObject image)
	{
		image.GetComponent<Image>().color = new Color(255, 255, 255);
	}
	public void Image_Darken(GameObject image)
	{
		image.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
	}
	public void DisplayCraftingPanels(bool bFlag)
	{
		m_goCraftingPanel.SetActive(bFlag);
	}
	public void DisplayInventoryPanels(bool bFlag)
	{
		m_goInventory.SetActive (bFlag);
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
		if(m_nPanelsThatHaveFinishedSliding == m_nNumberOfPanelsToWaitFor)
		{
			m_nNumberOfPanelsToWaitFor = 0;
			m_nPanelsThatHaveFinishedSliding = 0;
			m_bWaiting = false;
			PopulatePartyMembers();
		}
	}
	Texture2D TextureFromSprite(Sprite sprite)
	{
		if(sprite.rect.width != sprite.texture.width)
		{
			Texture2D newText = new Texture2D((int)sprite.rect.width,(int)sprite.rect.height);
			Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
				(int)sprite.textureRect.y,
				(int)sprite.textureRect.width,
				(int)sprite.textureRect.height);
			newText.SetPixels(newColors);
			newText.Apply();
			return newText;
		}
		else
			return sprite.texture;
	}
	#endregion
}
