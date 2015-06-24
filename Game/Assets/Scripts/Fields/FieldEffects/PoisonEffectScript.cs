﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//So the thing to remember is this has two "targets"   the first is the object that is moving about the field, to turn it green.
//The second is the actual character in your party that is taking damage.   Multiple units can be afflicted, therefor .. a list of strings maybe?

public class PoisonEffectScript : FieldBaseStatusEffectScript 
{
	//pointer to the object that turns green infront of the character
	public GameObject m_goPoisonClone;
	//amount of damage that is dealt to the effected units
	public int m_nDamagePerTick = 0;
	//how quickly these ticks should fire off
	float m_fRateOfTicks = 0.0f;
	//internal use for keeping track of tick fire rate
	float tickTimer = 0.0f;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public void Initialize(GameObject owner, List<string> szCharactersEffected, int damage, int tickAmount, float rateOfTick)
	{
		m_goOwner = owner;
		m_nDamagePerTick = damage;
		m_nAmountOfTicks = tickAmount;
		m_fRateOfTicks = rateOfTick;
		m_lEffectedUnits = szCharactersEffected;
		m_dFunc = Step;
	}

	void Step()
	{
		tickTimer += Time.deltaTime;
		if(tickTimer >= m_fRateOfTicks)
		{
			//reset timer
			tickTimer = 0.0f;
			//instantiate green image of player infront of the player on the field.
			m_goPoisonClone.GetComponent<SpriteRenderer>().sprite = m_goOwner.GetComponent<SpriteRenderer>().sprite;
			Vector3 cloneTransform = m_goOwner.transform.localScale;
			m_goPoisonClone.transform.localScale = cloneTransform;
			Vector3 pos = m_goOwner.transform.position;
			//adjust so the clone is behind the unit
			m_goPoisonClone.GetComponent<SpriteRenderer>().sortingOrder = 51;

			//create the green effect to show the player they're taking dmg
			GameObject poisonClone = Instantiate(m_goPoisonClone, pos, Quaternion.identity) as GameObject;
			poisonClone.GetComponent<PoisonCloneScript>().m_gOwner = m_goOwner;
			if(poisonClone)
				Destroy(poisonClone, 1.0f);


			//adjust the amount of times this effect can happen, as long as it's not infinite.
			if(m_nAmountOfTicks > -1)
			{
				m_nAmountOfTicks--;
				if(m_nAmountOfTicks <= 0)
				{
					m_bToBeRemoved = true;
				}
			
			}

			List<DCScript.CharacterData> characters = GameObject.Find("PersistantData").GetComponent<DCScript>().GetParty();
			if(m_lEffectedUnits != null)
			{
				foreach(string c in m_lEffectedUnits)
				{
					foreach(DCScript.CharacterData chara in characters)
					{
						if(chara.m_szCharacterName == c)
						{
							chara.m_nCurHP -= m_nDamagePerTick;
							if(chara.m_nCurHP <= 0)
								chara.m_nCurHP = 1;
						}
					}
				}
			}
		}
	}
}
