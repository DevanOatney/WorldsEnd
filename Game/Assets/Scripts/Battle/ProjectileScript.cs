using UnityEngine;
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
		m_bHasHitTarget = true;
		if(m_nDamageDealt >0)
		{
			//HIT!
			m_goTarget.GetComponent<UnitScript>().AdjustHP(m_nDamageDealt);
			Destroy(gameObject, 0.2f);
			Vector3 pos = m_goTarget.transform.position;
			pos.x += m_goTarget.GetComponent<BoxCollider>().size.x;
			pos.y += m_goTarget.GetComponent<BoxCollider>().size.y * 0.5f;
			transform.position = pos;
		}
		else
		{
			//MISS!
			Destroy(gameObject);
		}
	}

}
