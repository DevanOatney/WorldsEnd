using UnityEngine;
using System.Collections;

public class TextSpeedScript : MonoBehaviour {
	
	public AudioClip m_aTextSound;
	public int m_nSelectedIndex;
	public GameObject exitButton;

	Vector2 m_vOffset = new Vector2(90.0f, -25.0f);
	void Start()
	{
		GameObject GO = GameObject.Find("PersistantData");
		if(GO)
		{
			m_nSelectedIndex = GO.GetComponent<DCScript>().m_nTextSpeed;
		}
	}
	
	public  void ChangeIndex(int index)
	{
		m_nSelectedIndex = index;
		GameObject GO = GameObject.Find("PersistantData");
		if(GO)
		{
			GO.GetComponent<DCScript>().m_nTextSpeed = m_nSelectedIndex;
		}
	}

	void OnGUI()
	{
		Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
		pos.y = Screen.height - pos.y;
		GUI.Box(new Rect(pos.x + m_vOffset.x, pos.y + m_vOffset.y, 250, 50), "");
	}
}