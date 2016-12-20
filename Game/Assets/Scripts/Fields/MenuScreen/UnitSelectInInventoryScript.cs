using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UnitSelectInInventoryScript : MonoBehaviour, IPointerClickHandler {
	public GameObject m_goFIELDUI;
	public int m_nFormationIter;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		m_goFIELDUI.GetComponent<MenuScreenScript> ().UseItemOnCharacter (m_nFormationIter);
	}

	#endregion
}
