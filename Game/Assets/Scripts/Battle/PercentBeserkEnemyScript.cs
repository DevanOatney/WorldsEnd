using UnityEngine;
using System.Collections;

public class PercentBeserkEnemyScript : UnitScript {
	public enum States{eIDLE, eCHARGE, eRETURN, eATTACK, eDAMAGED, eDEAD};
	Animator anim;
	public Animator GetAnim() {return anim;}
	float m_fMovementSpeed = 8.0f;
	
	//timer for if the enemy is attacking, degrades if set and when at 0, deals damage to the player
	float m_fAttackBucket = 0.0f;
	public float GetAttackBucket() {return m_fAttackBucket;}
	float m_fDamagedBucket = 0.0f;
	public float GetDamagedBucket() {return m_fAttackBucket;}
	float m_fDeadBucket = 0.0f;
	//flag for if the hit proc has happened during this turn (after each turn)
	bool m_bHasSwung = false;
	
	
	//for fading text
	public GameObject m_goFadingText;
	//for basic attack audio
	public AudioClip m_acAttackAudio;
	//for getting hit
	public AudioClip m_acDamagedAudio;
	//For charging in (boss)
	public AudioClip m_acChargingAudio;
	//for dying
	public AudioClip m_acDyingAudio;
	
	//delay to make it look like some sort of calculation is happening when it's the enemies turn (lol)
	float m_fDelayBucket = 2.0f;
	float m_fDelayTimer = 0.0f;
	
	
	//animation for the unit dying
	public AnimationClip m_acDyingAnim;
	//animation for the unit attacking
	public AnimationClip m_acAttackAnim;
	
	//Stuff for the shadow clones that spawn during movement... maybe special attacks if I have time?
	public GameObject m_goShadowClone;
	float m_fShadowTimer = 0.0f;
	float m_fShadowTimerBucket = 0.1f;
	
	//Enemy Stats
	public TextAsset m_taStats;
	public float m_fPercentToEndFight = 0.25f;

	//Used for if this enemy can now actually be killed
	bool m_bShouldIgnorePercent = false;

	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		if(anim == null)
			anim = GetComponentInChildren<Animator>();
		SetUnitStats();
		m_vInitialPos = new Vector3();
		string szgoName = "Enemy_StartPos" + m_nPositionOnField.ToString();
		GameObject go = GameObject.Find(szgoName);
		m_vInitialPos.x = go.transform.position.x;
		m_vInitialPos.y = go.transform.position.y;
		m_vInitialPos.z = 0.0f;
		switch(m_nPositionOnField)
		{
		case 0:
		{
			//Middle
		}
			break;
		case 1:
		{
			//Top
			if(GetComponent<SpriteRenderer>() != null)
				GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
			else 
				GetComponentInChildren<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder - 1;
		}
			break;
		case 2:
		{
			//Bottom
			if(GetComponent<SpriteRenderer>() != null)
				GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
			else 
				GetComponentInChildren<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder + 1;
		}
			break;
		}
		transform.position = m_vInitialPos;


		CheckUnitChange();
	}

	void CheckUnitChange()
	{
		//used for if this unit can now actually be killed, unsure what this will become, currently just check if it's the
		//second time you fight the boar boss
		int result;
		GameObject dc = GameObject.Find("PersistantData");
		if(dc.GetComponent<DCScript>().m_dStoryFlagField.TryGetValue("ToAEvent", out result))
		{
			m_bShouldIgnorePercent = true;
		}
	}

	void SetUnitStats()
	{
		string[] stats = m_taStats.text.Split('\n');
		//Max HP
		SetMaxHP(int.Parse(stats[0].Trim()));
		SetCurHP(GetMaxHP());
		//STR
		SetSTR(int.Parse(stats[1].Trim()));
		//DEF
		SetDEF(int.Parse(stats[2].Trim()));
		//SPD
		SetSPD(int.Parse(stats[3].Trim()));
		//HIT
		SetHIT(int.Parse(stats[4].Trim()));
		//EVA
		SetEVA(int.Parse(stats[5].Trim()));
		//Lvl
		SetUnitLevel(int.Parse(stats[6].Trim()));
	}
	// Update is called once per frame
	void Update () 
	{
		
		if(m_bIsMyTurn && GameObject.Find("TurnWatcher").GetComponent<TurnWatcherScript>().GetAllyCount() > 0 && m_nState != (int)States.eDEAD)
		{
			//Make sure somethings even alive on the map to fight against
			
			m_fDelayTimer += Time.deltaTime;
			if(m_fDelayTimer >= m_fDelayBucket)
			{
				switch(m_nUnitType)
				{
				case (int)UnitScript.UnitTypes.PERCENTENEMY:
				{
					//Pick from the available enemy (the allies) targets, attack the one with the lowest HP
					GameObject WeakestTarget = null;
					int lowestHP = int.MaxValue;
					GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Ally");
					foreach(GameObject tar in posTargs)
					{
						if(tar.GetComponent<UnitScript>().GetCurHP() < lowestHP && tar.GetComponent<UnitScript>().GetCurHP() > 0)
						{
							WeakestTarget = tar;
							lowestHP = WeakestTarget.GetComponent<UnitScript>().GetCurHP();
						}
					}
					if(WeakestTarget != null)
					{
						m_nState = (int)States.eCHARGE;
						if(m_acChargingAudio)
							GetComponent<AudioSource>().PlayOneShot(m_acChargingAudio, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
						anim.SetBool("m_bIsMoving", true);
						m_bIsMyTurn = false;
						m_fDelayTimer = 0.0f;
						m_nTargetPositionOnField = WeakestTarget.GetComponent<UnitScript>().m_nPositionOnField;
					}
				}
					break;
				case (int)UnitScript.UnitTypes.BASICENEMY:
				{
					//Pick from the available enemy (the allies) targets, attack the one with the lowest HP
					GameObject WeakestTarget = null;
					int lowestHP = int.MaxValue;
					GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Ally");
					foreach(GameObject tar in posTargs)
					{
						if(tar.GetComponent<UnitScript>().GetCurHP() < lowestHP && tar.GetComponent<UnitScript>().GetCurHP() > 0)
						{
							WeakestTarget = tar;
							lowestHP = WeakestTarget.GetComponent<UnitScript>().GetCurHP();
						}
					}
					if(WeakestTarget != null)
					{
						m_nState = (int)States.eCHARGE;
						if(m_acChargingAudio)
							GetComponent<AudioSource>().PlayOneShot(m_acChargingAudio, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
						anim.SetBool("m_bIsMoving", true);
						m_bIsMyTurn = false;
						m_fDelayTimer = 0.0f;
						m_nTargetPositionOnField = WeakestTarget.GetComponent<UnitScript>().m_nPositionOnField;
					}
				}
					break;
				case (int)UnitScript.UnitTypes.MEWTWO:
				{
					GameObject WeakestTarget = null;
					int lowestHP = int.MaxValue;
					GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Ally");
					foreach(GameObject tar in posTargs)
					{
						if(tar.GetComponent<UnitScript>().GetCurHP() < lowestHP && tar.GetComponent<UnitScript>().GetCurHP() > 0)
						{
							WeakestTarget = tar;
							lowestHP = WeakestTarget.GetComponent<UnitScript>().GetCurHP();
						}
					}
					if(WeakestTarget != null)
					{
						GetComponent<MewtwoAttackScript>().BeginFiring(WeakestTarget);
						//TODO: make it so that the unit moves to a position in which to fire? or something.
						Vector3 newPos = transform.position;
						newPos.y = WeakestTarget.transform.position.y;
						newPos.x += 2.0f;
						GetComponentInChildren<Animator>().SetBool("m_bIsCasting", true);
						m_bIsMyTurn = false;
					}
				}
					break;
				}
			}
		}
		HandleStates();
	}
	
	void HandleStates()
	{
		switch(m_nState)
		{
		case (int)States.eIDLE:
		{
		}
			break;
		case (int)States.eCHARGE:
		{
			GameObject target = GameObject.Find("Near_Ally" + m_nTargetPositionOnField.ToString());
			if(target)
			{
				Vector3 targetPos = target.transform.position;
				Vector3 dir = targetPos - transform.position;
				dir.Normalize();
				Vector3 curPos = transform.position;
				curPos += dir * m_fMovementSpeed * Time.deltaTime;
				transform.position = curPos;
				
				if(m_fShadowTimer >= m_fShadowTimerBucket)
				{

					GameObject newShadow = Instantiate(m_goShadowClone, transform.position, Quaternion.identity) as GameObject;
					if(newShadow)
					{
						newShadow.GetComponent<SpriteRenderer>().sprite = anim.gameObject.GetComponent<SpriteRenderer>().sprite;
						Vector3 cloneTransform = anim.gameObject.transform.localScale;
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
		case (int)States.eRETURN:
		{
			GameObject target = GameObject.Find("Enemy_StartPos" + m_nPositionOnField.ToString());
			if(target)
			{
				Vector3 targetPos = target.transform.position;
				Vector3 dir = targetPos - transform.position;
				dir.Normalize();
				Vector3 curPos = transform.position;
				curPos += dir * m_fMovementSpeed * Time.deltaTime;
				transform.position = curPos;
			}
		}
			break;
		case (int)States.eATTACK:
		{
			m_fAttackBucket -= Time.deltaTime;
			if(m_fAttackBucket >= m_acAttackAnim.length * 0.5f && m_bHasSwung == false)
			{
				m_bHasSwung = true;
				GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Ally");
				foreach(GameObject tar in posTargs)
				{
					if(tar.GetComponent<UnitScript>().m_nPositionOnField == m_nTargetPositionOnField)
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
			}
			if(m_fAttackBucket <= 0.001f)
			{
				m_fAttackBucket = 0.0f;
				m_nState = (int)States.eRETURN;
				anim.SetBool("m_bIsAttacking", false);
				anim.SetBool("m_bIsMoving", true);
				m_bHasSwung = false;
			}
		}
			break;
		case (int) States.eDAMAGED:
		{
			m_fDamagedBucket -= Time.deltaTime;
			if(m_fDamagedBucket <= 0.0f)
			{
				m_nState = (int)States.eIDLE;
				anim.SetBool("m_bIsDamaged", false);
			}
		}
			break;
		case (int)States.eDEAD:
		{
			m_fDeadBucket -= Time.deltaTime;
			if(m_fDeadBucket <= 0.0f)
			{
				//end
				Destroy(gameObject);
			}
		}
			break;
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.name == "Near_Ally" + m_nTargetPositionOnField.ToString() || other.name == "Enemy_StartPos" + m_nPositionOnField.ToString())
			WaypointTriggered(other);
	}
	public void WaypointTriggered(Collider c)
	{
		
		if(c.name == "Near_Ally" + m_nTargetPositionOnField.ToString())
		{
			anim.SetBool("m_bIsMoving", false);
			anim.SetBool("m_bIsAttacking", true);
			m_nState = (int)States.eATTACK;
			m_fAttackBucket = m_acAttackAnim.length + 0.01f;
			c.enabled = false;
			GameObject wypnt = GameObject.Find("Enemy_StartPos" + m_nPositionOnField.ToString());
			if(wypnt)
			{
				wypnt.GetComponent<BoxCollider>().enabled = true;
			}
			GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Ally");
			foreach(GameObject tar in posTargs)
			{
				if(tar.GetComponent<UnitScript>().m_nPositionOnField == m_nTargetPositionOnField)
				{
					GameObject GO = GameObject.Find("PersistantData");
					if(GO != null)
					{
						GetComponent<AudioSource>().PlayOneShot(m_acAttackAudio, 0.5f + GO.GetComponent<DCScript>().m_fSFXVolume);
					}
					else
						GetComponent<AudioSource>().PlayOneShot(m_acAttackAudio, 0.5f);
					
				}
			}
			
			Invoke("EndMyTurn", 2.5f);
		}
		else if(c.name == "Enemy_StartPos" + m_nPositionOnField.ToString())
		{
			transform.position = m_vInitialPos;
			
			if(anim)
				anim.SetBool("m_bIsMoving", false);
			c.enabled = false;
			m_nState = (int)States.eIDLE;
			GameObject wypnt = GameObject.Find("Near_Ally" + m_nTargetPositionOnField.ToString());
			if(wypnt)
			{
				wypnt.GetComponent<BoxCollider>().enabled = true;
			}
		}
	}
	
	new public void Missed()
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
	
	new public void AdjustHP(int dmg)
	{
		GameObject newText = Instantiate(m_goFadingText);
		GameObject GO = GameObject.Find("PersistantData");
		if(dmg >= 0)
		{
			dmg = dmg - m_nDef;
			if(dmg < 1)
				dmg = 1;
		}
		if(dmg < 0)
			dmg = 0;
		m_nCurHP -= dmg;
		if(m_nCurHP / m_nMaxHP <= m_fPercentToEndFight && m_bShouldIgnorePercent == false)
		{
			m_nState = (int)States.eIDLE;
			GameObject tw = GameObject.Find("TurnWatcher");
			if(tw)
			{
				tw.GetComponent<TurnWatcherScript>().RemoveMeFromList(gameObject, m_acDyingAnim.length);
			}
		}
		else if(m_nCurHP <= 0)
		{
			m_nState = (int)States.eDEAD;
			m_fDeadBucket = m_acDyingAnim.length;
			anim.SetBool("m_bIsDying", true);
			if(m_acDyingAudio)
				GetComponent<AudioSource>().PlayOneShot(m_acDyingAudio, 0.5f + GO.GetComponent<DCScript>().m_fSFXVolume);
			GameObject tw = GameObject.Find("TurnWatcher");
			if(tw)
			{
				tw.GetComponent<TurnWatcherScript>().RemoveMeFromList(gameObject, m_acDyingAnim.length);
			}
		}
		else
		{
			if(m_acDamagedAudio)
				GetComponent<AudioSource>().PlayOneShot(m_acDamagedAudio, 0.5f + GO.GetComponent<DCScript>().m_fSFXVolume);
			anim.SetBool("m_bIsDamaged", true);
			m_nState = (int)States.eDAMAGED;
			m_fDamagedBucket = 1.0f;
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
	//Accessor to the function, original function can't be public as it breaks animation event accessibility
	public void EVENT_AttackAnimEnd()
	{
		AttackAnimationEnd();
	}
	void AttackAnimationEnd()
	{
		GameObject[] posTargs = GameObject.FindGameObjectsWithTag("Ally");
		foreach(GameObject tar in posTargs)
		{
			if(tar.GetComponent<UnitScript>().m_nPositionOnField == m_nTargetPositionOnField)
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
		m_nState = (int)States.eRETURN;
		anim.SetBool("m_bIsAttacking", false);
		anim.SetBool("m_bIsMoving", true);
	}
}
