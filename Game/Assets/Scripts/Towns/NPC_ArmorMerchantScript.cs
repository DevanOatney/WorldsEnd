using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC_ArmorMerchantScript : NPCScript
{
	//flag for if the player has interracted with the merchant and their shop is about to open.
	bool m_bAboutToBeActive = false;
	//items that this merchant will sell
	public TextAsset m_taWares;
	List<MerchantItem> m_lItems = new List<MerchantItem>();
	DCScript dc;
	//scroll position of items that the merchant is selling
	Vector2 m_vScrollPosition = new Vector2(0,0);
	//white texture for selecting the items
	public Texture2D m_t2dTexture;

	public Texture2D[] m_t2dIconTextures;

	public class MerchantItem
	{
		//the name of the item
		public string m_szItemName = "";
		//how much a single unit of that item costs
		public int m_nCost = -1;
		//amount that the player has.
		public int m_nAmountCarried = 0;
		//description of the item
		public string m_szItemDescription = "";
		//type ID of the item
		public int m_nItemType = -1;
		//amount that the player wants to buy/sell
		public int m_nAmountToBarter = 0;

	}
	// Use this for initialization
	void Start ()
	{
		dc = GameObject.Find("PersistantData").GetComponent<DCScript>();
		m_aAnim = GetComponent<Animator>();
		LoadSteps();
		LoadItems();
	}

	//Should only ever hit this if you're in the editor and the intro menu screen has been skipped
	void DelayStart()
	{
		Debug.Log ("Editor mode, loading merchant data...");
		if(m_taWares != null)
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
					ItemLibrary.CharactersItems cItem = dc.m_lItemLibrary.GetItemFromInventory (newItem.m_szItemName);
					if (cItem == null)
					{
						//player doesn't have any of this item.
						newItem.m_nAmountCarried = 0;
						ItemLibrary.ItemData _itemData = dc.m_lItemLibrary.GetItemFromDictionary (newItem.m_szItemName);
						newItem.m_nItemType = _itemData.m_nItemType;
						newItem.m_szItemDescription = _itemData.m_szDescription;
					}
					else
					{
						newItem.m_nAmountCarried = cItem.m_nItemCount;
						newItem.m_nItemType = cItem.m_nItemType;
						newItem.m_szItemDescription = dc.m_lItemLibrary.GetItemFromDictionary(newItem.m_szItemName).m_szDescription;
					}
					m_lItems.Add(newItem);
				}
			}
		}
	}

	void LoadItems()
	{
		if(m_taWares != null)
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
					ItemLibrary.CharactersItems cItem = dc.m_lItemLibrary.GetItemFromInventory (newItem.m_szItemName);
					if (cItem == null)
					{
						//player doesn't have any of this item.
						newItem.m_nAmountCarried = 0;
						ItemLibrary.ItemData _itemData = dc.m_lItemLibrary.GetItemFromDictionary (newItem.m_szItemName);
						if (_itemData == null)
						{
							//We're playing in the editor and the main menu screen was skipped.
							Invoke("DelayStart", 2.0f);
							return;
						}
						newItem.m_nItemType = _itemData.m_nItemType;
						newItem.m_szItemDescription = _itemData.m_szDescription;
					}
					else
					{
						newItem.m_nAmountCarried = cItem.m_nItemCount;
						newItem.m_nItemType = cItem.m_nItemType;
						newItem.m_szItemDescription = dc.m_lItemLibrary.GetItemFromDictionary(newItem.m_szItemName).m_szDescription;
					}
					m_lItems.Add(newItem);
				}
			}
		}
	}

	void LoadSellItems()
	{
		m_lItems.Clear();
		foreach(ItemLibrary.CharactersItems item in dc.m_lItemLibrary.m_lInventory)
		{
			MerchantItem newItem = new MerchantItem();
			newItem.m_szItemName = item.m_szItemName;
			newItem.m_nAmountCarried = item.m_nItemCount;
			newItem.m_nAmountToBarter = 0;
			newItem.m_nItemType = dc.m_lItemLibrary.GetItemFromDictionary(item.m_szItemName).m_nItemType;
			newItem.m_nCost = dc.m_lItemLibrary.GetItemFromDictionary(item.m_szItemName).m_nBaseValue;
			newItem.m_szItemDescription = dc.m_lItemLibrary.GetItemFromDictionary(item.m_szItemName).m_szDescription;
			m_lItems.Add(newItem);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		HandleMovement();
	}

	public void ExitShop()
	{
		GameObject player = GameObject.Find("Player");
		if(player)
		{
			player.GetComponent<FieldPlayerMovementScript>().ReleaseAllBinds();
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
	


	
	new public void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == "Action Box(Clone)")
		{
			if(GetComponentInChildren<MessageHandler>())
			{
				//set to about to be active
				m_bAboutToBeActive = true;
				m_bIsBeingInterractedWith = true;
				if(m_szDialoguePath != "")
				{
					GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
					GameObject.Find("Event system").GetComponent<BaseEventSystemScript>().HandleEvent(m_szDialoguePath);
				}
			}
		}
	}

	public void ActivateMerchantScreen()
	{
		if(m_bAboutToBeActive == true)
		{
			m_bAboutToBeActive = false;
			GameObject player = GameObject.Find("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			Input.ResetInputAxes();
          
			//GameObject.Find ("ShopScreen").GetComponent<ShopScreenScript> ().ActivateShop (m_lItems, gameObject);
		}
	}

	public void DontActivateMerchantScreen()
	{
		m_bAboutToBeActive = false;
	}
}
