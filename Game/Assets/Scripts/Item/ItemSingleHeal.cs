using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSingleHeal : BaseItemScript
{
	public bool m_bShouldActivate = false;

	void Start()
	{

	}
	
	void Update()
	{
		if(m_bShouldActivate == true)
		{
			m_bShouldActivate = false;
			GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
			foreach(GameObject ally in allies)
			{
				//find the unit the player is trying to heal
				if(ally.GetComponent<UnitScript>().FieldPosition == m_pOwner.GetComponent<UnitScript>().m_nTargetPositionOnField)
				{
					//Instantiate the animation at the target location
					Vector2 pos = ally.transform.position;
					GameObject newEffect = Instantiate(m_goEffect);
					newEffect.transform.position = pos;
					Destroy(newEffect, 1.4f);
				}
			}
			m_pOwner.GetComponent<UnitScript>().m_aAnim.SetBool("m_bIsCasting", true);
			//in x amount of time, the player's turn is over and it's time to destroy this object
			Invoke("DoneAnimating", 1.5f);
			//Decrement the amount of that item in the inventory
			List<ItemLibrary.CharactersItems> inventory = GameObject.Find("PersistantData").GetComponent<DCScript>().m_lItemLibrary.m_lInventory;
			foreach(ItemLibrary.CharactersItems item in inventory.ToArray())
				if(item.m_szItemName == GetItemName())
					GameObject.Find("PersistantData").GetComponent<DCScript>().m_lItemLibrary.RemoveItem(item);
		}
	}

	void DoneAnimating()
	{
		//end the animation
		m_pOwner.GetComponent<Animator>().SetBool("m_bIsCasting", false);
		//Do the effect
		GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
		foreach(GameObject ally in allies)
		{
			//find the unit the player is trying to heal
			if(ally.GetComponent<UnitScript>().FieldPosition == m_pOwner.GetComponent<UnitScript>().m_nTargetPositionOnField)
			{
				//heal the unit (adjust hp is for taking damage.. so sending a negative number should heal
				ally.GetComponent<UnitScript>().AdjustHP(GetHPMod() * -1);
			}
		}


		//end units turn
		m_pOwner.GetComponent<CAllyBattleScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.STATUS_EFFECTS;
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
	}
}