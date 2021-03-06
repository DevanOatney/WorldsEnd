﻿using UnityEngine;
using System.Collections;

public class ThunderScript  : BaseSpellBattleScript 
{
	public GameObject m_goThunderEffect;
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
				GameObject newThunderEffect = Instantiate(m_goThunderEffect);
				newThunderEffect.transform.position = pos;
				Destroy(newThunderEffect, 1.4f);
			}
			//in x amount of time, the player's turn is over and it's time to destroy this object
			Invoke("DoneAnimating", 1.5f);
			//turn off the flags for the item/inventory rendering
		}
	}

	public override void DoneAnimating()
	{
		//Do the effect
		GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject enemy in Enemies)
		{
			//damage the unit
			enemy.GetComponent<UnitScript>().AdjustHP(m_goOwner.GetComponent<UnitScript>().GetTempSTR() /2);
			foreach(string effect in m_lStatusEffect)
			{
				enemy.GetComponent<UnitScript>().AddStatusEffect(GameObject.Find("PersistantData").GetComponent<DCScript>().m_lStatusEffectLibrary.ConvertToDCStatusEffect(effect));
			}
		}
		

		base.DoneAnimating();
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
