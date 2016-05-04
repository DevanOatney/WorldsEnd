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
		m_goCraftingRequirementsContainer = GameObject.Find("CraftingItemHighlighter");
	}
	public void Initialize(ItemLibrary.CraftingItemData _item)
	{
		m_ilItemData = _item;
		GetComponentInChildren<Text>().text = m_ilItemData.m_szItemName;
		GameObject _highlighter = transform.parent.parent.FindChild("CraftingItemHighlighter").gameObject;
		if(_highlighter.GetComponent<CraftingItemHighlighterScript>().m_goHighlightedObject == null)
		{
			//This is the first item in the list.
			SetHighlighter();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SetHighlighter()
	{
		transform.parent.parent.FindChild("CraftingItemHighlighter").GetComponent<CraftingItemHighlighterScript>().SetHighlightedObject(gameObject);
	}

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		SetHighlighter();
	}

	#endregion
}
