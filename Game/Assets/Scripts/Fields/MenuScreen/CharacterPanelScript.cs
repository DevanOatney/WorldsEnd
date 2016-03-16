using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterPanelScript : MonoBehaviour 
{
	GameObject m_goListener = null;
	Vector3 m_vTargetPosition;
	public float m_fSlidingSpeed = 600.0f;
	bool m_bShouldSlide = false;
	Vector3 m_vOriginalPosition;

	//optimization variables so that the hover event does spam rapidly.
	float m_fHoverTimer = 1.0f;
	float m_fHoverBucket = 0.4f;

	public GameObject m_goTargetForTopTabs;

	// Use this for initialization
	void Start () 
	{
		m_vOriginalPosition = GetComponent<RectTransform>().localPosition;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(m_bShouldSlide == true)
		{
			Vector3 pos = GetComponent<RectTransform>().localPosition;
			Vector3 toTarget = m_vTargetPosition - pos;
			if(toTarget.sqrMagnitude < 100.0f)
			{
				GetComponent<RectTransform>().localPosition = m_vTargetPosition;
				m_bShouldSlide = false;
				m_goListener.SendMessage("PanelReachedSlot");
				m_goListener = null;
				m_vTargetPosition = Vector3.zero;
			}
			else
			{
				toTarget.Normalize();
				pos  += toTarget * m_fSlidingSpeed * Time.deltaTime;
				GetComponent<RectTransform>().localPosition = pos;
			}
		}
	}

	public void BeginSlide(GameObject listener, Vector3 targetPos)
	{
		m_vOriginalPosition = GetComponent<RectTransform>().localPosition;
		m_goListener = listener;
		m_bShouldSlide = true;
		m_vTargetPosition = targetPos;
		if(m_goTargetForTopTabs != null)
			m_vTargetPosition = m_goTargetForTopTabs.transform.localPosition;
	}
	public void ReturnToPosition(GameObject listener)
	{
		m_vTargetPosition = m_vOriginalPosition;
		m_goListener = listener;
		m_bShouldSlide = true;
	}

	public void PanelHighlighted(int nIndex)
	{
		if(transform.FindChild("CharacterName").GetComponent<Text>().text == "")
			return;
		GameObject.Find("FIELD_UI").GetComponent<MenuScreenScript>().CharacterHighlighted(nIndex);
	}
	public void PanelSelected(int nIndex)
	{
		if(transform.FindChild("CharacterName").GetComponent<Text>().text == "")
			return;
		GameObject.Find("FIELD_UI").GetComponent<MenuScreenScript>().CharacterSelected(nIndex);
	}
	public void MouseHover(int nIndex)
	{
		if(m_fHoverTimer >= m_fHoverBucket)
		{
			PanelHighlighted(nIndex);
			m_fHoverTimer = 0.0f;
		}
		else
			m_fHoverTimer += Time.deltaTime;
	}
}
