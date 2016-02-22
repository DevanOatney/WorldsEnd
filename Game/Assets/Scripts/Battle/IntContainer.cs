using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
		m_twTurnWatcher.GetComponent<ItemsAndSpellsContainer>().SelectionSelected();
	}

	public void DelayedHighlight()
	{
		Invoke("BeenHighlighted", 0.1f);
	}

}
