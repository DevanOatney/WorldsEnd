﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldBaseStatusEffectScript : MonoBehaviour 
{
	public GameObject m_goOwner;
	public delegate void m_delegate();
	public m_delegate m_dFunc;

	public bool m_bToBeRemoved = false;
	//mod for use (i.e., damage done per tick, chance for paralyze to happen each round 0-100, etc)
	public int m_nMod = 0;
	//list of all units in the party that are effected
	public List<DCScript.StatusEffect.cEffectedMember> m_lEffectedUnits;
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

}
