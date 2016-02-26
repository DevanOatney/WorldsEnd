using UnityEngine;
using System.Collections;

public class CharacterPanelScript : MonoBehaviour 
{
	GameObject m_goListener = null;
	Vector3 m_vTargetPosition;
	float m_fSlidingSpeed = 600.0f;
	bool m_bShouldSlide = false;
	Vector3 m_vOriginalPosition;
	// Use this for initialization
	void Start () 
	{
	
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
	}
	public void ReturnToPosition(GameObject listener)
	{
		m_vTargetPosition = m_vOriginalPosition;
		m_goListener = listener;
		m_bShouldSlide = true;
	}
}
