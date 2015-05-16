using UnityEngine;
using System.Collections;

public class BattleAnimationCheckboxScript : MonoBehaviour 
{
    private bool toUse = false;
	Vector3 pos;
	public GameObject exitButton;
	float m_fXAdjust = 150.0f;
	float m_fYAdjust = -8.5f;
	void Start()
	{
		pos = transform.position;
		pos = Camera.main.WorldToScreenPoint(pos);
		pos.y = Screen.height - pos.y;
		GameObject GO = GameObject.Find("PersistantData");
		if(GO != null) 
		{
			toUse = GO.GetComponent<DCScript>().m_bToUseBattleAnimations;
		}
	}
    void OnGUI() 
	{
		
        	toUse = GUI.Toggle(new Rect(pos.x + m_fXAdjust, pos.y + m_fYAdjust, 15, 15), toUse, "");
			if(exitButton.GetComponent<ExitButtonScript>().m_bSwitching == false)
			{
			if(GUI.changed)
			{
				GameObject GO = GameObject.Find("PersistantData");
				if(GO != null)
				{
					GO.GetComponent<DCScript>().m_bToUseBattleAnimations = toUse;
				}
			}
			}
    }
}