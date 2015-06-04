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
	DCScript dc;

	// Use this for initialization
	void Start ()
	{
		dc = GameObject.Find("PersistantData").GetComponent<DCScript>();
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
						//has decided to purchase a modification
						m_nConfirmIter = 0;
						m_bWeaponChosen = false;
					}
					else
					{
						m_nConfirmIter = 0;
						m_bWeaponChosen = false;
					}
				}
				else if(m_bEnhanceChosen == true && m_bWeaponChosen == true)
				{
					if(m_nConfirmIter == 0)
					{
						//has decided to confirm the enhance of the selected weapon
						m_nConfirmIter = 0;
						m_bWeaponChosen = false;
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
			GUI.Box(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.2f, Screen.height * 0.7f), "");

			if(m_bEnhanceChosen == true)
			{
				GUI.Box(new Rect(Screen.width * 0.27f, Screen.height * 0.12f, Screen.width * 0.3f, Screen.height * 0.66f), "");
				float yOffset = 0;
				float fTextHeight = 30.0f;
				foreach(DCScript.CharacterData character in dc.GetParty())
				{
					int tempFontHolder = GUI.skin.label.fontSize;
					GUI.skin.label.fontSize = 20;
					GUI.Box(new Rect(Screen.width * 0.27f, Screen.height * 0.12f + yOffset, Screen.width * 0.3f, Screen.height * 0.22f), "");
					GUI.Label(new Rect(Screen.width * 0.3f, Screen.height * 0.13f + yOffset, 200, fTextHeight), character.m_szCharacterName);
					GUI.Label(new Rect(Screen.width * 0.3f, Screen.height * 0.17f + yOffset, 200, fTextHeight), "[Weapon Name]" + "   [Lvl]");
					GUI.Label(new Rect(Screen.width * 0.3f, Screen.height * 0.21f + yOffset, 200, fTextHeight), "[Modifier]");


					GUI.Box(new Rect(Screen.width * 0.57f, Screen.height * 0.12f + yOffset, Screen.width * 0.2f, Screen.height * 0.22f), "[Portrait]");
					yOffset += Screen.height * 0.22f;
					GUI.skin.label.fontSize = tempFontHolder;
				}

			}
			else if(m_bModifyChosen == true)
			{
				GUI.Box(new Rect(Screen.width * 0.27f, Screen.height * 0.12f, Screen.width * 0.3f, Screen.height * 0.66f), "");
				float yOffset = 0;
				float fTextHeight = 30.0f;
				foreach(DCScript.CharacterData character in dc.GetParty())
				{
					int tempFontHolder = GUI.skin.label.fontSize;
					GUI.skin.label.fontSize = 20;
					GUI.Box(new Rect(Screen.width * 0.27f, Screen.height * 0.12f + yOffset, Screen.width * 0.3f, Screen.height * 0.22f), "");
					GUI.Label(new Rect(Screen.width * 0.3f, Screen.height * 0.13f + yOffset, 200, fTextHeight), character.m_szCharacterName);
					GUI.Label(new Rect(Screen.width * 0.3f, Screen.height * 0.17f + yOffset, 200, fTextHeight), "[Weapon Name]" + "   [Lvl]");
					GUI.Label(new Rect(Screen.width * 0.3f, Screen.height * 0.21f + yOffset, 200, fTextHeight), "[Modifier]");
					
					
					GUI.Box(new Rect(Screen.width * 0.57f, Screen.height * 0.12f + yOffset, Screen.width * 0.2f, Screen.height * 0.22f), "[Portrait]");
					yOffset += Screen.height * 0.22f;
					GUI.skin.label.fontSize = tempFontHolder;
				}
			}

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
		Input.ResetInputAxes();
		m_bShowScreen = true;
	}
}
