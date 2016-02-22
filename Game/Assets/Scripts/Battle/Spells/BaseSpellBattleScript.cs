using UnityEngine;
using System.Collections;

public class BaseSpellBattleScript : MonoBehaviour 
{
	public enum ELEMENTS{eWATER, eEARTH, eWIND, eFIRE, eLIGHTNING}
	public string m_szSpellName;
	public int m_nElementType;
	public int m_nMPCost;
	public int m_nHPMod;
	public int m_nMPMod;
	public int m_nPOWMod;
	public int m_nDEFMod;
	public int m_nSPDMod;
	public int m_nHITMod;

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

	public void DoneWithRuneEffect()
	{
	}
}
