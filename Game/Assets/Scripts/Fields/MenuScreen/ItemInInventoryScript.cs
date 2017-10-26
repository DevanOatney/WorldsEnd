using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ItemInInventoryScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler {

	ItemLibrary.CharactersItems m_iItem;
	GameObject m_goFIELDUI;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goItemSelectWindow.activeSelf == true || m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goUnitSelectWindow.activeSelf == true) {
			GetComponent<Button> ().interactable = false;
		}
		else
			GetComponent<Button> ().interactable = true;

	}

	public void Initialize(ItemLibrary.CharactersItems _iItem, GameObject _goFIELDUI)
	{
		m_iItem = _iItem;
		m_goFIELDUI = _goFIELDUI;
		transform.Find ("ItemName").GetComponentInChildren<Text> ().text = m_iItem.m_szItemName;
		int nItemType = m_iItem.m_nItemType;
		string szType = "";
		//1-4 : useable item, 5 : weapon, 6: armor, 7: junk, 8- Key Items
		if (nItemType <= 4)
			szType = "Consumable";
		else
			if (nItemType >= 4 && nItemType <= 10)
				szType = "Equipment";
			else
				if (nItemType == 11)
					szType = "Junk";
				else
					if (nItemType == 12)
						szType = "Key Item";
		transform.Find ("ItemType").GetComponentInChildren<Text> ().text = szType;
		transform.Find ("ItemCount").GetComponentInChildren<Text> ().text = "x" + m_iItem.m_nItemCount.ToString ();
	}

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left) {
			if (m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goItemSelectWindow.activeSelf == false && m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goUnitSelectWindow.activeSelf == false) {
				m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goItemSelectWindow.SetActive (true);
				m_goFIELDUI.GetComponent<MenuScreenScript> ().m_iSelectedItem = m_iItem;
				if (m_iItem.m_nItemType == (int)BaseItemScript.ITEM_TYPES.eKEYITEM) {
					m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goItemSelectWindow.transform.Find ("Use_Button").GetComponent<Button> ().interactable = false;
					m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goItemSelectWindow.transform.Find ("Discard_Button").GetComponent<Button> ().interactable = false;
				}
				else
				if (m_iItem.m_nItemType != (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL && m_iItem.m_nItemType != (int)BaseItemScript.ITEM_TYPES.eGROUP_HEAL) {
						m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goItemSelectWindow.transform.Find ("Use_Button").GetComponent<Button> ().interactable = false;
						m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goItemSelectWindow.transform.Find ("Discard_Button").GetComponent<Button> ().interactable = true;
				}
				else {
						m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goItemSelectWindow.transform.Find ("Use_Button").GetComponent<Button> ().interactable = true;
						m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goItemSelectWindow.transform.Find ("Discard_Button").GetComponent<Button> ().interactable = true;
				}
				Vector3 newPos = Input.mousePosition;
				newPos = Camera.main.ScreenToViewportPoint (newPos);
				newPos.x *= Screen.width;
				newPos.y *= Screen.height;
				m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goItemSelectWindow.GetComponent<RectTransform> ().position = newPos;
			}
		}
	}

	#endregion

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goItemSelectWindow.activeSelf == false && m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goUnitSelectWindow.activeSelf == false) {

			Transform _root = m_goFIELDUI.GetComponent<MenuScreenScript> ().m_goInventory.transform.Find ("ItemDescriptionPanel");
			_root.gameObject.SetActive (true);
			_root.Find ("ItemName").GetComponentInChildren<Text> ().text = m_iItem.m_szItemName;
			_root.Find ("ItemDescription").GetComponentInChildren<Text> ().text = m_iItem.m_szItemDesc;
			_root.Find ("ItemType").GetComponentInChildren<Text> ().text = m_iItem.GetItemTypeName ();
			Transform _statRoot = _root.Find ("ItemStats");
			ItemLibrary.ItemData _item = m_goFIELDUI.GetComponent<MenuScreenScript> ().dc.m_lItemLibrary.GetItemFromDictionary (m_iItem.m_szItemName);

			_statRoot.Find ("HP").GetComponent<Text> ().color = DetermineColor (_item.m_nHPMod);
			_statRoot.Find ("HP").GetComponent<Text> ().text = "HP: " + _item.m_nHPMod;

			_statRoot.Find ("MP").GetComponent<Text> ().color = DetermineColor (_item.m_nMPMod);
			_statRoot.Find ("MP").GetComponent<Text> ().text = "MP: " + _item.m_nMPMod;

			_statRoot.Find ("POW").GetComponent<Text> ().color = DetermineColor (_item.m_nPowMod);
			_statRoot.Find ("POW").GetComponent<Text> ().text = "POW: " + _item.m_nPowMod;

			_statRoot.Find ("DEF").GetComponent<Text> ().color = DetermineColor (_item.m_nDefMod);
			_statRoot.Find ("DEF").GetComponent<Text> ().text = "DEF: " + _item.m_nDefMod;

			_statRoot.Find ("SPD").GetComponent<Text> ().color = DetermineColor (_item.m_nSpdMod);
			_statRoot.Find ("SPD").GetComponent<Text> ().text = "SPD: " + _item.m_nSpdMod;

			_statRoot.Find ("EVA").GetComponent<Text> ().color = DetermineColor (_item.m_nEvaMod);
			_statRoot.Find ("EVA").GetComponent<Text> ().text = "EVA: " + _item.m_nEvaMod;

			_statRoot.Find ("HIT").GetComponent<Text> ().color = DetermineColor (_item.m_nHitMod);
			_statRoot.Find ("HIT").GetComponent<Text> ().text = "HIT: " + _item.m_nHitMod;
		}
	}

	#endregion

	Color DetermineColor(int _stat)
	{
		Color _returnCol = new Color ();
		if (_stat > 0)
			_returnCol = Color.green;
		else if (_stat < 0)
			_returnCol = Color.red;
		else
			_returnCol = Color.black;
		return _returnCol;
	}
}
