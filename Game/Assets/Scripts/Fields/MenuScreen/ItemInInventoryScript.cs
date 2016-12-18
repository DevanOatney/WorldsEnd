using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ItemInInventoryScript : MonoBehaviour, IPointerClickHandler {

	ItemLibrary.CharactersItems m_iItem;
	GameObject m_goFIELDUI;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(ItemLibrary.CharactersItems _iItem, GameObject _goFIELDUI)
	{
		m_iItem = _iItem;
		m_goFIELDUI = _goFIELDUI;
		transform.FindChild ("ItemName").GetComponentInChildren<Text> ().text = m_iItem.m_szItemName;
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
		transform.FindChild ("ItemType").GetComponentInChildren<Text> ().text = szType;
		transform.FindChild ("ItemCount").GetComponentInChildren<Text> ().text = "x" + m_iItem.m_nItemCount.ToString ();
	}

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		m_goFIELDUI.GetComponent<MenuScreenScript>().m_goItemSelectWindow.SetActive (true);
		m_goFIELDUI.GetComponent<MenuScreenScript> ().m_iSelectedItem = m_iItem;
		Vector3 newPos = Input.mousePosition;
		newPos = Camera.main.ScreenToViewportPoint(newPos);
		newPos.x *= Screen.width;
		newPos.y *= Screen.height;
		m_goFIELDUI.GetComponent<MenuScreenScript>().m_goItemSelectWindow.GetComponent<RectTransform> ().position = newPos;
	}

	#endregion
}
