using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CBattleActionsScript : MonoBehaviour 
{
	int m_nActionIter = 0;
	int m_nMaxActions = 0;
	public int ActionIndex(){return m_nActionIter;}

	// Use this for initialization
	void Start () 
	{
		m_nMaxActions = transform.childCount;
		AdjustActionSelector();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeIndex(int p_nIndex)
	{
		if(p_nIndex == m_nActionIter)
			return;
		m_nActionIter = p_nIndex;
		if(m_nActionIter >= m_nMaxActions)
			m_nActionIter = 0;
		else if(m_nActionIter < 0)
			m_nActionIter = m_nMaxActions - 1;
		
		AdjustActionSelector();
	}

	void AdjustActionSelector()
	{
		//change colors actions
		int _nCounter = 0;
		foreach(Transform child in transform)
		{
			if(_nCounter == m_nActionIter)
			{
				//this is the one selected
				child.FindChild("Collider").GetComponent<ActionPanelScript>().m_bIsHighlighted = true;
				child.FindChild("HoverImage").GetComponent<Image>().enabled = true;
				child.FindChild("Text").GetComponent<Text>().enabled = true;
			}
			else
			{
				child.FindChild("Collider").GetComponent<ActionPanelScript>().m_bIsHighlighted = false;
				child.FindChild("HoverImage").GetComponent<Image>().enabled = false;
				child.FindChild("Text").GetComponent<Text>().enabled = false;
			}
			++_nCounter;
		}
	}
}
