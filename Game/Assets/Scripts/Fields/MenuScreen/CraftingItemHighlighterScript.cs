using UnityEngine;
using System.Collections;

public class CraftingItemHighlighterScript : MonoBehaviour 
{
	GameObject m_goHighlightedObject;

	float m_fUpdateTimer = 0.0f;
	float m_fTimerBucket = 0.1f;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_fUpdateTimer >= m_fTimerBucket)
		{
			if(m_goHighlightedObject != null)
				GetComponent<RectTransform>().localPosition = m_goHighlightedObject.GetComponent<RectTransform>().localPosition;
			m_fUpdateTimer = 0.0f;
		}
		else
			m_fUpdateTimer += m_fTimerBucket;
	}
		
	public void SetHighlightedObject(GameObject _goObject)
	{
		m_goHighlightedObject = _goObject;
		GetComponent<RectTransform>().localPosition = m_goHighlightedObject.GetComponent<RectTransform>().localPosition;
	}
}
