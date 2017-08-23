using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModifierInListScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler {
	public GameObject m_goParent;
	public DCScript.cModifier m_nModifier;
	public GameObject m_goDescriptionWindow;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		m_goParent.GetComponent<BlacksmithShopUIScript> ().ModifierSelected (m_nModifier);
	}

	#endregion

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		m_goDescriptionWindow.GetComponentInChildren<Text> ().text = m_nModifier.m_szModifierDesc;
	}

	#endregion
}
