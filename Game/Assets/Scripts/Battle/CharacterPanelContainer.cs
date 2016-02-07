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
	// Use this for initialization
	void Start () 
	{
		m_goCharacterLevel = transform.FindChild("LVL");
		m_goCharacterEXP = transform.FindChild("EXP");
		m_goCharacterName = transform.FindChild("Character Name");
		m_goCharacterCurHP = transform.FindChild("HP").FindChild("CURHP");
		m_goCharacterMaxHP = transform.FindChild("HP").FindChild("MAXHP");
		m_goCharacterPortrait = transform.FindChild("Portrait");
	}
	
	// Update is called once per frame
	void Update () {
	}
}
