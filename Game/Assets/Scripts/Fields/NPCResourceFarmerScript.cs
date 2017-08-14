using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCResourceFarmerScript : NPCScript 
{
	public List<GameObject> m_lWaypoints;
	int m_nCurrentWaypointTarget = 0;
	bool m_bPausePathfinding = false;
	// Use this for initialization
	void Start () 
	{
		m_aAnim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_bPausePathfinding == false)
		{
			if (m_lWaypoints.Count > 0)
			{
				HandleMovement ();
			}
		}
	}

	override public void OnTriggerEnter2D(Collider2D 	c)
	{
		if (c.gameObject.name == "Waypoint")
		{
			SetIdle ();
			m_bPausePathfinding = true;
			if (m_lWaypoints [m_nCurrentWaypointTarget].tag == "Harvest Point")
			{
				//Start the harvest animation.
				if (m_lWaypoints [m_nCurrentWaypointTarget].transform.position.x > m_lWaypoints [m_nCurrentWaypointTarget - 1].transform.position.x)
				{
					GetComponent<SpriteRenderer> ().flipX = !GetComponent<SpriteRenderer> ().flipX;
					StartCoroutine (ResumePathfinding (true, 5.0f));
				}
				else
				{
					StartCoroutine (ResumePathfinding (false, 5.0f));
				}
				m_aAnim.SetTrigger ("m_tSwing");
			}
			else
			{
				StartCoroutine (ResumePathfinding (false, 0.2f));
			}
		}
		base.OnTriggerEnter2D (c);
	}


	IEnumerator ResumePathfinding(bool _bToFlip, float _fDelay)
	{
		yield return new WaitForSeconds (_fDelay);

		if (_bToFlip == true)
		{
			GetComponent<SpriteRenderer> ().flipX = !GetComponent<SpriteRenderer> ().flipX;
		}
		m_nCurrentWaypointTarget += 1;

		if (m_nCurrentWaypointTarget >= m_lWaypoints.Count)
			m_nCurrentWaypointTarget = 0;
		m_aAnim.SetInteger ("m_nFacingDir", m_nFacingDir);
		DHF_NPCMoveToGameobject (m_lWaypoints [m_nCurrentWaypointTarget], false,m_nFacingDir, false);
		m_bPausePathfinding = false;
	}
}
