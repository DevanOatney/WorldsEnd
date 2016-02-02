using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemGroupDamage : BaseItemScript
{
	bool m_bDraw = false;
	void Start()
	{

	}

	void Update()
	{
		if(m_bDraw == true)
		{

			if(Input.GetKeyUp(KeyCode.Return))
			{
				//turn off flag to even reach this
				m_bDraw = false;
				//Disable the rendering of the targetting cursors 
				DisableAllCursors();
				//start the animation
				//Do the effect
				GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
				foreach(GameObject enemy in Enemies)
				{
					//Instantiate the animation at the target location
					Vector2 pos = enemy.transform.position;
					GameObject animation = Instantiate(Resources.Load<GameObject>("Spell Effects/DamageItem")) as GameObject;
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
				//turn off the flags for the item/inventory rendering
				m_pOwner.GetComponent<CAllyBattleScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.USING_ITEM;
			}
			else if(Input.GetKeyUp(KeyCode.Escape))
			{
				m_bDraw = false;
				m_pOwner.GetComponent<CAllyBattleScript>().SetAllowInput(true);
				m_pOwner.GetComponent<CAllyBattleScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.USEITEM_CHOSEN;
				DisableAllCursors();
				Destroy(gameObject);
			}
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
		m_pOwner.GetComponent<Animator>().SetBool("m_bIsCasting", false);
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
		m_bDraw = true;
		m_pOwner.GetComponent<CAllyBattleScript>().SetAllowInput(false);
		GameObject[] Enemies = GameObject.FindGameObjectsWithTag(GetTargets());
		foreach(GameObject enemy in Enemies)
		{
			GameObject.Find("Enemy_Cursor" + enemy.GetComponent<UnitScript>().m_nPositionOnField).GetComponent<SpriteRenderer>().enabled = true;
		}
	}
}
