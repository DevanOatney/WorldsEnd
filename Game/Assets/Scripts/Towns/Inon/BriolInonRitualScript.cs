using UnityEngine;
using System.Collections;

public class BriolInonRitualScript : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public void MoveDownward()
	{
		GetComponent<NPCScript>().m_bActive = true;
		GetComponent<NPCScript>().m_bIsMoving = true;
		GetComponent<NPCScript>().m_nFacingDir = 0;
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == "BriolArriveAtRitual")
		{
			GetComponent<NPCScript>().m_bActive = false;
			GetComponent<NPCScript>().m_bIsMoving = false;
			GameObject.Find("Event system").GetComponent<BaseEventSystemScript>().HandleEvent("BriolArriveAtRitual");
		}
	}

	void OnCollisionEnter2D(Collision2D c)
	{
		if(c.collider.name == "Player")
		{
			Debug.Log ("hit1");
			GameObject.Find("PersistantData").GetComponent<DCScript>().m_lItemLibrary.AddItem("Boar Tusk");
		}
	}
}
