using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionPanelScript : MonoBehaviour 
{
	public int m_nActionIter;
	TurnWatcherScript m_twTurnWatcher;
	bool m_bIsHighlighted = false;
	// Use this for initialization
	void Start () 
	{
		m_twTurnWatcher = GameObject.Find("TurnWatcher").GetComponent<TurnWatcherScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0) && m_bIsHighlighted == true)
		{
			m_twTurnWatcher.ActionSelected(m_nActionIter);
		}
	}

	void OnMouseEnter()
	{
		m_bIsHighlighted = true;
		transform.parent.parent.GetComponent<CBattleActionsScript>().ChangeIndex(m_nActionIter);
		transform.parent.FindChild("HoverImage").GetComponent<Image>().enabled = true;
	}

	void OnMouseExit()
	{
		m_bIsHighlighted = false;
		transform.parent.FindChild("HoverImage").GetComponent<Image>().enabled = false;
	}
}
