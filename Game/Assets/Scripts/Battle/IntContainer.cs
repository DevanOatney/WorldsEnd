using UnityEngine;
using System.Collections;

public class IntContainer : MonoBehaviour 
{
	public int m_nInteger;
	public GameObject m_twTurnWatcher;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void BeenHighlighted()
	{
		m_twTurnWatcher.GetComponent<ItemsAndSpellsContainer>().SelectionChanged(m_nInteger);
	}

	public void BeenClicked()
	{
		m_twTurnWatcher.GetComponent<ItemsAndSpellsContainer>().SelectionSelected(m_nInteger);
	}

	void OnMouseEnter()
	{
		//float yAxis = GetComponent<RectTransform>().position.y;
		//if(yAxis < -10 && yAxis > -16)
		//	m_twTurnWatcher.GetComponent<ItemsAndSpellsContainer>().AdjustHighlighter(yAxis, m_nInteger);
	}

}
