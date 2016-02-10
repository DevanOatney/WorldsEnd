using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class UnitScript : MonoBehaviour 
{
	public enum UnitTypes{ALLY_MELEE, ALLY_RANGED, NPC, BASICENEMY, PERCENTENEMY};
	public bool m_bIsMyTurn = false;
	public int m_nState;
	//for knowing which script to call for TakeDamage
	public int m_nUnitType;
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

	//effectName - name of effect, nTicks - how many rounds this lasts (-1 for permanent), nMod - any adjuster, like damage dealt, chance effect happens each round, etc.
	public void AddStatusEffect(string effectName, int nTicks, int nMod)
	{
		for(int i = 0; i < m_lStatusEffects.Count; ++ i)
		{
			if(effectName == m_lStatusEffects[i].name)
			{
				switch(m_lStatusEffects[i].GetComponent<BattleBaseEffectScript>().m_nEffectType)
				{
				case (int)BattleBaseEffectScript.EFFECT_TYPES.ePOISON:
					{
						//GameObject owner, int damage, int tickAmount)
						BattlePoisonEffectScript poisonScript = m_lStatusEffects[i].GetComponent<BattlePoisonEffectScript>();
						if(poisonScript.m_nAmountOfTicks < nTicks || nTicks == -1)
							poisonScript.m_nAmountOfTicks = nTicks;
						if(poisonScript.m_nMod < nMod)
							poisonScript.m_nMod = nMod;
						return;
					}
				case (int)BattleBaseEffectScript.EFFECT_TYPES.ePARALYZE:
					{
						return;
					}
				case (int)BattleBaseEffectScript.EFFECT_TYPES.eSTONE:
					{
						return;
					}
				}
			}
		}
		switch(effectName)
		{
		case "Poison":
			{
			}
			break;
		case "Paralyze":
			{
			}
			break;
		case "Stone":
			{
			}
			break;
		}
	}

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
				gameObject.GetComponent<CAllyBattleScript>().AdjustHP(dmg);
			}
				break;
			case (int)UnitTypes.ALLY_RANGED:
			{
				gameObject.GetComponent<CAllyBattleScript>().AdjustHP(dmg);
			}
				break;
			case (int)UnitTypes.BASICENEMY:
			{
				gameObject.GetComponent<StandardEnemyScript>().AdjustHP(dmg);
			}
				break;
			case (int)UnitTypes.PERCENTENEMY:
			{
				gameObject.GetComponent<PercentBeserkEnemyScript>().AdjustHP(dmg);
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
				gameObject.GetComponent<CAllyBattleScript>().Missed();
			}
				break;
			case (int)UnitTypes.ALLY_RANGED:
			{
				gameObject.GetComponent<CAllyBattleScript>().Missed();
			}
				break;
			case (int)UnitTypes.BASICENEMY:
			{
				gameObject.GetComponent<StandardEnemyScript>().Missed();
			}
				break;
			case (int)UnitTypes.PERCENTENEMY:
			{
				gameObject.GetComponent<PercentBeserkEnemyScript>().Missed();
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
		GameObject tw = GameObject.Find("TurnWatcher");
		if(tw)
		{
			m_bIsMyTurn = false;
			tw.GetComponent<TurnWatcherScript>().MyTurnIsOver(gameObject);
			m_bFirstPass = true;
		}
	}

	public void AttackAnimationEnded()
	{
		switch(m_nUnitType)
		{
		case (int)UnitTypes.ALLY_MELEE:
		{

		}
			break;
		case (int)UnitTypes.ALLY_RANGED:
		{

		}
			break;
		case (int)UnitTypes.BASICENEMY:
		{
			gameObject.GetComponent<StandardEnemyScript>().AttackAnimEnd();
		}
			break;
		case (int)UnitTypes.PERCENTENEMY:
		{
			gameObject.GetComponent<PercentBeserkEnemyScript>().EVENT_AttackAnimEnd();
		}
			break;
		}
	}

	public void IDied()
	{
		switch(m_nUnitType)
		{
		case (int)UnitTypes.ALLY_MELEE:
			{

			}
			break;
		case (int)UnitTypes.ALLY_RANGED:
			{

			}
			break;
		case (int)UnitTypes.BASICENEMY:
			{
				gameObject.GetComponent<StandardEnemyScript>().IDied();
			}
			break;
		case (int)UnitTypes.PERCENTENEMY:
			{
			}
			break;
		}

	}

	bool CheckIfHit()
	{
		GameObject[] posTargs = null;
		switch(m_nUnitType)
		{
		case (int)UnitTypes.ALLY_MELEE:
			{
				posTargs = GameObject.FindGameObjectsWithTag("Ally");
			}
			break;
		case (int)UnitTypes.ALLY_RANGED:
			{
				posTargs = GameObject.FindGameObjectsWithTag("Ally");
			}
			break;
		case (int)UnitTypes.BASICENEMY:
			{
				posTargs = GameObject.FindGameObjectsWithTag("Enemy");
			}
			break;
		case (int)UnitTypes.PERCENTENEMY:
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
}
