using UnityEngine;
using System.Collections;

public class PoisonCloneScript : MonoBehaviour 
{
	public GameObject m_gOwner;
	float m_fFadeSpeed = 5.0f;
	SpriteRenderer m_srSprite;
	Color m_cColor;

	void Start () {
		m_srSprite = GetComponent<SpriteRenderer>();
		m_cColor = m_srSprite.color;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = m_gOwner.transform.position;
		m_srSprite.sprite = m_gOwner.GetComponentInChildren<SpriteRenderer>().sprite;
		gameObject.transform.localScale = m_gOwner.transform.localScale;
		m_cColor.r -= m_fFadeSpeed * Time.deltaTime;
		m_cColor.g += m_fFadeSpeed * Time.deltaTime;
		m_cColor.b -= m_fFadeSpeed * Time.deltaTime;
		m_cColor.a -= 1.0f * Time.deltaTime;
		m_srSprite.color = m_cColor;
	}
}
