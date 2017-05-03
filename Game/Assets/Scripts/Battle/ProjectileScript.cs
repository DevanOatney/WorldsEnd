﻿using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour 
{
	public float m_fSpeed;
	public float m_fRotationSpeed;
	public GameObject m_goTarget;
	public int m_nDamageDealt;
	bool m_bHasHitTarget = false;

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
		}
	}

	void OnCollisionEnter(Collision c)
	{
		if (m_bHasHitTarget == true)
			return;
		if(c.gameObject.name == m_goTarget.name)
		{
			m_bHasHitTarget = true;
			if(m_nDamageDealt >0)
			{
				//HIT!
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
				m_goTarget.GetComponent<UnitScript>().Missed();
				Destroy(gameObject);
			}
		}
	}


}
