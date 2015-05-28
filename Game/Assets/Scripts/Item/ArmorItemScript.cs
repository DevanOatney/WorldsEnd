using UnityEngine;
using System.Collections;

public class ArmorItemScript : BaseItemScript 
{

	int m_nSpecialItemType;
	public int GetSpecialItemType() {return m_nSpecialItemType;}
	public void SetSpecialItemType(int type) {m_nSpecialItemType = type;}
	int m_nSpecialItemModfier;
	public int GetSpecialItemModifier() {return m_nSpecialItemModfier;}
	public void SetSpecialItemModifier(int mod) {m_nSpecialItemModfier = mod;}


	// Use this for initialization
	void Start () 
	{
		m_nItemType = (int)ITEM_TYPES.eCHESTARMOR;
		m_dFunc = HealWearer;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	new public void Initialize()
	{
		m_nItemType = (int)ITEM_TYPES.eCHESTARMOR;
		m_dFunc = HealWearer;
	}

	public void HealWearer(GameObject pOwner)
	{
		switch(m_nSpecialItemType)
		{
		case 1:
			m_pOwner = pOwner;
			//heals the wearer when he takes damage
			Invoke("Heal", 0.2f);
			break;
		}

	}

	void Heal()
	{
		m_pOwner.GetComponent<UnitScript>().AdjustHP(-1*m_nSpecialItemModfier);
		Destroy(gameObject);
	}
}
