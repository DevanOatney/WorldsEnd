using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleAnimationCheckboxScript : MonoBehaviour 
{
	public GameObject m_goToggle;
	public GameObject exitButton;
	void Start()
	{
		GameObject GO = GameObject.Find("PersistantData");
		if(GO != null) 
		{
			m_goToggle.GetComponent<Toggle>().isOn = GO.GetComponent<DCScript>().m_bToUseBattleAnimations;
		}
	}
	public void ValueChange(bool bFlag)
	{
		GameObject GO = GameObject.Find("PersistantData");
		if(GO != null)
		{
			GO.GetComponent<DCScript>().m_bToUseBattleAnimations = bFlag;
		}
	}
}