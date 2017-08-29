using UnityEngine;
using System.Collections;

public class BoarRitualScript : MonoBehaviour 
{
	bool m_bIsActive = false;
	private bool m_bIsAttacking = false;
	public AnimationClip m_acAttackAnim;
	float m_fAttackBucket = 0.0f;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bIsActive == true)
		{
			GetComponent<Rigidbody2D>().velocity = new Vector2(-5,0);
		}
		else if(m_bIsAttacking == true)
		{
			m_fAttackBucket += Time.deltaTime;
			if(m_fAttackBucket >= m_acAttackAnim.length)
			{
				m_fAttackBucket = 0.0f;
				m_bIsAttacking = false;
				GameObject.Find("Event system").GetComponent<BaseEventSystemScript>().HandleEvent("StartBoarBattle");
			}
		}
	}

	public void ActivateBoar()
	{
		m_bIsActive = true;
		GetComponent<Animator>().Play("Boar_Charge");
	}

	void OnCollisionEnter2D(Collision2D c)
	{
		if(c.gameObject.name == "Player")
		{
			GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
			m_bIsActive = false;
			m_bIsAttacking = true;
			GetComponent<Animator>().SetBool("m_bAttack", true);
			Invoke ("DelayStart", 0.4f);
		}
	}

	void DelayStart()
	{
		Camera.main.GetComponent<CameraFollowTarget>().m_bShouldSwirl = true;
		Camera.main.GetComponent<VEffects>().SendMessage("StartBlur");
	}

}
