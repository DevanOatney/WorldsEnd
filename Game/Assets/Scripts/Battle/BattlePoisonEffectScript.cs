using UnityEngine;
using System.Collections;

public class BattlePoisonEffectScript : BattleBaseEffectScript 
{
	//pointer to the object that turns green infront of the character
	public GameObject m_goPoisonClone;
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bHasBeenRemoved == true)
			Destroy(gameObject);
	}
	
	public override void Initialize(GameObject owner, int nEffectType, int nTickCount, int nHP, int nMP, int nPOW, int nDEF, int nSPD, int nHIT, int nEVA)
	{
		m_goOwner = owner;
		m_nEffectType = nEffectType;
		m_nHPMod = nHP;
		m_nMPMod = nMP;
		m_nPOWMod = nPOW;
		m_nDEFMod = nDEF;
		m_nSPDMod = nSPD;
		m_nHITMod = nHIT;
		m_nEVAMod = nEVA;
		m_nAmountOfTicks = nTickCount;
		m_nEffectType = (int)EFFECT_TYPES.ePOISON;
		m_dFunc = Step;
	}
	
	void Step()
	{
		//instantiate green image of player infront of the player on the field.
		m_goPoisonClone.GetComponent<SpriteRenderer>().sprite = m_goOwner.GetComponent<SpriteRenderer>().sprite;
		Vector3 cloneTransform = m_goOwner.transform.localScale;
		m_goPoisonClone.transform.localScale = cloneTransform;
		Vector3 pos = m_goOwner.transform.position;
		//adjust so the clone is behind the unit
		pos.z -= 0.1f;
		
		//create the green effect to show the player they're taking dmg
		GameObject poisonClone = Instantiate(m_goPoisonClone, pos, Quaternion.identity) as GameObject;
		poisonClone.GetComponent<PoisonCloneScript>().m_gOwner = m_goOwner;
		if(poisonClone)
			Destroy(poisonClone, 1.0f);
		
		
		//adjust the amount of times this effect can happen, as long as it's not infinite.
		if(m_nAmountOfTicks > -1)
		{
			m_nAmountOfTicks--;
			if(m_nAmountOfTicks <= 0)
			{
				m_bToBeRemoved = true;
			}
			
		}
		m_goOwner.GetComponent<UnitScript>().AdjustHP(m_nHPMod);
	}
}
