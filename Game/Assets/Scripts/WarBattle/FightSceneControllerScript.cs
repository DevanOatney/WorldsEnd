using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightSceneControllerScript : MonoBehaviour 
{
	public class cWarUnit
	{
		public cWarUnit(GameObject _ldr, GameObject _unit, float _percent, int totalCount) {m_goLeaderSprite = _ldr; m_goSprite = _unit; m_fPercentRemaining = _percent; m_nTotalCount = totalCount;}
		public GameObject m_goLeaderSprite;
		public GameObject m_goSprite;
		public float m_fPercentRemaining;
		public int m_nTotalCount;
	}

	public GameObject m_goLeftSide, m_goRightSide, m_goLowerBound;
	List<GameObject> m_lLeftUnits = new List<GameObject>();
	List<GameObject> m_lRightUnits = new List<GameObject>();
	float m_fUnitYOffset = 15.0f;
	float m_fUnitXOffset = 15.0f;


	/// TEMP STUFF TO TEST
	public GameObject TEMP_goUnit;

	void TEMP_BuildTestBattle()
	{
		cWarUnit leftUnit = new cWarUnit(null, TEMP_goUnit, 1.0f, 10);
		cWarUnit rightUnit = new cWarUnit(null, TEMP_goUnit, 1.0f, 15);
		SetupBattleScene(leftUnit, rightUnit);

	}


	// Use this for initialization
	void Start () 
	{
	}
		
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Space))
			TEMP_BuildTestBattle();
		if(Input.GetKeyDown(KeyCode.Escape))
			End();
		if(Input.GetKeyDown(KeyCode.A))
		{
			foreach(GameObject _unit in m_lLeftUnits)
				_unit.GetComponent<WB_UnitScript>().TimeToMove();
			foreach(GameObject _unit in m_lRightUnits)
				_unit.GetComponent<WB_UnitScript>().TimeToMove();
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
	}

	public void SetupBattleScene(cWarUnit _leftSide, cWarUnit _rightSide)
	{
		m_lLeftUnits.Clear();
		m_lRightUnits.Clear();
		LoadInASide(1, _leftSide);
		LoadInASide(-1, _rightSide);
	}

	//-1 for if it's on the right, 1 for if it's on the left
	void LoadInASide(int _side, cWarUnit _unit)
	{
		float _fXOffset = 0.0f, _fYOffset = 0.0f;
		int _forceRemaining = (int)(_unit.m_nTotalCount * _unit.m_fPercentRemaining);
		if(_forceRemaining == 0)
			_forceRemaining = 1;
		int _amountOfDead = _unit.m_nTotalCount - _forceRemaining;
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
		int _nDeadSoFar = 0;
		for(int i = 0; i < _forceRemaining; ++i)
		{
			Vector3 _startPos = Vector3.zero;
			GameObject _char = null;
			if(i+1 == _forceRemaining && _unit.m_goLeaderSprite != null)
				_char = Instantiate(_unit.m_goLeaderSprite) as GameObject;
			else
			    _char = Instantiate(_unit.m_goSprite) as GameObject;
			_char.transform.SetParent(gameObject.transform);
			_char.transform.localScale = Vector3.one;
			if(_forceRemaining + _nDeadSoFar < _unit.m_nTotalCount)
			{
				//So if we're in here, there's still a chance to spawn a dead unit.
				int _nGap = _unit.m_nTotalCount - (_forceRemaining + _nDeadSoFar);
				if(_nGap < i+2)
				{
					//So in here it means that by the next iteration, there won't be enough units left to initialize to represent all of the dead units, so it has to be dead.
					_char.GetComponent<WB_UnitScript>().Initialize(m_goLeftSide, m_goRightSide, _side, true);
				}
				else
				{
					//We've still got time, so roll the dice!
					bool rndm = ShouldThisOneBeDead();
					if(rndm == true)
					{
						_nDeadSoFar += 1;
						_char.GetComponent<WB_UnitScript>().Initialize(m_goLeftSide, m_goRightSide, _side, true);
					}
					else
						_char.GetComponent<WB_UnitScript>().Initialize(m_goLeftSide, m_goRightSide, _side, false);
				}
			}
			else
				_char.GetComponent<WB_UnitScript>().Initialize(m_goLeftSide, m_goRightSide, _side, false);
		
			if(_side == 1)
			{
				_startPos = m_goLeftSide.transform.localPosition;
				_char.transform.localPosition = AdjustPositionOfUnit(_startPos, ref _fXOffset, ref _fYOffset);

				Vector3 _scale = _char.transform.localScale;
				_scale.x *= -1;
				_char.transform.localScale = _scale;
				m_lLeftUnits.Add(_char);
			}
			else
			{
				_startPos = m_goRightSide.transform.localPosition;
				_char.transform.localPosition = AdjustPositionOfUnit(_startPos, ref _fXOffset, ref _fYOffset);
				m_lRightUnits.Add(_char);
			}
			_fYOffset += m_fUnitYOffset;
		}

	}

	bool ShouldThisOneBeDead()
	{
		int _nCoinFlip = Random.Range(0, 1);
		if(_nCoinFlip == 0)
			return false;
		return true;
	}
	Vector3 AdjustPositionOfUnit(Vector3 _startPos, ref float _fXOffset, ref float _fYOffset)
	{
		if(_startPos.y - _fYOffset <= m_goLowerBound.transform.localPosition.y)
		{
			_fYOffset = 0.0f;
			_fXOffset += m_fUnitXOffset;
		}
		_startPos.x += _fXOffset;
		_startPos.y -= _fYOffset;
		return _startPos;
	}
}
