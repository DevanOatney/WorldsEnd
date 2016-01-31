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
        m_bShouldRun = _shouldIRun;
		m_vTargetLocation = _target.transform.position;
		m_vTargetLocation.y += _target.GetComponent<BoxCollider2D>().size.y * 0.5f;
	}
	public void DHF_NPCMoveToGameobject(GameObject _target, bool _shouldIRun, int _nextFacingDir)
	{
		m_bMoveTowardLocation = true;
        m_bShouldRun = _shouldIRun;
		m_vTargetLocation = _target.transform.position;
		m_vTargetLocation.y += _target.GetComponent<BoxCollider2D>().size.y * 0.5f;
		m_nNextFacingDir = _nextFacingDir;
	}
	public void DHF_NPCPathfindToGameobject(GameObject _target, bool _shouldIRun, int _nextFacingDir, bool _allowDiagonal)
	{
        m_bShouldRun = _shouldIRun;
		m_nNextFacingDir = _nextFacingDir;
        CPathRequestManager.RequestPath(transform.position, _target.transform.position, FinishedPathRequest);
	}
	#endregion

    void FinishedPathRequest(Vector3[] p_Path, bool p_bSuccess)
    {
        if (p_bSuccess == true)
        {
            //found a path
            m_vWaypoints = p_Path;
            m_bMoveTowardLocation = true;
            m_nWaypointIndex = 0;
        }
        else
        {
            //couldn't find path
        }
    }

	public enum FACINGDIR {eDOWN, eLEFT, eRIGHT, eUP}
	public int m_nFacingDir = 0;
	//If this NPC is active and should move/be interracted with
	public bool m_bActive = false;
	//Path directory to dialogue for this character in this scene
	public string m_szDialoguePath = "";

	public float m_fWalkingSpeed = 2.0f;
    bool m_bShouldRun = false;

	//Should the characters moving logic be active? (Must have m_bActive set to true for this to effect the NPC)
	public bool m_bIsMoving = false;
	//Is the NPC being interracted with?  Will stop moving if it is.
	public bool m_bIsBeingInterractedWith = false;
	//Turn this on to ignore collision with the player
	public bool m_bIsComingOutOfPlayer = false;
	public Animator m_aAnim;
	//For helper functions of moving, if -1 it doesn't do anything, else it faces the player this way after moving to the location
	int m_nNextFacingDir = -1;
	//cost for if it's an innkeeper
	public int m_nCost = 0;

	bool m_bReturnToPlayer = false;
	bool m_bMoveTowardLocation = false;
	Vector3 m_vTargetLocation = Vector3.zero;



    //pathfinding stuff
    Vector3[] m_vWaypoints;
    int m_nWaypointIndex = 0;



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
            
            if (m_vWaypoints.Length > 0)
            {
                //doing pathfinding
                m_vTargetLocation = m_vWaypoints[m_nWaypointIndex];

            }
            Vector3 npcPos = transform.position;
            Vector3 toTarget = m_vTargetLocation - npcPos;
			
			if(toTarget.x > 0.1f || toTarget.x < -0.1f)
			{
				ResetAnimFlagsExcept(-1);
				if(m_aAnim != null)
				{
					if(toTarget.x > 0.1f)
						m_aAnim.SetBool("m_bMoveRight", true);
					else
						m_aAnim.SetBool("m_bMoveLeft", true);
				}
				toTarget.y = 0.0f;
				toTarget.Normalize();
				toTarget.x *= m_fWalkingSpeed;
				gameObject.GetComponent<Rigidbody2D>().velocity = toTarget;
			}
			else if(toTarget.y > 0.1f || toTarget.y  < -0.1f)
			{
				ResetAnimFlagsExcept(-1);
				if(m_aAnim != null)
				{
					if(toTarget.y > 0.1f)
						m_aAnim.SetBool("m_bMoveUp", true);
					else
						m_aAnim.SetBool("m_bMoveDown", true);
				}
				toTarget.x = 0.0f;
				toTarget.Normalize();
				toTarget.y *= m_fWalkingSpeed;
				gameObject.GetComponent<Rigidbody2D>().velocity = toTarget;
			}
			else
			{
                //reached target location

                if (m_vWaypoints.Length > 0)
                {
                    //we're doing waypoints, so check to see if there's another location to go, if so, go there.
                    if (m_nWaypointIndex < m_vWaypoints.Length-1)
                    {
						ResetAnimFlagsExcept(-1);
                        m_nWaypointIndex++;
                    }
                    else
                    {
                        //reached the end of the waypoints (maybe later adjust for looping paths?)
                        StopMovement();
                    }
                }
                else
                {
                    //doing some non-waypoint movement (/shrug) stop movement now that destination is reached
                    StopMovement();
                }
				
			}
		}

		#region Scripted Pathing
        #endregion
    }

    void StopMovement()
    {
        ResetAnimFlagsExcept(-1);
        if (m_nNextFacingDir != -1)
        {
            m_aAnim.SetInteger("m_nFacingDir", m_nNextFacingDir);
            m_nNextFacingDir = -1;
        }
        m_bMoveTowardLocation = false;
        m_vTargetLocation = Vector3.zero;
		m_nWaypointIndex = 0;
    }

	// Update is called once per frame
	void Update () 
	{
		HandleMovement();
		if(Input.GetKeyDown(KeyCode.A))
			DHF_NPCPathfindToGameobject(GameObject.Find("Target"), false, 2, false);
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
        /*
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
         * */
	}

	public void ResetAnimFlagsExcept(int exception)
	{
		if(m_aAnim != null)
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
	}

	public void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == "Action Box(Clone)")
		{
			m_bIsBeingInterractedWith = true;
			if(GetComponent<MessageHandler>())
			{
				if(m_szDialoguePath != "")
				{
					GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
					GameObject.Find("Event system").GetComponent<BaseEventSystemScript>().HandleEvent(m_szDialoguePath);
				}
			}
		}
	}

	void OnCollisionEnter2D(Collision2D c)
	{
		if(m_bIsComingOutOfPlayer == false)
		{
			//m_bCanMove = false;
			if(GetComponent<Rigidbody2D>())
				GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		}
	}

	void OnCollisionExit2D(Collision2D c)
	{
		//m_bCanMove = true;
	}

	public void OnDrawGizmos()
	{
		if(m_vWaypoints != null)
		{
			for(int i = m_nWaypointIndex; i < m_vWaypoints.Length; ++i)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube(m_vWaypoints[i], Vector3.one);
				if(i == m_nWaypointIndex)
					Gizmos.DrawLine(transform.position, m_vWaypoints[i]);
				else
					Gizmos.DrawLine(m_vWaypoints[i-1], m_vWaypoints[i]);
			}
		}
	}

}
