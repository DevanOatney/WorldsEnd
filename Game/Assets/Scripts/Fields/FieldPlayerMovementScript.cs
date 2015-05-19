using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldPlayerMovementScript : MonoBehaviour 
{
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
	Vector3 m_vInitialScale;

	bool m_bIsRunning = false;
	float  m_fWalkingSpeed = 2.0f;
	float  m_fRunningSpeed = 6.0f;
	
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
	//public void SetAllowInput(bool flag) {m_bAllowInput = flag;}
	public bool GetAllowInput() {return m_bAllowInput;}


	
	// Use this for initialization
	void Start () 
	{
		m_aAnim = GetComponent<Animator>();
		m_vInitialScale = transform.localScale;
		//Set which direction the player should be idling toward when the screen loads
		m_aAnim.SetInteger("m_nFacingDir", m_nFacingDir);
		if(m_nFacingDir == 2)
		{
			Vector3 scale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
			transform.localScale = scale;
		}
		ResetAnimFlagsExcept(m_nFacingDir);


		//temp for testing status
		//DCScript.StatusEffect se = new DCScript.StatusEffect();
		//se.m_szName = "Poison";
		//se.m_nCount = 20;
		//se.m_lEffectedMembers = test;
		//GameObject.Find("PersistantData").GetComponent<DCScript>().m_lStatusEffects.Add(se);
		List<DCScript.StatusEffect> effects = GameObject.Find("PersistantData").GetComponent<DCScript>().m_lStatusEffects;
		foreach(DCScript.StatusEffect se in effects)
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

	void Update()
	{
		//temp for testing adding poison
		if(Input.GetKeyDown(KeyCode.Alpha2))
		{

		}
		if(m_bAllowInput == true)
		{
			if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
			{

				//temporarily suspend the ability for input
				m_bAllowInput = false;
				//reset anim flags so that the player goes into idle anim
				ResetAnimFlagsExcept(-1);
				BindInput();
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
			pos.y += GetComponent<BoxCollider>().bounds.size.y * 1.3f;
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
				m_aAnim.SetBool("m_bMoveRight", true);
				m_nFacingDir = 2;
			}
				break;
			case (int)States.eWALKDOWN:
			{
				m_bShouldMove = true;
				m_nFacingDir = 0;
				ResetAnimFlagsExcept(m_nFacingDir);
			}
				break;
			}
		}
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
				GetComponent<Rigidbody>().velocity = MoveVec * m_fRunningSpeed;
				newPos += MoveVec * m_fRunningSpeed * Time.deltaTime;
			}
			else
			{
				GetComponent<Rigidbody>().velocity = MoveVec * m_fWalkingSpeed;
				newPos += MoveVec * m_fWalkingSpeed * Time.deltaTime;
			}
			//transform.position = newPos;
			//if we can move and do stuff, handle predictor code

			newPos.x += MoveVec.x * GetComponent<BoxCollider>().bounds.size.x;

			if(MoveVec.y > 0)
				newPos.y += MoveVec.y * GetComponent<BoxCollider>().bounds.size.y * 0.5f;
			else if(MoveVec.y < 0)
				newPos.y += MoveVec.y * GetComponent<BoxCollider>().bounds.size.y * 1.5f;
			else
				newPos.y = transform.position.y - GetComponent<BoxCollider>().bounds.size.y * 0.5f;
			m_goPredictor.transform.position = newPos;


			//Update any of the status effects. (use a new list, as some of the master list may get removed
			for(int i = 0; i < m_lStatusEffects.Count; ++i)
			{
				if(m_lStatusEffects[i].GetComponent<FieldBaseStatusEffectScript>().m_bToBeRemoved == true)
				{

					GameObject.Find("PersistantData").GetComponent<DCScript>().m_lStatusEffects.RemoveAt(i);
					m_lStatusEffects.RemoveAt(i);
					i--;
				}
				else
				m_lStatusEffects[i].GetComponent<FieldBaseStatusEffectScript>().m_dFunc();
			}
		}


		//end of the function
		m_bShouldMove = false;
	}

	void OnGUI()
	{
	}



	//Check if the player is pushing one of the movement keys
	void GetMovementKey()
	{
		int moveDir = -1;
		if(Input.GetKey(KeyCode.DownArrow))
		{
			//set the scale to the initial scale incase we were facing right
			transform.localScale = m_vInitialScale;
			moveDir = 0;
			m_nFacingDir = 0;
			m_aAnim.SetInteger("m_nFacingDir", m_nFacingDir);
		}
		else if(Input.GetKey(KeyCode.LeftArrow))
		{
			//set the scale to the initial scale incase we were facing right
			transform.localScale = m_vInitialScale;
			moveDir = 1;
			m_nFacingDir = 1;
			m_aAnim.SetInteger("m_nFacingDir", m_nFacingDir);
		}
		else if(Input.GetKey(KeyCode.RightArrow))
		{
			moveDir = 2;
			m_nFacingDir = 2;
			m_aAnim.SetInteger("m_nFacingDir", m_nFacingDir);
		}
		else if(Input.GetKey(KeyCode.UpArrow))
		{
			//set the scale to the initial scale incase we were facing right
			transform.localScale = m_vInitialScale;
			moveDir = 3;
			m_nFacingDir = 3;
			m_aAnim.SetInteger("m_nFacingDir", m_nFacingDir);
		}

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
		if(moveDir != -1)
			m_bShouldMove = true;
		ResetAnimFlagsExcept(moveDir);
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
			m_aAnim.SetBool("m_bMoveLeft", false);
			m_aAnim.SetBool("m_bMoveUp", false);
			m_aAnim.SetBool("m_bMoveDown", true);
			m_aAnim.SetBool("m_bMoveRight", false);
		}
			break;
		case 1:
		{
			//Left
			m_aAnim.SetBool("m_bMoveUp", false);
			m_aAnim.SetBool("m_bMoveDown", false);
			m_aAnim.SetBool("m_bMoveLeft", true);
			m_aAnim.SetBool("m_bMoveRight", false);
		}
			break;
		case 2:
		{
			//Right
			m_aAnim.SetBool("m_bMoveUp", false);
			m_aAnim.SetBool("m_bMoveDown", false);
			m_aAnim.SetBool("m_bMoveLeft", false);
			m_aAnim.SetBool("m_bMoveRight", true);
		}
			break;
		case 3:
		{
			//Up
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
		}
			break;
		}
	}
	void OnTriggerEnter(Collider c)
	{
		if(c.tag == "Waypoint")
		{
			GameObject eventSys = GameObject.Find("EventWatcher");
			if(eventSys)
			{
				eventSys.GetComponent<EventHandler>().WaypointTriggered(c);
			}
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
