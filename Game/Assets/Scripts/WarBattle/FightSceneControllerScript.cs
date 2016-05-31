using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightSceneControllerScript : MonoBehaviour 
{
    enum Battle_States { eMeleeFight, eBowFight}
    Battle_States m_bsBattleState = Battle_States.eMeleeFight;
    [System.Serializable]
	public class cWarUnit
	{
		public cWarUnit(GameObject _ldr, GameObject _unit, string _szTeamName, float _percent, int totalCount, int _atp, int _def, int _lck, int _atkRange, int _movement) 
		{

			m_goLeaderSprite = _ldr; 
			m_goSprite = _unit;
            m_szTeamName = _szTeamName;
			m_fPercentRemaining = _percent; 
			m_nTotalCount = totalCount;
			m_nAttackPower = _atp;
			m_nDefensePower = _def;
			m_nLuck = _lck;
            m_nAttackRange = _atkRange;
            m_nMovementRange = _movement;
		}
        public string m_szTeamName;
		public GameObject m_goLeaderSprite;
		public GameObject m_goSprite;
		public float m_fPercentRemaining = 1.0f;
		public int m_nTotalCount = 10;
		public int m_nAttackPower;
		public int m_nDefensePower;
		public int m_nLuck;
        public int m_nAttackRange;
        public int m_nMovementRange;
	}

	public GameObject m_goLeftSide, m_goRightSide, m_goLowerBound, m_goLeftAttack, m_goRightAttack, m_goLeftStart, m_goRightStart;
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
    //Variables for ranged combat
    //The range required of a unit to participate in this fight.
    int m_nRangeRequired = 0;
    //0 = no damage, 1 = 33% loss, 2 = 66% loss, 3 = death
    int m_nLeftSideDmg = 0;
    int m_nRightSideDmg = 0;
    //The duration of time that the arrows will fly at each other (meaning the amount of time alotted for things to die.
    float m_fTotalArrowDuration = 10.0f;
    //the timer for how  long the arrows have been firing (ends once this exceeds m_fTotalArrowDuration
    float m_fArrowTimer = 0.0f;
    //The timer for how quickly each unit on the left/right side would need to die to be able to match it with the timer (set to m_fTotalArrowDuration+ for units to not die.)
    float m_fLeftSideDeathDuration = 10.0f;
    float m_fLeftSideDeathTimer = 0.0f;
    float m_fRightSideDeathDuration = 10.0f;
    float m_fRightSideDeathTimer = 0.0f;
    //flag for if we should be doing the ranged update call
    bool m_bRangedBegun = false;
    //Float representing the chance that each unit (both left/right) will shoot an arrow in a time step.
    float m_fShootArrowChance = 2.0f;
    //These two ints are to make sure the left/right side doesn't lose more than what we've calculated.
    int m_nLeftSideDeathCount = 0;
    int m_nRightSideDeathCount = 0;

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
               
                    if (m_bsBattleState == Battle_States.eMeleeFight)
                    {
                        MeleeUpdate();
                    }
                    else if (m_bsBattleState == Battle_States.eBowFight)
                    {
                        CalculateDeathsForRangedFight();
                    }
            }
        }
        if (m_bRangedBegun == true && m_bsBattleState == Battle_States.eBowFight)
            RangedUpdate();
	}

    //The name of this function is slightly misleading as it only gets called once.
    void MeleeUpdate()
    {
        
            foreach (GameObject _unit in m_lLeftUnits)
                _unit.GetComponent<WB_UnitScript>().TimeToMove();
            foreach (GameObject _unit in m_lRightUnits)
                _unit.GetComponent<WB_UnitScript>().TimeToMove();
    }

    void RangedUpdate()
    {
        //Have we finished shooting arrows?
        if (m_fArrowTimer <= m_fTotalArrowDuration)
        {
            m_fArrowTimer += Time.deltaTime;

            //Let's see if the random roll let's a left side unit shoot an arrow.
            int _nRoll = Random.Range(0, 101);
            if (_nRoll <= m_fShootArrowChance && m_cLeftWarUnit.m_nAttackRange >= m_nRangeRequired)
            {
                //We're good to fire an arrow! loop through and find a unit that isn't firing and tell it to fire!
                RandomUnitFireArrow(m_lLeftUnits);
            }
            //Now let's give the right side a chance to shoot.
            _nRoll = Random.Range(0, 101);
            if (_nRoll <= m_fShootArrowChance && m_cRightWarUnit.m_nAttackRange >= m_nRangeRequired)
            {
                //We're good to fire an arrow! loop through and find a unit that isn't firing and tell it to fire!
                RandomUnitFireArrow(m_lRightUnits);
            }


            //Check to see if a unit on the left needs to die
            if (m_fLeftSideDeathTimer >= m_fLeftSideDeathDuration)
            {
                m_fLeftSideDeathTimer = 0.0f;
                if (KillRandomUnit(m_lLeftUnits) == false)
                {
                    m_fLeftSideDeathDuration = m_fTotalArrowDuration * 5;
                }
                else
                {
                    m_nLeftSideDeathCount -= 1;
                    if(m_nLeftSideDeathCount <= 0)
                        m_fLeftSideDeathDuration = m_fTotalArrowDuration * 5;
                }
            }
            else
                m_fLeftSideDeathTimer += Time.deltaTime;
            //Check to see if a unit on the right side needs to die
            if (m_fRightSideDeathTimer >= m_fRightSideDeathDuration)
            {
                m_fRightSideDeathTimer = 0.0f;
                //if we weren't able to kill the unit, set the duration high so we stop iterating through this
                if (KillRandomUnit(m_lRightUnits) == false)
                {
                    m_fRightSideDeathDuration = m_fTotalArrowDuration * 5;
                }
                else
                {
                    m_nRightSideDeathCount -= 1;
                    if (m_nRightSideDeathCount <= 0)
                        m_fRightSideDeathDuration = m_fTotalArrowDuration * 5;
                }
            }
            else
                m_fRightSideDeathTimer += Time.deltaTime;
        }
        else
        {
            m_bRangedBegun = false;
            Invoke("End", 1.0f);
        }
    }

    //This calculates how many of each sides units need to die during the ranged attacking scene. (also sets the health remaining on each side here just so that we don't forget later)
    void CalculateDeathsForRangedFight()
    {
        m_bRangedBegun = true;
        if (m_cRightWarUnit.m_nAttackRange >= m_nRangeRequired)
        {
            m_nLeftSideDmg = DamageUnitReceives(m_cLeftWarUnit, m_cRightWarUnit);
            if (m_nLeftSideDmg <= 0)
            {
                m_fLeftSideDeathDuration = m_fTotalArrowDuration*5;
                //Let's give it a chance to kill 1 random unit at 1 random time throughout the scene, don't adjust the hp or anything, just do it for funsies so that there's more variety and fights are less stale.
                int rndmChance = Random.Range(0, 11);
                if (rndmChance < 3)
                {
                    //K, let's kill one random unit for no reason lol.
                    m_nLeftSideDeathCount = 1;
                    m_fLeftSideDeathDuration = Random.Range(0, m_fTotalArrowDuration);
                }
            }
            else
            {
                float dmgMod = ((float)m_nLeftSideDmg * 0.33f);
                int _nPrevUnitCnt = (int)(m_cLeftWarUnit.m_nTotalCount * m_cLeftWarUnit.m_fPercentRemaining);
                m_cLeftWarUnit.m_fPercentRemaining -= dmgMod;
                //So this int represents how many units need to die in this scene to make it look right (it's okay if this number is slightly low/high)
                int _nNewUnitCnt = (int)(m_cLeftWarUnit.m_nTotalCount * m_cLeftWarUnit.m_fPercentRemaining);
                m_nLeftSideDeathCount = _nPrevUnitCnt - _nNewUnitCnt;
                m_fLeftSideDeathDuration = m_fTotalArrowDuration / m_nLeftSideDeathCount;
            }

        }
        if (m_cLeftWarUnit.m_nAttackRange >= m_nRangeRequired)
        {
            m_nRightSideDmg = DamageUnitReceives(m_cRightWarUnit, m_cLeftWarUnit);
            if (m_nRightSideDmg <= 0)
            {
                m_fRightSideDeathDuration = m_fTotalArrowDuration*5;
                //Let's give it a chance to kill 1 random unit at 1 random time throughout the scene, don't adjust the hp or anything, just do it for funsies so that there's more variety and fights are less stale.
                int rndmChance = Random.Range(0, 11);
                if (rndmChance < 3)
                {
                    //K, let's kill one random unit for no reason lol.
                    m_nRightSideDeathCount = 1;
                    m_fRightSideDeathDuration = Random.Range(0, m_fTotalArrowDuration);
                }
            }
            else
            {
                float dmgMod = ((float)m_nRightSideDmg * 0.33f);
                int _nPrevUnitCnt = (int)(m_cRightWarUnit.m_nTotalCount * m_cRightWarUnit.m_fPercentRemaining);
                m_cRightWarUnit.m_fPercentRemaining -= dmgMod;
                //So this int represents how many units need to die in this scene to make it look right (it's okay if this number is slightly low/high)
                int _nNewUnitCnt = (int)(m_cRightWarUnit.m_nTotalCount * m_cRightWarUnit.m_fPercentRemaining);
                m_nRightSideDeathCount = _nPrevUnitCnt - _nNewUnitCnt;
                m_fRightSideDeathDuration = m_fTotalArrowDuration / m_nRightSideDeathCount;
            }
        }
    }

	void End()
	{
        Invoke("TurnOffWindow", 1.0f);
    }

	public void SetupBattleScene(cWarUnit _leftSide, cWarUnit _rightSide, int _rngRequired)
	{
		m_bHasArrivedAtEnd = false;
		m_bDamagePhaseEnded = false;
		m_nUnitsArrivedCounter = 0;
        foreach (GameObject _go in m_lLeftUnits)
            Destroy(_go);
        foreach (GameObject _go in m_lRightUnits)
            Destroy(_go);
        m_nRangeRequired = _rngRequired;
        if (m_nRangeRequired > 1)
            m_bsBattleState = Battle_States.eBowFight;
		m_lLeftUnits.Clear();
		m_lRightUnits.Clear();
		m_cLeftWarUnit = _leftSide;
		m_cRightWarUnit = _rightSide;
		LoadInASide(1, _leftSide);
		LoadInASide(-1, _rightSide);
        m_fTimerTillFightStarts = 0.0f;
        m_fArrowTimer = 0.0f;

	}

	//-1 for if it's on the right, 1 for if it's on the left
	void LoadInASide(int _side, cWarUnit _unit)
	{
		float _fXOffset = 0.0f, _fYOffset = 0.0f;
		int _forceRemaining = (int)(_unit.m_nTotalCount * _unit.m_fPercentRemaining);
        //Kept at -1 if no leader should spawn.
        int _nLeaderSpawnIter = -1;

        if (_unit.m_goLeaderSprite != null)
        {
            //There is a leader in this group, so we should probably spawn that somewheres.
            _nLeaderSpawnIter = Random.Range(0, _forceRemaining);
        }
		if(_forceRemaining == 0)
			_forceRemaining = 1;
		for(int i = 0; i < _forceRemaining; ++i)
		{
            if (i + 1 == _forceRemaining && _unit.m_goLeaderSprite != null)
            {
                //If there is only one unit remaining in this group, and we have a leader, let's have the leader be the last dude alive.
                SpawnUnit(_unit.m_goLeaderSprite, ref _fXOffset, ref _fYOffset, _side);
            }
            else
            {
                if (i == _nLeaderSpawnIter)
                {
                    //If there's a leader to spawn, and we're at that random iter, spawn the leader
                    SpawnUnit(_unit.m_goLeaderSprite, ref _fXOffset, ref _fYOffset, _side);
                }
                else
                    SpawnUnit(_unit.m_goSprite, ref _fXOffset, ref _fYOffset, _side);
            }
			_fYOffset += m_fUnitYOffset;
		}

	}

    void SpawnUnit(GameObject _unit, ref float _fXOffset, ref float _fYOffset, int _side)
    {
        Vector3 _startPos = Vector3.zero;
        GameObject _char = null;
        _char = Instantiate(_unit) as GameObject;
        _char.transform.SetParent(gameObject.transform);
        _char.transform.localScale = Vector3.one * 2;
        _char.GetComponent<WB_UnitScript>().Initialize(gameObject, _side, false);

        if (_side == 1)
        {
            _startPos = m_goLeftStart.transform.localPosition;
            _char.transform.localPosition = AdjustPositionOfUnit(_startPos, ref _fXOffset, ref _fYOffset, _side);

            Vector3 _scale = _char.transform.localScale;
            _scale.x *= -1;
            _char.transform.localScale = _scale;
            m_lLeftUnits.Add(_char);
        }
        else
        {
            _startPos = m_goRightStart.transform.localPosition;
            _char.transform.localPosition = AdjustPositionOfUnit(_startPos, ref _fXOffset, ref _fYOffset, _side);
            m_lRightUnits.Add(_char);
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
            if (dmgRec > 0)
            {
                float dmgMod = ((float)dmgRec * 0.33f);
                m_cLeftWarUnit.m_fPercentRemaining -= dmgMod;
                //if the percent left is below a certain percentage, let's call this good enough and just kill the entire unit
                if (m_cLeftWarUnit.m_fPercentRemaining <= 0.05f)
                {
                    m_cLeftWarUnit.m_fPercentRemaining = 0.0f;
                    foreach (GameObject _unit in m_lLeftUnits)
                    {
                        if (_unit.GetComponent<WB_UnitScript>().AmIDead() == false)
                        {
                            _unit.GetComponent<WB_UnitScript>().TimeToDie();
                        }
                    }
                }
                else
                {
                    int _nNewCount = (int)(m_cLeftWarUnit.m_nTotalCount * m_cLeftWarUnit.m_fPercentRemaining);
                    int _nCounter = _nNewCount;
                    foreach (GameObject _unit in m_lLeftUnits)
                    {
                        if (_unit.GetComponent<WB_UnitScript>().AmIDead() == false)
                        {
                            _nCounter -= 1;
                            _unit.GetComponent<WB_UnitScript>().TimeToDie();
                            if (_nCounter <= 0)
                                break;
                        }
                    }
                }

            }
            else
            {
                //Let's give it a chance to kill 1 random unit at 1 random time throughout the scene, don't adjust the hp or anything, just do it for funsies so that there's more variety and fights are less stale.
                int rndmChance = Random.Range(0, 11);
                if (rndmChance < 3)
                {
                    //K, let's kill one random unit for no reason lol.
                    KillRandomUnit(m_lLeftUnits);
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
            else
            {
                //Let's give it a chance to kill 1 random unit at 1 random time throughout the scene, don't adjust the hp or anything, just do it for funsies so that there's more variety and fights are less stale.
                int rndmChance = Random.Range(0, 11);
                if (rndmChance < 3)
                {
                    //K, let's kill one random unit for no reason lol.
                    KillRandomUnit(m_lLeftUnits);
                }
            }
        }
	}
	public void UnitReachedDestination()
	{
		if(m_bHasArrivedAtEnd == false && m_bsBattleState == Battle_States.eMeleeFight)
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
                End();
            }
            
		}
	}

    void TurnOffWindow()
    {
        foreach (GameObject _unit in m_lLeftUnits)
            Destroy(_unit);
        foreach (GameObject _unit in m_lRightUnits)
            Destroy(_unit);
        m_lLeftUnits.Clear();
        m_lRightUnits.Clear();
        m_cLeftWarUnit = null;
        m_cRightWarUnit = null;
        m_nLeftSideDmg = 0;
        m_nRightSideDmg = 0;
        m_bsBattleState = Battle_States.eMeleeFight;
        m_nUnitsArrivedCounter = 0;
        m_bHasArrivedAtEnd = true;
        m_bRangedBegun = false;
        m_fArrowTimer = 0.0f;
        m_fLeftSideDeathDuration = m_fTotalArrowDuration * 5;
        m_fRightSideDeathDuration = m_fTotalArrowDuration * 5;
        m_nLeftSideDeathCount = 0;
        m_nRightSideDeathCount = 0;
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
			_startPos.x += _fXOffset + Random.Range(0.0f, m_fXRangeOffset+1);
		else
			_startPos.x -= _fXOffset + Random.Range(0.0f, m_fXRangeOffset+1);
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

    //Kills a random unit from this list, returns false if every unit in this list is already dead.
    bool KillRandomUnit(List<GameObject> _lUnits)
    {
        List<int> _lItersOfAliveUnits = new List<int>();
        int _nCounter = 0;
        bool _bFoundAtleastOne = false;
        foreach (GameObject _go in _lUnits)
        {
            if (_go.GetComponent<WB_UnitScript>().AmIDead() == false)
            {
                _lItersOfAliveUnits.Add(_nCounter);
                _bFoundAtleastOne = true;
            }
            _nCounter++;
        }
        if (_bFoundAtleastOne == false)
            return false;
        int rndmRoll = Random.Range(0, _lItersOfAliveUnits.Count);
        _lUnits[rndmRoll].GetComponent<WB_UnitScript>().TimeToDie();
        return true;
    }

    void RandomUnitFireArrow(List<GameObject> _lUnits)
    {
        List<int> _lItersOfAvailableUnits = new List<int>();
        int _nCounter = 0;
        bool _bFoundAtleastOne = false;
        foreach (GameObject _go in _lUnits)
        {
            if (_go.GetComponent<WB_UnitScript>().AmIDead() == false && _go.GetComponent<WB_UnitScript>().m_bIsShootingArrow == false)
            {
                _lItersOfAvailableUnits.Add(_nCounter);
                _bFoundAtleastOne = true;
            }
            _nCounter++;
        }
        if (_bFoundAtleastOne == false)
            return;
        int rndmRoll = Random.Range(0, _lItersOfAvailableUnits.Count);
        _lUnits[rndmRoll].GetComponent<WB_UnitScript>().TimeToAttack(true);
    }

}
