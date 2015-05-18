using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC_ArmorMerchantScript : NPCScript
{
	//flag for if the player has interracted with the merchant and their shop is about to open.
	bool m_bAboutToBeActive = false;
	bool m_bShowScreen = false;
	public TextAsset m_taWares;
	List<MerchantItem> m_lItems = new List<MerchantItem>();
	DCScript dc;

	class MerchantItem
	{
		public string m_szItemName = "";
		public int m_nCost = -1;
		//amount that the player has.
		public int m_nAmountCarried = 0;
	}
	// Use this for initialization
	void Start ()
	{
		dc = GameObject.Find("PersistantData").GetComponent<DCScript>();
		m_aAnim = GetComponent<Animator>();
		LoadSteps();
		LoadItems();
	}

	void LoadItems()
	{
		m_lItems.Clear();
		string[] itemLines = m_taWares.text.Split('\n');
		foreach(string item in itemLines)
		{
			string[] piece = item.Split(',');
			MerchantItem newItem = new MerchantItem();
			newItem.m_szItemName = piece[0].Trim();
			newItem.m_nCost = int.Parse(piece[1].Trim());
			DCScript.CharactersItems cItem = dc.GetItemFromInventory(newItem.m_szItemName);
			if(cItem == null)
				Debug.Log("Merchant item loading data failed");
			else
			{

			}
			m_lItems.Add(newItem);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		HandleMovement();
		HandleMerchanting();
	}

	void HandleMerchanting()
	{

	}

	void OnGUI()
	{
		if(m_bShowScreen == true)
		{
			GUI.Box(new Rect(0, 0, 200, 200), "");
		}
	}

	//Get the direction the NPC is looking
	Vector2 GetNPCFacing()
	{
		//Down, Left, Right, Up
		switch(m_nFacingDir)
		{
		case 0:
			return new Vector2(0, -1);
		case 1:
			return new Vector2(-1, 0);
		case 2:
			return new Vector2(1, 0);
		case 3:
			return new Vector2(0, 1);
		}
		return new Vector2(0, 0);
	}
	
	//Load the steps of the NPC
	void LoadSteps()
	{
		string[] lines = m_taPathing.text.Split('\n');
		foreach(string step in lines)
		{
			cSteps newStep = new cSteps();
			string[] pieces = step.Split(',');
			newStep.nDirection = int.Parse(pieces[0].Trim());
			newStep.fTime = float.Parse(pieces[1].Trim());
			newStep.bMove = bool.Parse(pieces[2].Trim());
			newStep.bOneShot = bool.Parse(pieces[3].Trim());
			m_lSteps.Add(newStep);
		}
	}

	
	new public void OnTriggerEnter(Collider c)
	{
		if(c.name == "Action Box(Clone)")
		{
			if(GetComponent<MessageHandler>())
			{
				//set to about to be active
				m_bAboutToBeActive = true;
				m_bIsMoving = false;
				m_bIsBeingInterractedWith = true;
				if(m_szDialoguePath != "")
					GameObject.Find("Event System").GetComponent<BaseEventSystemScript>().HandleEvent(m_szDialoguePath);
			}
		}
	}

	public void ActivateMerchantScreen()
	{
		if(m_bAboutToBeActive == true)
		{
			m_bAboutToBeActive = false;
			m_bShowScreen = true;
			GameObject player = GameObject.Find("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
		}
	}
}
