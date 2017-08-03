using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ItemInEquipmentList : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	ItemLibrary.ArmorData m_iArmorData;
	GameObject m_goFieldUI;
	int m_nTrinketSlotNum = -1;

	public void Initialize(ItemLibrary.ArmorData _armor,int _trinketSlotNum, GameObject _fieldUI)
	{
		m_iArmorData = _armor;
		m_nTrinketSlotNum = _trinketSlotNum;
		m_goFieldUI = _fieldUI;
		GetComponentInChildren<Text> ().text = m_iArmorData.m_szItemName;

	}

	void UpdateModuleWindow()
	{
		Transform _tModule = m_goFieldUI.GetComponent<MenuScreenScript> ().m_goEquipmentItemDescModuleWindow.transform;
		_tModule.Find ("ItemName").GetComponent<Text> ().text = m_iArmorData.m_szItemName;
		_tModule.Find ("ItemDescription").GetComponent<Text> ().text = m_iArmorData.m_szDescription;
		Transform _tStatParent = _tModule.Find ("ItemStats");
		_tStatParent.Find ("HP").GetComponent<Text> ().text = "HP: " + m_iArmorData.m_nHPMod;
		_tStatParent.Find ("MP").GetComponent<Text> ().text = "MP: " + m_iArmorData.m_nMPMod;
		_tStatParent.Find ("POW").GetComponent<Text> ().text = "POW: " + m_iArmorData.m_nPowMod;
		_tStatParent.Find ("DEF").GetComponent<Text> ().text = "DEF: " + m_iArmorData.m_nDefMod;
		_tStatParent.Find ("SPD").GetComponent<Text> ().text = "SPD: " + m_iArmorData.m_nSpdMod;
		_tStatParent.Find ("EVA").GetComponent<Text> ().text = "EVA: " + m_iArmorData.m_nEvaMod;
		_tStatParent.Find ("HIT").GetComponent<Text> ().text = "HIT: " + m_iArmorData.m_nHitMod;
	}

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		
		ItemLibrary.ArmorData _oldArmor = m_goFieldUI.GetComponent<MenuScreenScript> ().m_cSelectedCharacter.EquipArmor (m_iArmorData, m_nTrinketSlotNum);
		if (_oldArmor != null) {
			GameObject.Find ("PersistantData").GetComponent<DCScript> ().m_lItemLibrary.RemoveItem (m_iArmorData.m_szItemName, 1);
			GameObject.Find ("PersistantData").GetComponent<DCScript> ().m_lItemLibrary.AddItem (_oldArmor.m_szItemName);
		}
		m_goFieldUI.GetComponent<MenuScreenScript> ().UpdateEquipmentScreen (m_goFieldUI.GetComponent<MenuScreenScript> ().m_cSelectedCharacter);
		m_goFieldUI.GetComponent<MenuScreenScript> ().m_goEquipmentListRoot.SetActive (false);
		m_goFieldUI.GetComponent<MenuScreenScript> ().m_goEquipmentItemDescModuleWindow.SetActive (false);
		m_goFieldUI.GetComponent<MenuScreenScript> ().m_nEquipmentScreenIter = 1;
	}

	#endregion

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		UpdateModuleWindow ();
		m_goFieldUI.GetComponent<MenuScreenScript> ().m_goEquipmentItemDescModuleWindow.SetActive (true);
	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
		m_goFieldUI.GetComponent<MenuScreenScript> ().m_goEquipmentItemDescModuleWindow.SetActive (false);
	}

	#endregion
}
