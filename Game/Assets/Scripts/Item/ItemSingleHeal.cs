﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSingleHeal : BaseItemScript
{
	bool m_bDraw = false;
	bool m_bEnterHasBeenPressed = false;

	void Start()
	{

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
					GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
					foreach(GameObject ally in allies)
					{
						//find the unit the player is trying to heal
						if(ally.GetComponent<UnitScript>().m_nPositionOnField == m_pOwner.GetComponent<UnitScript>().m_nTargetPositionOnField)
						{
							//Instantiate the animation at the target location
							Vector2 pos = ally.transform.position;
							GameObject animation = Instantiate(Resources.Load<GameObject>("Spell Effects/HealingItem")) as GameObject;
							animation.transform.position = pos;
							Destroy(animation, 1.4f);
						}
					}
					m_pOwner.GetComponent<Animator>().SetBool("m_bIsCasting", true);
					if(m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm != null)
					{
						m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm.GetComponentInChildren<Animator>().SetBool("m_bIsCasting", true);
						m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm.GetComponent<SpriteRenderer>().enabled = true;
					}
					//in x amount of time, the player's turn is over and it's time to destroy this object
					Invoke("DoneAnimating", 1.5f);
					//Decrement the amount of that item in the inventory
					List<DCScript.CharactersItems> inventory = GameObject.Find("PersistantData").GetComponent<DCScript>().GetInventory();
					foreach(DCScript.CharactersItems item in inventory.ToArray())
						if(item.m_szItemName == GetItemName())
							GameObject.Find("PersistantData").GetComponent<DCScript>().RemoveItem(item);
					//turn off the flags for the item/inventory rendering
					m_pOwner.GetComponent<PlayerBattleScript>().m_bIsMyTurn = false;
					m_pOwner.GetComponent<PlayerBattleScript>().SetItemChosen(false);
					m_pOwner.GetComponent<PlayerBattleScript>().SetChoosingItemFlag(false);

				}
			}
			else if(Input.GetKeyUp(KeyCode.Escape))
			{
				m_bDraw = false;
				m_pOwner.GetComponent<PlayerBattleScript>().SetAllowInput(true);
				m_pOwner.GetComponent<PlayerBattleScript>().TurnOffFlags();
				DisableAllCursors();
				Destroy(gameObject);
			}
		}
	}

	void DoneAnimating()
	{
		//end the animation
		m_pOwner.GetComponent<Animator>().SetBool("m_bIsCasting", false);
		if(m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm != null)
		{
			m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm.GetComponentInChildren<Animator>().SetBool("m_bIsCasting", false);
			m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm.GetComponent<SpriteRenderer>().enabled = false;
		}
		//Do the effect
		GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
		foreach(GameObject ally in allies)
		{
			//find the unit the player is trying to heal
			if(ally.GetComponent<UnitScript>().m_nPositionOnField == m_pOwner.GetComponent<UnitScript>().m_nTargetPositionOnField)
			{
				//heal the unit (adjust hp is for taking damage.. so sending a negative number should heal
				ally.GetComponent<UnitScript>().AdjustHP(GetHPMod() * -1);
			}
		}


		//end units turn
		m_pOwner.GetComponent<PlayerBattleScript>().EndMyTurn();
		m_pOwner.GetComponent<PlayerBattleScript>().SetAllowInput(true);
		Destroy(gameObject);
	}


	new public void Initialize()
	{
		m_nItemType = (int)ITEM_TYPES.eSINGLE_HEAL;
		m_dFunc = SingleHealFunction;
		SetTargets("Ally");
	}

	public void SingleHealFunction(GameObject pOwner)
	{
		m_pOwner = pOwner;
		m_bDraw = true;
		m_pOwner.GetComponent<PlayerBattleScript>().SetAllowInput(false);
		UnitScript us = m_pOwner.GetComponent<UnitScript>();
		InitializeTargetReticle("Ally");
		#region Enable/Disable Cursors
		if(us.m_nTargetPositionOnField == 0)
		{
			//disable 1, 2
			GameObject.Find("Ally_Cursor1").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find("Ally_Cursor2").GetComponent<SpriteRenderer>().enabled = false;
			//enable 0
			GameObject.Find("Ally_Cursor0").GetComponent<SpriteRenderer>().enabled = true;
		}
		else if(us.m_nTargetPositionOnField == 1)
		{
			//disable 0, 2
			GameObject.Find("Ally_Cursor0").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find("Ally_Cursor2").GetComponent<SpriteRenderer>().enabled = false;
			//enable 1
			GameObject.Find("Ally_Cursor1").GetComponent<SpriteRenderer>().enabled = true;
		}
		else if(us.m_nTargetPositionOnField == 2)
		{
			//disable 1, 0
			GameObject.Find("Ally_Cursor1").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find("Ally_Cursor0").GetComponent<SpriteRenderer>().enabled = false;
			//enable 0
			GameObject.Find("Ally_Cursor2").GetComponent<SpriteRenderer>().enabled = true;
		}
		#endregion
	}
}