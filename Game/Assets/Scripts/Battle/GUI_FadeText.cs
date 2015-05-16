using UnityEngine;
using System.Collections;

public class GUI_FadeText : MonoBehaviour {

	public float m_fTextFadeSpeed;
	public float m_fTextFadeDecriment;
	bool m_bShouldFloat = true;
	public float m_fFloatSpeed;
	public bool m_bIsRed = true;
	// Use this for initialization
	void Start () 
	{
		GetComponent<GUIText>().color = Color.red;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Fade();
		Float ();
	}
	void Fade()
	{
		if(GetComponent<GUIText>().material.color.a > 0)
		{
			Color temp = GetComponent<GUIText>().material.color;
			if(m_bIsRed == false)
				GetComponent<GUIText>().color = Color.green;
			else
				GetComponent<GUIText>().color = Color.red;
			temp.a = GetComponent<GUIText>().material.color.a;
			temp.a -= m_fTextFadeDecriment * Time.deltaTime * m_fTextFadeSpeed; 
			GetComponent<GUIText>().material.color = temp;
		}
		else
			Destroy(gameObject);
	}
	void Float()
	{
		if(m_bShouldFloat == true)
		{
			Vector2 pos =  transform.position;
			pos.y += (m_fFloatSpeed * Time.deltaTime);
			transform.position = pos;
		}
	}
	public void SetText(string txt)
	{
		GetComponent<GUIText>().text = txt;
	}
	public void SetShouldFloat(bool bFloat)
	{
		m_bShouldFloat = bFloat;
	}
	public void SetColor(bool _isRed)
	{
		m_bIsRed = _isRed;
	}
}
