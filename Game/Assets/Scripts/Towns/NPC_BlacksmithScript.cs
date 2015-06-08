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
	//white texture for selecting the items
	public Texture2D m_t2dTexture;
	//timer and bucket for delayed input when the player is pressing and holding right/left
	float m_fIncDecTimer = 0.0f;
	float m_fIncDecBucket = 0.2f;


	//The maximum amount that this blacksmith can enhance to
	public int m_nMaxEnhancementLevel = 3;

	// Use this for initialization
	void Start ()
	{
		dc = GameObject.Find("PersistantData").GetComponent<DCScript>();
		LoadSteps();
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_fIncDecTimer += Time.deltaTime;
		HandleInput();
		HandleMovement();
	}

	void HandleInput() 
	{
		if(m_bShowScreen == true)
		{
			if(Input.GetKeyUp(KeyCode.Return))
			{
				if(m_bEnhanceChosen == false && m_bModifyChosen == false)
				{
					if(m_nInitialIter == 0)
						m_bEnhanceChosen = true;
					else if(m_nInitialIter == 1)
						m_bModifyChosen = true;
					else
					{
						ResetValues();
						GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
					}
			
				}
				else if((m_bEnhanceChosen == true || m_bModifyChosen == true) && m_bWeaponChosen == false)
					m_bWeaponChosen = true;
				else if(m_bModifyChosen == true && m_bWeaponChosen == true)
				{
					if(m_nConfirmIter == 0)
					{
						//purchase has been confirmed, do it!
						int sum = CalculateCost(dc.GetParty()[m_nWeaponIter].m_nWeaponLevel);
						if(sum != -1 && sum <= dc.m_nGold && dc.GetParty()[m_nWeaponIter].m_nWeaponLevel < m_nMaxEnhancementLevel)
						{
							dc.m_nGold -= sum;
							dc.GetParty()[m_nWeaponIter].m_nWeaponLevel++;
							dc.GetParty()[m_nWeaponIter].m_nWeaponDamageModifier += 5;
							m_bWeaponChosen = false;
						}
						else
						{
							//Either cannot afford, or the level of the weapon is too high.
						}
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
			else if(Input.GetKeyUp(KeyCode.Escape))
			{
				if((m_bEnhanceChosen == true || m_bModifyChosen == true) && m_bWeaponChosen == false)
				{
					m_bEnhanceChosen = false;
					m_bModifyChosen = false;
					m_nWeaponIter = 0;
				}
				else if(m_bWeaponChosen == true)
				{
					m_bWeaponChosen = false;
					m_nConfirmIter = 0;
				}
				else if(m_bEnhanceChosen == false && m_bModifyChosen == false)
				{
					ResetValues();
					GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
				}
			}
			else if(Input.GetKey(KeyCode.UpArrow))
			{
				if(m_bEnhanceChosen == false && m_bModifyChosen == false)
				{
					if(m_fIncDecTimer >= m_fIncDecBucket)
					{
						m_nInitialIter--;
						if(m_nInitialIter < 0)
							m_nInitialIter = 2;
						m_fIncDecTimer = 0.0f;
					}
				}
				else if((m_bEnhanceChosen == true || m_bModifyChosen == true) && m_bWeaponChosen == false)
				{
					if(m_fIncDecTimer >= m_fIncDecBucket)
					{
						m_nWeaponIter--;
						if(m_nWeaponIter < 0)
							m_nWeaponIter = dc.GetParty().Count - 1;
						m_fIncDecTimer = 0.0f;
					}
				}
				else if(m_bWeaponChosen == true)
				{
					if(m_fIncDecTimer >= m_fIncDecBucket)
					{
						m_nConfirmIter--;
						if(m_nConfirmIter < 0)
							m_nConfirmIter = 1;
						m_fIncDecTimer = 0.0f;
					}
				}
			}
			else if(Input.GetKey(KeyCode.DownArrow))
			{
				if(m_bEnhanceChosen == false && m_bModifyChosen == false)
				{
					if(m_fIncDecTimer >= m_fIncDecBucket)
					{
						m_nInitialIter++;
						if(m_nInitialIter > 2)
							m_nInitialIter = 0;
						m_fIncDecTimer = 0.0f;
					}
				}
				else if((m_bEnhanceChosen == true || m_bModifyChosen == true) && m_bWeaponChosen == false)
				{
					if(m_fIncDecTimer >= m_fIncDecBucket)
					{
						m_nWeaponIter++;
						if(m_nWeaponIter >= dc.GetParty().Count)
							m_nWeaponIter = 0;
						m_fIncDecTimer = 0.0f;
					}
				}
				else if(m_bWeaponChosen == true)
				{
					if(m_fIncDecTimer >= m_fIncDecBucket)
					{
						m_nConfirmIter++;
						if(m_nConfirmIter > 1)
							m_nConfirmIter = 0;
						m_fIncDecTimer = 0.0f;
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
			float fTextHeight = 30.0f;
			int tempFontHolder = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = 20;
			GUI.Box(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.2f, Screen.height * 0.7f), "");
			if(GUI.Button(new Rect(Screen.width * 0.13f, Screen.height * 0.12f, Screen.width * 0.15f, fTextHeight),"Enhance", "Label" ))
			{
				m_bEnhanceChosen = true;
				m_bModifyChosen = false;
				m_bWeaponChosen = false;
				m_nInitialIter = 0;
				m_nWeaponIter = 0;
				m_nConfirmIter = 0;
			}
			if(GUI.Button(new Rect(Screen.width * 0.13f, Screen.height * 0.12f + fTextHeight, Screen.width * 0.15f, fTextHeight), "Modify", "Label"))
			{
				m_bModifyChosen = true;
				m_bEnhanceChosen = false;
				m_bWeaponChosen = false;
				m_nInitialIter = 1;
				m_nWeaponIter = 0;
				m_nConfirmIter = 0;
			}
			if(GUI.Button(new Rect(Screen.width * 0.13f, Screen.height * 0.12f + fTextHeight*2, Screen.width * 0.15f, fTextHeight), "Exit", "Label"))
			{
				ResetValues();
				GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
			GUIStyle selectorStyle = new GUIStyle(GUI.skin.box);
			selectorStyle.normal.background = m_t2dTexture;
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);

			GUI.Box((new Rect(Screen.width * 0.12f,  Screen.height * 0.12f + fTextHeight * m_nInitialIter ,
			                  Screen.width * 0.15f, fTextHeight + 2)), "",selectorStyle);
			GUI.skin.label.fontSize = tempFontHolder;
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);


			if(m_bEnhanceChosen == true)
			{
				GUI.Box(new Rect(Screen.width * 0.27f, Screen.height * 0.12f, Screen.width * 0.3f, Screen.height * 0.66f), "");
				float yOffset = 0;
				int counter = 0;
				foreach(DCScript.CharacterData character in dc.GetParty())
				{
					tempFontHolder = GUI.skin.label.fontSize;
					GUI.skin.label.fontSize = 20;

					GUI.Box(new Rect(Screen.width * 0.27f, Screen.height * 0.12f + yOffset, Screen.width * 0.3f, Screen.height * 0.22f), "");
					GUI.Label(new Rect(Screen.width * 0.3f, Screen.height * 0.13f + yOffset, 200, fTextHeight), character.m_szCharacterName);
					if(GUI.Button(new Rect(Screen.width * 0.3f, Screen.height * 0.17f + yOffset, 200, fTextHeight),
					              character.m_szWeaponName + "   " + character.m_nWeaponLevel.ToString(), "Label"))
					{
						m_nWeaponIter = counter;
						m_bWeaponChosen = true;
						m_nConfirmIter = 0;
					}
					GUI.Label(new Rect(Screen.width * 0.3f, Screen.height * 0.21f + yOffset, 200, fTextHeight), character.m_szWeaponModifierName);


					GUI.Box(new Rect(Screen.width * 0.57f, Screen.height * 0.12f + yOffset, Screen.width * 0.2f, Screen.height * 0.22f), "[Portrait]");
					yOffset += Screen.height * 0.22f;
					GUI.skin.label.fontSize = tempFontHolder;
					counter++;
				}
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
				GUI.Box(new Rect(Screen.width * 0.3f, Screen.height * 0.17f + (Screen.height * 0.22f * m_nWeaponIter), 200, fTextHeight), "",selectorStyle);
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			}
			else if(m_bModifyChosen == true)
			{
				GUI.Box(new Rect(Screen.width * 0.27f, Screen.height * 0.12f, Screen.width * 0.3f, Screen.height * 0.66f), "");
				float yOffset = 0;
				int counter = 0;
				foreach(DCScript.CharacterData character in dc.GetParty())
				{
					tempFontHolder = GUI.skin.label.fontSize;
					GUI.skin.label.fontSize = 20;
					GUI.Box(new Rect(Screen.width * 0.27f, Screen.height * 0.12f + yOffset, Screen.width * 0.3f, Screen.height * 0.22f), "");
					GUI.Label(new Rect(Screen.width * 0.3f, Screen.height * 0.13f + yOffset, 200, fTextHeight), character.m_szCharacterName);
					GUI.Label(new Rect(Screen.width * 0.3f, Screen.height * 0.17f + yOffset, 200, fTextHeight), character.m_szWeaponName + "   " + character.m_nWeaponLevel.ToString());
					if(GUI.Button(new Rect(Screen.width * 0.3f, Screen.height * 0.21f + yOffset, 200, fTextHeight), character.m_szWeaponModifierName, "Label"))
					{
						m_nWeaponIter = counter;
						m_bWeaponChosen = true;
						m_nConfirmIter = 0;
					}
					
					
					GUI.Box(new Rect(Screen.width * 0.57f, Screen.height * 0.12f + yOffset, Screen.width * 0.2f, Screen.height * 0.22f), "[Portrait]");
					yOffset += Screen.height * 0.22f;
					GUI.skin.label.fontSize = tempFontHolder;
					counter++;
				}
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
				GUI.Box(new Rect(Screen.width * 0.3f, Screen.height * 0.21f + (Screen.height * 0.22f * m_nWeaponIter), 200, fTextHeight), "",selectorStyle);
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			}
			if(m_bWeaponChosen == true)
			{
				if(m_bEnhanceChosen == true)
				{
					//confirm purchase
					GUI.skin.label.fontSize = 20;
					GUI.Box(new Rect(Screen.width * 0.28f, Screen.height * 0.35f, Screen.width * 0.3f, Screen.height * 0.25f), "");
					GUI.Label(new Rect(Screen.width * 0.28f, Screen.height * 0.35f, Screen.width * 0.3f, Screen.height * 0.25f), 
					          "This will cost " + CalculateCost(dc.GetParty()[m_nWeaponIter].m_nWeaponLevel).ToString() + " Spyr\n" + 
					          "Do you wish to purchase this enhancement?");


					if(GUI.Button(new Rect(Screen.width * 0.32f, Screen.height * 0.52f, Screen.width * 0.05f, fTextHeight), "Yes", "Label"))
					{
						//purchase has been confirmed, do it!
						int sum = CalculateCost(dc.GetParty()[m_nWeaponIter].m_nWeaponLevel);
						if(sum != -1 && sum <= dc.m_nGold && dc.GetParty()[m_nWeaponIter].m_nWeaponLevel < m_nMaxEnhancementLevel)
						{
							dc.m_nGold -= sum;
							dc.GetParty()[m_nWeaponIter].m_nWeaponLevel++;
							dc.GetParty()[m_nWeaponIter].m_nWeaponDamageModifier += 5;
							m_bWeaponChosen = false;
						}
						else
						{
							//Either cannot afford, or the level of the weapon is too high.
						}
					}
					if(GUI.Button(new Rect(Screen.width * 0.46f, Screen.height * 0.52f, Screen.width * 0.05f, fTextHeight), "No", "Label"))
					{
						//purchase has been cancelled, just back up one
						m_nConfirmIter = 0;
						m_bWeaponChosen = false;
					}
					GUI.skin.label.fontSize = tempFontHolder;
					GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
					if(m_nConfirmIter == 0)
						GUI.Box(new Rect(Screen.width * 0.32f, Screen.height * 0.52f, Screen.width * 0.05f, fTextHeight), "",selectorStyle);
					else if(m_nConfirmIter == 1)
						GUI.Box(new Rect(Screen.width * 0.46f, Screen.height * 0.52f, Screen.width * 0.05f, fTextHeight), "",selectorStyle);
					GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				}
				else
				{
					//draw the modifier options
					GUI.Box(new Rect(Screen.width * 0.28f, Screen.height * 0.35f, Screen.width * 0.3f, Screen.height * 0.25f), "");

				}

			}
		}
	}
	int CalculateCost (int wpnLvl)
	{
		int sum = -1;
		if(wpnLvl < 4)
		{
			return (int)Mathf.Pow(10, wpnLvl+1);
		}
		else if(wpnLvl < 10)
		{
			int x = (int)Mathf.Pow(10, wpnLvl+1);
			for(int i = 0; i < wpnLvl - 3; ++i)
			{
				x *= 2;
			}
			return x;
		}
		return sum;
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
		GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
		Input.ResetInputAxes();
		m_bShowScreen = true;
	}
}
