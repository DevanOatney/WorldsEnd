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
	Vector2 m_vScrollPosition = new Vector2(0,0);
	class MerchantItem
	{
		public string m_szItemName = "";
		public int m_nCost = -1;
		//amount that the player has.
		public int m_nAmountCarried = 0;
		public string m_szItemDescription = "";
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
			if(piece.Length > 1)
			{
				MerchantItem newItem = new MerchantItem();
				newItem.m_szItemName = piece[0].Trim();
				newItem.m_nCost = int.Parse(piece[2].Trim());
				DCScript.CharactersItems cItem = dc.GetItemFromInventory(newItem.m_szItemName);
				if(cItem == null)
					Debug.Log("Merchant item loading data failed");
				else
				{
					newItem.m_nAmountCarried = cItem.m_nItemCount;
					newItem.m_szItemDescription = dc.GetItemFromDictionary(newItem.m_szItemName).m_szDescription;
				}
				m_lItems.Add(newItem);
			}
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
			float screenHeight = Screen.height;
			float screenWidth = Screen.width;
			//Draw the background of the merchant screen
			GUI.Box(new Rect(0, 0, screenWidth, screenHeight), "");



			//Draw the background for BUY/SELL/EXIT
			GUI.Box (new Rect(0, 0, screenWidth * 0.295f, screenHeight * 0.285f), "BUY\nSELL\nEXIT");

			/*
				m_vItemScrollPosition = GUI.BeginScrollView(theBox,m_vItemScrollPosition, new Rect(0, 0, theBox.width*0.8f, yHeight * itemCount + heightAdjustment),false, false);
				foreach(DCScript.CharactersItems item in theInv)
				{
					if(item.m_nItemType >= (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL && item.m_nItemType <= (int)BaseItemScript.ITEM_TYPES.eGROUP_DAMAGE)
					{
						GUI.Button(new Rect(xPos, yPos, xWidth, yHeight), item.m_szItemName);
						yPos += yHeight;
					}
				}

				//draw selector
				GUIStyle selectorStyle = new GUIStyle(GUI.skin.box);
				selectorStyle.normal.background = m_t2dTexture;
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
				GUI.Box((new Rect(-2, m_vItemSecondScroll.y+2, xWidth +2, yHeight -2)), "",selectorStyle);

			 */
			//the height of the text
			float fTextHeight = 18.5f;
			//the current height to print text at
			float fHeight = fTextHeight;
			//the gap in between the bottom of one line and the top of the next line
			float fHeightAdjustment = 12.5f;
			Rect MerchantWareBox = new Rect(0, screenHeight*0.285f, screenWidth * 0.61f, screenHeight * 0.547f);
			GUI.Box(MerchantWareBox, "");
			//Draw the background for the Merchants Wares
			m_vScrollPosition = GUI.BeginScrollView(MerchantWareBox, m_vScrollPosition, 
			                                        new Rect(0, 0, MerchantWareBox.width * 0.8f, screenHeight *0.5f),false, false);
			foreach(MerchantItem item in m_lItems)
			{
				GUI.Label(new Rect(5, fHeight + fHeightAdjustment, 100, 18), item.m_szItemName);
				fHeight += fTextHeight;
			}
			GUI.EndScrollView();

			//Draw the background for comparisson of what the characters currently have equipped
			GUI.Box(new Rect(screenWidth*0.61f, screenHeight*0.285f, screenWidth * 0.391f, screenHeight*0.547f), "");



			//Draw the background for the description of the item selected
			GUI.Box(new Rect(0, screenHeight*0.832f, screenWidth, screenHeight * 0.166f), "");






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
