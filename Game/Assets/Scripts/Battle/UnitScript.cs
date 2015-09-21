using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitScript : MonoBehaviour 
{
	public enum UnitTypes{ALLY_MELEE, ALLY_RANGED, NPC, BASICENEMY, PERCENTENEMY, MEWTWO};
	public bool m_bIsMyTurn = false;
	public int m_nState;
	//for knowing which script to call for TakeDamage
	public int m_nUnitType;
	//iter for which position on the field you're in 0-2
	//0 : Center
	//1 : Top
	//2 : Bottom
	public int m_nPositionOnField = 0;
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
	protected int m_nStr;
	protected int m_nDef;
	protected int m_nSpd;
	protected int m_nEva;
	protected int m_nHit;

	//accessors
	public int GetMaxHP() {return m_nMaxHP;}
	public int GetCurHP() {return m_nCurHP;}
	public int GetSTR() {return m_nStr;}
	public int GetDEF() {return m_nDef;}
	public int GetSPD() {return m_nSpd;}
	public int GetEVA() {return m_nEva;}
	public int GetHIT() {return m_nHit;}
	//mutators
	public void SetMaxHP(int hp) {m_nMaxHP = hp;}
	public void SetCurHP(int hp) {m_nCurHP = hp;}
	public void SetSTR(int str) {m_nStr = str;}
	public void SetDEF(int def) {m_nDef = def;}
	public void SetSPD(int spd) {m_nSpd = spd;}
	public void SetEVA(int eva) {m_nEva = eva;}
	public void SetHIT(int hit) {m_nHit = hit;}


	//The current level of the character
	int m_nUnitLevel = 1;
	public int GetUnitLevel() {return m_nUnitLevel;}
	public void SetUnitLevel(int lvl) {m_nUnitLevel = lvl;}



	//List of status effects that could be effecting the player/units   Poison, Confusion, Paralyze, Stone (examples)
	public List<GameObject> m_lStatusEffects = new List<GameObject>();
	public void RemoveStatusEffect(string effectName)
	{
		for (int i = m_lStatusEffects.Count - 1; i >= 0; i--)
		{
			if(m_lStatusEffects[i].name == effectName)
			{
				m_lStatusEffects.RemoveAt(i);
			}
		}
	}
	//temp for status effect testing
	public GameObject m_poison;

	void Awake()
	{
		GameObject tw = GameObject.Find("TurnWatcher");
		if(tw)
		{
			tw.GetComponent<TurnWatcherScript>().AddMeToList(gameObject);
		}
	}
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//negative numbers are for healing
	public void AdjustHP(int dmg)
	{
		if(GetCurHP() > 0)
		{
			switch(m_nUnitType)
			{
			case (int)UnitTypes.ALLY_MELEE:
			{
				gameObject.GetComponent<PlayerBattleScript>().AdjustHP(dmg);
			}
				break;
			case (int)UnitTypes.ALLY_RANGED:
			{
				gameObject.GetComponent<PlayerBattleScript>().AdjustHP(dmg);
			}
				break;
			case (int)UnitTypes.BASICENEMY:
			{
				gameObject.GetComponent<BeserkEnemyScript>().AdjustHP(dmg);
			}
				break;
			case (int)UnitTypes.PERCENTENEMY:
			{
				gameObject.GetComponent<PercentBeserkEnemyScript>().AdjustHP(dmg);
			}
				break;
			case (int)UnitTypes.MEWTWO:
			{
				gameObject.GetComponent<BeserkEnemyScript>().AdjustHP(dmg);
			}
				break;
			}
		}
	}

	public void Missed()
	{
		if(GetCurHP() > 0)
		{
			switch(m_nUnitType)
			{
			case (int)UnitTypes.ALLY_MELEE:
			{
				gameObject.GetComponent<PlayerBattleScript>().Missed();
			}
				break;
			case (int)UnitTypes.ALLY_RANGED:
			{
				gameObject.GetComponent<PlayerBattleScript>().Missed();
			}
				break;
			case (int)UnitTypes.BASICENEMY:
			{
				gameObject.GetComponent<BeserkEnemyScript>().Missed();
			}
				break;
			case (int)UnitTypes.PERCENTENEMY:
			{
				gameObject.GetComponent<PercentBeserkEnemyScript>().Missed();
			}
				break;
			case (int)UnitTypes.MEWTWO:
			{
				gameObject.GetComponent<BeserkEnemyScript>().Missed();
			}
				break;
			}
		}
	}

	public void StartMyTurn()
	{
		//Update any of the status effects. (use a new list, as some of the master list may get removed
		for(int i = 0; i < m_lStatusEffects.Count; ++i)
		{
			if(m_lStatusEffects[i].GetComponent<BattleBaseEffectScript>().m_bToBeRemoved == true)
			{
				
				GameObject.Find("PersistantData").GetComponent<DCScript>().RemoveMeFromStatus(name, i);
				m_lStatusEffects.RemoveAt(i);
				i--;
			}
			else
				m_lStatusEffects[i].GetComponent<BattleBaseEffectScript>().m_dFunc();
		}
		m_bAllowInput = true;
	}
	
	public void EndMyTurn()
	{
		GameObject tw = GameObject.Find("TurnWatcher");
		if(tw)
		{
			tw.GetComponent<TurnWatcherScript>().MyTurnIsOver(gameObject);
			m_bFirstPass = true;
		}
	}

}
