using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ItemInEquipmentList : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	ItemLibrary.ArmorData m_iArmorData;
	GameObject m_goFieldUI;

	public void Initialize(ItemLibrary.ArmorData _armor, GameObject _fieldUI)
	{
		m_iArmorData = _armor;
		m_goFieldUI = _fieldUI;
		GetComponentInChildren<Text> ().text = m_iArmorData.m_szItemName;

	}

	void UpdateModuleWindow()
	{
		Transform _tModule = m_goFieldUI.GetComponent<MenuScreenScript> ().m_goEquipmentItemDescModuleWindow.transform;
		_tModule.FindChild ("ItemName").GetComponent<Text> ().text = m_iArmorData.m_szItemName;
		_tModule.FindChild ("ItemDescription").GetComponent<Text> ().text = m_iArmorData.m_szDescription;
		Transform _tStatParent = _tModule.FindChild ("ItemStats");
		_tStatParent.FindChild ("HP").GetComponent<Text> ().text = "HP: " + m_iArmorData.m_nHPMod;
		_tStatParent.FindChild ("MP").GetComponent<Text> ().text = "MP: " + m_iArmorData.m_nMPMod;
		_tStatParent.FindChild ("POW").GetComponent<Text> ().text = "POW: " + m_iArmorData.m_nPowMod;
		_tStatParent.FindChild ("DEF").GetComponent<Text> ().text = "DEF: " + m_iArmorData.m_nDefMod;
		_tStatParent.FindChild ("SPD").GetComponent<Text> ().text = "SPD: " + m_iArmorData.m_nSpdMod;
		_tStatParent.FindChild ("EVA").GetComponent<Text> ().text = "EVA: " + m_iArmorData.m_nEvaMod;
		_tStatParent.FindChild ("HIT").GetComponent<Text> ().text = "HIT: " + m_iArmorData.m_nHitMod;
	}

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		throw new System.NotImplementedException ();
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
