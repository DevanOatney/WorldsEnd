using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContinueMenuHighlightScript : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
	public int m_nIndex;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		GameObject.Find ("Highlighter").GetComponent<ContinueHighlightInputScript> ().ChangeHighlightedPosition (m_nIndex);
	}

	#endregion

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		GameObject.Find ("Highlighter").GetComponent<ContinueHighlightInputScript> ().SelectSavePoint (m_nIndex);
	}

	#endregion
}
