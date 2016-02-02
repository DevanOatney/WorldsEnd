using UnityEngine;
using System.Collections;

public class ThunderScript : MonoBehaviour 
{
	GameObject m_pOwner;
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
				GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
				foreach(GameObject enemy in Enemies)
				{
					//Instantiate the animation at the target location
					Vector2 pos = enemy.transform.position;
					GameObject animation = Instantiate(Resources.Load<GameObject>("Spell Effects/WindOrb")) as GameObject;
					animation.transform.position = pos;
					Destroy(animation, 1.4f);
				}
				m_pOwner.GetComponent<Animator>().SetBool("m_bIsCasting", true);
				//in x amount of time, the player's turn is over and it's time to destroy this object
				Invoke("DoneAnimating", 1.5f);
				//turn off the flags for the item/inventory rendering
				m_pOwner.GetComponent<CAllyBattleScript>().m_bIsMyTurn = false;
				m_pOwner.GetComponent<CAllyBattleScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.STATUS_EFFECTS;
			}
			else if(Input.GetKeyUp(KeyCode.Escape))
			{
				m_bDraw = false;
				m_pOwner.GetComponent<CAllyBattleScript>().SetAllowInput(true);
				m_pOwner.GetComponent<CAllyBattleScript>().m_nState = (int)CAllyBattleScript.ALLY_STATES.USEITEM_CHOSEN;
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
		GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject enemy in Enemies)
		{
			//damage the unit
			enemy.GetComponent<UnitScript>().AdjustHP(m_pOwner.GetComponent<UnitScript>().GetSTR() /2);
		}
		
		
		//end units turn
		GameObject tw = GameObject.Find("TurnWatcher");
		if(tw)
			tw.GetComponent<TurnWatcherScript>().MyTurnIsOver(m_pOwner);
		m_pOwner.GetComponent<CAllyBattleScript>().SetAllowInput(true);
		Destroy(gameObject);
	}
	
	public void ThunderFunction(GameObject pOwner)
	{
		m_pOwner = pOwner;
		m_bDraw = true;
		m_pOwner.GetComponent<CAllyBattleScript>().SetAllowInput(false);
		GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject enemy in Enemies)
		{
			GameObject.Find("Enemy_Cursor" + enemy.GetComponent<UnitScript>().m_nPositionOnField).GetComponent<SpriteRenderer>().enabled = true;
		}
	}
	
	public void DisableAllCursors()
	{
		for(int i = 0; i < 3; ++i)
			GameObject.Find("Enemy_Cursor" + i).GetComponent<SpriteRenderer>().enabled = false;
		for(int i = 0; i < 3; ++i)
			GameObject.Find("Ally_Cursor" + i).GetComponent<SpriteRenderer>().enabled = false;
	}
}
