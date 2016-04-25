using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharacterUIPanelScript : MonoBehaviour 
{
	GameObject m_goCharacterName;
	GameObject m_goCurrentHP;
	GameObject m_goMaxHP;
	GameObject m_goCurrentMP;
	GameObject m_goMaxMP;
	GameObject m_goHP;
	GameObject m_goMP;
	GameObject m_goDivider;
	GameObject m_goDivider2;

	public int m_nFormationIter;

	float m_fUpdateTimer = 3.0f;
	float m_fUpdateDelayBucket = 2.0f;

	GameObject m_goTurnWatcher;
	// Use this for initialization
	void Start () 
	{
		m_goTurnWatcher = GameObject.Find("TurnWatcher");
		m_goCharacterName = transform.FindChild("CharacterName").gameObject;
		m_goCurrentHP = transform.FindChild("CurHP").gameObject;
		m_goMaxHP = transform.FindChild("MaxHP").gameObject;
		m_goCurrentMP = transform.FindChild("CurMP").gameObject;
		m_goMaxMP = transform.FindChild("MaxMP").gameObject;

		m_goHP = transform.FindChild("HP").gameObject;
		m_goMP = transform.FindChild("MP").gameObject;
		m_goDivider = transform.FindChild("divider").gameObject;
		m_goDivider2 = transform.FindChild("divider2").gameObject;
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_fUpdateTimer += Time.deltaTime;
		if(m_fUpdateTimer >= m_fUpdateDelayBucket)
		{
			bool _bFoundUnit = false;
			List<GameObject> party = m_goTurnWatcher.GetComponent<TurnWatcherScript>().m_goUnits;
			foreach(GameObject unit in party)
			{
				if(unit.GetComponent<UnitScript>().FieldPosition == m_nFormationIter && unit.GetComponent<UnitScript>().m_nUnitType <= (int)UnitScript.UnitTypes.NPC)
				{
					_bFoundUnit = true;
					m_goCharacterName.GetComponent<Text>().text = unit.name;
					m_goCurrentHP.GetComponent<Text>().text = unit.GetComponent<UnitScript>().GetCurHP().ToString();
					m_goMaxHP.GetComponent<Text>().text = unit.GetComponent<UnitScript>().GetMaxHP().ToString();
					m_goCurrentMP.GetComponent<Text>().text = unit.GetComponent<UnitScript>().GetCurMP().ToString();
					m_goMaxMP.GetComponent<Text>().text = unit.GetComponent<UnitScript>().GetMaxMP().ToString();

					m_goHP.SetActive(true);
					m_goMP.SetActive(true);
					m_goDivider.SetActive(true);
					m_goDivider2.SetActive(true);
					break;
				}
			}
			if(_bFoundUnit == false)
			{
				m_goCharacterName.GetComponent<Text>().text = "";
				m_goCurrentHP.GetComponent<Text>().text = "";
				m_goMaxHP.GetComponent<Text>().text = "";
				m_goCurrentMP.GetComponent<Text>().text = "";
				m_goMaxMP.GetComponent<Text>().text = "";
				m_goHP.SetActive(false);
				m_goMP.SetActive(false);
				m_goDivider.SetActive(false);
				m_goDivider2.SetActive(false);
			}
			m_fUpdateTimer = 0.0f;
		}
	}
}
