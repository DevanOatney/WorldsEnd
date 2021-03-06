﻿using UnityEngine;
using System.Collections;

public class WB_UnitScript : MonoBehaviour 
{
	//Flags
	bool m_bStartDeath = false;
	bool m_bHasDied = false;
	bool m_bShouldAttack = false;
    bool m_bHasReachedDestination = false;
    public bool m_bIsShootingArrow = false;
	public bool m_bShouldMove = false;
	//Stats
	float m_fMovementSpeed = 100.0f;
	//Add in a bit of an offset to where we stop, this is to make the fight happen quick and the retreat take a little longer.
	float m_fStopOffset = 0.0f;
	//-1 to move left, +1 to move right
	int m_nDirection = -1;
	//Hooks
	public GameObject m_goDeathFlash;
    public GameObject m_goArrow;
	GameObject m_goController;
	GameObject m_goLeftBound, m_goRightBound, m_goLeftAttack, m_goRightAttack;

	public void Initialize(GameObject _controller, int _nDirection, bool _amIDead)
	{
		m_goController = _controller;
		m_goLeftBound = _controller.GetComponent<FightSceneControllerScript>().m_goLeftSide;
		m_goRightBound = _controller.GetComponent<FightSceneControllerScript>().m_goRightSide;
		m_goLeftAttack = _controller.GetComponent<FightSceneControllerScript>().m_goLeftAttack;
		m_goRightAttack = _controller.GetComponent<FightSceneControllerScript>().m_goRightAttack;
		m_nDirection = _nDirection;
		if(_amIDead == true)
		{
			m_bStartDeath = true;
			m_bHasDied = true;
			GetComponent<Animator>().SetBool("m_bAlreadyDead", true);
		}
	}

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_goController.GetComponent<FightSceneControllerScript> ().m_bPauseFightScene == false)
		{
			//Check to see if we're in the "swinging zone" and if we haven't started swinging, swing.  If we leave the swinging zone, stop swinging
			if (transform.GetComponent<RectTransform> ().anchoredPosition.x >= m_goLeftAttack.GetComponent<RectTransform> ().anchoredPosition.x && transform.GetComponent<RectTransform> ().anchoredPosition.x <= m_goRightAttack.GetComponent<RectTransform> ().anchoredPosition.x && m_bStartDeath == false)
			{
				//in here, we're in the bounds of the swinging zone.
				if (m_bShouldAttack == false)
				{
					//in here, we haven't started swinging yet.
					m_bShouldAttack = true;
					GetComponent<Animator> ().SetBool ("m_bAttack", true);
					Invoke ("SendMSGTimeToAttack", 1.0f);
				}
			}
			else
			if (m_bShouldAttack == true && m_bStartDeath == false)
			{
				//In here we've left the swinging zone after attacking.
				m_bShouldAttack = false;
				GetComponent<Animator> ().SetBool ("m_bAttack", false);
			}
			//Check if we should move, if we should, do it until we get to the end location
			if (AmIDead () == false && m_bShouldMove == true)
			{
				if (m_nDirection == -1)
				{
					if (transform.localPosition.x + m_fStopOffset <= m_goLeftBound.transform.localPosition.x && m_bHasReachedDestination == false)
					{
						m_bHasReachedDestination = true;
						m_goController.GetComponent<FightSceneControllerScript> ().UnitReachedDestination ();
					}
				}
				else
				{
					if (transform.localPosition.x - m_fStopOffset >= m_goRightBound.transform.localPosition.x && m_bHasReachedDestination == false)
					{
						m_bHasReachedDestination = true;
						m_goController.GetComponent<FightSceneControllerScript> ().UnitReachedDestination ();
					}
				}
				Vector3 vecDir = new Vector3 (m_nDirection * m_fMovementSpeed * Time.deltaTime, 0, 0);
				GetComponent<RectTransform> ().Translate (vecDir);
			}
		}
	}

	void SendMSGTimeToAttack()
	{
		m_goController.GetComponent<FightSceneControllerScript>().StartAttackingPhase();
	}

	public void TimeToMove()
	{
        if (m_bHasDied == false)
        {
            m_bHasReachedDestination = false;
            m_bShouldMove = true;
        }
	}

	public void TimeToAttack(bool _isShootingArrow = false)
	{
        if (m_bHasDied == false)
        {
            if (_isShootingArrow == false)
                GetComponent<Animator>().SetBool("m_bAttack", true);
            else
            {
                //In here it means this unit should pull out their bow and shoot an arrow
                m_bIsShootingArrow = true;
                GetComponent<Animator>().SetTrigger("m_tShoot");
            }
        }
	}

	public void TimeToDie()
	{
		if(AmIDead() == false)
		{
            m_goController.GetComponent<FightSceneControllerScript>().UnitReachedDestination();
            m_bStartDeath = true;
			GetComponent<Animator>().SetBool("m_bDie", true);
			transform.SetSiblingIndex (0);
		}
	}

	public void HasDied()
	{
		m_bHasDied = true;
	}

	public bool AmIDead()
	{
		if(m_bHasDied == true || m_bStartDeath == true)
			return true;
		return false;
	}

    public void FireArrow()
    {
        m_bIsShootingArrow = false;

        if (m_nDirection == 1)
        {
            GameObject _arrow = Instantiate(m_goArrow, Vector3.zero, Quaternion.Euler(new Vector3(0, 0, 180))) as GameObject;
            _arrow.transform.SetParent(gameObject.transform.parent);
            _arrow.transform.localPosition = gameObject.transform.localPosition;
            _arrow.GetComponentInChildren<Animator>().SetTrigger("m_tToRight");
			_arrow.GetComponentInChildren<WB_ArrowScript> ().m_eSide = WB_ArrowScript.SIDE.eRIGHT;
			_arrow.GetComponentInChildren<WB_ArrowScript> ().m_goWarBattleWatcher = m_goController;
        }
        else
        {
            GameObject _arrow = Instantiate(m_goArrow, Vector3.zero, Quaternion.Euler(new Vector3(0, 0, 0))) as GameObject;
            _arrow.transform.SetParent(gameObject.transform.parent);
            _arrow.transform.localPosition = gameObject.transform.localPosition;
			_arrow.GetComponentInChildren<WB_ArrowScript> ().m_eSide = WB_ArrowScript.SIDE.eLEFT;
			_arrow.GetComponentInChildren<WB_ArrowScript> ().m_goWarBattleWatcher = m_goController;
        }

    }
}
