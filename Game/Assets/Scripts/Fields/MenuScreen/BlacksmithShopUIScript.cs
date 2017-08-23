using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlacksmithShopUIScript : MonoBehaviour 
{
	public GameObject m_goCanvas;
	public GameObject m_goModifierInList;
	public GameObject m_goConfirmationWindow;
	public GameObject m_goOptionSelectWindow;
	public GameObject m_goCharacterList;
	public GameObject m_goModifierList;
	public GameObject m_goContentsInList;
	public GameObject m_goModifierDescriptionWindow;
	public GameObject m_goSpyrTotal;
	public GameObject m_goMainMenu;
	public GameObject[] m_goCharacterSlots;

	//Not putting the confirmation window as that would then hide whether or not this is an enhance/modify selection (so just always assume we need to turn off the confirm window if going backwards ;)  )
	enum eActiveWindow {eMainMenu, eEnhanceWindow, eModifyWindow, eModifierListWindow, eDisabled}
	eActiveWindow m_eActiveWindow;

	GameObject m_goSender;
	int m_nCharacterSelectIter = 0;
	DCScript.cModifier m_mModSelected = null;
	int m_nModifierIter = 0;
	DCScript dc;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void TurnOn(GameObject _sender)
	{
		m_goSender = _sender;
		m_eActiveWindow = eActiveWindow.eMainMenu;
		m_goOptionSelectWindow.SetActive (true);
		m_goCanvas.SetActive (true);
		m_goSpyrTotal.SetActive (true);
		dc = GameObject.Find ("PersistantData").GetComponent<DCScript> ();
		m_goSpyrTotal.GetComponentInChildren<Text> ().text = dc.m_nGold.ToString();
		m_goMainMenu.SetActive (false);
	}

	void PurchaseWeaponEnhancement(int _nCharacterIter)
	{
		int sum = m_goSender.GetComponent<NPC_BlacksmithScript>().CalculateCost((dc.GetParty()[_nCharacterIter].m_nWeaponLevel));
		if(sum != -1 && sum <= dc.m_nGold && dc.GetParty()[_nCharacterIter].m_nWeaponLevel < m_goSender.GetComponent<NPC_BlacksmithScript>().m_nMaxEnhancementLevel)
		{
			dc.m_nGold -= sum;
			foreach(DCScript.WeaponData weapon in dc.GetWeaponList())
			{
				if(weapon.m_szOwnerName == dc.GetParty()[_nCharacterIter].m_szCharacterName)
				{
					dc.GetParty()[_nCharacterIter].m_nWeaponLevel++;
					foreach(DCScript.LevelingWeapon wpn in weapon.m_lLevels)
					{
						if(dc.GetParty()[_nCharacterIter].m_nLevel == wpn.m_nLevel)
						{
							dc.GetParty()[_nCharacterIter].m_szWeaponName = wpn.m_szWeaponName;
							dc.GetParty()[_nCharacterIter].m_nWeaponDamageModifier  = wpn.m_nDamage;
						}
					}
				}
			}
		}
		m_goSpyrTotal.GetComponentInChildren<Text> ().text = dc.m_nGold.ToString();
	}

	void PurchaseWeaponModification(int _nModifierIter, int _nCharacterIter)
	{
		//purchase has been confirmed, do it!
		int sum = dc.GetModifierList()[_nModifierIter].m_nModCost;
		if(sum != -1 && sum <= dc.m_nGold && dc.GetParty()[_nCharacterIter].m_szWeaponModifierName != dc.GetModifierList()[_nModifierIter].m_szModifierName)
		{
			dc.m_nGold -= sum;
			dc.GetParty () [_nCharacterIter].m_szWeaponModifierName = m_mModSelected.m_szModifierName;
		}
		m_goSpyrTotal.GetComponentInChildren<Text> ().text = dc.m_nGold.ToString();
	}

	public void YesSelected()
	{
		if (m_eActiveWindow == eActiveWindow.eEnhanceWindow)
		{
			//Purchase an upgrade for the character that was selected.  (if the player can afford it)
			PurchaseWeaponEnhancement(m_nCharacterSelectIter);
		}
		else if (m_eActiveWindow == eActiveWindow.eModifierListWindow)
		{
			//Purchase the selected modifier to the selected character. (if the player can afford it)
			PurchaseWeaponModification(m_nModifierIter , m_nCharacterSelectIter);
		}
		UpdateCharacterRoster ();
		ToggleWindows ();
	}

	public void NoSelected()
	{
		if (m_eActiveWindow == eActiveWindow.eEnhanceWindow)
		{
			//back out of the confirmation window.
			ToggleWindows();
		}
		else if (m_eActiveWindow == eActiveWindow.eModifierListWindow)
		{
			//just disable the confirmation window
			ToggleWindows ();
		}
	}

	public void EnhanceSelected()
	{
		m_eActiveWindow = eActiveWindow.eEnhanceWindow;
		UpdateCharacterRoster ();
		ToggleWindows ();
	}

	void UpdateCharacterRoster()
	{
		int _cntr = 0;
		foreach (GameObject _go in m_goCharacterSlots)
			_go.SetActive (false);
		foreach (DCScript.CharacterData _character in dc.GetParty())
		{
			m_goCharacterSlots [_cntr].SetActive (true);
			m_goCharacterSlots [_cntr].transform.Find ("Name").GetComponentInChildren<Text> ().text = _character.m_szCharacterName;
			m_goCharacterSlots [_cntr].transform.Find ("Weapon").GetComponentInChildren<Text> ().text = _character.m_szWeaponName;
			m_goCharacterSlots [_cntr].transform.Find ("Weapon Level").GetComponentInChildren<Text> ().text = _character.m_nWeaponLevel.ToString();
			m_goCharacterSlots [_cntr].transform.Find ("Weapon Modifier").GetComponentInChildren<Text> ().text = _character.m_szWeaponModifierName;
			_cntr += 1;
			Debug.Log (_cntr);
		}
	}

	public void ModifySelected()
	{
		m_eActiveWindow = eActiveWindow.eModifyWindow;
		UpdateCharacterRoster ();
		ToggleWindows ();
	}

	public void ExitSelected()
	{
		m_goCanvas.SetActive (false);
		m_goSpyrTotal.SetActive (false);
		m_eActiveWindow = eActiveWindow.eDisabled;
		ToggleWindows ();
		m_goSender.GetComponent<NPC_BlacksmithScript> ().CloseShop ();
		m_goSender = null;
	}

	public void CharacterSelected(int _nCharacterIndex)
	{
		DCScript.CharacterData _character = dc.GetCharacter (m_goCharacterSlots [_nCharacterIndex-1].transform.Find ("Name").GetComponent<Text> ().text);
		if (_character == null )
			return;
		if (_character.m_nWeaponLevel + 1 > m_goSender.GetComponent<NPC_BlacksmithScript> ().m_nMaxEnhancementLevel)
		{
			//Character cannot improve their weapon any further with this blacksmith.
			return;
		}
		m_nCharacterSelectIter = _nCharacterIndex - 1;
		if (m_eActiveWindow == eActiveWindow.eEnhanceWindow)
		{
			//set up the confirm purchase window
			m_goConfirmationWindow.SetActive(true);
			m_goConfirmationWindow.transform.Find("Confirmation").GetComponent<Text>().text = "This will cost " + m_goSender.GetComponent<NPC_BlacksmithScript>().CalculateCost(_character.m_nWeaponLevel) + " Spyr, are you sure you want to make this purchase?";

		}
		else if (m_eActiveWindow == eActiveWindow.eModifyWindow)
		{
			//Activate the window to show the possible modifiers to select for this character's weapons.
			m_eActiveWindow = eActiveWindow.eModifierListWindow;
			List<DCScript.cModifier> _mods = dc.GetModifierList ();
			foreach (Transform t in m_goContentsInList.transform)
			{
				Destroy (t.gameObject);
			}
			foreach (DCScript.cModifier mod in _mods)
			{
				GameObject modInList = Instantiate(m_goModifierInList);
				modInList.GetComponent<ModifierInListScript> ().m_goParent = gameObject;
				modInList.GetComponent<ModifierInListScript> ().m_nModifier = mod;
				modInList.GetComponent<ModifierInListScript> ().m_goDescriptionWindow = m_goModifierDescriptionWindow;
				modInList.GetComponentInChildren<Text> ().text = mod.m_szModifierName;
				m_goModifierDescriptionWindow.GetComponentInChildren<Text> ().text = mod.m_szModifierDesc;
				modInList.transform.SetParent(m_goContentsInList.transform);
				modInList.transform.localScale = new Vector3(1, 1, 1);
			}
			ToggleWindows ();
		}
	}

	public void ModifierSelected(DCScript.cModifier _nMod)
	{
		//Show the confirmation window, keep track of the modification selected.
		if (_nMod.m_szModifierName == dc.GetParty () [m_nCharacterSelectIter].m_szWeaponModifierName)
			return;
		m_mModSelected = _nMod;
		m_goConfirmationWindow.SetActive(true);
		m_goConfirmationWindow.transform.Find("Confirmation").GetComponent<Text>().text = "This modification will cost " + m_mModSelected.m_nModCost + " Spyr, are you sure you want to make this modification?";
	}

	void ToggleWindows()
	{
		switch (m_eActiveWindow)
		{
			case eActiveWindow.eMainMenu:
				{
					//Just the "Enhance, Modify, Exit" options should be available.
					m_goOptionSelectWindow.SetActive(true);
					m_goConfirmationWindow.SetActive(false);
					m_goCharacterList.SetActive(false);
					m_goModifierList.SetActive(false);

					m_goModifierDescriptionWindow.SetActive(false);
				}
				break;
			case eActiveWindow.eEnhanceWindow:
				{
					//Just the main option window and the character select window should be open.
					m_goOptionSelectWindow.SetActive(true);
					m_goConfirmationWindow.SetActive(false);
					m_goCharacterList.SetActive(true);
					m_goModifierList.SetActive(false);
					m_goModifierDescriptionWindow.SetActive(false);
				}
				break;
			case eActiveWindow.eModifyWindow:
				{
					//A character hasn't been selected yet.  just show the main option window and the character select window
					m_goOptionSelectWindow.SetActive(true);
					m_goConfirmationWindow.SetActive(false);
					m_goCharacterList.SetActive(true);
					m_goModifierList.SetActive(false);
					m_goModifierDescriptionWindow.SetActive(false);

				}
				break;
			case eActiveWindow.eModifierListWindow:
				{
					//A character has been selected for weapon modification, show everthing but the confirmation window.
					m_goOptionSelectWindow.SetActive(true);
					m_goConfirmationWindow.SetActive(false);
					m_goCharacterList.SetActive(true);
					m_goModifierList.SetActive(true);
					m_goContentsInList.SetActive (true);
					m_goModifierDescriptionWindow.SetActive(true);
				}
				break;
			case eActiveWindow.eDisabled:
				{
					//Completely disabled, just turn off each of the windows.
					m_goOptionSelectWindow.SetActive(false);
					m_goConfirmationWindow.SetActive(false);
					m_goCharacterList.SetActive(false);
					m_goModifierList.SetActive(false);
					m_goModifierDescriptionWindow.SetActive(false);
				}
				break;
		}
	}

}
