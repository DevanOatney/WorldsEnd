using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCScript : MonoBehaviour 
{
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

	//This is toggled when there's a collision/trigger hit.  
	bool m_bCanMove = true;
	//cost for if it's an innkeeper
	public int m_nCost = 0;

	public bool m_bReturnToPlayer = false;

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
		if(m_bReturnToPlayer == true)
		{
			Vector2 playerPos = GameObject.Find("Player").transform.position;
			Vector2 npcPos = transform.position;
			Vector2 toPlayer = playerPos - npcPos;
			if(toPlayer.x > 0.1f || toPlayer.x < -0.1f)
			{
				toPlayer.y = 0.0f;
				toPlayer.Normalize();
				toPlayer.x *= m_fWalkingSpeed;
				gameObject.GetComponent<Rigidbody2D>().velocity = toPlayer;
			}
			else if(toPlayer.y > 0.1f || toPlayer.y  < -0.1f)
			{
				toPlayer.x = 0.0f;
				toPlayer.Normalize();
				toPlayer.y *= m_fWalkingSpeed;
				gameObject.GetComponent<Rigidbody2D>().velocity = toPlayer;
			}
			else
			{
				m_bReturnToPlayer = false;
				GetComponent<BoxCollider2D>().enabled = true;
				gameObject.SetActive(false);
				GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
		}
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
