using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC_ArmorMerchantScript : NPCScript
{
	enum FACINGDIR {eDOWN, eLEFT, eRIGHT, eUP}
	public int m_nFacingDir = 0;
	public TextAsset m_taPathing;
	public bool m_bActive = false;
	public string m_szDialoguePath = "";
	
	float m_fWalkingSpeed = 2.0f;
	bool m_bIsMoving = false;
	bool m_bIsBeingInterractedWith = false;
	float m_fTimer = 0.0f;
	int m_nStepsIter = 0;
	Animator m_aAnim;
	
	class cSteps
	{
		//direction to face
		public int nDirection;
		//time to do this step
		public float fTime;
		//should the npc be moving in this direction
		public bool bMove;
		//after doing this step, should it be deleted
		public bool bOneShot;
	}
	
	List<cSteps> m_lSteps = new List<cSteps>();
	
	// Use this for initialization
	void Start ()
	{
		m_aAnim = GetComponent<Animator>();
		LoadSteps();
	}
	
	// Update is called once per frame
	void Update () 
	{
		HandleMovement();
	}
	
	//Get the direction the NPC is looking
	Vector2 GetNPCFacing()
	{
		//Down, Left, Right, Up
		switch(m_nFacingDir)
		{
		case 0:
			return new Vector2(0, -1);
		case 1:
			return new Vector2(-1, 0);
		case 2:
			return new Vector2(1, 0);
		case 3:
			return new Vector2(0, 1);
		}
		return new Vector2(0, 0);
	}
	
	//Load the steps of the NPC
	void LoadSteps()
	{
		string[] lines = m_taPathing.text.Split('\n');
		foreach(string step in lines)
		{
			cSteps newStep = new cSteps();
			string[] pieces = step.Split(',');
			newStep.nDirection = int.Parse(pieces[0].Trim());
			newStep.fTime = float.Parse(pieces[1].Trim());
			newStep.bMove = bool.Parse(pieces[2].Trim());
			newStep.bOneShot = bool.Parse(pieces[3].Trim());
			m_lSteps.Add(newStep);
		}
	}

	
	public void OnTriggerEnter(Collider c)
	{
		if(c.name == "Action Box(Clone)")
		{
			if(GetComponent<MessageHandler>())
			{
				m_bIsMoving = false;
				m_bIsBeingInterractedWith = true;
				if(m_szDialoguePath != "")
					GameObject.Find("Event System").GetComponent<BaseEventSystemScript>().HandleEvent(m_szDialoguePath);
			}
		}
	}
	
	public void RestartPathing()
	{
		m_bIsMoving = true;
		m_bIsBeingInterractedWith = false;
		//ResetAnimFlagsExcept(m_lSteps[m_nStepsIter].nDirection);
	}
}
