using UnityEngine;
using System.Collections;

public class WB_UnitScript : MonoBehaviour 
{
	//Flags
	bool m_bStartDeath = false;
	bool m_bHasDied = false;
	bool m_bShouldAttack = false;
	public bool m_bShouldMove = false;
	//Stats
	public float m_fMovementSpeed = 5.0f;
	//Add in a bit of an offset to where we stop, this is to make the fight happen quick and the retreat take a little longer.
	float m_fStopOffset = 0.0f;
	//-1 to move left, +1 to move right
	int m_nDirection = -1;
	//Hooks
	public GameObject m_goDeathFlash;
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
		//Check to see if we're in the "swinging zone" and if we haven't started swinging, swing.  If we leave the swinging zone, stop swinging
		if(transform.GetComponent<RectTransform>().anchoredPosition.x >= m_goLeftAttack.GetComponent<RectTransform>().anchoredPosition.x && transform.GetComponent<RectTransform>().anchoredPosition.x <= m_goRightAttack.GetComponent<RectTransform>().anchoredPosition.x && m_bStartDeath == false)
		{
			//in here, we're in the bounds of the swinging zone.
			if(m_bShouldAttack == false)
			{
				//in here, we haven't started swinging yet.
				m_bShouldAttack = true;
				GetComponent<Animator>().SetBool("m_bAttack", true);
				Invoke("SendMSGTimeToAttack", 1.0f);
			}
		}
		else if(m_bShouldAttack == true && m_bStartDeath == false)
		{
			//In here we've left the swinging zone after attacking.
			m_bShouldAttack = false;
			GetComponent<Animator>().SetBool("m_bAttack", false);
		}
		//Check if we should move, if we should, do it until we get to the end location
		if(AmIDead() == false && m_bShouldMove == true)
		{
			if(m_nDirection == -1)
			{
				if(transform.localPosition.x + m_fStopOffset <= m_goLeftBound.transform.localPosition.x)
				{
					m_goController.GetComponent<FightSceneControllerScript>().UnitReachedDestination();
				}
			}
			else
			{
				if(transform.localPosition.x - m_fStopOffset  >= m_goRightBound.transform.localPosition.x)
				{
					m_goController.GetComponent<FightSceneControllerScript>().UnitReachedDestination();
				}
			}
			Vector3 vecDir = new Vector3(m_nDirection*m_fMovementSpeed*Time.deltaTime, 0, 0);
			GetComponent<RectTransform>().Translate(vecDir);
		}

	}

	void SendMSGTimeToAttack()
	{
		m_goController.GetComponent<FightSceneControllerScript>().StartAttackingPhase();
	}

	public void TimeToMove()
	{
		if(m_bHasDied == false)
			m_bShouldMove = true;
	}

	public void TimeToAttack()
	{
		if(m_bHasDied == false)
			GetComponent<Animator>().SetBool("m_bAttack", true);
	}

	public void TimeToDie()
	{
		if(AmIDead() == false)
		{
			m_bStartDeath = true;
			GetComponent<Animator>().SetBool("m_bDie", true);
			int index = transform.GetSiblingIndex();
			transform.SetSiblingIndex (index + 1);
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
}
