using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RosterScreenCellScript : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	//Position that the character should be in, in the actual formation
	public int m_nFormationIter;
	//The character data of the unit this cell is containing
	public DCScript.CharacterData m_cCharacter;
	//The point that the sprite should animate in, in the cell
	Vector3 m_vCharacterStartPos;
	//Hook to the game object while you drag it around (need to do this because we have to move the parent so that the positional data is right (weird Unity UI thing)
	public Transform m_goDraggedObject = null;
	//Name of the panel that the character began being dragged in.
	public static string m_szPanelOfDraggedCharacter = "";
	//flag for if the character gets dropped in the same panel.
	public bool m_bDroppedInSamePanel = false;
	//flag for if the object was dropped in a valid panel or not
	public bool m_bPanelDropped = false;

	// Use this for initialization
	void Start () 
	{
		m_vCharacterStartPos = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.P))
			Debug.Break();
	}

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		if(GetComponent<RectTransform>().childCount > 0)
		{
			m_goDraggedObject = GetComponent<RectTransform>().GetChild(0);
			m_goDraggedObject.SetParent(m_goDraggedObject.parent.parent);
			m_szPanelOfDraggedCharacter = name;
			CharacterInRosterScript.m_szCharacterBeingDragged = m_cCharacter.m_szCharacterName;
		}
		else
			CharacterInRosterScript.m_szCharacterBeingDragged = "";
	}


	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		if(m_goDraggedObject != null)
		{
			Vector3 newPos = Input.mousePosition;
			newPos = Camera.main.ScreenToViewportPoint(newPos);
			newPos.x *= Screen.width - (m_goDraggedObject.GetComponent<RectTransform>().sizeDelta.x * 0.5f);
			newPos.y *= Screen.height + (m_goDraggedObject.GetComponent<RectTransform>().sizeDelta.y * 0.5f);
			m_goDraggedObject.GetComponent<RectTransform>().position = newPos;
			//newPos.x = newPos.x - (Screen.width * 0.5f) - (m_goDraggedObject.GetComponent<RectTransform>().sizeDelta.x * 1.5f);
			//newPos.y = newPos.y - (Screen.height * 0.5f) + (m_goDraggedObject.GetComponent<RectTransform>().sizeDelta.y * 0.5f);
			//m_goDraggedObject.GetComponent<RectTransform>().localPosition = newPos;
		}

	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		RemoveCharacter();

	} 

	#endregion

	#region IDropHandler implementation

	public void OnDrop (PointerEventData eventData)
	{
		
		DCScript dc = GameObject.Find("PersistantData").GetComponent<DCScript>();

		DCScript.CharacterData character;
		string charName = CharacterInRosterScript.m_szCharacterBeingDragged;
		if (charName == "")
			return;
		if (dc.GetRosteredCharacterData (charName).m_bIsInParty == true)
			character = dc.GetCharacter (CharacterInRosterScript.m_szCharacterBeingDragged);
		else
			character = dc.GetRosteredCharacterData (charName);
		CharacterInRosterScript.m_szCharacterBeingDragged = "";

		if(character != null)
		{
			m_bPanelDropped = true;
			if (m_szPanelOfDraggedCharacter == "")
			{
				//in here means that the character was not dragged from another cell, and was instead from the list on the left instead.
				foreach (DCScript.CharacterData c in dc.GetParty())
				{
					//if the character is already in the party (shouldn't happen.. but check anyway) don't do anything
					if (c.m_szCharacterName == character.m_szCharacterName)
					{
						return;
					}
				}
				InstantiateCharacter (character);
			}
			else if (name != m_szPanelOfDraggedCharacter)
			{
				//Swapping from one panel to another
				SwapCharacters (gameObject, GameObject.Find (m_szPanelOfDraggedCharacter));
			}
			else if (m_cCharacter.m_nFormationIter == m_nFormationIter)
			{
				//object has been dropped back into this same cell.
				m_bDroppedInSamePanel = true;
				m_goDraggedObject.GetComponent<RectTransform> ().SetParent (GetComponent<RectTransform> ());
				m_goDraggedObject.GetComponent<RectTransform> ().rotation = Quaternion.identity;
				Vector3 characterPosition = m_vCharacterStartPos;
				characterPosition.y += m_goDraggedObject.GetComponent<RectTransform> ().sizeDelta.y * 0.5f;
				m_goDraggedObject.GetComponent<RectTransform> ().localPosition = characterPosition;
				m_goDraggedObject = GetComponent<RectTransform> ().GetChild (0);
				m_cCharacter.m_nFormationIter = m_nFormationIter;
			}
		}

	}

	#endregion

	public void InstantiateCharacter(DCScript.CharacterData character)
	{
		m_cCharacter = character;
		if(m_cCharacter != null)
		{
			//the following if/else stuff is an exit condition- if this is a support character being put into a non-support position, escape.
			bool _bOkayToContinue = false;
			if (m_cCharacter.m_bCombatCharacter == false)
			{
				//non-combat character
				if (m_nFormationIter == 6)
				{
					_bOkayToContinue = true;
				}
				else
				{
					_bOkayToContinue = false;
				}
			}
			else
			{
				_bOkayToContinue = true;
			}
			if (_bOkayToContinue == false)
			{
				return;
			}
			GameObject.Find("PersistantData").GetComponent<DCScript>().AddPartyMember (m_cCharacter.m_szCharacterName);
			GameObject newCharacter = Instantiate(Resources.Load<GameObject>("Units/Ally/" + m_cCharacter.m_szCharacterName + "/" + m_cCharacter.m_szCharacterName + "_UIAnimated")) as GameObject;
			newCharacter.name = m_cCharacter.m_szCharacterName + "_UIAnimated";
			newCharacter.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
			newCharacter.GetComponent<RectTransform>().rotation = Quaternion.identity;
			Vector3 characterPosition = m_vCharacterStartPos;
			characterPosition.y += newCharacter.GetComponent<RectTransform>().sizeDelta.y * 0.5f;
			newCharacter.GetComponent<RectTransform>().localPosition = characterPosition;
			m_goDraggedObject = newCharacter.transform;
			m_cCharacter.m_nFormationIter = m_nFormationIter;
		}
	}


	void RemoveCharacter()
	{
		if(m_goDraggedObject != null && m_bDroppedInSamePanel == false)
		{
			if(m_bPanelDropped == false)
			{
				//character was dropped outside of a valid panel, if it's not the main character, remove from party.  If it is the main character, just put him back where he was
				if(m_cCharacter.m_szCharacterName == "Callan")
				{
					m_goDraggedObject.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
					m_goDraggedObject.GetComponent<RectTransform>().rotation = Quaternion.identity;
					Vector3 characterPosition = m_vCharacterStartPos;
					characterPosition.y += m_goDraggedObject.GetComponent<RectTransform>().sizeDelta.y * 0.5f;
					m_goDraggedObject.GetComponent<RectTransform>().localPosition = characterPosition;
					m_goDraggedObject = m_goDraggedObject.transform;
					m_szPanelOfDraggedCharacter = "";
				}
				else
				{
					GameObject.Find("PersistantData").GetComponent<DCScript>().RemovePartyMember(m_cCharacter);
					Destroy(m_goDraggedObject.gameObject);
					m_goDraggedObject = null;
					m_cCharacter = null;
					m_szPanelOfDraggedCharacter = "";
				}
			}
			else
			{
				//panel has been dropped in another valid panel
				Destroy(m_goDraggedObject.gameObject);
				m_goDraggedObject = null;
				m_cCharacter = null;
				m_szPanelOfDraggedCharacter = "";
			}
		}
		if(m_bDroppedInSamePanel == true)
			m_bDroppedInSamePanel = false;
		m_bPanelDropped = false;
	}

	public void Remove()
	{
		if(transform.childCount > 0)
		{
			Destroy(transform.GetChild(0).gameObject);
			m_goDraggedObject = null;
			m_cCharacter = null;
			m_szPanelOfDraggedCharacter = "";
		}
	}
	public static void SwapCharacters(GameObject p_firstCell, GameObject p_secondCell)
	{
		DCScript.CharacterData firstChar = null;
		DCScript.CharacterData secondChar = null;
		if(p_firstCell.GetComponent<RosterScreenCellScript>().m_goDraggedObject != null)
			firstChar = GameObject.Find("PersistantData").GetComponent<DCScript>().GetCharacter(p_firstCell.GetComponent<RosterScreenCellScript>().m_goDraggedObject.name.Split('_')[0].Trim());
		if(p_secondCell.GetComponent<RosterScreenCellScript>().m_goDraggedObject != null)
			secondChar = GameObject.Find("PersistantData").GetComponent<DCScript>().GetCharacter(p_secondCell.GetComponent<RosterScreenCellScript>().m_goDraggedObject.name.Split('_')[0].Trim());
		p_firstCell.GetComponent<RosterScreenCellScript>().m_bPanelDropped = true;
		p_firstCell.GetComponent<RosterScreenCellScript>().RemoveCharacter();
		p_secondCell.GetComponent<RosterScreenCellScript>().m_bPanelDropped = true;
		p_secondCell.GetComponent<RosterScreenCellScript>().RemoveCharacter();
		if(secondChar != null)
		{
			p_firstCell.GetComponent<RosterScreenCellScript>().InstantiateCharacter(secondChar);
		}
		if(firstChar != null)
		{
			
			p_secondCell.GetComponent<RosterScreenCellScript>().m_bDroppedInSamePanel = true;
			p_secondCell.GetComponent<RosterScreenCellScript>().InstantiateCharacter(firstChar);
		}

	}
}
