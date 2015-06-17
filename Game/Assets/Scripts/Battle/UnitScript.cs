using UnityEngine;
using System.Collections;

public class UnitScript : MonoBehaviour 
{
	public enum UnitTypes{Ally, NPC, BASICENEMY, CHARACTERREF, MEWTWO};
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
			case (int)UnitTypes.Ally:
			{
				gameObject.GetComponent<PlayerBattleScript>().AdjustHP(dmg);
			}
				break;
			case (int)UnitTypes.BASICENEMY:
			{
				gameObject.GetComponent<BeserkEnemyScript>().AdjustHP(dmg);
			}
				break;
			case (int)UnitTypes.CHARACTERREF:
			{
				gameObject.GetComponent<BeserkEnemyScript>().AdjustHP(dmg);
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
}
