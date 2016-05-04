using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextSpeedScript : MonoBehaviour 
{
	public int m_nSelectedIndex;
	public GameObject exitButton;
	public GameObject m_goDropDown;

	void Start()
	{
		m_nSelectedIndex = 1;
		GameObject GO = GameObject.Find("PersistantData");
		if(GO)
		{
			m_nSelectedIndex = GO.GetComponent<DCScript>().m_nTextSpeed;
		}
		m_goDropDown.GetComponent<Dropdown>().value = m_nSelectedIndex;
	}
	
	public  void ChangeIndex(int index)
	{
		m_nSelectedIndex = index+1;
		GameObject GO = GameObject.Find("PersistantData");
		if(GO)
		{
			GO.GetComponent<DCScript>().m_nTextSpeed = m_nSelectedIndex;
		}
	}
}