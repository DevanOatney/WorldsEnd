using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ItemsAndSpellsContainer : MonoBehaviour 
{
	[HideInInspector]
	public GameObject m_goItemAndSpellSelector;
	[HideInInspector]
	public GameObject m_goItemAndSpellDescriptor;
	[HideInInspector]
	public GameObject m_dcDataCanister;
	public GameObject m_goSelection;
	public GameObject m_goSelectionRoot;
	public Sprite[] m_goSpriteIcons;

	List<cData> m_lElementList = new List<cData>();
	public class cData{public string m_szName;public int m_nAmount;public int m_nIconType;}
	//0 - Items, 1 - Magic, ----(could be more, like abilities/skills/etc)
	int m_nKey = 0;
	//Character data of current character (for spells lists)
	DCScript.CharacterData m_cCurrentCharacter = null;
	//How many items/spells can be viewed at once in the scroll view.
	int m_nElementCount = 0;
	// Use this for initialization
	void Start () 
	{
		m_goItemAndSpellSelector = GameObject.Find("ItemAndSpellSelector");
		m_goItemAndSpellDescriptor = GameObject.Find("ItemAndSpellDescriptor");
		m_dcDataCanister = GameObject.Find("PersistantData");
		m_goItemAndSpellSelector.SetActive(false);
		m_goItemAndSpellDescriptor.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public void SwitchContainerTypeTo(DCScript.CharacterData cCharacter, int nKey)
	{
		m_cCurrentCharacter = cCharacter;
		m_nKey = nKey;
		UpdateContainer();
	}
	void UpdateContainer()
	{
		m_goItemAndSpellSelector.SetActive(true);
		m_goItemAndSpellDescriptor.SetActive(true);
		GameObject[] previousItems = GameObject.FindGameObjectsWithTag("UI_Battle_Selection");
		m_lElementList.Clear();
		//How many items are in the selection.
		m_nElementCount = 0;
		foreach(GameObject go in previousItems)
			Destroy(go);
		switch(m_nKey)
		{
		case 0:
			{
				//Items - Grab all of the useable items and how many there are of them and set those values.
				List<ItemLibrary.CharactersItems> lInventory = m_dcDataCanister.GetComponent<DCScript>().m_lItemLibrary.GetItemsOfType(0);
				Debug.Log(lInventory.Count);
				Button prevButton = null;
				foreach(ItemLibrary.CharactersItems item in lInventory)
				{
					//Create a new button and initialize it's data
					GameObject newCell = Instantiate(m_goSelection);
					newCell.transform.FindChild("Selection Name").GetComponent<Text>().text = item.m_szItemName;
					newCell.transform.FindChild("Selection Count").GetComponent<Text>().text = item.m_nItemCount.ToString();
					//Hardcoded because I can't think of a time when an items icon would be different, since they're all just useable items (maybe by item type eventually when we have more artists?)
					newCell.transform.FindChild("Icon").GetComponent<Image>().sprite = m_goSpriteIcons[0];
					newCell.GetComponent<IntContainer>().m_nInteger = m_nElementCount;
					newCell.GetComponent<IntContainer>().m_twTurnWatcher = gameObject;
					newCell.transform.SetParent(m_goSelectionRoot.transform);
					newCell.transform.localScale = new Vector3(1, 1, 1);
					//Set up button Navigation
					if(prevButton == null)
					{
						newCell.GetComponent<Button>().Select();
						prevButton = GetComponent<Button>();
					}
					else
					{
						prevButton = GetComponent<Button>();
					}

					//Create a new data member for the container of elements for later iteration access.
					cData newData = new cData();
					newData.m_szName = item.m_szItemName;
					newData.m_nAmount = item.m_nItemCount;
					//Hardcoded because I can't think of a time when an items icon would be different, since they're all just useable items (maybe by item type eventually when we have more artists?)
					newData.m_nIconType = 0;
					m_lElementList.Add(newData);

					//increment amount of elements total.. though we could just use the count of the list of elements.. but... idk maybe that will not be a thing later?
					m_nElementCount++;


				}
			}
			break;
		case 1:
			{
				//Spells - Grab all of the spells this character knows and their mp costs.
				foreach(string spell in m_cCurrentCharacter.m_lSpellsKnown)
				{
				}
			}
			break;
		}
		//initialize the description box to the first thing in the list.
		if(m_nElementCount > 0)
		{
			SelectionChanged(0);
		}
	}
	public void SelectionSelected(int nIndex)
	{
		switch(m_nKey)
		{
		case 0:
			{
				GameObject.Find(m_cCurrentCharacter.m_szCharacterName).GetComponent<CAllyBattleScript>().ItemToUseSelected(m_lElementList[nIndex].m_szName);
				m_goItemAndSpellSelector.SetActive(false);
				m_goItemAndSpellDescriptor.SetActive(false);
			}
			break;
		case 1:
			{
			}
			break;
		}

	}
	public void SelectionChanged(int nIndex)
	{
		switch(m_nKey)
		{
		case 0:
			{
				//Item
				Transform EleIcon = m_goItemAndSpellDescriptor.transform.FindChild("Element Icon");
				EleIcon.GetComponent<Image>().sprite = m_goSpriteIcons[m_lElementList[nIndex].m_nIconType];
				//Figure out which icon to change this to, should probably do something like this for the button as well.
				Transform EleName = m_goItemAndSpellDescriptor.transform.FindChild("Element Name");
				EleName.GetComponent<Text>().text = m_lElementList[nIndex].m_szName;
				//This is either the amount of items, or the cost of the spell (so for spells compare m_nAmount with the amount the current characters mp.  If it's too much, we need to grey this selection out.
				Transform EleCount = m_goItemAndSpellDescriptor.transform.FindChild("Element Count");
				EleCount.GetComponent<Text>().text = m_lElementList[nIndex].m_nAmount.ToString();
				//Single Enemy, All Enemy, Single Ally, All Ally
				Transform targetType = m_goItemAndSpellDescriptor.transform.FindChild("Target Type");
				ItemLibrary.ItemData item = m_dcDataCanister.GetComponent<DCScript>().m_lItemLibrary.GetItemFromDictionary(m_lElementList[nIndex].m_szName);
				//So this is the description of what the item is going to do.
				Transform EleDesc = m_goItemAndSpellDescriptor.transform.FindChild("Element Description");
				switch(item.m_nItemType)
				{
				case (int)BaseItemScript.ITEM_TYPES.eSINGLE_DAMAGE:
					{
						targetType.GetComponent<Text>().text = "Single Enemy Target";
						EleDesc.GetComponent<Text>().text = "Damages a single enemy unit for " + item.m_nHPMod + " damage.";
					}
					break;
				case (int)BaseItemScript.ITEM_TYPES.eGROUP_DAMAGE:
					{
						targetType.GetComponent<Text>().text = "Group Enemy Target";
						EleDesc.GetComponent<Text>().text = "Damages all enemy units for " + item.m_nHPMod + " damage.";
					}
					break;
				case (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL:
					{
						targetType.GetComponent<Text>().text = "Single Ally Target";
						EleDesc.GetComponent<Text>().text = "Heals a single ally unit for " + item.m_nHPMod + " health.";
					}
					break;
				case (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL:
					{
						targetType.GetComponent<Text>().text = "Group Ally Target";
						EleDesc.GetComponent<Text>().text = "Heals all ally units for " + item.m_nHPMod + " health.";
					}
					break;
				}
				//So this is for if there's flavor text.
				Transform EleFlavor = m_goItemAndSpellDescriptor.transform.FindChild("Element Flavor");
				EleFlavor.GetComponent<Text>().text = item.m_szDescription;

			}
			break;
		case 1:
			{
				//Magic
			}
			break;
		}

	}

}
