using UnityEngine;
using System.Collections;

public class BaseEventSystemScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	virtual public void AdjustBuildings()
	{
	}
	virtual public void SetWaypoints()
	{
	}
	virtual public void HandleEvent(string eventID)
	{
		if (eventID.Contains (".ResourceFarming"))
		{
			//This is just a dialogue pop-up for an npc gathering resources.
			string[] _Pieces = eventID.Split('.');

			GameObject messageSystem = GameObject.Find(_Pieces[0].Trim());
			if (messageSystem)
			{

				messageSystem.GetComponentInChildren<MessageHandler> ().BeginDialogue (_Pieces [0].Trim ());
			}
			return;
		}
		switch (eventID)
		{
			case "EndDialogue":
				{
					//turn off all dialogues happening, release bind on input.. umn.. i think that's it?
					GameObject[] gObjs = GameObject.FindObjectsOfType<GameObject>();
					foreach(GameObject g in gObjs)
					{
						if(g.GetComponentInChildren<MessageHandler>() != null)
						{
							if(g.GetComponent<NPCScript>() != null)
								g.GetComponent<NPCScript>().m_bIsBeingInterractedWith = false;
							g.GetComponentInChildren<MessageHandler>().m_bShouldDisplayDialogue = false;
						}
					}
					GameObject player = GameObject.FindGameObjectWithTag("Player");
					if(player)
					{
						player.GetComponent<FieldPlayerMovementScript>().ReleaseAllBinds();
					}
				}
				break;
		}
	}
	virtual public void WaypointTriggered(Collider2D c)
	{
	}
}
