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
		if(m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm != null)
		{
			m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm.GetComponentInChildren<Animator>().SetBool("m_bIsCasting", false);
			m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm.GetComponent<SpriteRenderer>().enabled = false;
		}
		//Do the effect
		GameObject[] Enemies = GameObject.FindGameObjectsWithTag(GetTargets());
		foreach(GameObject enemy in Enemies)
		{
			//damage the unit
			enemy.GetComponent<UnitScript>().AdjustHP(GetItemModifier());
		}
		
		
		//end units turn
		GameObject tw = GameObject.Find("TurnWatcher");
		if(tw)
			tw.GetComponent<TurnWatcherScript>().MyTurnIsOver(m_pOwner);
		m_pOwner.GetComponent<PlayerBattleScript>().SetAllowInput(true);
		Destroy(gameObject);
	}

	public void GroupDamageFunction(GameObject pOwner)
	{
		m_pOwner = pOwner;
		m_bDraw = true;
		m_pOwner.GetComponent<PlayerBattleScript>().SetAllowInput(false);
		GameObject[] Enemies = GameObject.FindGameObjectsWithTag(GetTargets());
		foreach(GameObject enemy in Enemies)
		{
			GameObject.Find("Enemy_Cursor" + enemy.GetComponent<UnitScript>().m_nPositionOnField).GetComponent<SpriteRenderer>().enabled = true;
		}
	}
}
