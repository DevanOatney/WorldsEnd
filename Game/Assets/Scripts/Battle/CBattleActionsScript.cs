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
		foreach(Transform child in transform)
			++m_nMaxActions;
		AdjustActionSelector();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeIndex(int p_nIndex)
	{
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
				child.gameObject.GetComponent<Image>().color = Color.black;
			}
			else
			{
				child.gameObject.GetComponent<Image>().color = Color.white;
			}
			++_nCounter;
		}
	}
}
