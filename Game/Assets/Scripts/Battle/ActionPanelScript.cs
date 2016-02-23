using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionPanelScript : MonoBehaviour 
{
	public int m_nActionIter;
	TurnWatcherScript m_twTurnWatcher;
	[HideInInspector]
	public bool m_bIsHighlighted = false;

	//optimization variables so that the hover event does spam rapidly.
	float m_fHoverTimer = 1.0f;
	float m_fHoverBucket = 0.4f;
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
		//transform.parent.FindChild("HoverImage").GetComponent<Image>().enabled = true;
	}

	void OnMouseOver()
	{
		if(m_fHoverTimer >= m_fHoverBucket)
		{
			OnMouseEnter();
			m_fHoverTimer = 0.0f;
		}
		else
			m_fHoverTimer += Time.deltaTime;
	}
}
