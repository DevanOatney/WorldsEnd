using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RosterScreenCellScript : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public int m_nFormationIter;
	DCScript.CharacterData m_cCharacter;
	Vector3 m_vCharacterStartPos;

	// Use this for initialization
	void Start () {
		m_vCharacterStartPos = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
	}


	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		if(GetComponent<RectTransform>().childCount > 0)
		{
			Vector3 newPos = Input.mousePosition;
			newPos.x = newPos.x - (Screen.width * 0.5f) + (GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>().sizeDelta.x *2);
			newPos.y = newPos.y - (Screen.height * 0.5f)+ (GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>().sizeDelta.y *2);
			GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>().localPosition = newPos;
		}
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		Destroy(GetComponent<RectTransform>().GetChild(0).gameObject);
	}

	#endregion

	#region IDropHandler implementation

	public void OnDrop (PointerEventData eventData)
	{
		m_cCharacter = GameObject.Find("PersistantData").GetComponent<DCScript>().GetCharacter(CharacterInRosterScript.m_szCharacterBeingDragged);
		Debug.Log(m_cCharacter.m_szCharacterName);
		GameObject newCharacter = Instantiate(Resources.Load<GameObject>("Units/Ally/" + m_cCharacter.m_szCharacterName + "/" + m_cCharacter.m_szCharacterName + "_UIAnimated")) as GameObject;

		newCharacter.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
		newCharacter.GetComponent<RectTransform>().rotation = Quaternion.identity;
		Vector3 characterPosition = m_vCharacterStartPos;
		characterPosition.y += newCharacter.GetComponent<RectTransform>().sizeDelta.y * 0.5f;
		newCharacter.GetComponent<RectTransform>().localPosition = characterPosition;
	}

	#endregion
}
