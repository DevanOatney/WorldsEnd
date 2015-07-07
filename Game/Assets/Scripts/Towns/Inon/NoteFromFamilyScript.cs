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
			int value;
			if(GameObject.Find("PersistantData").GetComponent<DCScript>().m_dStoryFlagField.TryGetValue("Inon_HasMoved",out value) == true)
				GameObject.Find("Event System").GetComponent<BaseEventSystemScript>().HandleEvent("NoteInterractedWith");
		}
	}
}
