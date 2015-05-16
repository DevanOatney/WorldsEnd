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
				if(m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm != null)
				{
					m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm.GetComponentInChildren<Animator>().SetBool("m_bIsCasting", true);
					m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm.GetComponent<SpriteRenderer>().enabled = true;
				}
				//in x amount of time, the player's turn is over and it's time to destroy this object
				Invoke("DoneAnimating", 1.5f);

				//turn off the flags for the item/inventory rendering
				m_pOwner.GetComponent<PlayerBattleScript>().m_bIsMyTurn = false;
				m_pOwner.GetComponent<PlayerBattleScript>().SetMagicChosen(false);
				m_pOwner.GetComponent<PlayerBattleScript>().SetChoosingMagicFlag(false);
			}
			else if(Input.GetKeyUp(KeyCode.Escape))
			{
				m_bDraw = false;
				m_pOwner.GetComponent<PlayerBattleScript>().SetAllowInput(true);
				m_pOwner.GetComponent<PlayerBattleScript>().TurnOffFlags();
				DisableAllCursors();
				Destroy(gameObject);
			}
		}
	}
	
	void DoneAnimating()
	{
		//end the animation
		m_pOwner.GetComponent<Animator>().SetBool("m_bIsCasting", false);
		if(m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm != null)
		{
			m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm.GetComponentInChildren<Animator>().SetBool("m_bIsCasting", false);
			m_pOwner.GetComponent<PlayerBattleScript>().m_goUnitArm.GetComponent<SpriteRenderer>().enabled = false;
		}
		//Do the effect
		GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
		foreach(GameObject ally in allies)
		{
			//heal the unit (adjust hp is for taking damage.. so sending a negative number should heal
			ally.GetComponent<UnitScript>().AdjustHP(m_pOwner.GetComponent<UnitScript>().GetSTR() * -1);
			
		}
		
		
		//end units turn
		GameObject tw = GameObject.Find("TurnWatcher");
		if(tw)
			tw.GetComponent<TurnWatcherScript>().MyTurnIsOver(m_pOwner);
		m_pOwner.GetComponent<PlayerBattleScript>().SetAllowInput(true);
		Destroy(gameObject);
	}
	
	public void Initialize()
	{

	}
	
	public void KindRainFunction(GameObject pOwner)
	{
		m_pOwner = pOwner;
		m_bDraw = true;
		m_pOwner.GetComponent<PlayerBattleScript>().SetAllowInput(false);
		
		GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");
		foreach(GameObject ally in Allies)
		{
			GameObject.Find("Ally_Cursor" + ally.GetComponent<UnitScript>().m_nPositionOnField).GetComponent<SpriteRenderer>().enabled = true;
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
