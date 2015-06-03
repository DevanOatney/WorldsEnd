using UnityEngine;
using System.Collections;

public class NPC_BlacksmithScript : NPCScript 
{
	bool m_bShowScreen = false;
	int  m_nInitialIter = 0;
	bool m_bEnhanceChosen = false;
	bool m_bModifyChosen = false;
	int  m_nWeaponIter = 0;
	bool m_bWeaponChosen = false;
	bool m_bModifierChosen = false;
	int  m_nConfirmIter = 0;

	// Use this for initialization
	void Start ()
	{
		LoadSteps();
	}
	
	// Update is called once per frame
	void Update () 
	{
		HandleInput();
		HandleMovement();
	}

	void HandleInput()
	{
		if(m_bShowScreen == true)
		{
			if(Input.GetKey(KeyCode.Return))
			{
				if(m_bEnhanceChosen == false && m_bModifyChosen == false)
				{
					if(m_nInitialIter == 0)
						m_bEnhanceChosen = true;
					else if(m_nInitialIter == 1)
						m_bModifyChosen = true;
					else
						ResetValues();
			
				}
				else if(m_bEnhanceChosen == true || m_bModifyChosen == true && m_bWeaponChosen == false)
					m_bWeaponChosen = true;
				else if(m_bModifyChosen == true && m_bWeaponChosen == true)
				{
					if(m_nConfirmIter == 0)
					{
						//has decided to buy
					}
					else
					{
						m_nConfirmIter = 0;
						m_bWeaponChosen = false;
					}
				}
			}
		}
	}

	void ResetValues()
	{
		m_bShowScreen = false;
		m_nInitialIter = 0;
		m_bEnhanceChosen = false;
		m_bModifyChosen = false;
		m_nWeaponIter = 0;
		m_bWeaponChosen = false;
		m_bModifierChosen = false;
		m_nConfirmIter = 0;
	}
	void OnGUI()
	{
		if(m_bShowScreen)
		{
			Debug.Log("working~");
			GUI.Box(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.2f, Screen.height * 0.7f), "");
		}
	}

	new public void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == "Action Box(Clone)")
		{
			if(GetComponent<MessageHandler>())
			{
				//set to about to be active
				m_bIsMoving = false;
				m_bIsBeingInterractedWith = true;
				if(m_szDialoguePath != "")
					GameObject.Find("Event System").GetComponent<BaseEventSystemScript>().HandleEvent(m_szDialoguePath);
			}
		}
	}

	public void ActivateScreen()
	{
		Debug.Log ("activate");
		m_bShowScreen = true;
	}
}
