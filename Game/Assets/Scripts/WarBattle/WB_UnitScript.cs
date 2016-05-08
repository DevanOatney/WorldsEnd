using UnityEngine;
using System.Collections;

public class WB_UnitScript : MonoBehaviour 
{
	//Flags
	bool m_bStartDeath = false;
	[HideInInspector]
	public  bool m_bHasDied = false;
	bool m_bShouldAttack = false;
	bool m_bShouldMove = false;
	//Stats
	public float m_fMovementSpeed = 5.0f;
	//-1 to move left, +1 to move right
	int m_nDirection = -1;
	//Hooks
	public GameObject m_goDeathFlash;
	public GameObject m_goLeftBound, m_goRightBound;

	public void Initialize(GameObject _leftBound, GameObject _rightBound, int _nDirection, bool _amIDead)
	{
		m_goLeftBound = _leftBound;
		m_goRightBound = _rightBound;
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

		//Check if we should move, if we should, do it until we get to the end location
		if(m_bStartDeath == false && m_bShouldMove == true)
		{
			if(m_nDirection == -1)
			{
				if(transform.localPosition.x <= m_goLeftBound.transform.localPosition.x)
					ReachedDestination();
			}
			else
			{
				if(transform.localPosition.x >= m_goRightBound.transform.localPosition.x)
					ReachedDestination();
			}
			float dir = m_nDirection * m_fMovementSpeed * Time.deltaTime;
			Vector3 vecDir = new Vector3(m_nDirection*m_fMovementSpeed*Time.deltaTime, 0, 0);
			GetComponent<RectTransform>().Translate(vecDir);
		}

	}

	void ReachedDestination()
	{
		m_bShouldMove = false;
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
		GetComponent<Animator>().SetBool("m_bDie", true);
	}

	public void HasDied()
	{
		m_bHasDied = true;
	}
}
