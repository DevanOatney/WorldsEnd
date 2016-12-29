using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class EquipmentSlotScript : MonoBehaviour, IPointerClickHandler
{
	public GameObject m_goFIELDUI;
	public enum EquipmentSlotID { eHELM, eSHOULDER, eCHEST, eGLOVES, eWAIST, eLEG, eTRINKET1, eTRINKET2};
	public EquipmentSlotID m_eqSlotID;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData)
	{
		if(m_goFIELDUI.GetComponent<MenuScreenScript>().m_nEquipmentScreenIter == 1)
			m_goFIELDUI.GetComponent<MenuScreenScript> ().EquipmentSlotSelected ((int)m_eqSlotID);
	}
	#endregion

}
