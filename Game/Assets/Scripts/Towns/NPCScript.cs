using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCScript : MonoBehaviour 
{
	protected enum FACINGDIR {eDOWN, eLEFT, eRIGHT, eUP}
	public int m_nFacingDir = 0;
	public TextAsset m_taPathing;
	public bool m_bActive = false;
	public string m_szDialoguePath = "";

	protected float m_fWalkingSpeed = 2.0f;
	protected bool m_bIsMoving = false;
	protected bool m_bIsBeingInterractedWith = false;
	protected float m_fTimer = 0.0f;
	protected int m_nStepsIter = 0;
	protected Animator m_aAnim;

	protected class cSteps
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

	protected List<cSteps> m_lSteps = new List<cSteps>();

	// Use this for initialization
	void Start ()
	{
		m_aAnim = GetComponent<Animator>();
		LoadSteps();
	}

	protected void HandleMovement()
	{
		if(m_bActive == true)
		{
			if(m_bIsBeingInterractedWith == false)
			{
				//Handle step logic
				if(m_lSteps.Count > 0)
				{
					m_fTimer += Time.deltaTime;
					if(m_fTimer >= m_lSteps[m_nStepsIter].fTime)
					{
						m_fTimer = 0.0f;
						if(m_lSteps[m_nStepsIter].bOneShot == true)
						{
							m_lSteps.RemoveAt(m_nStepsIter);
							if(m_nStepsIter >= m_lSteps.Count)
								m_nStepsIter = 0;
						}
						else
						{
							m_nStepsIter++;
							if(m_nStepsIter >= m_lSteps.Count)
								m_nStepsIter = 0;
						}
						
						if(m_lSteps.Count > 0)
						{
							//We're now doing a new step
							m_nFacingDir = m_lSteps[m_nStepsIter].nDirection;
							if(m_lSteps[m_nStepsIter].bMove == true)
							{
								m_bIsMoving = true;
								m_aAnim.SetBool("m_bIsMoving", true);
								Vector2 dir = GetNPCFacing();
								m_aAnim.SetFloat("m_fXDir", dir.x);
								m_aAnim.SetFloat("m_fYDir", dir.y);
								//ResetAnimFlagsExcept(m_nFacingDir);
							}
							else
							{
								m_bIsMoving = false;
								m_aAnim.SetBool("m_bIsMoving", false);
								Vector2 dir = GetNPCFacing();
								m_aAnim.SetFloat("m_fXDir", dir.x);
								m_aAnim.SetFloat("m_fYDir", dir.y);
								//ResetAnimFlagsExcept(-1);
							}
							
						}
						
					}
				}
				
				//Handle movement logic
				if(m_bIsMoving == true)
				{
					Vector3 moveDir = new Vector2();
					switch(m_nFacingDir)
					{
					case (int)FACINGDIR.eDOWN:
						moveDir.y = -1;
						break;
					case (int)FACINGDIR.eLEFT:
						moveDir.x = -1;
						break;
					case (int)FACINGDIR.eRIGHT:
						moveDir.x = 1;
						break;
					case (int)FACINGDIR.eUP:
						moveDir.y = 1;
						break;
					}
					GetComponent<Rigidbody>().velocity = moveDir * m_fWalkingSpeed;
				}
			}
			else
			{
				//Player is currently interracting with this NPC.
				
				
				
				//Face the NPC toward the direction of the player.
				Vector2 DirOfPlayer = GameObject.Find("Player").transform.position - transform.position;
				m_aAnim.SetFloat("m_fXDir", DirOfPlayer.x);
				m_aAnim.SetFloat("m_fYDir", DirOfPlayer.y);
			}
		}
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

	public void ResetAnimFlagsExcept(int exception)
	{
		switch(exception)
		{
		case 0:
		{
			//Down
			m_aAnim.SetBool("m_bMoveLeft", false);
			m_aAnim.SetBool("m_bMoveRight", false);
			m_aAnim.SetBool("m_bMoveUp", false);
			m_aAnim.SetBool("m_bMoveDown", true);
		}
			break;
		case 1:
		{
			//Left
			m_aAnim.SetBool("m_bMoveRight", false);
			m_aAnim.SetBool("m_bMoveUp", false);
			m_aAnim.SetBool("m_bMoveDown", false);
			m_aAnim.SetBool("m_bMoveLeft", true);
		}
			break;
		case 2:
		{
			//Right
			m_aAnim.SetBool("m_bMoveDown", false);
			m_aAnim.SetBool("m_bMoveLeft", false);
			m_aAnim.SetBool("m_bMoveRight", true);
			m_aAnim.SetBool("m_bMoveUp", false);
		}
			break;
		case 3:
		{
			m_aAnim.SetBool("m_bMoveUp", true);
			m_aAnim.SetBool("m_bMoveDown", false);
			m_aAnim.SetBool("m_bMoveLeft", false);
			m_aAnim.SetBool("m_bMoveRight", false);
		}
			break;
		default:
		{
			m_aAnim.SetBool("m_bMoveUp", false);
			m_aAnim.SetBool("m_bMoveDown", false);
			m_aAnim.SetBool("m_bMoveLeft", false);
			m_aAnim.SetBool("m_bMoveRight", false);
		}
			break;
		}
	}

	public void OnTriggerEnter(Collider c)
	{
		if(c.name == "Action Box(Clone)")
		{
			m_bIsBeingInterractedWith = true;
			if(GetComponent<MessageHandler>())
			{
				if(m_szDialoguePath != "")
					GameObject.Find("Event System").GetComponent<BaseEventSystemScript>().HandleEvent(m_szDialoguePath);
			}
		}
	}

	public void RestartPathing()
	{
		m_bIsBeingInterractedWith = false;
		//ResetAnimFlagsExcept(m_lSteps[m_nStepsIter].nDirection);
	}
}
