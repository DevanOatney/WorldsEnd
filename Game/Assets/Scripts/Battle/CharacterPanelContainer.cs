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
	void Awake()
	{
		m_goCharacterLevel = transform.Find("LVL");
		m_goCharacterEXP = transform.Find("EXP");
		m_goCharacterName = transform.Find("Character Name");
		m_goCharacterCurHP = transform.Find("HP").Find("CURHP");
		m_goCharacterMaxHP = transform.Find("HP").Find("MAXHP");
		m_goCharacterPortrait = transform.Find("Portrait");
	}
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () {
	}
}
