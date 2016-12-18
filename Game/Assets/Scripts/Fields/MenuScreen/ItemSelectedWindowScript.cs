using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ItemSelectedWindowScript : MonoBehaviour, IPointerClickHandler{
	public enum ITEMSELECTED {eUSE, eDISCARD};
	public ITEMSELECTED m_isType;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IPointerClickHandler implementation


	public void OnPointerClick (PointerEventData eventData)
	{
		if (m_isType == ITEMSELECTED.eDISCARD) {
			transform.GetComponentInParent<MenuScreenScript> ().DiscardItem ();
			return;
		}
		if (m_isType == ITEMSELECTED.eUSE) {
			transform.GetComponentInParent<MenuScreenScript> ().UpdateUnitsOnInventory ();
			transform.GetComponentInParent<MenuScreenScript> ().m_goUnitSelectWindow.SetActive (true);
			transform.GetComponentInParent<MenuScreenScript> ().m_goItemSelectWindow.SetActive (false);
		}
	}


	#endregion

}
