using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCScript : MonoBehaviour 
{
	#region DESIGNER_HELPER_FUNCTIONS
	//moves into the player

	public void DHF_NPCMoveIntoPlayer()
	{
		m_bMoveIntoPlayer = true;
		DHF_NPCMoveToGameobject (GameObject.Find ("Player"), false, 0, false);
		GetComponent<Collider2D> ().enabled = false;

	}

	//Move toward location (single node)
	public void DHF_NPCMoveToGameobject(GameObject _target, bool _shouldIRun, int _nextFacingDir, bool _comingFromPlayer = false)
	{
		m_bShouldRun = _shouldIRun;
		m_nNextFacingDir = _nextFacingDir;
		m_bIsComingOutOfPlayer = _comingFromPlayer;
		if(m_bIsComingOutOfPlayer == true)
			Invoke("DelayedCollisionActivation", 0.35f);
		Vector3 _pos = _target.transform.position;
		float _offset = GetComponent<Collider2D> ().bounds.size.y * 0.5f;
		_pos.y += _offset;
		m_lNodes.Add (_pos);
	}

	//Move toward location (multiple nodes)
	public void DHF_NPCMoveToGameobject(GameObject[] _targets, bool _shouldRun, int _nextFacingDir, bool _comingFromPlayer = false)
	{
		foreach (GameObject _target in _targets)
		{
			Vector3 _pos = _target.transform.position;
			_pos.y += GetComponent<Collider2D> ().bounds.size.y * 0.5f;
			m_lNodes.Add (_pos);
		}
		m_bShouldRun = _shouldRun;
		m_nNextFacingDir = _nextFacingDir;
		m_bIsComingOutOfPlayer = _comingFromPlayer;
		if (m_bIsComingOutOfPlayer == true)
			Invoke ("DelayedCollisionActivation", 0.3f);
	}



	#endregion

	public enum FACINGDIR {eDOWN, eLEFT, eRIGHT, eUP}
	public int m_nFacingDir = 0;
	//If this NPC is active and should move/be interracted with
	public bool m_bActive = false;
	//Path directory to dialogue for this character in this scene
	public string m_szDialoguePath = "";

	public float m_fWalkingSpeed = 2.0f;
	public float m_fRunningSpeed = 4.0f;
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
	bool m_bMoveIntoPlayer = false;



    //pathfinding stuff
	List<Vector3> m_lNodes = new List<Vector3>();
	int m_nNodeIndex = 0;



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
		if (m_lNodes.Count > 0)
		{
			//So, there is somewhere we should move- so let's do this shit!
			float _fSpeed = 0.0f;
			if (m_bShouldRun == false)
				_fSpeed = m_fWalkingSpeed * Time.deltaTime;
			else
				_fSpeed = m_fRunningSpeed * Time.deltaTime;
			Vector3 _vMovePos = Vector3.MoveTowards (transform.position, m_lNodes [m_nNodeIndex], _fSpeed);

			if (_vMovePos == transform.position)
			{
				//reached current destination
				m_nNodeIndex += 1;
				if (m_nNodeIndex >= m_lNodes.Count)
				{
					if (m_bMoveIntoPlayer == true)
					{
						//this was us moving back to the player, so this character should be set to innactive and hidden.
						m_bMoveIntoPlayer = false;
						GetComponent<Collider2D>().enabled = true;
						gameObject.GetComponent<SpriteRenderer>().enabled = false;
						gameObject.GetComponent<Collider2D>().enabled = false;
						GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
						Camera.main.GetComponent<CameraFollowTarget>().m_goNextTarget = GameObject.Find("Player");
						StopMovement ();
					}


					//We've reached the end of the current node needed to reach, move to the next one.
					m_nNodeIndex += 1;
					if (m_nNodeIndex >= m_lNodes.Count)
					{
						//We've reached the final node in the list, stop movement and face the direction desired.
						m_nNodeIndex = 0;
						m_lNodes.Clear ();
						StopMovement ();
					}
				}
			}
			else
			{
				//Haven't reached destination yet.
				Vector3 _toVec = (_vMovePos - transform.position).normalized;
				m_aAnim.SetBool ("m_bMoving", true);
				m_aAnim.SetFloat ("X", _toVec.x);
				m_aAnim.SetFloat ("Y", _toVec.y);
				SetNPCFacing (_toVec);
				transform.position = _vMovePos;
			}


		}
    }

    protected void StopMovement()
    {
		if (m_aAnim != null)
		{
			m_aAnim.SetBool ("m_bMoving", false);
			if(m_nNextFacingDir != -1)
				m_aAnim.SetFloat ("m_nFacingDir", m_nNextFacingDir);
			m_lNodes.Clear ();
			m_nNodeIndex = 0;
		}
    }

	// Update is called once per frame
	void Update () 
	{
		HandleMovement();
	}

	//Set the direction the unit is facing based on the direction to the parameter passed in.
	void SetNPCFacing(Vector3 _direction)
	{
		if (_direction.x > 0.8f)
		{
			//right
			m_nFacingDir = 2;
		}
		else
		if (_direction.x < -0.8f)
		{
			//Left
			m_nFacingDir = 1;
		}
		else
		if (_direction.y > 0.8f)
		{
			//up
			m_nFacingDir = 3;
		}
		else
		if (_direction.y < -0.8f)
		{
			//down
			m_nFacingDir = 0;
		}
		m_aAnim.SetFloat ("m_nFacingDir", m_nFacingDir);
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

	//Used for if interracted with by the player.   If there is dialogue to display, do it!
	virtual public void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == "Action Box(Clone)")
		{
			m_bIsBeingInterractedWith = true;
			if(GetComponentInChildren<MessageHandler>())
			{
				if(m_szDialoguePath != "")
				{
					GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
					GameObject.Find("Event system").GetComponent<BaseEventSystemScript>().HandleEvent(m_szDialoguePath);
				}
			}
		}
	}

	//If you collide with something, don't keep moving forward.
	void OnCollisionEnter2D(Collision2D c)
	{
		if(m_bIsComingOutOfPlayer == false)
		{
			//m_bCanMove = false;
			if(GetComponent<Rigidbody2D>())
				GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		}
	}

	//Display the path that the character is walking.
	public void OnDrawGizmos()
	{
		if(m_lNodes.Count > 0)
		{
				for (int i = m_nNodeIndex; i < m_lNodes.Count; ++i)
				{
					Gizmos.color = Color.black;
					Gizmos.DrawCube (m_lNodes [i], Vector3.one);
					if (i == m_nNodeIndex)
						Gizmos.DrawLine (transform.position, m_lNodes [i]);
					else
					Gizmos.DrawLine (m_lNodes [i - 1], m_lNodes [i]);
				}
		}	}

		
	void DelayedCollisionActivation()
	{
		GetComponent<BoxCollider2D> ().enabled = true;
	}

}
