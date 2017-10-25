using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SaveSlotScript : MonoBehaviour , IPointerClickHandler, IPointerEnterHandler
{
	public int m_nSlotIndex;
	public GameObject m_goSavePointOrbOrigin;
	public GameObject m_goHighlighter;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		if (m_goSavePointOrbOrigin != null)
		{
			m_goSavePointOrbOrigin.GetComponent<SavePointOrbScript> ().SaveFile (m_nSlotIndex);

			//Object.FindObjectOfType<SavePointOrbScript> ().SaveFile (m_nSlotIndex);
		}
	}

	#endregion

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		m_goHighlighter.transform.localPosition = transform.localPosition;
	}

	#endregion
}
