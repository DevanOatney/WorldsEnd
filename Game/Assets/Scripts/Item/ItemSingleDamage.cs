using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSingleDamage : BaseItemScript
{
	bool m_bDraw = false;
	bool m_bEnterHasBeenPressed = false;
	void Start()
	{
		m_nItemType = (int)ITEM_TYPES.eSINGLE_DAMAGE;
		m_dFunc = SingleDamageFunction;
	}
	
	void Update()
	{
		if(m_bDraw == true)
		{
			if(Input.GetKeyUp(KeyCode.DownArrow))
				PositionTargetReticleDown(GetTargets());
			else if(Input.GetKeyUp(KeyCode.UpArrow))
			{
				PositionTargetReticleUp(GetTargets());
			}
			else if(Input.GetKeyUp(KeyCode.Return))
			{
				if(m_bEnterHasBeenPressed == false)
				{
					//turn on flag so the player can't keep using the item as the effect is happening
					m_bEnterHasBeenPressed = true;
					//turn off flag to even reach this .. so.. that last line of code was kind of pointless but.. meh
					m_bDraw = false;
					//Disable the rendering of the targetting cursors 
					DisableAllCursors();
					//start the animation
					//Do the effect
					GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
					foreach(GameObject enemy in Enemies)
					{
						//find the unit the player is trying to damage
						if(enemy.GetComponent<UnitScript>().m_nPositionOnField == m_pOwner.GetComponent<UnitScript>().m_nTargetPositionOnField)
						{
							//Instantiate the animation at the target location
							Vector2 pos = enemy.transform.position;
							GameObject animation = Instantiate(Resources.Load<GameObject>("Spell Effects/DamageItem")) as GameObject;
							animation.transform.position = pos;
							Destroy(animation, 1.4f);
						}
					}
					m_pOwner.GetComponent<Animator>().SetBool("m_bIsCasting", true);
					//in x amount of time, the player's turn is over and it's time to destroy this object
					Invoke("DoneAnimating", 1.5f);
					//Decrement the amount of that item in the inventory
					List<ItemLibrary.CharactersItems> inventory = GameObject.Find("PersistantData").GetComponent<DCScript>().m_lItemLibrary.m_lInventory;
					foreach(ItemLibrary.CharactersItems item in inventory.ToArray())
						if(item.m_szItemName == GetItemName())
							GameObject.Find("PersistantData").GetComponent<DCScript>().m_lItemLibrary.RemoveItem(item);
					//turn off the flags for the item/inventory rendering
					m_pOwner.GetComponent<CAllyBattleScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.USING_ITEM;

				}
			}
			else if(Input.GetKeyUp(KeyCode.Escape))
			{
				m_bDraw = false;
				m_pOwner.GetComponent<CAllyBattleScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.USEITEM_CHOSEN;
				DisableAllCursors();
				Destroy(gameObject);
			}
		}
	}

	void DoneAnimating()
	{
		//end the animation
		m_pOwner.GetComponent<Animator>().SetBool("m_bIsCasting", false);
		//Do the effect
		GameObject[] Enemies = GameObject.FindGameObjectsWithTag(GetTargets());
		foreach(GameObject enemy in Enemies)
		{
			//find the unit the player is trying to damage
			if(enemy.GetComponent<UnitScript>().m_nPositionOnField == m_pOwner.GetComponent<UnitScript>().m_nTargetPositionOnField)
			{
				//damage the unit
				enemy.GetComponent<UnitScript>().AdjustHP(GetHPMod());
			}
		}
		
		
		//end units turn
		m_pOwner.GetComponent<CAllyBattleScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.STATUS_EFFECTS;
		Destroy(gameObject);
	}

	new public void Initialize()
	{
		m_nItemType = (int)ITEM_TYPES.eSINGLE_DAMAGE;
		m_dFunc = SingleDamageFunction;
		SetTargets("Enemy");
	}

	public void SingleDamageFunction(GameObject pOwner)
	{
		m_pOwner = pOwner;
		m_bDraw = true;
		m_pOwner.GetComponent<CAllyBattleScript>().SetAllowInput(false);
		UnitScript us = m_pOwner.GetComponent<UnitScript>();
		InitializeTargetReticle(GetTargets());
		#region Enable/Disable Cursors
		if(us.m_nTargetPositionOnField == 0)
		{
			//disable 1, 2
			GameObject.Find("Enemy_Cursor1").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find("Enemy_Cursor2").GetComponent<SpriteRenderer>().enabled = false;
			//enable 0
			GameObject.Find("Enemy_Cursor0").GetComponent<SpriteRenderer>().enabled = true;
		}
		else if(us.m_nTargetPositionOnField == 1)
		{
			//disable 0, 2
			GameObject.Find("Enemy_Cursor0").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find("Enemy_Cursor2").GetComponent<SpriteRenderer>().enabled = false;
			//enable 1
			GameObject.Find("Enemy_Cursor1").GetComponent<SpriteRenderer>().enabled = true;
		}
		else if(us.m_nTargetPositionOnField == 2)
		{
			//disable 1, 0
			GameObject.Find("Enemy_Cursor1").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find("Enemy_Cursor0").GetComponent<SpriteRenderer>().enabled = false;
			//enable 0
			GameObject.Find("Enemy_Cursor2").GetComponent<SpriteRenderer>().enabled = true;
		}
		#endregion
	}
}