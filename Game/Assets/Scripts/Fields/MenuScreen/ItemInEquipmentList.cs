using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ItemInEquipmentList : MonoBehaviour, IPointerClickHandler {

	ItemLibrary.ArmorData m_iArmorData;
	GameObject m_goFieldUI;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(ItemLibrary.ArmorData _armor, GameObject _fieldUI)
	{
		m_iArmorData = _armor;
		m_goFieldUI = _fieldUI;
	}

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		throw new System.NotImplementedException ();
	}

	#endregion
}
