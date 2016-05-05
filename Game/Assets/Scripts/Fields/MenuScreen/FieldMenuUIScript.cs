using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class FieldMenuUIScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
	GameObject m_goUIRoot;
	public MenuScreenScript.MENU_STATES m_nStateToGoTo;

	void Start()
	{
		m_goUIRoot = GameObject.Find("FIELD_UI");
	}

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		m_goUIRoot.GetComponent<MenuScreenScript>().TabHighlighted(m_nStateToGoTo);
	}

	#endregion

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		if(eventData.button == PointerEventData.InputButton.Left)
		{
			m_goUIRoot.GetComponent<MenuScreenScript>().TabSelected(m_nStateToGoTo);
		}
	}

	#endregion


}
