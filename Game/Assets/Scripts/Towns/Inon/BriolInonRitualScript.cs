using UnityEngine;
using System.Collections;

public class BriolInonRitualScript : MonoBehaviour 
{
	bool m_bIsMoving = false;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bIsMoving == true)
		{
			GetComponent<Rigidbody2D>().velocity = new Vector2(0,-1);
		}
	}

	public void MoveDownward()
	{
		m_bIsMoving = true;
		GetComponent<Animator>().SetBool("m_bMoveDown", true);
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == "BriolArriveAtRitual")
		{
			m_bIsMoving = false;
			GetComponent<Animator>().SetBool("m_bMoveDown", false);
			GameObject.Find("Event system").GetComponent<BaseEventSystemScript>().HandleEvent("BriolArriveAtRitual");
		}
	}
}
