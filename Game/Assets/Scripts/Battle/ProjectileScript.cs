﻿using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour 
{
	public float m_fSpeed;
	public float m_fRotationSpeed;
	public GameObject m_goTarget;
	public int m_nDamageDealt;
	public DCScript.cModifier m_mModToInflict = null;
	bool m_bHasHitTarget = false;
	public GameObject m_goPoisonArrowTrail;

	float m_fTrailTimer = 0.0f;
	float m_fTrailTimerBucket = 0.05f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bHasHitTarget == false)
		{
			Vector2 dest = m_goTarget.transform.position;
			Vector2 src = transform.position;
			Vector2 dir = dest - src;
			dir.Normalize();
			Vector3 pos = src + dir * m_fSpeed * Time.deltaTime;
			transform.position = pos;
			transform.Rotate(new Vector2(m_fRotationSpeed * Time.deltaTime, 0.0f));

			if (m_mModToInflict != null)
			{
				if (m_mModToInflict.m_eModifierType == DCScript.cModifier.eModifierType.ePOISON && m_goPoisonArrowTrail != null)
				{
					if (m_fTrailTimer >= m_fTrailTimerBucket)
					{
						m_fTrailTimer = 0.0f;
						GameObject _trail = Instantiate (m_goPoisonArrowTrail, transform.position, Quaternion.identity) as GameObject;
						Destroy (_trail, 0.2f);
					}
					else
					{
						m_fTrailTimer += Time.deltaTime;
					}
				}
			}
		}
	}

	void OnCollisionEnter(Collision c)
	{
		if (m_bHasHitTarget == true)
			return;
		if(c.gameObject.name == m_goTarget.name)
		{
			m_bHasHitTarget = true;
			if(m_nDamageDealt > 0)
			{
				//HIT!
				if (m_mModToInflict != null)
				{
					//There is some sort of modification on this attack, so handle that first.
					switch (m_mModToInflict.m_eModifierType)
					{
						case DCScript.cModifier.eModifierType.ePOISON:
							{
								if (m_mModToInflict.m_szModifierName == "Poison Shot")
								{
									//This is the first type of poison shot that the player can have. 
									DCScript.StatusEffect se = GameObject.Find("PersistantData").GetComponent<DCScript>().m_lStatusEffectLibrary.ConvertToDCStatusEffect("Poison");
									//Poison damage is equivalant to a percent of the damage that is going to be dealt.
									se.m_nHPMod = m_nDamageDealt * (m_mModToInflict.m_nModPower/100);
									m_goTarget.GetComponent<UnitScript> ().AddStatusEffect (se);
								}
							}
							break;
						case DCScript.cModifier.eModifierType.eDAMAGEINCREASE:
							{
								if (m_mModToInflict.m_szModifierName == "Charge Shot")
								{
									//This mod increases the damage, but reduces the hit chance.
									m_nDamageDealt *= (1 + (m_mModToInflict.m_nModPower/100));

									//Not really sure how to check the "hit chance" again since some of the data has gone, so let's just give it a .. 1 in 5 chance that the attack misses?
									if(Random.Range(1, 5) <= 1)
									{
										//so if we're in here, it's a miss.
										Miss();
										return;
									}
								}
							}
							break;
					}
				}
				m_goTarget.GetComponent<UnitScript>().AdjustHP(m_nDamageDealt);
				Destroy(gameObject, 0.2f);
				//set the position to the center'ish of the target (keeping in mind that this may be on either the left or right side of the screen)
				if (m_goTarget.tag == "Ally") {
					//target is on the right, put projectile on the center left of the collision box
					Vector3 pos = m_goTarget.transform.position;
					//pos.x -= m_goTarget.GetComponent<BoxCollider>().size.x;
					//pos.y -= m_goTarget.GetComponent<BoxCollider>().size.y * 0.5f;
					transform.position = pos;
				}
				else {
					//target is on the left, put projectile on the center right of the collision box
					Vector3 pos = m_goTarget.transform.position;
					pos.x += m_goTarget.GetComponent<BoxCollider>().size.x;
					pos.y += m_goTarget.GetComponent<BoxCollider>().size.y * 0.5f;
					transform.position = pos;
				}

			}
			else
			{
				//MISS!
				Miss();
			}
		}
	}

	void Miss()
	{
		m_goTarget.GetComponent<UnitScript>().Missed();
		Destroy(gameObject);
	}


}
