using UnityEngine;
using System.Collections;

public class NoteFromFamilyScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == "Action Box(Clone)")
		{
			GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
			int value;
			if(GameObject.Find("PersistantData").GetComponent<DCScript>().m_dStoryFlagField.TryGetValue("Inon_HasMoved",out value) == true)
				GameObject.Find("Event system").GetComponent<BaseEventSystemScript>().HandleEvent("NoteInterractedWith");
		}
	}
}
