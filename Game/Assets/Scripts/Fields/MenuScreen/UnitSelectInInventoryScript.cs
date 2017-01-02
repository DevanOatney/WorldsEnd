using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UnitSelectInInventoryScript : MonoBehaviour, IPointerClickHandler {
	public GameObject m_goFIELDUI;
	public int m_nFormationIter;
	public bool m_bIsEquipmentButtons = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		if (m_goFIELDUI.GetComponent<MenuScreenScript> ().m_nMenuState == (int)MenuScreenScript.MENU_STATES.eITEMTAB)
			m_goFIELDUI.GetComponent<MenuScreenScript> ().UseItemOnCharacter (m_nFormationIter);
		else
		if (m_goFIELDUI.GetComponent<MenuScreenScript> ().m_nMenuState == (int)MenuScreenScript.MENU_STATES.eEQUIPMENT_SUBTAB) {
			if (m_goFIELDUI.GetComponent<MenuScreenScript> ().m_nEquipmentScreenIter == 0) {
				m_goFIELDUI.GetComponent<MenuScreenScript> ().AdjustEquipmentScreenCharacter (m_nFormationIter);
			}
		}
		else
		if (m_goFIELDUI.GetComponent<MenuScreenScript> ().m_nMenuState == (int)MenuScreenScript.MENU_STATES.eMAGICTAB) {
			m_goFIELDUI.GetComponent<MenuScreenScript> ().DisplayMagicPanel (m_nFormationIter);
		}
	}

	#endregion
}
