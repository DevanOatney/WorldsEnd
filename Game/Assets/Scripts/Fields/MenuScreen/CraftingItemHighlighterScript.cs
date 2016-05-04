using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CraftingItemHighlighterScript : MonoBehaviour 
{
	[HideInInspector]
	public GameObject m_goHighlightedObject;
	public Text[] m_goRequiredItems;
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
		for(int i = 0; i < m_goRequiredItems.Length; ++i)
		{
			if( i < _goObject.GetComponent<CraftingItemScript>().m_ilItemData.m_lRequiredItems.Count)
				m_goRequiredItems[i].text = _goObject.GetComponent<CraftingItemScript>().m_ilItemData.m_lRequiredItems[i].m_szItemName + "  x" + _goObject.GetComponent<CraftingItemScript>().m_ilItemData.m_lRequiredItems[i].m_nItemCount.ToString();
			else
				m_goRequiredItems[i].text = "";
		}
	}
}
