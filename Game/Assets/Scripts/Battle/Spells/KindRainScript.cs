using UnityEngine;
using System.Collections;

public class KindRainScript : BaseSpellBattleScript 
{

	void Start()
	{

	}
	
	void Update()
	{
		if(m_bShouldActivate == true)
		{
			GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
			foreach(GameObject ally in allies)
			{
				//Instantiate the animation at the target location
				Vector2 pos = ally.transform.position;
				GameObject animation = Instantiate(Resources.Load<GameObject>("Spell Effects/WaterHeal")) as GameObject;
				animation.transform.position = pos;
				Destroy(animation, 1.2f);
			}
			m_goOwner.GetComponent<Animator>().SetBool("m_bIsCasting", true);
			//in x amount of time, the player's turn is over and it's time to destroy this object
			Invoke("DoneAnimating", 1.5f);

			//turn off the flags for the item/inventory rendering
			m_goOwner.GetComponent<CAllyBattleScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.STATUS_EFFECTS;
		}
	}
	
	void DoneAnimating()
	{
		//end the animation
		m_goOwner.GetComponent<Animator>().SetBool("m_bIsCasting", false);
		//Do the effect
		GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
		foreach(GameObject ally in allies)
		{
			//heal the unit (adjust hp is for taking damage.. so sending a negative number should heal
			ally.GetComponent<UnitScript>().AdjustHP(m_goOwner.GetComponent<UnitScript>().GetSTR() * -1);
		}
		Destroy(gameObject);
	}
	
	public void Initialize()
	{

	}
	
	public void KindRainFunction(GameObject pOwner)
	{
		m_goOwner = pOwner;
	}
	new public void DoneWithRuneEffect()
	{
		
	}
}
