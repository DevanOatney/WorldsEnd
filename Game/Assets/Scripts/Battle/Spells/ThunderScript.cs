using UnityEngine;
using System.Collections;

public class ThunderScript  : BaseSpellBattleScript 
{
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
				GameObject animation = Instantiate(Resources.Load<GameObject>("Spell Effects/WindOrb")) as GameObject;
				animation.transform.position = pos;
				Destroy(animation, 1.4f);
			}
			m_goOwner.GetComponent<Animator>().SetBool("m_bIsCasting", true);
			//in x amount of time, the player's turn is over and it's time to destroy this object
			Invoke("DoneAnimating", 1.5f);
			//turn off the flags for the item/inventory rendering

		}
	}

	void DoneAnimating()
	{
		//end the animation
		m_goOwner.GetComponent<Animator>().SetBool("m_bIsCasting", false);
		//Do the effect
		GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject enemy in Enemies)
		{
			//damage the unit
			enemy.GetComponent<UnitScript>().AdjustHP(m_goOwner.GetComponent<UnitScript>().GetSTR() /2);
		}
		
		m_goOwner.GetComponent<CAllyBattleScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.STATUS_EFFECTS;
		Destroy(gameObject);
	}

	public override void Initialize(GameObject _pOwner)
	{
		m_goOwner = _pOwner;
		GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject enemy in Enemies)
		{
			GameObject.Find("Enemy_Cursor" + enemy.GetComponent<UnitScript>().FieldPosition).GetComponent<SpriteRenderer>().enabled = true;
		}
	}
	new public void DoneWithRuneEffect()
	{

	}
}
