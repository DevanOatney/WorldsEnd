using UnityEngine;
using System.Collections;

public class NPC_InnKeeperScript : MonoBehaviour 
{
	public int m_nCost = 10;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public void OnTriggerEnter2D(Collider2D c)
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
