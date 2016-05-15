using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightSceneControllerScript : MonoBehaviour 
{
	public class cWarUnit
	{
		public cWarUnit(GameObject _ldr, GameObject _unit, float _percent, int totalCount, int _atp, int _def, int _lck, int _atkRange, int _movement) 
		{
			m_goLeaderSprite = _ldr; 
			m_goSprite = _unit; 
			m_fPercentRemaining = _percent; 
			m_nTotalCount = totalCount;
			m_nAttackPower = _atp;
			m_nDefensePower = _def;
			m_nLuck = _lck;
            m_nAttackRange = _atkRange;
            m_nMovementRange = _movement;
		}
		public GameObject m_goLeaderSprite;
		public GameObject m_goSprite;
		public float m_fPercentRemaining;
		public int m_nTotalCount;
		public int m_nAttackPower;
		public int m_nDefensePower;
		public int m_nLuck;
        public int m_nAttackRange;
        public int m_nMovementRange;
	}

	public GameObject m_goLeftSide, m_goRightSide, m_goLowerBound, m_goLeftAttack, m_goRightAttack;
	List<GameObject> m_lLeftUnits = new List<GameObject>();
	List<GameObject> m_lRightUnits = new List<GameObject>();
	cWarUnit m_cLeftWarUnit, m_cRightWarUnit;
	float m_fUnitYOffset = 25.0f;
	float m_fYRangeOffset = 15.0f;
	float m_fUnitXOffset = 25.0f;
	float m_fXRangeOffset = 15.0f;

	bool m_bDamagePhaseEnded = false;
	bool m_bHasArrivedAtEnd = false;
	int m_nUnitsArrivedCounter = 0;

    float m_fTimerTillFightStartsBucket = 2.0f;
    float m_fTimerTillFightStarts = 2.2f;

    public GameObject m_goWatcher;


	// Use this for initialization
	void Start () 
	{
	}
		
	// Update is called once per frame
	void Update () 
	{
        if (m_fTimerTillFightStarts < m_fTimerTillFightStartsBucket)
        {
            m_fTimerTillFightStarts += Time.deltaTime;
            if (m_fTimerTillFightStarts >= m_fTimerTillFightStartsBucket)
            {
                foreach (GameObject _unit in m_lLeftUnits)
                    _unit.GetComponent<WB_UnitScript>().TimeToMove();
                foreach (GameObject _unit in m_lRightUnits)
                    _unit.GetComponent<WB_UnitScript>().TimeToMove();
            }
        }
	}

	void End()
	{
		foreach(GameObject _unit in m_lLeftUnits)
			Destroy(_unit);
		foreach(GameObject _unit in m_lRightUnits)
			Destroy(_unit);
		m_lLeftUnits.Clear();
		m_lRightUnits.Clear();
		m_cLeftWarUnit = null;
		m_cRightWarUnit = null;
	}

	public void SetupBattleScene(cWarUnit _leftSide, cWarUnit _rightSide)
	{
		m_bHasArrivedAtEnd = false;
		m_bDamagePhaseEnded = false;
		m_nUnitsArrivedCounter = 0;
        foreach (GameObject _go in m_lLeftUnits)
            Destroy(_go);
        foreach (GameObject _go in m_lRightUnits)
            Destroy(_go);
		m_lLeftUnits.Clear();
		m_lRightUnits.Clear();
		m_cLeftWarUnit = _leftSide;
		m_cRightWarUnit = _rightSide;
		LoadInASide(1, _leftSide);
		LoadInASide(-1, _rightSide);
        m_fTimerTillFightStarts = 0.0f;
	}

	//-1 for if it's on the right, 1 for if it's on the left
	void LoadInASide(int _side, cWarUnit _unit)
	{
		float _fXOffset = 0.0f, _fYOffset = 0.0f;
		int _forceRemaining = (int)(_unit.m_nTotalCount * _unit.m_fPercentRemaining);
		if(_forceRemaining == 0)
			_forceRemaining = 1;
		if(_forceRemaining == 1)
		{
			if(_unit.m_goLeaderSprite == null)
			{
				//if this unit group doesn't have a leader and is down to their last unit, only spawn 1 basic unit.
			}
			else
			{
				//If the unit is down to it's last person and you do have a leader, only spawn the leader and call it good.
			}
		}
		for(int i = 0; i < _forceRemaining; ++i)
		{
			Vector3 _startPos = Vector3.zero;
			GameObject _char = null;
			if(i+1 == _forceRemaining && _unit.m_goLeaderSprite != null)
				_char = Instantiate(_unit.m_goLeaderSprite) as GameObject;
			else
			    _char = Instantiate(_unit.m_goSprite) as GameObject;
			_char.transform.SetParent(gameObject.transform);
			_char.transform.localScale = Vector3.one * 2;
			_char.GetComponent<WB_UnitScript>().Initialize(gameObject, _side, false);
		
			if(_side == 1)
			{
				_startPos = m_goLeftSide.transform.localPosition;
				_char.transform.localPosition = AdjustPositionOfUnit(_startPos, ref _fXOffset, ref _fYOffset, _side);

				Vector3 _scale = _char.transform.localScale;
				_scale.x *= -1;
				_char.transform.localScale = _scale;
				m_lLeftUnits.Add(_char);
			}
			else
			{
				_startPos = m_goRightSide.transform.localPosition;
				_char.transform.localPosition = AdjustPositionOfUnit(_startPos, ref _fXOffset, ref _fYOffset, _side);
				m_lRightUnits.Add(_char);
			}
			_fYOffset += m_fUnitYOffset;
		}

	}

	public void StartAttackingPhase()
	{
		if(m_bDamagePhaseEnded == false)
		{
			//first, set this flag to true so that it only gets called once.
			m_bDamagePhaseEnded = true;

			//check if the left side took any damage
			int dmgRec = DamageUnitReceives(m_cLeftWarUnit, m_cRightWarUnit);
			if(dmgRec > 0)
			{
				float dmgMod = ((float)dmgRec * 0.33f);
				m_cLeftWarUnit.m_fPercentRemaining -= dmgMod;
				//if the percent left is below a certain percentage, let's call this good enough and just kill the entire unit
				if(m_cLeftWarUnit.m_fPercentRemaining <= 0.05f)
				{
					m_cLeftWarUnit.m_fPercentRemaining = 0.0f;
					foreach(GameObject _unit in m_lLeftUnits)
					{
						if(_unit.GetComponent<WB_UnitScript>().AmIDead() == false)
						{
							_unit.GetComponent<WB_UnitScript>().TimeToDie();
						}
					}
				}
				else
				{
					int _nNewCount = (int)(m_cLeftWarUnit.m_nTotalCount * m_cLeftWarUnit.m_fPercentRemaining);
					int _nCounter = _nNewCount;
					foreach(GameObject _unit in m_lLeftUnits)
					{
						if(_unit.GetComponent<WB_UnitScript>().AmIDead() == false)
						{
							_nCounter -= 1;
							_unit.GetComponent<WB_UnitScript>().TimeToDie();
							if(_nCounter <= 0)
								break;
						}
					}
				}

			}
			//check if the right side took any dmg
			dmgRec = DamageUnitReceives(m_cRightWarUnit, m_cLeftWarUnit);
			if(dmgRec > 0)
			{
				float dmgMod = ((float)dmgRec * 0.33f);
				m_cRightWarUnit.m_fPercentRemaining -= dmgMod;
				//if the percent left is below a certain percentage, let's call this good enough and just kill the entire unit
				if(m_cRightWarUnit.m_fPercentRemaining <= 0.05f)
				{
					m_cRightWarUnit.m_fPercentRemaining = 0.0f;
					foreach(GameObject _unit in m_lRightUnits)
					{
						if(_unit.GetComponent<WB_UnitScript>().AmIDead() == false)
						{
							_unit.GetComponent<WB_UnitScript>().TimeToDie();
						}
					}
				}
				else
				{
					int _nNewCount = (int)(m_cRightWarUnit.m_nTotalCount * m_cRightWarUnit.m_fPercentRemaining);
					int _nCounter = _nNewCount;
					foreach(GameObject _unit in m_lRightUnits)
					{
						if(_unit.GetComponent<WB_UnitScript>().AmIDead() == false)
						{
							_nCounter -= 1;
							_unit.GetComponent<WB_UnitScript>().TimeToDie();
							if(_nCounter <= 0)
								break;
						}
					}
				}

			}
		}
	}
	public void UnitReachedDestination()
	{
		if(m_bHasArrivedAtEnd == false)
		{
			m_nUnitsArrivedCounter += 1;
			if(m_nUnitsArrivedCounter >= m_lLeftUnits.Count + m_lRightUnits.Count)
			{
				foreach(GameObject _unit in m_lLeftUnits)
				{
					if(_unit.GetComponent<WB_UnitScript>().AmIDead() == false)
					{
						_unit.GetComponent<WB_UnitScript>().m_bShouldMove = false;
						Vector3 _scale = _unit.transform.localScale;
						_scale.x *= -1;
						_unit.transform.localScale = _scale;
					}
				}
				foreach(GameObject _unit in m_lRightUnits)
				{
					if(_unit.GetComponent<WB_UnitScript>().AmIDead() == false)
					{
						_unit.GetComponent<WB_UnitScript>().m_bShouldMove = false;
						Vector3 _scale = _unit.transform.localScale;
						_scale.x *= -1;
						_unit.transform.localScale = _scale;
					}
				}
				m_nUnitsArrivedCounter = 0;
				m_bHasArrivedAtEnd = true;
                Invoke("TurnOffWindow", 1.0f);
            }
            
		}
	}

    void TurnOffWindow()
    {
        m_goWatcher.GetComponent<WarBattleWatcherScript>().BattleSceneEnded(m_cLeftWarUnit, m_cRightWarUnit);
        gameObject.SetActive(false);
    }
	bool ShouldThisOneBeDead()
	{
		int _nCoinFlip = Random.Range(0, 2);
		if(_nCoinFlip == 0)
			return false;
		return true;
	}
	Vector3 AdjustPositionOfUnit(Vector3 _startPos, ref float _fXOffset, ref float _fYOffset, int _side)
	{
		
		if(_startPos.y - _fYOffset <= m_goLowerBound.transform.localPosition.y)
		{
			_fYOffset = 0.0f;
			_fXOffset += m_fUnitXOffset + Random.Range(0, m_fXRangeOffset+1);
		}
		if(_side == -1)
			_startPos.x -= _fXOffset + Random.Range(0.0f, m_fXRangeOffset+1);
		else
			_startPos.x += _fXOffset + Random.Range(0.0f, m_fXRangeOffset+1);
		_startPos.y -= _fYOffset + Random.Range(0.0f, m_fYRangeOffset+1);
		return _startPos;
	}

	//returns int for how much damage this group should receive.  0 = 0.0f loss. 1 = 0.3f loss, 2 = 0.6f loss, 3 = 1.0f loss
	int DamageUnitReceives(cWarUnit _defending, cWarUnit _attacking)
	{
		//Chance of damaging enemies in mass combat: IF ((Attack Power + 4) - Enemy DEF) > Random Number (0-19), then damage.
		if(((_attacking.m_nAttackPower + 4) - _defending.m_nDefensePower) > Random.Range(0, 20))
		{
			//Dealt damage, check crit/multiple hits
			return 1;
		}
		return 0;
	}
}
