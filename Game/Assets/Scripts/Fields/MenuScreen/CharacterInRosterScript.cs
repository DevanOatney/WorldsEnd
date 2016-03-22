using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterInRosterScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	//prefab hook for the image of the dragged character
	public GameObject m_goDraggedObject;
	//Hook to the parented object so that the sorting order will work
	public GameObject m_goBackgroundObject;
	//for the newly instantiated object to be dragged around
	GameObject m_goNewDraggedObject;
	public static string m_szCharacterBeingDragged = "";


	void OnDestroy()
	{
		if(m_goNewDraggedObject != null)
		{
			Destroy(m_goNewDraggedObject);
			m_goNewDraggedObject = null;
		}
	}

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		m_szCharacterBeingDragged = GetComponent<RectTransform>().FindChild("CharacterName").GetComponent<Text>().text;
		m_goNewDraggedObject = Instantiate(m_goDraggedObject);
		m_goNewDraggedObject.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>().parent.parent.parent);
		m_goNewDraggedObject.GetComponent<RectTransform>().FindChild("CharacterName").GetComponent<Text>().text = GetComponent<RectTransform>().FindChild("CharacterName").GetComponent<Text>().text;
		m_goNewDraggedObject.GetComponent<RectTransform>().FindChild("CharacterLVL").GetComponent<Text>().text = GetComponent<RectTransform>().FindChild("CharacterLVL").GetComponent<Text>().text;
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		Vector3 newPos = Input.mousePosition;
		newPos.x = newPos.x - (Screen.width * 0.5f);
		newPos.y = newPos.y - (Screen.height * 0.5f);
		m_goNewDraggedObject.GetComponent<RectTransform>().anchoredPosition = newPos;
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		if(m_goNewDraggedObject != null)
		{
			Destroy(m_goNewDraggedObject);
			m_goNewDraggedObject = null;
		}
	}

	#endregion
}
