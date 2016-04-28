using UnityEngine;
using System.Collections;

public class InterractableQuestItemScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == "Action Box(Clone)")
		{
			GameObject.Find("Event system").GetComponent<BaseEventSystemScript>().HandleEvent(gameObject.name);
			gameObject.SetActive(false);
		}
	}
}
