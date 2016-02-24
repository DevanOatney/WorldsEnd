using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleBaseEffectScript : MonoBehaviour 
{
	public enum EFFECT_TYPES{ePOISON, ePARALYZE, eSTONE};
	public int m_nEffectType = -1;
	public GameObject m_goOwner;
	public delegate void m_delegate();
	public m_delegate m_dFunc;
	public bool m_bToBeRemoved = false;
	//used for if this effect has an amount of charges, set to -1 for infinite charges.. but.. you shouldn't.
	public int m_nAmountOfTicks = 0;
	public int m_nHPMod;
	public int m_nMPMod;
	public int m_nPOWMod;
	public int m_nDEFMod;
	public int m_nSPDMod;
	public int m_nHITMod;
	public int m_nEVAMod;
	//if this is set, destroy the game object
	public bool m_bHasBeenRemoved = false;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public virtual void Initialize(GameObject owner, int nEffectType, int nTickCount, int nHP, int nMP, int nPOW, int nDEF, int nSPD, int nHIT, int nEVA)
	{
		
	}

	public virtual bool RefreshEffect(DCScript.StatusEffect newEffect)
	{
		if(m_nAmountOfTicks < newEffect.m_nAmountOfTicks)
			m_nAmountOfTicks = newEffect.m_nAmountOfTicks;
		if(m_nHPMod < newEffect.m_nHPMod)
			m_nHPMod = newEffect.m_nHPMod;
		if(m_nMPMod < newEffect.m_nMPMod)
			m_nMPMod = newEffect.m_nMPMod;
		if(m_nPOWMod < newEffect.m_nPOWMod)
			m_nPOWMod = newEffect.m_nPOWMod;
		if(m_nDEFMod < newEffect.m_nDEFMod)
			m_nDEFMod = newEffect.m_nDEFMod;
		if(m_nSPDMod < newEffect.m_nSPDMod)
			m_nSPDMod = newEffect.m_nSPDMod;
		if(m_nHITMod < newEffect.m_nHITMod)
			m_nHITMod = newEffect.m_nHITMod;
		if(m_nEVAMod < newEffect.m_nEVAMod)
			m_nEVAMod = newEffect.m_nEVAMod;
		return false;
	}

	public virtual DCScript.StatusEffect ConvertToDCStatusEffect()
	{
		DCScript.StatusEffect newEffect = new DCScript.StatusEffect();
		newEffect.m_szEffectName = name;
		newEffect.m_nEffectType = m_nEffectType;
		newEffect.m_nAmountOfTicks = m_nAmountOfTicks;
		newEffect.m_nHPMod = m_nHPMod;
		newEffect.m_nMPMod = m_nMPMod;
		newEffect.m_nPOWMod = m_nPOWMod;
		newEffect.m_nDEFMod = m_nDEFMod;
		newEffect.m_nSPDMod = m_nSPDMod;
		newEffect.m_nHITMod = m_nHITMod;
		newEffect.m_nEVAMod = m_nEVAMod;
		newEffect.m_lEffectedMembers = new List<string>();
		newEffect.m_lEffectedMembers.Add(m_goOwner.name);
		return newEffect;
	}
}
