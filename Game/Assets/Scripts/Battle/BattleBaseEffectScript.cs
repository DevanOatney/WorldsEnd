using UnityEngine;
using System.Collections;

public class BattleBaseEffectScript : MonoBehaviour 
{
	public GameObject m_goOwner;
	public delegate void m_delegate();
	public m_delegate m_dFunc;
	public bool m_bToBeRemoved = false;
	//used for if this effect has an amount of charges, set to -1 for infinite charges.. but.. you shouldn't.
	public int m_nAmountOfTicks = 0;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
