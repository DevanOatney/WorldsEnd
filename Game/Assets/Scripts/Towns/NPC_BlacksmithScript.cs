using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC_BlacksmithScript : NPCScript 
{

	DCScript dc;

	//The maximum amount that this blacksmith can enhance to
	public int m_nMaxEnhancementLevel = 3;

	GameObject m_goBlacksmithShopUI;


	// Use this for initialization
	void Start ()
	{
		dc = GameObject.Find("PersistantData").GetComponent<DCScript>();
		m_goBlacksmithShopUI = GameObject.Find ("BlacksmithShopUI");
		LoadSteps();
	}
	
	// Update is called once per frame
	void Update () 
	{
		HandleMovement();
	}
		
	public void CloseShop()
	{
		GameObject.Find ("Player").GetComponent<FieldPlayerMovementScript> ().ReleaseAllBinds ();
		m_bIsBeingInterractedWith = false;
	}

	public int CalculateCost (int wpnLvl)
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

	override public void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == "Action Box(Clone)")
		{
			if(GetComponentInChildren<MessageHandler>())
			{
				//set to about to be active
				m_bIsBeingInterractedWith = true;
				if(m_szDialoguePath != "")
				{
					GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
					GameObject.Find("Event system").GetComponent<BaseEventSystemScript>().HandleEvent(m_szDialoguePath);
				}
			}
		}
	}

	public void ActivateScreen()
	{
		GameObject.Find ("Player").GetComponent<FieldPlayerMovementScript> ().BindInput ();
		Input.ResetInputAxes ();
		m_goBlacksmithShopUI.GetComponent<BlacksmithShopUIScript> ().TurnOn (gameObject);
	}
}
