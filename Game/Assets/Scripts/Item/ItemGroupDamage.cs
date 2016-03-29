using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemGroupDamage : BaseItemScript
{
	public bool m_bShouldActivate;
	void Start()
	{

	}

	void Update()
	{
		if(m_bShouldActivate == true)
		{
			m_bShouldActivate = false;
			GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach(GameObject enemy in Enemies)
			{
				//Instantiate the animation at the target location
				Vector2 pos = enemy.transform.position;
				GameObject newEffect = Instantiate(m_goEffect);
				newEffect.transform.position = pos;
				Destroy(newEffect, 1.4f);
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

	new public void Initialize()
	{
		m_nItemType = (int)ITEM_TYPES.eGROUP_DAMAGE;
		m_dFunc = GroupDamageFunction;
		SetTargets("Enemy");
	}

	void DoneAnimating()
	{
		//end the animation
		m_pOwner.GetComponent<UnitScript>().m_aAnim.SetBool("m_bIsCasting", false);
		//Do the effect
		GameObject[] Enemies = GameObject.FindGameObjectsWithTag(GetTargets());
		foreach(GameObject enemy in Enemies)
		{
			//damage the unit
			enemy.GetComponent<UnitScript>().AdjustHP(GetHPMod());
		}
		
		
		//end units turn
		m_pOwner.GetComponent<CAllyBattleScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.STATUS_EFFECTS;
		Destroy(gameObject);
	}

	public void GroupDamageFunction(GameObject pOwner)
	{
		m_pOwner = pOwner;
		GameObject[] Enemies = GameObject.FindGameObjectsWithTag(GetTargets());
		foreach(GameObject enemy in Enemies)
		{
			GameObject.Find("Enemy_Cursor" + enemy.GetComponent<UnitScript>().FieldPosition).GetComponent<SpriteRenderer>().enabled = true;
		}
	}
}
