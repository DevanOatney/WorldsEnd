using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class CraftingItemScript : MonoBehaviour, IPointerClickHandler
{
	public GameObject m_goCraftingRequirementsContainer;
	public ItemLibrary.CraftingItemData m_ilItemData;
	// Use this for initialization
	void Start () 
	{
	}
	public void Initialize(ItemLibrary.CraftingItemData _item)
	{
		GetComponentInChildren<Text>().text = m_ilItemData.m_szItemName;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		transform.parent.parent.FindChild("CraftingItemHighlighter").GetComponent<CraftingItemHighlighterScript>().SetHighlightedObject(gameObject);
		Text[] _tReqs = m_goCraftingRequirementsContainer.GetComponentsInChildren<Text>();
		for(int i = 0; i < _tReqs.Length; ++i)
		{
			if( i < m_ilItemData.m_lRequiredItems.Count)
				_tReqs[i].text = m_ilItemData.m_lRequiredItems[i].m_szItemName + "  x" + m_ilItemData.m_lRequiredItems[i].m_nItemCount.ToString();
			else
				_tReqs[i].text = "";
		}
	}

	#endregion
}
