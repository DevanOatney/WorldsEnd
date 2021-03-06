﻿using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class UnitScript : MonoBehaviour 
{
	public enum UnitTypes{ALLY_MELEE, ALLY_RANGED, NPC, BASICENEMY, PERCENTENEMY, RANGEDENEMY};
	public bool m_bIsMyTurn = false;
	public int m_nState;
	//for knowing which script to call for TakeDamage
	public UnitTypes m_nUnitType;
	//iter for which position on the field you're in 0-2
	//0 : Center
	//1 : Top
	//2 : Bottom
	int m_nPositionOnField = 0;
	public int FieldPosition { get {return m_nPositionOnField;} set {m_nPositionOnField = value;}}
	//iter for your targets position on field
	public int m_nTargetPositionOnField = 0;
	//vector to hold the initial position of the unit on the field for each time the unit moves
	protected Vector3 m_vInitialPos;
	//bool for it being the start of this units turn
	protected bool m_bFirstPass = true;
	//Should input be allowed?
	protected bool m_bAllowInput = true;
	public void SetAllowInput(bool flag) {m_bAllowInput = flag;}

	//units stats
	protected int m_nMaxHP;
	protected int m_nCurHP;
	protected int m_nMaxMP;
	protected int m_nCurMP;
	protected int m_nStr;
	protected int m_nDef;
	protected int m_nSpd;
	protected int m_nEva;
	protected int m_nHit;

	//temp unit stats (effected by equipment, weapons, status effects.. and unit skills(?)
	protected int m_nTempMaxHP;
	protected int m_nTempMaxMP;
	protected int m_nTempStr;
	protected int m_nTempDef;
	protected int m_nTempSpd;
	protected int m_nTempEva;
	protected int m_nTempHit;

	//accessors
	public int GetMaxHP() {return m_nMaxHP;}
	public int GetCurHP() {return m_nCurHP;}
	public int GetMaxMP() {return m_nMaxMP;}
	public int GetCurMP() {return m_nCurMP;}
	public int GetSTR() {return m_nStr;}
	public int GetDEF() {return m_nDef;}
	public int GetSPD() {return m_nSpd;}
	public int GetEVA() {return m_nEva;}
	public int GetHIT() {return m_nHit;}

	public int GetTempMaxHP() {return m_nTempMaxHP;}
	public int GetTempMaxMP() {return m_nTempMaxMP;}
	public int GetTempSTR() {return m_nTempStr;}
	public int GetTempDEF() {return m_nTempDef;}
	public int GetTempSPD() {return m_nTempSpd;}
	public int GetTempEVA() {return m_nTempEva;}
	public int GetTempHIT() {return m_nTempHit;}
	//mutators
	public void SetMaxHP(int hp) {m_nMaxHP = hp;}
	public void SetCurHP(int hp) {m_nCurHP = hp;}
	public void SetMaxMP(int mp) {m_nMaxMP = mp;}
	public void SetCurMP(int mp) {m_nCurMP = mp;}
	public void SetSTR(int str) {m_nStr = str;}
	public void SetDEF(int def) {m_nDef = def;}
	public void SetSPD(int spd) {m_nSpd = spd;}
	public void SetEVA(int eva) {m_nEva = eva;}
	public void SetHIT(int hit) {m_nHit = hit;}

	public void SetTempMaxHP(int hp) {m_nTempMaxHP = hp;}
	public void SetTempMaxMP(int mp) {m_nTempMaxMP = mp;}
	public void SetTempSTR(int str) {m_nTempStr = str;}
	public void SetTempDEF(int def) {m_nTempDef = def;}
	public void SetTempSPD(int spd) {m_nTempSpd = spd;}
	public void SetTempEVA(int eva) {m_nTempEva = eva;}
	public void SetTempHIT(int hit) {m_nTempHit = hit;}


	//The current level of the character
	int m_nUnitLevel = 1;
	public int GetUnitLevel() {return m_nUnitLevel;}
	public void SetUnitLevel(int lvl) {m_nUnitLevel = lvl;}


	protected TurnWatcherScript m_twTurnWatcher;
	public Animator m_aAnim;	
	//List of status effects that could be effecting the player/units   Poison, Confusion, Paralyze, Stone (examples)
	public List<GameObject> m_lStatusEffects = new List<GameObject>();



	//effectName - name of effect, nTicks - how many rounds this lasts (-1 for permanent), nMod - any adjuster, like damage dealt, chance effect happens each round, etc.
	public void AddStatusEffect(DCScript.StatusEffect se)
	{
		for(int i = 0; i < m_lStatusEffects.Count; ++ i)
		{
			if(se.m_nEffectType == m_lStatusEffects[i].GetComponent<BattleBaseEffectScript>().m_nEffectType)
			{
				//This effect already exists, re-apply it.
				BattleBaseEffectScript effectScript = m_lStatusEffects[i].GetComponent<BattleBaseEffectScript>();
				effectScript.RefreshEffect(se);
				if (m_twTurnWatcher.m_bHasStarted == true)
				{
					StartCoroutine (StatusEffectAnimation (se));
				}
				return;
					
			}
		}
		if(se.GetMember(name) != null)
		{
			//this means that the status effect being added is being carried in and the unit has already taken ticks of this effect, so set the tick count to whatever they have left.
			GameObject newEffect = m_twTurnWatcher.FindStatusEffect(se.m_szEffectName);
			newEffect = Instantiate(newEffect);
			newEffect.name = se.m_szEffectName;
			newEffect.GetComponent<BattleBaseEffectScript>().Initialize(gameObject,se.m_nEffectType, se.GetMember(name).m_nTicksLeft, se.m_nHPMod, se.m_nMPMod, se.m_nPOWMod, se.m_nDEFMod, se.m_nSPDMod, se.m_nHITMod, se.m_nEVAMod);
			m_lStatusEffects.Add(newEffect);
			if (m_twTurnWatcher.m_bHasStarted == true)
			{
				StartCoroutine (StatusEffectAnimation (se));
			}
		}
		else
		{
			//If we've gotten this far, it means this character is not currently effected by this type of effect.  Add this effect to the list of effects currently effecting this character
			GameObject newEffect = m_twTurnWatcher.FindStatusEffect(se.m_szEffectName);
			newEffect = Instantiate(newEffect);
			newEffect.name = se.m_szEffectName;
			newEffect.GetComponent<BattleBaseEffectScript>().Initialize(gameObject,se.m_nEffectType, se.m_nStartingTickCount, se.m_nHPMod, se.m_nMPMod, se.m_nPOWMod, se.m_nDEFMod, se.m_nSPDMod, se.m_nHITMod, se.m_nEVAMod);
			m_lStatusEffects.Add(newEffect);
			if (m_twTurnWatcher.m_bHasStarted == true)
			{
				StartCoroutine (StatusEffectAnimation (se));
			}
		}


	}

	IEnumerator StatusEffectAnimation(DCScript.StatusEffect se)
	{
		yield return new WaitForSeconds (1.0f);
		if (se.m_szEffectName == "Poison")
		{
			GameObject seAnimation = Instantiate(Resources.Load("Animation Effects/Spell Effects/DarkEffects/Dark_002"), transform.position, Quaternion.identity) as GameObject;
			seAnimation.GetComponent<SpriteRenderer> ().sortingOrder = gameObject.GetComponentInChildren<SpriteRenderer> ().sortingOrder + 1;
			Destroy (seAnimation, 0.5f);
		}
	}


	public void RemoveStatusEffect(string effectName)
	{
		for (int i = m_lStatusEffects.Count - 1; i >= 0; i--)
		{
			if(m_lStatusEffects[i].name == effectName)
			{
				m_lStatusEffects[i].GetComponent<BattleBaseEffectScript>().m_bHasBeenRemoved = true;
				m_lStatusEffects.RemoveAt(i);
			}
		}
	}

	void Awake()
	{
		m_twTurnWatcher = GameObject.Find("TurnWatcher").GetComponent<TurnWatcherScript>();
		m_twTurnWatcher.AddMeToList(gameObject);
	}
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//negative numbers are for healing
	public virtual void AdjustHP(int dmg)
	{
		if(GetCurHP() > 0)
		{
			switch(m_nUnitType)
			{
			case UnitTypes.ALLY_MELEE:
			{
				gameObject.GetComponent<CAllyBattleScript>().AdjustHP(dmg);
			}
				break;
			case UnitTypes.ALLY_RANGED:
			{
				gameObject.GetComponent<CAllyBattleScript>().AdjustHP(dmg);
			}
				break;
			case UnitTypes.BASICENEMY:
			{
				gameObject.GetComponent<StandardEnemyScript>().AdjustHP(dmg);
			}
				break;
			case UnitTypes.PERCENTENEMY:
			{
				gameObject.GetComponent<PercentStandardEnemyScript>().AdjustHP(dmg);
			}
				break;
			}
		}
	}

	public void AdjustMP(int mpAdjustment)
	{
		m_nCurMP = Mathf.Clamp(m_nCurMP + mpAdjustment, 0, m_nMaxMP);
	}

	public virtual void AdjustTempStats()
	{
		m_nTempMaxHP = m_nMaxHP;
		m_nTempMaxMP = m_nMaxMP;
		m_nTempStr = m_nStr;
		m_nTempDef = m_nDef;
		m_nTempSpd = m_nSpd;
		m_nTempHit = m_nHit;
		m_nTempEva = m_nEva;
	}

	public virtual void Missed()
	{
		if(GetCurHP() > 0)
		{
			switch(m_nUnitType)
			{
			case UnitTypes.ALLY_MELEE:
			{
				gameObject.GetComponent<CAllyBattleScript>().Missed();
			}
				break;
			case UnitTypes.ALLY_RANGED:
			{
				gameObject.GetComponent<CAllyBattleScript>().Missed();
			}
				break;
			case UnitTypes.BASICENEMY:
			{
				gameObject.GetComponent<StandardEnemyScript>().Missed();
			}
				break;
			case UnitTypes.PERCENTENEMY:
			{
				gameObject.GetComponent<PercentStandardEnemyScript>().Missed();
			}
				break;
			}
		}
	}

	public void StartMyTurn()
	{
		
	}
	
	public void EndMyTurn()
	{
		m_bIsMyTurn = false;
		m_twTurnWatcher.MyTurnIsOver(gameObject);
		m_bFirstPass = true;
	}

	public virtual void AttackAnimationEnd()
	{
		switch(m_nUnitType)
		{
		case UnitTypes.ALLY_MELEE:
		{

		}
			break;
		case UnitTypes.ALLY_RANGED:
		{

		}
			break;
		case UnitTypes.BASICENEMY:
		{
			gameObject.GetComponent<StandardEnemyScript>().AttackAnimEnd();
		}
			break;
		case UnitTypes.RANGEDENEMY:
		{
			gameObject.GetComponent<StandardEnemyScript> ().RangedAttackAnimationEnded ();
		}
			break;
		case UnitTypes.PERCENTENEMY:
		{
			gameObject.GetComponent<PercentStandardEnemyScript>().AttackAnimEnd();
		}
			break;
		}
	}

	public virtual void CastingAnimationEnd()
	{
	}

	public virtual void DamagedAnimationOver()
	{
	}

	virtual public void IDied()
	{

	}

	public virtual void ChargingOver()
	{
	}

	bool CheckIfHit()
	{
		GameObject[] posTargs = null;
		switch(m_nUnitType)
		{
		case UnitTypes.ALLY_MELEE:
			{
				posTargs = GameObject.FindGameObjectsWithTag("Ally");
			}
			break;
		case UnitTypes.ALLY_RANGED:
			{
				posTargs = GameObject.FindGameObjectsWithTag("Ally");
			}
			break;
		case UnitTypes.BASICENEMY:
			{
				posTargs = GameObject.FindGameObjectsWithTag("Enemy");
			}
			break;
		case UnitTypes.PERCENTENEMY:
			{
				posTargs = GameObject.FindGameObjectsWithTag("Enemy");
			}
			break;
		}

		foreach(GameObject tar in posTargs)
		{
			if(tar.GetComponent<UnitScript>().m_nPositionOnField == m_nTargetPositionOnField)
			{
				int nChanceToHit = UnityEngine.Random.Range(0,100);
				int nRange = 85 + m_nHit - tar.GetComponent<UnitScript>().GetEVA();
				if(nRange < 5)
					nRange = 5;
				Debug.Log("Chance: " + nChanceToHit + "    Range: " + nRange);
				if(nChanceToHit <	nRange)
				{
					//Target was hit
					return true;
				}
				else
				{
					//target was missed
					return false;
				}
			}
		}
		return false;
	}
	//if it returns -1, switch wasn't possible.  Else it returns 0-5 in relation to where this unit is able to switch to in the formation.
	protected int TryToSwitch(bool _isAlly)
	{
		GameObject[] _units;
		int posToSwitch = -1;
		int nMod = 1;
		if(_isAlly == true)
		{
			_units = GameObject.FindGameObjectsWithTag("Ally");
		}
		else
		{
			_units = GameObject.FindGameObjectsWithTag("Enemy");
		}
		if(FieldPosition > 2)
		{
			//front row, switching to back row
			nMod = -1;
		}
		else
		{
			//back row, switching to front row
			nMod = 1;
		}



		List<int> lValidPos = new List<int>();
		foreach(GameObject e in _units)
		{
			lValidPos.Add(e.GetComponent<UnitScript>().FieldPosition);
		}
		if(lValidPos.Contains(FieldPosition + (3*nMod)))
		{
			//able to switch directly behind
			posToSwitch = FieldPosition + (3*nMod);
		}
		else if(FieldPosition == 3 || FieldPosition == 2)
		{
			if(lValidPos.Contains(FieldPosition + (1*nMod)))
			{
				posToSwitch = FieldPosition + (1*nMod);
			}
		}
		else if(FieldPosition == 5 || FieldPosition == 0)
		{
			if(lValidPos.Contains(FieldPosition + (5*nMod)))
			{
				posToSwitch = FieldPosition + (5*nMod);
			}
		}
		else
		{
			if(lValidPos.Contains(FieldPosition + (2*nMod)))
			{
				posToSwitch = FieldPosition + (2*nMod);
			}
			else if(lValidPos.Contains(FieldPosition + (4*nMod)))
			{
				posToSwitch = FieldPosition + (4*nMod);
			}
		}

		return posToSwitch;
	}
}
