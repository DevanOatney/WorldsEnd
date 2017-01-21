using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StandardEnemyScript : UnitScript 
{
	public enum ENEMY_STATES {eIDLE, eCHARGE, eRETURN, eATTACK, eDAMAGED, eSTATUS_EFFECT, eDEAD};
	//for fading text
	public GameObject m_goFadingText;
	//Enemy Stats
	public TextAsset m_taStats;
	float m_fMovementSpeed = 8.0f;
	//delay to make it look like some sort of calculation is happening when it's the enemies turn (lol)
	protected float m_fDelayBucket = 1.0f;
	protected float m_fDelayTimer = 0.0f;
	//Stuff for the shadow clones that spawn during movement... maybe special attacks if I have time?
	public GameObject m_goShadowClone;
	protected float m_fShadowTimer = 0.0f;
	protected float m_fShadowTimerBucket = 0.1f;
	public Vector3 m_vTargetPosition = new Vector3();
	public int m_nGoldDropped = 0;

	public List<DroppedItem> m_lItemsThatCanDrop;
	public class DroppedItem
	{
		public string m_szItemName;
		public int m_nAmount;
		public int m_nChance;
	}

	// Use this for initialization
	void Start () 
	{
		Initialize();
	}

	protected void Initialize()
	{
		SetUnitStats();
		m_vInitialPos = new Vector3();
		m_aAnim = GetComponent<Animator>();
		if(m_aAnim == null)
		{
			m_aAnim = GetComponentInChildren<Animator>();
		}
		UpdatePositionOnField();
	}
	
	// Update is called once per frame
	void Update () 
	{
		HandleStates();
	}

	protected void HandleStates()
	{
		switch(m_nState)
		{
		case (int)ENEMY_STATES.eIDLE:
			{
				if(m_bIsMyTurn == true && GameObject.Find("TurnWatcher").GetComponent<TurnWatcherScript>().GetAllyCount() > 0)
				{
					m_fDelayTimer += Time.deltaTime;
					if(m_fDelayTimer >= m_fDelayBucket)
					{
						switch(m_nUnitType)
						{
						case UnitScript.UnitTypes.PERCENTENEMY:
							{
								//Pick from the available enemy (the allies) targets, attack the one with the lowest HP
								GameObject WeakestTarget = null;
								int lowestHP = int.MaxValue;
								GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Ally");
								foreach(GameObject tar in posTargs)
								{
									if(tar.name.Contains("(Clone)"))
										continue;
									if(tar.GetComponent<UnitScript>().GetCurHP() < lowestHP && tar.GetComponent<UnitScript>().GetCurHP() > 0)
									{
										WeakestTarget = tar;
										lowestHP = WeakestTarget.GetComponent<UnitScript>().GetCurHP();
									}
								}
								if(WeakestTarget != null)
								{
									m_nState = (int)ENEMY_STATES.eCHARGE;
									m_aAnim.SetBool("m_bIsMoving", true);
									m_fDelayTimer = 0.0f;
									m_nTargetPositionOnField = WeakestTarget.GetComponent<UnitScript>().FieldPosition;
									GameObject targetPosition = GameObject.Find("Near_Ally" + m_nTargetPositionOnField);
									m_vTargetPosition = targetPosition.transform.position;
								}
							}
							break;
						case UnitScript.UnitTypes.BASICENEMY:
							{
								//Pick from the available enemy (the allies) targets, attack the one with the lowest HP
								GameObject WeakestTarget = null;
								int lowestHP = int.MaxValue;
								GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Ally");
								foreach(GameObject tar in posTargs)
								{
									if(tar.name.Contains("(Clone)"))
										continue;
									if(tar.GetComponent<UnitScript>().GetCurHP() < lowestHP && tar.GetComponent<UnitScript>().GetCurHP() > 0)
									{
										WeakestTarget = tar;
										lowestHP = WeakestTarget.GetComponent<UnitScript>().GetCurHP();
									}
								}
								if(WeakestTarget != null)
								{
									m_nState = (int)ENEMY_STATES.eCHARGE;
									m_aAnim.SetBool("m_bIsMoving", true);
									m_fDelayTimer = 0.0f;
									m_nTargetPositionOnField = WeakestTarget.GetComponent<UnitScript>().FieldPosition;
									GameObject targetPosition = GameObject.Find("Near_Ally" + m_nTargetPositionOnField);
									m_vTargetPosition = targetPosition.transform.position;
								}
							}
							break;
						}
					}
				}
			}
			break;
		case (int)ENEMY_STATES.eCHARGE:
			{
				Vector3 dir = m_vTargetPosition - transform.position;
				if(dir.sqrMagnitude <= 0.1f)
				{
					//Reached target
					m_aAnim.SetBool("m_bIsMoving", false);
					m_aAnim.SetTrigger("m_bIsAttacking");
					m_nState = (int)ENEMY_STATES.eATTACK;
				}
				else
				{
					//continue moving toward target
					dir.Normalize();
					Vector3 curPos = transform.position;
					curPos += dir * m_fMovementSpeed * Time.deltaTime;
					transform.position = curPos;
					
					if(m_fShadowTimer >= m_fShadowTimerBucket)
					{
						if(m_goShadowClone != null)
						{
							GameObject newShadow = Instantiate(m_goShadowClone, transform.position, Quaternion.identity) as GameObject;
							newShadow.GetComponent<SpriteRenderer>().sprite = m_aAnim.gameObject.GetComponent<SpriteRenderer>().sprite;
							Vector3 cloneTransform = m_aAnim.gameObject.transform.localScale;
							newShadow.transform.localScale = cloneTransform;
							//adjust so the clone is behind the unit
							if(GetComponent<SpriteRenderer>() != null)
								newShadow.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
							else
								newShadow.GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder - 1;
							Destroy(newShadow, m_fShadowTimerBucket*3);
							m_fShadowTimer = 0.0f;
						}
					}
					else
						m_fShadowTimer += Time.deltaTime;
				}

			}
			break;
		case (int)ENEMY_STATES.eRETURN:
			{
				Vector3 targetPos = m_vTargetPosition;
				Vector3 dir = targetPos - transform.position;
				if(dir.sqrMagnitude <= 0.1f)
				{
					transform.position = m_vInitialPos;
					m_nState = (int)ENEMY_STATES.eSTATUS_EFFECT;
					m_aAnim.SetBool("m_bIsMoving", false);
				}
				else
				{
					dir.Normalize();
					Vector3 curPos = transform.position;
					curPos += dir * m_fMovementSpeed * Time.deltaTime;
					transform.position = curPos;
				}
			}
			break;
		case (int)ENEMY_STATES.eATTACK:
			{

			}
			break;
		case (int) ENEMY_STATES.eDAMAGED:
			{
			}
			break;
		case (int) ENEMY_STATES.eSTATUS_EFFECT:
			{
				//Update any of the status effects. (use a new list, as some of the master list may get removed
				for(int i = 0; i < m_lStatusEffects.Count; ++i)
				{
					if(m_lStatusEffects[i].GetComponent<BattleBaseEffectScript>().m_bToBeRemoved == true)
					{
						m_lStatusEffects.RemoveAt(i);
						i--;
					}
					else
						m_lStatusEffects[i].GetComponent<BattleBaseEffectScript>().m_dFunc();
				}
				m_nState = (int)ENEMY_STATES.eIDLE;
				EndMyTurn();
			}
			break;
		case (int)ENEMY_STATES.eDEAD:
			{
			}
			break;
		}
	}

	protected void SetUnitStats()
	{
		string[] stats = m_taStats.text.Split('\n');
		List<string> _lStats = new List<string>();
		foreach(string stat in stats)
		{
			string[] piece = stat.Split(':');
			if(piece.Length > 2)
			{
				//This is not a standard stat, just add the whole thing for later
				_lStats.Add(stat);
			}
			//error catch for if there's an empty line.
			else if(piece.Length <= 1)
				continue;
			else
				_lStats.Add(piece[1].Trim());
		}
		//Max HP
		SetMaxHP(int.Parse(_lStats[0].Trim()));
		SetCurHP(GetMaxHP());
		//STR
		SetSTR(int.Parse(_lStats[1].Trim()));
		//DEF
		SetDEF(int.Parse(_lStats[2].Trim()));
		//SPD
		SetSPD(int.Parse(_lStats[3].Trim()));
		//HIT
		SetHIT(int.Parse(_lStats[4].Trim()));
		//EVA
		SetEVA(int.Parse(_lStats[5].Trim()));
		//Lvl
		SetUnitLevel(int.Parse(_lStats[6].Trim()));
		//Gold dropped
		m_nGoldDropped = int.Parse(_lStats[7].Trim());
		int itemCount = int.Parse(_lStats[8].Trim());
		m_lItemsThatCanDrop = new List<DroppedItem>();
		int totalChance = 0;
		for(int i = 9; i-9 < itemCount; ++i)
		{
			string[] itemPieces = _lStats[i].Split(',');
			DroppedItem item = new DroppedItem();
			item.m_szItemName = itemPieces[0].Trim().Split(':')[1].Trim();
			item.m_nAmount = int.Parse(itemPieces[1].Trim().Split(':')[1].Trim());
			item.m_nChance = int.Parse(itemPieces[2].Trim().Split(':')[1].Trim());
			totalChance += item.m_nChance;
			m_lItemsThatCanDrop.Add(item);
		}
		if(totalChance < 100)
		{
			//there's space, add in an empty null space for nothing to drop
			DroppedItem item = new DroppedItem();
			item.m_szItemName = "";
			item.m_nAmount = 0;
			item.m_nChance = 100 - totalChance;
			m_lItemsThatCanDrop.Add(item);
		}
	}

	override public void AdjustHP(int dmg)
	{
		GameObject newText = Instantiate(m_goFadingText);
		if(dmg >= 0)
		{
			dmg = dmg - m_nDef;
			if(dmg < 1)
				dmg = 1;
		}
		if(dmg < 0)
			dmg = 0;
		m_nCurHP -= dmg;
		if(m_nCurHP <= 0)
		{
			m_nState = (int)ENEMY_STATES.eDEAD;
			m_aAnim.SetBool("m_bIsDying", true);
		}
		else
		{
			m_aAnim.SetTrigger("m_bIsDamaged");
		}
		if(dmg >= 0)
			newText.GetComponent<GUI_FadeText>().SetColor(true);
		else
			newText.GetComponent<GUI_FadeText>().SetColor(false);

		newText.GetComponent<GUI_FadeText>().SetText((Mathf.Abs(dmg)).ToString());
		newText.GetComponent<GUI_FadeText>().SetShouldFloat(true);
		Vector3 textPos = transform.GetComponent<Collider>().transform.position;
		textPos.y += (gameObject.GetComponent<BoxCollider>().size.y * 0.75f);
		textPos = Camera.main.WorldToViewportPoint(textPos);
		newText.transform.position = textPos;
	}

	public void AttackAnimEnd()
	{
		GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Ally");
		foreach(GameObject tar in posTargs)
		{
			if(tar.GetComponent<UnitScript>().FieldPosition == m_nTargetPositionOnField)
			{
				int nChanceToHit = UnityEngine.Random.Range(0,100);
				int nRange = 60 + m_nHit - tar.GetComponent<UnitScript>().GetEVA();
				if(nRange < 5)
					nRange = 5;
				if(nChanceToHit <	nRange)
				{
					int dmgAdjustment = UnityEngine.Random.Range(0, m_nStr/2);
					if(dmgAdjustment + m_nStr < 1)
						tar.GetComponent<UnitScript>().AdjustHP(1);
					else
						tar.GetComponent<UnitScript>().AdjustHP(m_nStr + dmgAdjustment);
				}
				else
					tar.GetComponent<UnitScript>().Missed();

				break;
			}
		}
		m_nState = (int)ENEMY_STATES.eRETURN;
		m_aAnim.SetBool("m_bIsMoving", true);
		m_vTargetPosition = GameObject.Find("Enemy_StartPos" + FieldPosition).transform.position;
	}

	override public void Missed()
	{
		GameObject newText = Instantiate(m_goFadingText);
		newText.GetComponent<GUI_FadeText>().SetColor(true);
		newText.GetComponent<GUI_FadeText>().SetText("Miss");
		newText.GetComponent<GUI_FadeText>().SetShouldFloat(true);
		Vector3 textPos = transform.GetComponent<Collider>().transform.position;
		textPos.y += (gameObject.GetComponent<BoxCollider>().size.y * 0.75f);
		textPos = Camera.main.WorldToViewportPoint(textPos);
		newText.transform.position = textPos;
	}

	override public void IDied()
	{
		GameObject.Find("TurnWatcher").GetComponent<TurnWatcherScript>().RemoveMeFromList(gameObject, 0.0f);
		Destroy(gameObject);
	}

	public void UpdatePositionOnField()
	{
		string szgoName = "Enemy_StartPos" + FieldPosition.ToString();
		GameObject go = GameObject.Find(szgoName);
		m_vInitialPos.x = go.transform.position.x;
		m_vInitialPos.y = go.transform.position.y;
		m_vInitialPos.z = 0.0f;
		switch(FieldPosition)
		{
		case 0:
			{
				//Top Left
			}
			break;
		case 1:
			{
				//Middle Left
				m_aAnim.gameObject.GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder + 1;
			}
			break;
		case 2:
			{
				//Bottom Left
				m_aAnim.gameObject.GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder + 2;
			}
			break;
		case 3:
			{
				//Top Right
			}
			break;
		case 4:
			{
				//Middle Right
				m_aAnim.gameObject.GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder + 1;
			}
			break;
		case 5:
			{
				//Bottom Right
				m_aAnim.gameObject.GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder + 2;
			}
			break;
		}
		transform.position = m_vInitialPos;
	}


}
