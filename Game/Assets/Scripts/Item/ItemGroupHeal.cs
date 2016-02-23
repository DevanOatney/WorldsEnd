using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemGroupHeal : BaseItemScript
{
	public bool m_bShouldActivate = false;

	void Start()
	{
		m_nItemType = (int)ITEM_TYPES.eGROUP_HEAL;
		m_dFunc = GroupHealFunction;
	}
	
	void Update()
	{
		if(m_bShouldActivate == true)
		{
			m_bShouldActivate = false;
			GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
			foreach(GameObject ally in allies)
			{
				//Instantiate the animation at the target location
				Vector2 pos = ally.transform.position;
				GameObject animation = Instantiate(Resources.Load<GameObject>("Spell Effects/HealingItem")) as GameObject;
				animation.transform.position = pos;
				Destroy(animation, 1.4f);
			}
			m_pOwner.GetComponent<Animator>().SetBool("m_bIsCasting", true);
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
			//heal the unit (adjust hp is for taking damage.. so sending a negative number should heal
			ally.GetComponent<UnitScript>().AdjustHP(GetHPMod() * -1);

		}
		
		
		//end units turn
		m_pOwner.GetComponent<CAllyBattleScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.STATUS_EFFECTS;
		Destroy(gameObject);
	}

	new public void Initialize()
	{
		m_nItemType = (int)ITEM_TYPES.eGROUP_HEAL;
		m_dFunc = GroupHealFunction;
		SetTargets("Ally");
	}

	public void GroupHealFunction(GameObject pOwner)
	{
		m_pOwner = pOwner;

		GameObject[] Allies = GameObject.FindGameObjectsWithTag(GetTargets());
		foreach(GameObject ally in Allies)
		{
			//disable 1, 2
			GameObject.Find("Ally_Cursor" + ally.GetComponent<UnitScript>().FieldPosition).GetComponent<SpriteRenderer>().enabled = true;
		}
	}
}