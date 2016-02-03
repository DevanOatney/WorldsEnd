using UnityEngine;
using System.Collections;

public class KindRainScript : MonoBehaviour 
{
	public GameObject m_pOwner;
	bool m_bDraw = false;

	void Start()
	{

	}
	
	void Update()
	{
		if(m_bDraw == true)
		{
			
			if(Input.GetKeyUp(KeyCode.Return))
			{
				//turn off flag to even reach this
				m_bDraw = false;
				//Disable the rendering of the targetting cursors 
				DisableAllCursors();
				//start the animation
				//Do the effect
				GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
				foreach(GameObject ally in allies)
				{
					//Instantiate the animation at the target location
					Vector2 pos = ally.transform.position;
					GameObject animation = Instantiate(Resources.Load<GameObject>("Spell Effects/WaterHeal")) as GameObject;
					animation.transform.position = pos;
					Destroy(animation, 1.2f);
				}
				m_pOwner.GetComponent<Animator>().SetBool("m_bIsCasting", true);
				//in x amount of time, the player's turn is over and it's time to destroy this object
				Invoke("DoneAnimating", 1.5f);

				//turn off the flags for the item/inventory rendering
				m_pOwner.GetComponent<CAllyBattleScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.STATUS_EFFECTS;
			}
			else if(Input.GetKeyUp(KeyCode.Escape))
			{
				m_bDraw = false;
				m_pOwner.GetComponent<CAllyBattleScript>().SetAllowInput(true);
				//TODO: inform unit to go back to magic selection screen?
				m_pOwner.GetComponent<CAllyBattleScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.USEMAGIC_CHOSEN;
				DisableAllCursors();
				Destroy(gameObject);
			}
		}
	}
	
	void DoneAnimating()
	{
		//end the animation
		m_pOwner.GetComponent<Animator>().SetBool("m_bIsCasting", false);
		//Do the effect
		GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
		foreach(GameObject ally in allies)
		{
			//heal the unit (adjust hp is for taking damage.. so sending a negative number should heal
			ally.GetComponent<UnitScript>().AdjustHP(m_pOwner.GetComponent<UnitScript>().GetSTR() * -1);
		}

		Destroy(gameObject);
	}
	
	public void Initialize()
	{

	}
	
	public void KindRainFunction(GameObject pOwner)
	{
		m_pOwner = pOwner;
		m_bDraw = true;
		
		GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");
		foreach(GameObject ally in Allies)
		{
			GameObject.Find("Ally_Cursor" + ally.GetComponent<UnitScript>().FieldPosition).GetComponent<SpriteRenderer>().enabled = true;
		}
	}

	public void DisableAllCursors()
	{
		for(int i = 0; i < 5; ++i)
			GameObject.Find("Enemy_Cursor" + i).GetComponent<SpriteRenderer>().enabled = false;
		for(int i = 0; i < 5; ++i)
			GameObject.Find("Ally_Cursor" + i).GetComponent<SpriteRenderer>().enabled = false;
	}
}
