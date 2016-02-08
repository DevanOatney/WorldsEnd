using UnityEngine;
using System.Collections;

public class StandardEnemyScript : UnitScript 
{
	public enum ENEMY_STATES {eIDLE, eCHARGE, eRETURN, eATTACK, eDAMAGED, eDEAD};
	Animator m_aAnim;
	//for fading text
	public GameObject m_goFadingText;
	//Enemy Stats
	public TextAsset m_taStats;

	void Awake()
	{
		m_aAnim = GetComponent<Animator>();
		if(m_aAnim == null)
		{
			m_aAnim = GetComponentInChildren<Animator>();
		}
	}

	// Use this for initialization
	void Start () 
	{
		SetUnitStats();
		m_vInitialPos = new Vector3();
		UpdatePositionOnField();
	}
	
	// Update is called once per frame
	void Update () {
	
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
		if(m_nCurHP <= 0)
		{
			m_nState = (int)ENEMY_STATES.eDEAD;
			m_aAnim.SetBool("m_bIsDying", true);
		}
		else
		{
			m_aAnim.SetBool("m_bIsDamaged", true);
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
		m_aAnim.SetBool("m_bIsAttacking", false);
		m_aAnim.SetBool("m_bIsMoving", true);
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
