using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITextChoiceHighlightingScript : MonoBehaviour , IPointerEnterHandler
{
	public int m_nIndex;
	GameObject m_goHighlighter;
	// Use this for initialization
	void Start () 
	{
		m_goHighlighter = GameObject.Find ("Highlighter");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (gameObject.GetComponent<Text> ().text != "")
		{
			m_goHighlighter.GetComponent<UIDialogueChoiceMouseInputScript> ().DialogueChoiceHighlighted (m_nIndex);
		}
	}

	#endregion
}
