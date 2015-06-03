using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC_ArmorMerchantScript : NPCScript
{
	//flag for if the player has interracted with the merchant and their shop is about to open.
	bool m_bAboutToBeActive = false;
	bool m_bShowScreen = false;
	//items that this merchant will sell
	public TextAsset m_taWares;
	List<MerchantItem> m_lItems = new List<MerchantItem>();
	DCScript dc;
	//scroll position of items that the merchant is selling
	Vector2 m_vScrollPosition = new Vector2(0,0);
	//white texture for selecting the items
	public Texture2D m_t2dTexture;

	//flags for which options have been selected
	bool m_bBuyIsChosen = false;
	bool m_bSellIsChosen = false;
	bool m_bItemIsChosen = false;

	//iterators for the different windows
	int m_nInitialSelectionIter = 0;
	int m_nItemIter = 0;
	int m_nConfirmIter = 0;

	//timer and bucket for delayed input when the player is pressing and holding right/left
	float m_fIncDecTimer = 0.0f;
	float m_fIncDecBucket = 0.1f;

	public Texture2D[] m_t2dIconTextures;

	class MerchantItem
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
					newItem.m_nItemType = cItem.m_nItemType;
					newItem.m_szItemDescription = dc.GetItemFromDictionary(newItem.m_szItemName).m_szDescription;
				}
				m_lItems.Add(newItem);
			}
		}
	}

	void LoadSellItems()
	{
		m_lItems.Clear();
		foreach(DCScript.CharactersItems item in dc.GetInventory())
		{
			MerchantItem newItem = new MerchantItem();
			newItem.m_szItemName = item.m_szItemName;
			newItem.m_nAmountCarried = item.m_nItemCount;
			newItem.m_nAmountToBarter = 0;
			newItem.m_nItemType = dc.GetItemFromDictionary(item.m_szItemName).m_nItemType;
			newItem.m_nCost = dc.GetItemFromDictionary(item.m_szItemName).m_nBaseValue;
			newItem.m_szItemDescription = dc.GetItemFromDictionary(item.m_szItemName).m_szDescription;
			m_lItems.Add(newItem);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		m_fIncDecTimer += Time.deltaTime;
		HandleMovement();
		HandleMerchanting();
		HandleInput();
	}

	void HandleMerchanting()
	{

	}

	void HandleInput()
	{
		if(m_bShowScreen)
		{
			if(Input.GetKeyUp(KeyCode.Escape))
			{
				//cycle back one selection until you're at the beginning of the merchant screen or exit
				if(m_bItemIsChosen == true)
				{
					m_bItemIsChosen = false;
					//reset final confirmation iter
					m_nConfirmIter = 0;
				}
				else if(m_bBuyIsChosen == true || m_bSellIsChosen == true)
				{
					m_bBuyIsChosen = m_bSellIsChosen = false;
					//reset item selected iter
					m_nItemIter = 0;
				}
				else
				{
					m_bShowScreen = false;
					m_nInitialSelectionIter = 0;
					GameObject player = GameObject.Find("Player");
					if(player)
					{
						player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
					}
				}
			}
			else if(Input.GetKeyUp(KeyCode.Return))
			{
				if(m_bBuyIsChosen == true && m_bItemIsChosen == false)
				{
					int sum = 0;
					foreach(MerchantItem item in m_lItems)
					{
						sum += item.m_nCost * item.m_nAmountToBarter;
					}
					if(sum > 0)
						m_bItemIsChosen = true;
					else if(m_lItems[m_nItemIter].m_nCost <= dc.m_nGold)
						m_bItemIsChosen = true;
				}
				else if(m_bSellIsChosen == true && m_bItemIsChosen == false)
				{
					m_bItemIsChosen = true;
				}
				else if(m_bBuyIsChosen == true && m_bItemIsChosen == true)
				{
					//purchase any items that have been selected, or just buy the item the player currently has selected
					if(m_nConfirmIter == 0)
					{
						dc.m_nGold -= GetTotalOwed(3);
						int counter = 0;
						foreach(MerchantItem item in m_lItems)
						{
							if(item.m_nAmountToBarter > 0)
							{
								counter++;
								DCScript.CharactersItems newItem = new DCScript.CharactersItems();
								newItem.m_szItemName = item.m_szItemName;
								newItem.m_nItemType = item.m_nItemType;
								newItem.m_nItemCount = item.m_nAmountToBarter;
								dc.AddItem(newItem);
								item.m_nAmountToBarter = 0;
							}
						}
						if(counter == 0)
						{
							DCScript.CharactersItems newItem = new DCScript.CharactersItems();
							newItem.m_szItemName = m_lItems[m_nItemIter].m_szItemName;
							newItem.m_nItemType = m_lItems[m_nItemIter].m_nItemType;
							newItem.m_nItemCount = m_lItems[m_nItemIter].m_nAmountToBarter;
							dc.AddItem(newItem);
						}
					}
					m_nConfirmIter = 0;
					m_bItemIsChosen = false;
					LoadItems();

				}
				else if(m_bSellIsChosen == true && m_bItemIsChosen == true)
				{
					//sell any items the player has indicated, or just sell 1 of the item that the player currently has selected
					if(m_nConfirmIter == 0)
					{
						dc.m_nGold += GetTotalOwed(2);
						int counter = 0;
						foreach(MerchantItem item in m_lItems)
						{
							if(item.m_nAmountToBarter > 0)
							{
								counter++;
								DCScript.CharactersItems newItem = new DCScript.CharactersItems();
								newItem.m_szItemName = item.m_szItemName;
								newItem.m_nItemCount = item.m_nAmountToBarter;
								dc.RemoveItem(newItem);
							}
							if(counter == 0)
							{

							}
						}
					}
					m_nConfirmIter = 0;
					m_nItemIter = 0;
					m_bItemIsChosen = false;
					LoadSellItems();
				}
				else if(m_bBuyIsChosen == false && m_bSellIsChosen == false)
				{
					//toggle buy or sell depending on what is selected
					if(m_nInitialSelectionIter == 0)
					{
						m_bBuyIsChosen = true;
						LoadItems();
					}
					else if(m_nInitialSelectionIter == 1)
					{
						m_bSellIsChosen = true;
						LoadSellItems();
					}
					else
					{
						//player chose to exit.
						m_bShowScreen = false;
						//reset initial iter
						m_nInitialSelectionIter = 0;
						GameObject player = GameObject.Find("Player");
						if(player)
						{
							player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
						}
					}
				}
			}
			else if(Input.GetKeyDown(KeyCode.UpArrow))
			{
				if(m_bBuyIsChosen == true  && m_bItemIsChosen == false)
				{
					m_nItemIter--;
					if(m_nItemIter < 0)
						m_nItemIter = m_lItems.Count-1;
				}
				else if( m_bSellIsChosen == true && m_bItemIsChosen == false)
				{
					m_nItemIter--;
					if(m_nItemIter < 0)
						m_nItemIter = m_lItems.Count-1;
				}
				else if(m_bBuyIsChosen == false && m_bSellIsChosen == false)
				{
					//toggle buy or sell depending on what is selected
					m_nInitialSelectionIter--;
					if(m_nInitialSelectionIter < 0)
						m_nInitialSelectionIter = 2;
				}
			}
			else if(Input.GetKeyDown(KeyCode.DownArrow))
			{
				if(m_bBuyIsChosen == true  && m_bItemIsChosen == false)
				{
					m_nItemIter++;
					if(m_nItemIter >= m_lItems.Count)
						m_nItemIter = 0;
				}
				else if( m_bSellIsChosen == true && m_bItemIsChosen == false)
				{
					m_nItemIter++;
					if(m_nItemIter >= m_lItems.Count)
						m_nItemIter = 0;
				}
				else if(m_bBuyIsChosen == false && m_bSellIsChosen == false)
				{
					//toggle buy or sell depending on what is selected
					m_nInitialSelectionIter++;
					if(m_nInitialSelectionIter > 2)
						m_nInitialSelectionIter = 0;
				}
			}
			else if(Input.GetKey(KeyCode.LeftArrow))
			{
				if(m_bBuyIsChosen == true && m_bItemIsChosen == false)
				{
					if(m_lItems[m_nItemIter].m_nAmountToBarter > 0)
					{
						if(m_fIncDecTimer >= m_fIncDecBucket)
						{
							m_lItems[m_nItemIter].m_nAmountToBarter--;
							m_fIncDecTimer = 0.0f;
						}
					}
				}
				else if(m_bSellIsChosen == true && m_bItemIsChosen == false)
				{
					//check to see if the amount of the item highlighted is greater than zero, if true decrement by one, else do nothing
					if(m_fIncDecTimer >= m_fIncDecBucket)
					{
						if(m_lItems.Count > 0)
						{
							if(m_lItems[m_nItemIter].m_nAmountToBarter > 0)
								m_lItems[m_nItemIter].m_nAmountToBarter--;
							m_fIncDecTimer = 0.0f;
						}
					}

				}
				else if(m_bBuyIsChosen == true || m_bSellIsChosen == true && m_bItemIsChosen == true)
				{
					if(m_fIncDecTimer >= m_fIncDecBucket + 0.2f)
					{
						m_nConfirmIter--;
						if(m_nConfirmIter < 0)
							m_nConfirmIter = 1;
						m_fIncDecTimer = 0.0f;
					}
				}
			}
			else if(Input.GetKey(KeyCode.RightArrow))
			{
				if(m_bBuyIsChosen == true && m_bItemIsChosen == false)
				{
					if(m_lItems[m_nItemIter].m_nAmountToBarter + m_lItems[m_nItemIter].m_nAmountCarried < 45)
					{
						if(GetTotalOwed(1) + m_lItems[m_nItemIter].m_nCost <= dc.m_nGold)
						{
							if(m_fIncDecTimer >= m_fIncDecBucket)
							{
								m_lItems[m_nItemIter].m_nAmountToBarter++;
								m_fIncDecTimer = 0.0f;
							}
						}
					}
				}
				else if(m_bSellIsChosen == true && m_bItemIsChosen == false)
				{
					//check to see if the amount of the item highlighted is greater than max count that can be purchased, if true increment by one, else do nothing
					if(m_fIncDecTimer >= m_fIncDecBucket)
					{
						if(m_lItems.Count > 0)
						{
							if(m_lItems[m_nItemIter].m_nAmountToBarter < m_lItems[m_nItemIter].m_nAmountCarried)
								m_lItems[m_nItemIter].m_nAmountToBarter++;
							m_fIncDecTimer = 0.0f;
						}
					}
				}
				else if(m_bBuyIsChosen == true || m_bSellIsChosen == true && m_bItemIsChosen == true)
				{
					if(m_fIncDecTimer >= m_fIncDecBucket + 0.2f)
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

	//flag - 3 Confirmation Buy, 2 Confirmation Sell, 1 BUY, 0 SELL
	int GetTotalOwed(int bsFlag)
	{
		if(bsFlag == 3)
		{
			//buying a single highlighted item from merchant
			bool noItemsSelected = true;
			int sum = 0;
			foreach(MerchantItem item in m_lItems)
			{
				if(item.m_nAmountToBarter > 0)
				{
					sum += item.m_nAmountToBarter * item.m_nCost;
					noItemsSelected = false;
				}
			}
			if(noItemsSelected == true)
				return m_lItems[m_nItemIter].m_nCost;
			return sum;

		}
		else if(bsFlag == 2)
		{
			bool noItemsSelected = true;
			int sum = 0;
			foreach(MerchantItem item in m_lItems)
			{
				if(item.m_nAmountToBarter > 0)
				{
					sum += item.m_nAmountToBarter * item.m_nCost;
					noItemsSelected = false;
				}
			}
			if(noItemsSelected == true)
				return m_lItems[m_nItemIter].m_nCost;
			return sum;
		}
		else if(bsFlag == 1)
		{
			int sum = 0;
			//buying stuff from the merchant
			foreach(MerchantItem item in m_lItems)
			{
				if(item.m_nAmountToBarter > 0)
					sum += item.m_nAmountToBarter * item.m_nCost;
			}
			return sum;
		}
		else
		{
			//selling stuff to the merchant
			int sum = 0;
			foreach(MerchantItem item in m_lItems)
			{
				if(item.m_nAmountToBarter > 0)
				{
					sum += item.m_nAmountToBarter * item.m_nCost;
				}
			}

			return sum;
		}
	}

	void OnGUI()
	{
		if(m_bShowScreen == true)
		{
			float screenHeight = Screen.height;
			float screenWidth = Screen.width;
			//Draw the background of the merchant screen
			GUI.Box(new Rect(0, 0, screenWidth, screenHeight), "");


			float firstWidth = screenWidth * 0.295f;
			float firstHeight = screenHeight * 0.285f;
			float firstLabelHeight = 20.0f;
			//Draw the background for BUY/SELL/EXIT
			GUI.Box (new Rect(0, 0, firstWidth, firstHeight), "BUY\nSELL\nEXIT");
			//draw selector
			GUIStyle selectorStyle = new GUIStyle(GUI.skin.box);
			selectorStyle.normal.background = m_t2dTexture;
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
			GUI.Box((new Rect(firstWidth*0.5f,  firstHeight*0.5f + firstLabelHeight * m_nInitialSelectionIter ,
			                 100.0f, firstLabelHeight + 5)), "",selectorStyle);
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

			//draw window for player's gold
			GUI.Box(new Rect(screenWidth*0.61f, firstHeight - screenHeight*0.05f, screenWidth *0.4f, screenHeight *0.05f), "");
			//draw icon for gold
			GUI.DrawTexture(new Rect(screenWidth*0.8f, firstHeight - screenHeight*0.05f+2, 25, 20), m_t2dIconTextures[10]);
			//draw out the amount of gold the player would have after the current transaction goes through
			int sum = 0;
			if(m_bBuyIsChosen)
				sum = GetTotalOwed(1) * -1;
			else if(m_bSellIsChosen)
				sum = GetTotalOwed(0);
			GUI.Label(new Rect(screenWidth*0.85f, firstHeight - screenHeight*0.05f+2, 100, 20), (dc.m_nGold + sum).ToString());
			/*
				Stuff used for all in the Wares window
			 */


			//the height of the text
			float fTextHeight = 18.5f;
			//the current height to print text at
			float fHeight = 0;
			//the gap in between the bottom of one line and the top of the next line
			float fHeightAdjustment = 12.5f;
			//distance from the left edge of the box
			float fWidthAdjustment = 12.5f;
			//the icon width of the item type 
			float fIconWidth = 32.0f;
			Rect MerchantWareBox = new Rect(0, screenHeight*0.285f, screenWidth * 0.61f, screenHeight * 0.547f);
			GUI.Box(MerchantWareBox, "");
			float itemCountYAdjust = (m_lItems.Count - (screenHeight%(int)fTextHeight)) * 10;
			if(itemCountYAdjust < 0)
				itemCountYAdjust = 0;
			//Draw the background for the Merchants Wares
			m_vScrollPosition = GUI.BeginScrollView(MerchantWareBox, m_vScrollPosition, 
			                                        new Rect(0, 0, MerchantWareBox.width * 0.8f, screenHeight *0.547f + itemCountYAdjust),false, true);

			/* IF BUY IS CHOSEN */
			if(m_bBuyIsChosen == true)
			{
				int itemCount = 0;
				foreach(MerchantItem item in m_lItems)
				{
					//Draw Icon of the item.
					if(item.m_nItemType >= (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL && item.m_nItemType <= (int)BaseItemScript.ITEM_TYPES.eGROUP_DAMAGE)
					{
						GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[0]);
					}
					else
					{
						switch(item.m_nItemType)
						{
						case (int)BaseItemScript.ITEM_TYPES.eHELMARMOR:
						{
							GUI.DrawTexture(new Rect(5, m_vScrollPosition.y + fHeightAdjustment + m_nItemIter * fTextHeight, 20, 20), m_t2dIconTextures[1]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eSHOULDERARMOR:
						{
							GUI.DrawTexture(new Rect(5, m_vScrollPosition.y + fHeightAdjustment + m_nItemIter * fTextHeight, 20, 20), m_t2dIconTextures[2]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eCHESTARMOR:
						{
							GUI.DrawTexture(new Rect(5, m_vScrollPosition.y + fHeightAdjustment + m_nItemIter * fTextHeight, 20, 20), m_t2dIconTextures[3]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eGLOVEARMOR:
						{
							GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[4]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eBELTARMOR:
						{
							GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[5]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eLEGARMOR:
						{
							GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[6]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eTRINKET:
						{
							GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[7]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eJUNK:
						{
							GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[8]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eKEYITEM:
						{
							GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[9]);
						}
							break;
							
						}
					}
					float xOff = fIconWidth + fWidthAdjustment;
					GUI.Label(new Rect(xOff, fHeight + fHeightAdjustment, 100, 18), item.m_szItemName);
					xOff += MerchantWareBox.width * 0.6f;
					GUI.Label(new Rect(xOff, fHeight + fHeightAdjustment, 50, 18), item.m_nCost.ToString());
					xOff += MerchantWareBox.width * 0.12f;
					GUI.Label(new Rect(xOff, fHeight + fHeightAdjustment, 25, 18), item.m_nAmountToBarter.ToString());
					xOff += MerchantWareBox.width * 0.1f;
					GUI.Label(new Rect(xOff, fHeight + fHeightAdjustment, 25, 18), item.m_nAmountCarried.ToString());
					fHeight += fTextHeight;
					itemCount++;
				}

				//draw selector
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
				GUI.Box((new Rect(fIconWidth + fWidthAdjustment-5,  fHeightAdjustment + m_nItemIter * fTextHeight,
				                  MerchantWareBox.width - (fIconWidth + fWidthAdjustment*2), fTextHeight-2)), "",selectorStyle);
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			}

			/*IF SELL IS CHOSEN*/
			if(m_bSellIsChosen == true)
			{
				//m_vScrollPosition.y + fHeightAdjustment + m_nItemIter * fTextHeight   (for position of where the selector is)
				foreach(MerchantItem item in m_lItems)
				{
					//Draw Icon of the item.
					if(item.m_nItemType >= (int)BaseItemScript.ITEM_TYPES.eSINGLE_HEAL && item.m_nItemType <= (int)BaseItemScript.ITEM_TYPES.eGROUP_DAMAGE)
					{
						GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[0]);
					}
					else
					{
						switch(item.m_nItemType)
						{
						case (int)BaseItemScript.ITEM_TYPES.eHELMARMOR:
						{
							GUI.DrawTexture(new Rect(5,  fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[1]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eSHOULDERARMOR:
						{
							GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[2]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eCHESTARMOR:
						{
							GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[3]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eGLOVEARMOR:
						{
							GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[4]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eBELTARMOR:
						{
							GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[5]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eLEGARMOR:
						{
							GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[6]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eTRINKET:
						{
							GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[7]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eJUNK:
						{
							GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[8]);
						}
							break;
						case (int)BaseItemScript.ITEM_TYPES.eKEYITEM:
						{
							GUI.DrawTexture(new Rect(5, fHeight + fHeightAdjustment, 20, 20), m_t2dIconTextures[9]);
						}
							break;
							
						}
					}
					//name
					float xOff = fIconWidth + fWidthAdjustment;
					GUI.Label(new Rect(xOff, fHeight + fHeightAdjustment, 100, 18), item.m_szItemName);
					xOff += MerchantWareBox.width * 0.6f;
					GUI.Label(new Rect(xOff, fHeight + fHeightAdjustment, 50, 18), item.m_nCost.ToString());
					xOff += MerchantWareBox.width * 0.12f;
					GUI.Label(new Rect(xOff, fHeight + fHeightAdjustment, 25, 18), item.m_nAmountToBarter.ToString());
					xOff += MerchantWareBox.width * 0.1f;
					GUI.Label(new Rect(xOff, fHeight + fHeightAdjustment, 25, 18), item.m_nAmountCarried.ToString());
					fHeight += fTextHeight;
				}

				//draw selector
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
				GUI.Box((new Rect(fIconWidth + fWidthAdjustment-5, fHeightAdjustment + m_nItemIter * fTextHeight,
				                  MerchantWareBox.width - (fIconWidth + fWidthAdjustment*2), fTextHeight-2)), "",selectorStyle);
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			}
			GUI.EndScrollView();


			//Draw the background for comparisson of what the characters currently have equipped
			GUI.Box(new Rect(screenWidth*0.61f, screenHeight*0.285f, screenWidth * 0.391f, screenHeight*0.547f), "");



			//Draw the background for the description of the item selected
			GUI.Box(new Rect(0, screenHeight*0.832f, screenWidth, screenHeight * 0.166f), "");
			if(m_bBuyIsChosen == true && m_lItems.Count > 0)
				GUI.Label(new Rect(0, screenHeight*0.832f, screenWidth, screenHeight * 0.166f), m_lItems[m_nItemIter].m_szItemDescription);
			if(m_bSellIsChosen == true && m_lItems.Count > 0)
				GUI.Label(new Rect(screenWidth*0.02f, screenHeight*0.84f, screenWidth, screenHeight * 0.166f), dc.GetItemFromDictionary(dc.GetInventory()[m_nItemIter].m_szItemName).m_szDescription);

			//Draw the background for the items stat block
			GUI.Box(new Rect(screenWidth * 0.7f, screenHeight*0.832f, screenWidth, screenHeight), "");
			GUI.Box(new Rect(screenWidth * 0.85f, screenHeight*0.84f, screenWidth * 0.15f, screenHeight * 0.035f), "HP: ");
			GUI.Box(new Rect(screenWidth * 0.85f, screenHeight*0.875f, screenWidth * 0.15f, screenHeight * 0.035f), "POW: ");
			GUI.Box(new Rect(screenWidth * 0.85f, screenHeight*0.91f, screenWidth * 0.15f, screenHeight * 0.035f), "DEF: ");
			GUI.Box(new Rect(screenWidth * 0.85f, screenHeight*0.945f, screenWidth * 0.15f, screenHeight * 0.035f), "SPD: ");


			DrawStatBlock();

			if(m_bItemIsChosen == true)
			{
				if(m_bBuyIsChosen == true)
				{
					string szMessage = "\n\nThis will cost " + GetTotalOwed(3).ToString();
					GUI.Box(new Rect(screenWidth * 0.4f, screenHeight * 0.4f, screenWidth * 0.2f, screenHeight * 0.2f), szMessage);


					GUI.Label(new Rect(screenWidth * 0.45f, screenHeight* 0.5f, 50, fTextHeight), "Yes");
					GUI.Label(new Rect(screenWidth * 0.525f, screenHeight* 0.5f, 50, fTextHeight), "No");
					//draw selector
					GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
					if(m_nConfirmIter == 0)
						GUI.Box((new Rect(screenWidth * 0.445f,  screenHeight* 0.5f,
						                  screenWidth * 0.0325f, fTextHeight)), "",selectorStyle);
					else if(m_nConfirmIter == 1)
						GUI.Box((new Rect(screenWidth * 0.520f,  screenHeight* 0.5f,
						                  screenWidth * 0.03f, fTextHeight)), "",selectorStyle);
					GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

				}
				else if(m_bSellIsChosen == true)
				{
					string szMessage = "\n\nThis will earn you " + GetTotalOwed(2).ToString();
					GUI.Box(new Rect(screenWidth * 0.4f, screenHeight * 0.4f, screenWidth * 0.2f, screenHeight * 0.2f), szMessage);
					
					
					GUI.Label(new Rect(screenWidth * 0.45f, screenHeight* 0.5f, 50, fTextHeight), "Yes");
					GUI.Label(new Rect(screenWidth * 0.525f, screenHeight* 0.5f, 50, fTextHeight), "No");
					//draw selector
					GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
					if(m_nConfirmIter == 0)
						GUI.Box((new Rect(screenWidth * 0.445f,  screenHeight* 0.5f,
						                  screenWidth * 0.0325f, fTextHeight)), "",selectorStyle);
					else if(m_nConfirmIter == 1)
						GUI.Box((new Rect(screenWidth * 0.520f,  screenHeight* 0.5f,
						                  screenWidth * 0.03f, fTextHeight)), "",selectorStyle);
					GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

				}
			}




		}
	}

	void DrawStatBlock()
	{
		if(m_lItems.Count > 0)
		{
			string szName;
			szName = m_lItems[m_nItemIter].m_szItemName;
			
			DCScript.ItemData item = dc.GetItemFromDictionary(szName);
			
			GUI.Box(new Rect(Screen.width * 0.9f, Screen.height*0.84f, Screen.width * 0.15f, Screen.height * 0.035f), item.m_nHPMod.ToString());
			GUI.Box(new Rect(Screen.width * 0.9f, Screen.height*0.875f, Screen.width * 0.15f, Screen.height * 0.035f), item.m_nPowMod.ToString());
			GUI.Box(new Rect(Screen.width * 0.9f, Screen.height*0.91f, Screen.width * 0.15f, Screen.height * 0.035f), item.m_nDefMod.ToString());
			GUI.Box(new Rect(Screen.width * 0.9f, Screen.height*0.945f, Screen.width * 0.15f, Screen.height * 0.035f), item.m_nSpdMod.ToString());
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
			Input.ResetInputAxes();
		}
	}
}
