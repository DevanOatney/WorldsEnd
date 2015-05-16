using UnityEngine;
using System.Collections;

public class FadeSpriteScript : MonoBehaviour {

	float m_fFadeSpeed = 5.0f;
	// Use this for initialization
	SpriteRenderer m_srSprite;
	Color m_cColor;
	void Start () {
		m_srSprite = GetComponent<SpriteRenderer>();
		m_cColor = m_srSprite.color;
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_cColor.r -= m_fFadeSpeed * Time.deltaTime;
		m_cColor.g -= m_fFadeSpeed * Time.deltaTime;
		m_cColor.b -= m_fFadeSpeed * Time.deltaTime;
		m_srSprite.color = m_cColor;
	}
}
