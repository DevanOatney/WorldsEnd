﻿using UnityEngine;
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
	

	void OnTriggerEnter2D(Collider2D c)
	{
		if(c.tag == "Treasure" || c.tag == "Interractable" || c.tag == "Merchant")
		{
			if(c.tag == "Treasure")
			{
				if(c.gameObject.GetComponent<TreasureChestScript>().m_bIsOpened == true)
				{
					m_goOwner.GetComponent<FieldPlayerMovementScript>().DeactivatePrompter();
					return;
				}
			}

			if (m_goOwner.GetComponent<FieldPlayerMovementScript> ().GetAllowInput () == true) {
				//Has found an interractable object nearby, and the player is allowed to interract with it currently.
				m_goOwner.GetComponent<FieldPlayerMovementScript> ().ActivatePrompter ();
			} else {
				m_goOwner.GetComponent<FieldPlayerMovementScript>().DeactivatePrompter();
			}
		}
	}

	void OnTriggerStay2D(Collider2D c)
	{
		if (c.tag == "Treasure" || c.tag == "Interractable" || c.tag == "Merchant") {
			if (c.tag == "Treasure") {
				if (c.gameObject.GetComponent<TreasureChestScript> ().m_bIsOpened == true) {
					m_goOwner.GetComponent<FieldPlayerMovementScript> ().DeactivatePrompter ();
					return;
				}
			}
			if (c.enabled == false || c.GetComponent<Collider2D>().enabled == false) {
				m_goOwner.GetComponent<FieldPlayerMovementScript> ().DeactivatePrompter ();
				return;
			}
			if (m_goOwner.GetComponent<FieldPlayerMovementScript> ().GetAllowInput () == true) {
				//Has found an interractable object nearby, and the player is allowed to interract with it currently.
				m_goOwner.GetComponent<FieldPlayerMovementScript> ().ActivatePrompter ();
			}
		} 
	}
	void OnTriggerExit2D(Collider2D c)
	{
		m_goOwner.GetComponent<FieldPlayerMovementScript>().DeactivatePrompter();
	}
}
