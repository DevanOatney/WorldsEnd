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

	public delegate void m_delegate(GameObject pOwner);
	public m_delegate m_dFunc;
	protected GameObject m_pOwner;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Initialize()
	{

	}

	public void InitializeTargetReticle(string szTargets)
	{
		if(m_pOwner == null)
			return;
		UnitScript us = m_pOwner.GetComponent<UnitScript>();
		GameObject[] _Targets = GameObject.FindGameObjectsWithTag(szTargets);
		GameObject zero = null, one = null, two = null;
		foreach(GameObject t in _Targets)
		{
			if(t.GetComponent<UnitScript>().GetCurHP() > 0)
				switch(t.GetComponent<UnitScript>().m_nPositionOnField)
			{
				case 0:
				{
					zero = t;
				}
				break;
				case 1:
				{
					one = t;
				}
				break;
				case 2:
				{
					two = t;
				}
				break;
			}
		}
		
		if(zero)
			us.m_nTargetPositionOnField = 0;
		else if(one)
			us.m_nTargetPositionOnField = 1;
		else if(two)
			us.m_nTargetPositionOnField = 2;
	}




	public void PositionTargetReticleUp(string szTargets)
	{
		if(m_pOwner == null)
			return;
		UnitScript us = m_pOwner.GetComponent<UnitScript>();
		#region Acquire Enemies
		//acquire the available enmies on field, set flags for which positions
		//on the field are able to be targetted
		GameObject[] _Targets = GameObject.FindGameObjectsWithTag(szTargets);
		bool zero = false, one = false, two = false;
		foreach(GameObject t in _Targets)
		{
			if(t.GetComponent<UnitScript>().GetCurHP() > 0)
				switch(t.GetComponent<UnitScript>().m_nPositionOnField)
			{
				case 0:
				{
					zero = true;
				}
				break;
				case 1:
				{
					one = true;
				}
				break;
				case 2:
				{
					two = true;
				}
				break;
			}
		}
		#endregion

		//Set the new targetting iter and make sure it stays in bounds
		switch(us.m_nTargetPositionOnField)
		{
		case 0:
			us.m_nTargetPositionOnField = 1;
			break;
		case 1:
			us.m_nTargetPositionOnField = 2;
			break;
		case 2:
			us.m_nTargetPositionOnField = 0;
			break;
		}


		#region AdjustTargetting
		//Adjust the targetting iter if it's at an unavailable target
		if(us.m_nTargetPositionOnField == 0)
		{
			//Was at bottom target, trying to go up one.
			if(zero == true)
			{
				//target is available.
			}
			else
			{
				//target unavailable, try moving target to the top position
				if(one == true)
				{
					//top target available
					us.m_nTargetPositionOnField = 1;
				}
				else
				{
					//no other targets available, remain at the bottom target.
					us.m_nTargetPositionOnField = 2;
				}
			}
		}
		else if(us.m_nTargetPositionOnField == 1)
		{
			//Was at middle, trying to go to top target
			if(one == true)
			{
				//Top target available
			}
			else
			{
				//Top target was unavailable, try moving target to the bottom position
				if(two == true)
				{
					//Bottom target is available
					us.m_nTargetPositionOnField = 2;
				}
				else
				{
					//No other target available, remain at the middle position target
					us.m_nTargetPositionOnField = 0;
				}
			}
		}
		else
		{
			//Was at the top, trying to wrap around to the bottom target
			if(two == true)
			{
				//bottom target available
			}
			else
			{
				//try to wrap to the middle target
				if(zero == true)
				{
					//middle target available
					us.m_nTargetPositionOnField = 0;
				}
				else
				{
					//no other target is available, remain at the top target.
					us.m_nTargetPositionOnField = 1;
				}
			}
		}
		#endregion
		
		#region Enable/Disable Cursors
		if(us.m_nTargetPositionOnField == 0)
		{
			//disable 1, 2
			GameObject.Find(szTargets + "_Cursor1").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find(szTargets + "_Cursor2").GetComponent<SpriteRenderer>().enabled = false;
			//enable 0
			GameObject.Find(szTargets + "_Cursor0").GetComponent<SpriteRenderer>().enabled = true;
		}
		else if(us.m_nTargetPositionOnField == 1)
		{
			//disable 0, 2
			GameObject.Find(szTargets + "_Cursor0").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find(szTargets + "_Cursor2").GetComponent<SpriteRenderer>().enabled = false;
			//enable 1
			GameObject.Find(szTargets + "_Cursor1").GetComponent<SpriteRenderer>().enabled = true;
		}
		else if(us.m_nTargetPositionOnField == 2)
		{
			Debug.Log("Two");
			//disable 1, 0
			GameObject.Find(szTargets + "_Cursor1").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find(szTargets + "_Cursor0").GetComponent<SpriteRenderer>().enabled = false;
			//enable 0
			GameObject.Find(szTargets + "_Cursor2").GetComponent<SpriteRenderer>().enabled = true;
		}
		#endregion
		
	}
	
	public void PositionTargetReticleDown(string szTargets)
	{
		if(m_pOwner == null)
			return;
		UnitScript us = m_pOwner.GetComponent<UnitScript>();
		#region Acquire Enemies
		//acquire the available enmies on field, set flags for which positions
		//on the field are able to be targetted
		GameObject[] _Targets = GameObject.FindGameObjectsWithTag(szTargets);
		bool zero = false, one = false, two = false;
		foreach(GameObject t in _Targets)
		{
			if(t.GetComponent<UnitScript>().GetCurHP() > 0)
				switch(t.GetComponent<UnitScript>().m_nPositionOnField)
			{
				case 0:
				{
					zero = true;
				}
				break;
				case 1:
				{
					one = true;
				}
				break;
				case 2:
				{
					two = true;
				}
				break;
			}
		}
		#endregion

		switch(us.m_nTargetPositionOnField)
		{
		case 0:
			us.m_nTargetPositionOnField = 2;
			break;
		case 1:
			us.m_nTargetPositionOnField = 0;
			break;
		case 2:
			us.m_nTargetPositionOnField = 1;
			break;
		}

		#region Adjust Targetting
		if(us.m_nTargetPositionOnField == 0)
		{
			//Player is moving target from top to middle
			if(zero == true)
			{
				//middle target is available
			}
			else
			{
				//target unavailable, try targetting bottom target
				if(two == true)
				{
					//Bottom target available
					us.m_nTargetPositionOnField = 2;
				}
				else
				{
					//No other target available, retarget the top target
					us.m_nTargetPositionOnField = 1;
				}
			}
		}
		else if(us.m_nTargetPositionOnField == 1)
		{
			//Player is moving target from bottom to top
			if(one == true)
			{
				//target available
			}
			else
			{
				//target unavailable, try to wrap to middle target instead
				if(zero == true)
				{
					//Middle target available
					us.m_nTargetPositionOnField = 0;
				}
				else
				{
					//no other target available, retarget the bottom target
					us.m_nTargetPositionOnField = 2;
				}
			}
		}
		else
		{
			//Player is moving from middle to bottom target
			if(two == true)
			{
				//Initial target available
			}
			else
			{
				//Try to wrap to the top target
				if(one == true)
				{
					//Top target is available
					us.m_nTargetPositionOnField = 1;
				}
				else
				{
					//no other target available, retarget the middle target
					us.m_nTargetPositionOnField = 0;
				}
			}
		}
		
		#endregion
		#region Enable/Disable Cursors
		if(us.m_nTargetPositionOnField == 0)
		{
			//disable 1, 2
			GameObject.Find(szTargets + "_Cursor1").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find(szTargets + "_Cursor2").GetComponent<SpriteRenderer>().enabled = false;
			//enable 0
			GameObject.Find(szTargets + "_Cursor0").GetComponent<SpriteRenderer>().enabled = true;
		}
		else if(us.m_nTargetPositionOnField == 1)
		{
			//disable 0, 2
			GameObject.Find(szTargets + "_Cursor0").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find(szTargets + "_Cursor2").GetComponent<SpriteRenderer>().enabled = false;
			//enable 1
			GameObject.Find(szTargets + "_Cursor1").GetComponent<SpriteRenderer>().enabled = true;
		}
		else if(us.m_nTargetPositionOnField == 2)
		{
			//disable 1, 0
			GameObject.Find(szTargets + "_Cursor1").GetComponent<SpriteRenderer>().enabled = false;
			GameObject.Find(szTargets + "_Cursor0").GetComponent<SpriteRenderer>().enabled = false;
			//enable 0
			GameObject.Find(szTargets + "_Cursor2").GetComponent<SpriteRenderer>().enabled = true;
		}
		#endregion
	}
	
	public void DisableAllCursors()
	{
		for(int i = 0; i < 3; ++i)
			GameObject.Find("Enemy_Cursor" + i).GetComponent<SpriteRenderer>().enabled = false;
		for(int i = 0; i < 3; ++i)
			GameObject.Find("Ally_Cursor" + i).GetComponent<SpriteRenderer>().enabled = false;
	}


}