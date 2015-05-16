using UnityEngine;
using System.Collections;

public class BossScript : MonoBehaviour
{
	public enum States {eIDLE, eWALKLEFT, eWALKRIGHT, eWALKUP, eWALKDOWN, eRUNLEFT, eRUNRIGHT, eRUNUP, eRUNDOWN, eATTACK};
	int m_nState;
	public int GetState() {return m_nState;}
	public void SetState(int state) {m_nState = state;}
	Animator m_aAnim;
	public Animator GetAnimator() {return m_aAnim;}
		//Direction the player is facing
		//0 : Down
		//1 : Left
		//2 : Right
		//3 : Up
	public int m_nFacingDir = 0;
	float m_fWalkingSpeed = 2.0f;
	float m_fAttackTime;
	public AnimationClip m_acAttack;
	// Use this for initialization
	void Start () 
	{
		m_fAttackTime = m_acAttack.length;
		m_aAnim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
		HandleState();
	}

	void HandleState()
	{
		switch(m_nState)
		{
		case (int)States.eIDLE:
		{
		}
			break;
		case (int)States.eWALKLEFT:
		{
			Vector3 newPos = transform.position;
			Vector3 MoveVec = new Vector3(-1, 0, 0);
			newPos += MoveVec * m_fWalkingSpeed * Time.deltaTime;
			transform.position = newPos;
		}
			break;
		case (int)States.eATTACK:
		{
			m_fAttackTime -= Time.deltaTime;
			if(m_fAttackTime <= 0.0f)
			{
				m_fAttackTime = m_acAttack.length;
				m_aAnim.SetBool("m_bIsAttacking", false);
				m_nState = (int)States.eIDLE;
			}

		}
			break;
		}
	}

	void OnTriggerEnter(Collider c)
	{
		if(c.tag == "Waypoint")
		{
			GameObject eventSys = GameObject.Find("EventWatcher");
			if(eventSys)
			{
				eventSys.GetComponent<EventHandler>().WaypointTriggered(c);
			}
		}
	}

	public void BeginAttack()
	{
		m_aAnim.SetBool("m_bIsAttacking", true);
		m_nState = (int)States.eATTACK;
	}
}
