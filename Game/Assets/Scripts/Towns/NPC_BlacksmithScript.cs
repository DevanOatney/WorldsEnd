using UnityEngine;
using System.Collections;

public class NPC_BlacksmithScript : NPCScript 
{
	bool m_bShowScreen = false;
	// Use this for initialization
	void Start ()
	{
		LoadSteps();
	}
	
	// Update is called once per frame
	void Update () 
	{
		HandleMovement();
	}

	void OnGUI()
	{
		if(m_bShowScreen)
		{
			GUI.Box(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.2f, Screen.height * 0.7f), "");
		}
	}

	new public void OnTriggerEnter(Collider c)
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
}
