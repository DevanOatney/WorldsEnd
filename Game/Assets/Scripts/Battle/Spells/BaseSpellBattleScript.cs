using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseSpellBattleScript : MonoBehaviour 
{
	public enum ELEMENTS{eWATER, eEARTH, eWIND, eFIRE, eLIGHTNING}
	public string m_szSpellName;
	public int m_nTargetType;//1 - single heal, 2- aoe heal, 3 - single dmg, 4 - aoe dmg
	public int m_nElementType;
	public int m_nMPCost;
	public int m_nHPMod;
	public int m_nMPMod;
	public int m_nPOWMod;
	public int m_nDEFMod;
	public int m_nSPDMod;
	public int m_nHITMod;
	public int m_nEVAMod;
	public List<string> m_lStatusEffect;

	[HideInInspector]
	public bool m_bShouldActivate;
	[HideInInspector]
	public GameObject m_goOwner;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public virtual void Initialize(GameObject _pOwner)
	{
		m_goOwner = _pOwner;
	}

	public virtual void DoneAnimating()
	{
		m_goOwner.GetComponent<Animator>().SetBool("m_bIsCasting", false);
		m_goOwner.GetComponent<UnitScript>().AdjustMP(m_nMPCost * -1);
		m_goOwner.GetComponent<UnitScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.STATUS_EFFECTS;
		Destroy(gameObject);
	}
	public void DoneWithRuneEffect()
	{
	}
}
