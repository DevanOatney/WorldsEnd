using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCScript : MonoBehaviour 
{
	#region DESIGNER_HELPER_FUNCTIONS
	public void DHF_NPCMoveIntoPlayer()
	{
		m_bReturnToPlayer = true;
	}
	public void DHF_NPCMoveToGameobject(GameObject _target, bool _shouldIRun)
	{
		m_bMoveTowardLocation = true;
		m_vTargetLocation = _target.transform.position;
		m_vTargetLocation.y += _target.GetComponent<BoxCollider2D>().size.y * 0.5f;
	}
	public void DHF_NPCMoveToGameobject(GameObject _target, bool _shouldIRun, int _nextFacingDir)
	{
		m_bMoveTowardLocation = true;
		m_vTargetLocation = _target.transform.position;
		m_vTargetLocation.y += _target.GetComponent<BoxCollider2D>().size.y * 0.5f;
		m_nNextFacingDir = _nextFacingDir;
	}
	#endregion



	public enum FACINGDIR {eDOWN, eLEFT, eRIGHT, eUP}
	public int m_nFacingDir = 0;
	//Tool for if the character has scripted pathing
	public TextAsset m_taPathing;
	//If this NPC is active and should move/be interracted with
	public bool m_bActive = false;
	//Path directory to dialogue for this character in this scene
	public string m_szDialoguePath = "";

	public float m_fWalkingSpeed = 2.0f;
	//Should the characters moving logic be active? (Must have m_bActive set to true for this to effect the NPC)
	public bool m_bIsMoving = false;
	//Is the NPC being interracted with?  Will stop moving if it is.
	public bool m_bIsBeingInterractedWith = false;
	//Turn this on to ignore collision with the player
	public bool m_bIsComingOutOfPlayer = false;
	protected float m_fTimer = 0.0f;
	protected int m_nStepsIter = 0;
	public Animator m_aAnim;
	//For helper functions of moving, if -1 it doesn't do anything, else it faces the player this way after moving to the location
	int m_nNextFacingDir = -1;
	//This is toggled when there's a collision/trigger hit.  
	bool m_bCanMove = true;
	//cost for if it's an innkeeper
	public int m_nCost = 0;

	bool m_bReturnToPlayer = false;
	bool m_bMoveTowardLocation = false;
	Vector3 m_vTargetLocation = Vector3.zero;



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
		#region Return To Player
		if(m_bReturnToPlayer == true)
		{
			GetComponent<BoxCollider2D>().enabled = false;
			Vector2 playerPos = GameObject.Find("Player").transform.position;
			Vector2 npcPos = transform.position;
			Vector2 toPlayer = playerPos - npcPos;
			if(toPlayer.x > 0.1f || toPlayer.x < -0.1f)
			{
				ResetAnimFlagsExcept(-1);
				if(toPlayer.x > 0.1f)
					m_aAnim.SetBool("m_bMoveRight", true);
				else
					m_aAnim.SetBool("m_bMoveLeft", true);
				toPlayer.y = 0.0f;
				toPlayer.Normalize();
				toPlayer.x *= m_fWalkingSpeed;
				gameObject.GetComponent<Rigidbody2D>().velocity = toPlayer;
			}
			else if(toPlayer.y > 0.1f || toPlayer.y  < -0.1f)
			{
				ResetAnimFlagsExcept(-1);
				if(toPlayer.y > 0.1f)
					m_aAnim.SetBool("m_bMoveUp", true);
				else
					m_aAnim.SetBool("m_bMoveDown", true);
				toPlayer.x = 0.0f;
				toPlayer.Normalize();
				toPlayer.y *= m_fWalkingSpeed;
				gameObject.GetComponent<Rigidbody2D>().velocity = toPlayer;
			}
			else
			{
				m_bReturnToPlayer = false;
				GetComponent<BoxCollider2D>().enabled = true;
				gameObject.GetComponent<SpriteRenderer>().enabled = false;
				gameObject.GetComponent<BoxCollider2D>().enabled = false;
				GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
				Camera.main.GetComponent<CameraFollowTarget>().m_goNextTarget = GameObject.Find("Player");
			}
		}
		#endregion
		else if(m_bMoveTowardLocation == true)
		{
			Vector3 npcPos = transform.position;
			Vector3 toTarget = m_vTargetLocation - npcPos;
			if(toTarget.x > 0.1f || toTarget.x < -0.1f)
			{
				ResetAnimFlagsExcept(-1);
				if(toTarget.x > 0.1f)
					m_aAnim.SetBool("m_bMoveRight", true);
				else
					m_aAnim.SetBool("m_bMoveLeft", true);
				toTarget.y = 0.0f;
				toTarget.Normalize();
				toTarget.x *= m_fWalkingSpeed;
				gameObject.GetComponent<Rigidbody2D>().velocity = toTarget;
			}
			else if(toTarget.y > 0.1f || toTarget.y  < -0.1f)
			{
				ResetAnimFlagsExcept(-1);
				if(toTarget.y > 0.1f)
					m_aAnim.SetBool("m_bMoveUp", true);
				else
					m_aAnim.SetBool("m_bMoveDown", true);
				toTarget.x = 0.0f;
				toTarget.Normalize();
				toTarget.y *= m_fWalkingSpeed;
				gameObject.GetComponent<Rigidbody2D>().velocity = toTarget;
			}
			else
			{
				ResetAnimFlagsExcept(-1);
				if(m_nNextFacingDir != -1)
				{
					m_aAnim.SetInteger("m_nFacingDir", m_nNextFacingDir);
					m_nNextFacingDir = -1;
				}
				m_bMoveTowardLocation = false;
				m_vTargetLocation = Vector3.zero;
			}
		}

		#region Scripted Pathing
		else if(m_bActive == true)
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
							//GetComponent<Rigidbody2D>().isKinematic = false;
							m_bCanMove = true;
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
			#endregion



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
					if(m_bCanMove == true)
					{
						GetComponent<Rigidbody2D>().velocity = moveDir * m_fWalkingSpeed;
					}
				}
			}
			else
			{
				//Player is currently interracting with this NPC.
				
				
				
				//Face the NPC toward the direction of the player.
				Vector2 DirOfPlayer = GameObject.Find("Player").transform.position - transform.position;
				if(DirOfPlayer.x > 0.1f || DirOfPlayer.x < -0.1f)
				{
					ResetAnimFlagsExcept(-1);
					if(DirOfPlayer.x > 0.1f)
						m_aAnim.SetInteger("m_nFacingDir", 2);
					else
						m_aAnim.SetInteger("m_nFacingDir", 1);
				}
				else if(DirOfPlayer.y > 0.1f || DirOfPlayer.y  < -0.1f)
				{
					ResetAnimFlagsExcept(-1);
					if(DirOfPlayer.y > 0.1f)
						m_aAnim.SetInteger("m_nFacingDir", 3);
					else
						m_aAnim.SetInteger("m_nFacingDir", 0);
				}
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
	protected void LoadSteps()
	{
		if(m_taPathing != null)
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
	}

	public void ResetAnimFlagsExcept(int exception)
	{
		m_aAnim.SetInteger("m_nFacingDir", exception);
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

	public void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == "Action Box(Clone)")
		{
			m_bIsBeingInterractedWith = true;
			if(GetComponent<MessageHandler>())
			{
				if(m_szDialoguePath != "")
					GameObject.Find("Event system").GetComponent<BaseEventSystemScript>().HandleEvent(m_szDialoguePath);
			}
		}
	}

	void OnCollisionEnter2D(Collision2D c)
	{
		if(m_bIsComingOutOfPlayer == false)
		{
			m_bCanMove = false;
			if(GetComponent<Rigidbody2D>())
				GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		}
	}

	void OnCollisionExit2D(Collision2D c)
	{
		//GetComponent<Rigidbody2D>().isKinematic = false;
		m_bCanMove = true;
	}

	public void RestartPathing()
	{
		m_bIsBeingInterractedWith = false;
		//ResetAnimFlagsExcept(m_lSteps[m_nStepsIter].nDirection);
	}
}
