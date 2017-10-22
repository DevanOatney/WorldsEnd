using UnityEngine;
using System.Collections;

public class CharacterPanelContainer : MonoBehaviour 
{
	[HideInInspector]
	public Transform m_goCharacterLevel;
	[HideInInspector]
	public Transform m_goCharacterEXP;
	[HideInInspector]
	public Transform m_goCharacterName;
	[HideInInspector]
	public Transform m_goCharacterPortrait;
	[HideInInspector]
	public Transform m_goCharacterCurHP;
	[HideInInspector]
	public Transform m_goCharacterMaxHP;
	[HideInInspector]
	public Transform m_goStatPagePanel;
	[HideInInspector]
	public Transform m_tHP, m_tPOW, m_tDEF, m_tSPD, m_tEVA, m_tHIT;

	void Awake()
	{
		m_goCharacterLevel = transform.Find("LVL");
		m_goCharacterEXP = transform.Find("EXP");
		m_goCharacterName = transform.Find("Character Name");
		m_goCharacterCurHP = transform.Find("HP").Find("CURHP");
		m_goCharacterMaxHP = transform.Find("HP").Find("MAXHP");
		m_goCharacterPortrait = transform.Find("Portrait");
		m_goStatPagePanel = transform.Find ("StatPage");
		m_tHP = m_goStatPagePanel.Find ("HP");
		m_tPOW = m_goStatPagePanel.Find ("POW");
		m_tDEF = m_goStatPagePanel.Find ("DEF");
		m_tSPD = m_goStatPagePanel.Find ("SPD");
		m_tEVA = m_goStatPagePanel.Find ("EVA");
		m_tHIT = m_goStatPagePanel.Find ("HIT");
	}
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () {
	}
}
