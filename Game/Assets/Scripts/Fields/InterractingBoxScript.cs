using UnityEngine;
using System.Collections;

public class InterractingBoxScript : MonoBehaviour 
{
	GameObject m_goOwner;
	public void SetOwner(GameObject owner) {m_goOwner = owner;}
	public GameObject GetOwner() {return m_goOwner;}

	// Use this for initialization
	void Start () 
	{
		m_goOwner = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	

	void OnDestroy()
	{
		if(m_goOwner)
			m_goOwner.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
	}

	void OnTriggerEnter(Collider c)
	{
		if(c.tag == "Treasure" || c.tag == "Interractable")
		{
			if(m_goOwner.GetComponent<FieldPlayerMovementScript>().GetAllowInput() == true)
			{
				//Has found an interractable object nearby, and the player is allowed to interract with it currently.
				m_goOwner.GetComponent<FieldPlayerMovementScript>().ActivatePrompter();
			}
		}
	}

	void OnTriggerStay(Collider c)
	{
		if(c.tag == "Treasure" || c.tag == "Interractable")
		{
		
			if(m_goOwner.GetComponent<FieldPlayerMovementScript>().GetAllowInput() == true)
				{
				//Has found an interractable object nearby, and the player is allowed to interract with it currently.
				m_goOwner.GetComponent<FieldPlayerMovementScript>().ActivatePrompter();
			}
		}
	}
	void OnTriggerExit(Collider c)
	{
		m_goOwner.GetComponent<FieldPlayerMovementScript>().DeactivatePrompter();
	}
}
