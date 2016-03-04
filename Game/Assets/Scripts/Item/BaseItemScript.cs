using UnityEngine;
using System.Collections;




public class BaseItemScript : MonoBehaviour 
{
	public enum ITEM_TYPES {
							//useable items
							eSINGLE_HEAL, eGROUP_HEAL, eSINGLE_DAMAGE, eGROUP_DAMAGE,
							//armor items
							eHELMARMOR, eSHOULDERARMOR, eCHESTARMOR, eGLOVEARMOR, eBELTARMOR, eLEGARMOR,
							//trinkets
							eTRINKET,
							//non-useable items
							eJUNK, 
							//Key items
							eKEYITEM};
	protected int m_nItemType;
	public int GetItemType() {return m_nItemType;}
	public void SetItemType(int t) {m_nItemType = t;}
	string m_szItemName;
	public string GetItemName() {return m_szItemName;}
	public void SetItemName(string name) {m_szItemName = name;}
	int m_nModifier;
	public int GetItemModifier() {return m_nModifier;}
	public void SetItemModifier(int m) {m_nModifier = m;}
	int m_nBaseValue;
	public int GetItemBaseValue() {return m_nBaseValue;}
	public void SetItemBaseValue(int value) {m_nBaseValue = value;}
	string m_szTargets;
	public string GetTargets() {return m_szTargets;}
	public void SetTargets(string s) {m_szTargets = s;}
	string m_szDescription;
	public string GetDescription() {return m_szDescription;}
	public void SetDescription(string s) {m_szDescription = s;}
	int m_nPowMod;
	public int GetPowMod() {return m_nPowMod;}
	public void SetPowMod(int pow) {m_nPowMod = pow;}
	int m_nHPMod;
	public int GetHPMod() {return m_nHPMod;}
	public void SetHPMod(int hp) {m_nHPMod = hp;}
	int m_nDefMod;
	public int GetDefMod() {return m_nDefMod;}
	public void SetDefMod(int def) {m_nDefMod = def;}
	int m_nSpdMod;
	public int GetSpdMod() {return m_nSpdMod;}
	public void SetSpdMod(int spd) {m_nSpdMod = spd;}
	public bool m_bCanTargetDeadUnits = false;

	public delegate void m_delegate(GameObject pOwner);
	public m_delegate m_dFunc;
	protected GameObject m_pOwner;


	public GameObject m_goEffect;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Initialize()
	{

	}


}