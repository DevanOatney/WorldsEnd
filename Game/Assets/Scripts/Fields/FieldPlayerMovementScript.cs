using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldPlayerMovementScript : MonoBehaviour 
{
	#region DESIGNER_HELPER_FUNCTIONS
	//Function for the player to move to a target game object, set the flag to true if you want the player to run.
	public void DHF_PlayerMoveToGameObject(GameObject _TargetLocation)
	{
		BindInput();
		m_bIsMovingToLocation = true;
		m_vTargetLocation = (Vector2)_TargetLocation.transform.position;
		if(_TargetLocation.GetComponent<BoxCollider2D>() != null)
		{
			m_vTargetLocation.y += _TargetLocation.GetComponent<BoxCollider2D>().size.y * 0.5f;
		}
		m_bIsRunning = false;
		m_aAnim.SetBool("m_bRunButtonIsPressed", m_bIsRunning);
		
	}
	public void DHF_PlayerMoveToGameObject(GameObject _TargetLocation, bool _bShouldIRun)
	{
		BindInput();
		m_bIsMovingToLocation = true;
		m_vTargetLocation = (Vector2)_TargetLocation.transform.position;
		if(_TargetLocation.GetComponent<BoxCollider2D>() != null)
		{
			m_vTargetLocation.y += _TargetLocation.GetComponent<BoxCollider2D>().size.y * 0.5f;
		}
		m_bIsRunning = _bShouldIRun;
		m_aAnim.SetBool("m_bRunButtonIsPressed", m_bIsRunning);
		
	}
	public void DHF_PlayerMoveToGameObject(GameObject _TargetLocation, bool _bShouldIRun, int _nextFacingDirection)
	{
		BindInput();
		m_bIsMovingToLocation = true;
		m_vTargetLocation = (Vector2)_TargetLocation.transform.position;
		if(_TargetLocation.GetComponent<BoxCollider2D>() != null)
		{
			m_vTargetLocation.y += _TargetLocation.GetComponent<BoxCollider2D>().size.y * 0.5f;
		}
		m_bIsRunning = _bShouldIRun;
		m_nNextFacingDir = _nextFacingDirection;
		m_aAnim.SetBool("m_bRunButtonIsPressed", m_bIsRunning);
		
	}

	public void DHF_StopMovingFaceDirection(int _facingDir)
	{
		m_bIsMovingToLocation = false;
		m_vTargetLocation = Vector3.zero;
		m_bIsRunning = false;
		m_aAnim.SetBool("m_bRunButtonIsPressed", false);
		m_nFacingDir = _facingDir;
		m_aAnim.SetInteger("m_nFacingDir", m_nFacingDir);
	}
	#endregion




	public enum States {eIDLE, eWALKLEFT, eWALKRIGHT, eWALKUP, eWALKDOWN, eRUNLEFT, eRUNRIGHT, eRUNUP, eRUNDOWN};
	int m_nState;
	public int GetState() {return m_nState;}
	public void SetState(int state) {m_nState = state;}

	Animator m_aAnim;
	public Animator GetAnimator() {return m_aAnim;}
	//Direction the player is facing
	//0 : Down
	//1 : Left
	//2 : Right
	//3 : Up
	public int m_nFacingDir = 0;
	//For helper functions of moving, if -1 it doesn't do anything, else it faces the player this way after moving to the location
	int m_nNextFacingDir = -1;

	bool m_bIsRunning = false;
	public void SetIsRunning(bool flag) {m_bIsRunning = flag;}
	public bool GetIsRunning() {return m_bIsRunning;}
	float  m_fWalkingSpeed = 4.0f;
	float  m_fRunningSpeed = 8.0f;
	//flag for if the player is being moved without player input
	bool m_bShouldMove = false;

	


	//List of status effects that could be effecting the player/units   Poison, Confusion, Paralyze, Stone (examples)
	public List<GameObject> m_lStatusEffects = new List<GameObject>();
	public void RemoveStatusEffect(string effectName)
	{
		for (int i = m_lStatusEffects.Count - 1; i >= 0; i--)
		{
			if(m_lStatusEffects[i].name == effectName)
			{
				m_lStatusEffects.RemoveAt(i);
			}
		}
	}
	//temp for status effect testing
	public GameObject m_poison;

	//used like the action box, just to see if something is interractable based on the direction the player is facing... have it always on, and just change it's position based on the direction the player is facing
	public GameObject m_goPredictor;
	//used for when the player is trying to interract with the environment
	public GameObject m_goActionBox;
	//bubble that appears above the players head when prompted
	public GameObject m_goPrompter;
	//letter that appears in that bubble
	public GameObject m_goTextInPrompt;
	//flag to tell if the prompt should be showing
	bool m_bPromptShouldRender = false;
	float m_fTextBlinkTimer = 0.0f;
	float m_fTextBlinkBucket = 0.75f;

	//bool for allowing input (turn off during.. events... screen switching... opening treasure chests?.. dialogue... other things I haven't thought of?
	bool m_bAllowInput = true;
	//amount of things restricting player input
	int m_nBinders = 0;
	public void BindInput() {m_nBinders++; m_bAllowInput = false;}
	public void ReleaseBind() {m_nBinders--;  if(m_nBinders <= 0) {m_nBinders = 0; m_bAllowInput = true;}}
	public void ReleaseAllBinds() {m_nBinders = 0; m_bAllowInput = false;}
	//public void SetAllowInput(bool flag) {m_bAllowInput = flag;}
	public bool GetAllowInput() {return m_bAllowInput;}

	Vector3 m_vTargetLocation = Vector3.zero;
	bool m_bIsMovingToLocation = false;


	void Awake()
	{
		m_aAnim = GetComponent<Animator>();
		m_aAnim.SetInteger("m_nFacingDir", m_nFacingDir);
		ResetAnimFlagsExcept(m_nFacingDir);
	}

	// Use this for initialization
	void Start () 
	{
		//temp for testing status
		//DCScript.StatusEffect tse = new DCScript.StatusEffect();
		//tse.m_szName = "Poison";
		//tse.m_nCount = 20;
		//tse.m_lEffectedMembers.Add("Callan");
		//GameObject.Find("PersistantData").GetComponent<DCScript>().GetStatusEffects().Add(tse);
		List<DCScript.StatusEffect> effects = GameObject.Find("PersistantData").GetComponent<DCScript>().GetStatusEffects();
		foreach(DCScript.StatusEffect se in effects)
		{
			bool alreadyIn = false;
			foreach(GameObject go in m_lStatusEffects)
			{

				if(se.m_szName.Trim() == go.name.Trim())
					alreadyIn = true;
			}
			if(alreadyIn == false)
			{
				switch(se.m_szName)
				{
				case "Poison":
				{
					m_poison.GetComponent<PoisonEffectScript>().Initialize(gameObject, se.m_lEffectedMembers, 1, se.m_nCount, 1.0f);
					m_lStatusEffects.Add(m_poison);
				}
					break;
				case "Paralyze":
				{
				}
					break;
				case "Stone":
				{
				}
					break;
				case "Confuse":
				{
				}
					break;
				}
			}
		}
	}

	void Update()
	{

		if(m_bAllowInput == true)
		{
			if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
			{

				//temporarily suspend the ability for input
				//reset anim flags so that the player goes into idle anim
				GameObject box = Instantiate(m_goActionBox, m_goPredictor.transform.position, Quaternion.identity) as GameObject;
				Destroy(box, 0.25f);
				box.GetComponent<InterractingBoxScript>().SetOwner(gameObject);
				//Handle results of the collision in that objects script
			}
		}

		//if we should display the prompt, count down to blinking the E so it's more noticeable
		if(m_bPromptShouldRender == true)
		{
			Vector3 pos = transform.position;
			pos.y += GetComponent<BoxCollider2D>().bounds.size.y * 1.3f;
			m_goPrompter.transform.position = pos;
			m_fTextBlinkTimer += Time.deltaTime;
			if(m_fTextBlinkTimer >= m_fTextBlinkBucket)
			{
				m_goTextInPrompt.GetComponent<SpriteRenderer>().enabled = !m_goTextInPrompt.GetComponent<SpriteRenderer>().enabled;
				m_fTextBlinkTimer = 0.0f;
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		if(m_bAllowInput == true)
		{
			GetMovementKey();

		}
		else
		{
			//Let's check if the player is supposed to be moving toward a location, if so figure out which direction we should be moving right now
			if(m_bIsMovingToLocation == true)
			{
				Vector2 toTarget = m_vTargetLocation - transform.position;
				if(toTarget.x > 0.1f || toTarget.x < -0.1f)
				{

					if(toTarget.x > 0.1f)
					{
						m_nState = (int)States.eWALKRIGHT;
					}
					else
					{
						m_nState = (int)States.eWALKLEFT;
					}
				}
				else if(toTarget.y > 0.1f || toTarget.y  < -0.1f)
				{
					if(toTarget.y > 0.1f)
					{
						m_nState = (int)States.eWALKUP;
					}
					else
					{
						m_nState = (int)States.eWALKDOWN;
					}
				}
				else
				{
					ResetAnimFlagsExcept(-1);
					if(m_nNextFacingDir != -1)
					{
						m_aAnim.SetInteger("m_nFacingDir", m_nNextFacingDir);
						m_nNextFacingDir = -1;
					}
					m_bIsRunning = false;
					m_bIsMovingToLocation = false;
					m_bShouldMove = false;
					m_vTargetLocation = Vector3.zero;
					m_nState = (int)States.eIDLE;
					m_aAnim.SetBool("m_bRunButtonIsPressed", false);
					ReleaseBind();
				}
			}

			//Going to just assume that if input is being stopped there's an event playing (even though sometimes it's during opening 
			//treasure chests, and switching maps/scenes)  So just make sure to keep players state at eIDLE any other time
			switch(m_nState)
			{
			case (int)States.eIDLE:
			{
				//Just to make sure
				m_bShouldMove = false;
			}
				break;
			case (int)States.eWALKRIGHT:
			{
				m_bShouldMove = true;
				m_nFacingDir = 2;
				ResetAnimFlagsExcept(m_nFacingDir);
			}
				break;
			case (int)States.eWALKLEFT:
			{
				m_bShouldMove = true;
				m_nFacingDir = 1;
				ResetAnimFlagsExcept(m_nFacingDir);
			}
				break;
			case (int)States.eWALKDOWN:
			{
				m_bShouldMove = true;
				m_nFacingDir = 0;
				ResetAnimFlagsExcept(m_nFacingDir);
			}
				break;
			case (int)States.eWALKUP:
			{
				m_bShouldMove = true;
				m_nFacingDir = 3;
				ResetAnimFlagsExcept(m_nFacingDir);
			}
				break;
			}
		}
		#region Handle_MovementLogic
		//Now that the movement logic is done, check to see if the result is to move.
		if(m_bShouldMove == true)
		{

			Vector3 MoveVec = new Vector2(0, 0);
			switch(m_nFacingDir)
			{
			case 0:
			{
				MoveVec.y = -1;
			}
				break;
			case 1:
			{
				MoveVec.x = -1;
			}
				break;
			case 2:
			{
				MoveVec.x = 1;
			}
				break;
			case 3:
			{
				MoveVec.y = 1;
			}
				break;
			}

			Vector3 newPos = transform.position;
			if(m_bIsRunning == true)
			{
				GetComponent<Rigidbody2D>().velocity = MoveVec * m_fRunningSpeed;
				newPos += MoveVec * m_fRunningSpeed * Time.deltaTime;
			}
			else
			{
				GetComponent<Rigidbody2D>().velocity = MoveVec * m_fWalkingSpeed;
				newPos += MoveVec * m_fWalkingSpeed * Time.deltaTime;
			}
			//transform.position = newPos;
			//if we can move and do stuff, handle predictor code

			newPos.x += MoveVec.x * GetComponent<BoxCollider2D>().bounds.size.x * 0.5f;

			if(MoveVec.y > 0)
				newPos.y += MoveVec.y * GetComponent<BoxCollider2D>().bounds.size.y * 0.5f;
			else if(MoveVec.y < 0)
				newPos.y += MoveVec.y * GetComponent<BoxCollider2D>().bounds.size.y * 1.5f;
			else
				newPos.y = transform.position.y - GetComponent<BoxCollider2D>().bounds.size.y ;
			m_goPredictor.transform.position = newPos;
			#endregion

			//Update any of the status effects. (use a new list, as some of the master list may get removed
			for(int i = 0; i < m_lStatusEffects.Count; ++i)
			{
				if(m_lStatusEffects[i].GetComponent<FieldBaseStatusEffectScript>().m_bToBeRemoved == true)
				{

					GameObject.Find("PersistantData").GetComponent<DCScript>().GetStatusEffects().RemoveAt(i);
					m_lStatusEffects.RemoveAt(i);
					i--;
				}
				else
				{
					m_lStatusEffects[i].GetComponent<FieldBaseStatusEffectScript>().m_dFunc();
				}
			}
		}


		//end of the function
		m_bShouldMove = false;
	}



	//Check if the player is pushing one of the movement keys
	void GetMovementKey()
	{
		List<int> lDirs = new List<int>();
		if(Input.GetKey(KeyCode.DownArrow))
			lDirs.Add(0);
		if(Input.GetKey(KeyCode.LeftArrow))
			lDirs.Add(1);
		if(Input.GetKey(KeyCode.RightArrow))
			lDirs.Add(2);
		if(Input.GetKey(KeyCode.UpArrow))
			lDirs.Add(3);

		if(Input.GetKey(KeyCode.LeftShift))
	    {
			m_aAnim.SetBool("m_bRunButtonIsPressed", true);
			m_bIsRunning = true;
		}
		else if(Input.GetKey(KeyCode.RightShift))
		{
			m_aAnim.SetBool("m_bRunButtonIsPressed", true);
			m_bIsRunning = true;
		}
		else
		{
			m_aAnim.SetBool("m_bRunButtonIsPressed", false);
			m_bIsRunning = false;
		}
		if(lDirs.Count > 0)
		{
			int moveDir = m_nFacingDir;
			if(lDirs.Count > 1)
			{
				bool hasCurDir = false;
				foreach(int i in lDirs)
					if(i == moveDir)
						hasCurDir = true;
				if(hasCurDir == false)
				{
					moveDir = lDirs[Random.Range(0, lDirs.Count-1)];
				}
			}
			else
				moveDir = lDirs[0];
			m_nFacingDir = moveDir;
			GetAnimator().SetInteger("m_nFacingDir", m_nFacingDir);
			ResetAnimFlagsExcept(moveDir);
			m_bShouldMove = true;
		}
		else
		{
			m_bShouldMove = false;
			ResetAnimFlagsExcept(-1);
		}

	}

	//set movement flags to false except the one we want
	//0 - Down
	//1 - Left+Right
	//2 - Up
	public void ResetAnimFlagsExcept(int exception)
	{
		switch(exception)
		{
		case 0:
		{
			//Down
			m_aAnim.SetInteger("m_nFacingDir", m_nFacingDir);
			m_aAnim.SetBool("m_bMoveLeft", false);
			m_aAnim.SetBool("m_bMoveUp", false);
			m_aAnim.SetBool("m_bMoveDown", true);
			m_aAnim.SetBool("m_bMoveRight", false);
		}
			break;
		case 1:
		{
			//Left
			m_aAnim.SetInteger("m_nFacingDir", m_nFacingDir);
			m_aAnim.SetBool("m_bMoveUp", false);
			m_aAnim.SetBool("m_bMoveDown", false);
			m_aAnim.SetBool("m_bMoveLeft", true);
			m_aAnim.SetBool("m_bMoveRight", false);
		}
			break;
		case 2:
		{
			//Right
			m_aAnim.SetInteger("m_nFacingDir", m_nFacingDir);
			m_aAnim.SetBool("m_bMoveUp", false);
			m_aAnim.SetBool("m_bMoveDown", false);
			m_aAnim.SetBool("m_bMoveLeft", false);
			m_aAnim.SetBool("m_bMoveRight", true);
		}
			break;
		case 3:
		{
			//Up
			m_aAnim.SetInteger("m_nFacingDir", m_nFacingDir);
			m_aAnim.SetBool("m_bMoveDown", false);
			m_aAnim.SetBool("m_bMoveLeft", false);
			m_aAnim.SetBool("m_bMoveUp", true);
			m_aAnim.SetBool("m_bMoveRight", false);
		}
			break;
		default:
		{
			m_aAnim.SetBool("m_bMoveUp", false);
			m_aAnim.SetBool("m_bMoveDown", false);
			m_aAnim.SetBool("m_bMoveLeft", false);
			m_aAnim.SetBool("m_bMoveRight", false);
			m_aAnim.SetBool("m_bRunButtonIsPressed", false);
		}
			break;
		}
	}

	public void ActivatePrompter()
	{
		m_bPromptShouldRender = true;
		m_goPrompter.GetComponent<SpriteRenderer>().enabled = true;
	}
	public void DeactivatePrompter()
	{
		m_bPromptShouldRender = false;
		m_goPrompter.GetComponent<SpriteRenderer>().enabled = false;
		m_goTextInPrompt.GetComponent<SpriteRenderer>().enabled = false;
	}

}
