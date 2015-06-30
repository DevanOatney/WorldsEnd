using UnityEngine;
using System.Collections;

public class NPC_InnKeeperScript : NPCScript 
{
	int m_nSelectionIter = 0;
	public Texture2D m_t2dSelectionTexture;
	public int m_nCost = 10;
	public bool m_bShowScreen = false;
	//timer and bucket for delayed input when the player is pressing and holding right/left
	float m_fIncDecTimer = 0.0f;
	float m_fIncDecBucket = 0.2f;
	// Use this for initialization
	void Start () 
	{
		
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
	}

	void OnGUI()
	{
		if(m_bShowScreen)
		{
			GUI.Box(new Rect(Screen.width*0.1f, Screen.height*0.1f, Screen.width*0.7f, Screen.height*0.7f), "");
			GUIStyle selectorStyle = new GUIStyle(GUI.skin.box);
			selectorStyle.normal.background = m_t2dSelectionTexture;
			float fTextHeight = 30.0f;
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
			
			GUI.Box((new Rect(Screen.width * 0.12f,  Screen.height * 0.12f + fTextHeight * m_nSelectionIter ,
			                  Screen.width * 0.15f, fTextHeight + 2)), "",selectorStyle);
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
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
		GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
		Input.ResetInputAxes();
		m_bShowScreen = true;
	}
}
