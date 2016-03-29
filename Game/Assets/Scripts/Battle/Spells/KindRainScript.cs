using UnityEngine;
using System.Collections;

public class KindRainScript : BaseSpellBattleScript 
{
	public GameObject m_goHealEffect;
	void Start()
	{

	}
	
	void Update()
	{
		if(m_bShouldActivate == true)
		{
			m_bShouldActivate = false;
			GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
			foreach(GameObject ally in allies)
			{
				if(ally.GetComponent<UnitScript>().FieldPosition == m_goOwner.GetComponent<UnitScript>().m_nTargetPositionOnField)
				{
					//Instantiate the animation at the target location
					Vector2 pos = ally.transform.position;
					//GameObject animation = Instantiate(Resources.Load<GameObject>("Spell Effects/WaterHeal")) as GameObject;
					GameObject newHealEffect = Instantiate(m_goHealEffect);
					newHealEffect.transform.position = pos;
					Destroy(newHealEffect, 1.2f);
					break;
				}
			}
			m_goOwner.GetComponent<UnitScript>().m_aAnim.SetBool("m_bIsCasting", true);
			//in x amount of time, the player's turn is over and it's time to destroy this object
			Invoke("DoneAnimating", 1.5f);


		}
	}
	
	public override void DoneAnimating()
	{
		//Do the effect
		GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
		foreach(GameObject ally in allies)
		{
			//heal the unit (adjust hp is for taking damage.. so sending a negative number should heal
			if(ally.GetComponent<UnitScript>().FieldPosition == m_goOwner.GetComponent<UnitScript>().m_nTargetPositionOnField)
			{
				ally.GetComponent<UnitScript>().AdjustHP(m_goOwner.GetComponent<UnitScript>().GetTempSTR() * -1);
				break;
			}
		}
		base.DoneAnimating();
	}
	
	public override void Initialize(GameObject _pOwner)
	{
		m_goOwner = _pOwner;
	}

	new public void DoneWithRuneEffect()
	{
		
	}
}
