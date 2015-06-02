using UnityEngine;
using System.Collections;

public class InonEventHandler : BaseEventSystemScript 
{
	DCScript ds;
	
	public GameObject[] Phase1_waypoints;
	public GameObject[] Phase2_waypoints;
	public GameObject[] Phase3_waypoints;
	
	// Use this for initialization
	void Start () 
	{
		ds = GameObject.Find("PersistantData").GetComponent<DCScript>();

		//int result;
		//if(ds.m_dStoryFlagField.TryGetValue("Inon_CrossedBridge", out result))

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	override public void HandleEvent(string eventID)
	{
		switch(eventID)
		{
		case "FemaleDancerDialogue":
			int result;
			if(ds.m_dStoryFlagField.TryGetValue("Inon_CrossedBridge", out result))
			{
				//If the player has previously gone over the bridge
				BeginDialogue(1);
			}
			else
			{
				//If the player hasn't gone over the bridge
				BeginDialogue(0);
			}
			break;
		case "Inon_Merchant1":
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
			GameObject messageSystem = GameObject.Find("NPC_Merchant1");
			if(messageSystem)
			{
				messageSystem.GetComponent<MessageHandler>().BeginDialogue(0);
			}
		}

			break;
		case "EndDialogue":
		{
			//turn off all dialogues happening, release bind on input.. umn.. i think that's it?
			GameObject[] gObjs = GameObject.FindObjectsOfType<GameObject>();
			foreach(GameObject g in gObjs)
			{
				if(g.GetComponent<MessageHandler>() != null)
				{
					if(g.GetComponent<NPCScript>() != null)
						g.GetComponent<NPCScript>().RestartPathing();
					g.GetComponent<MessageHandler>().m_bShouldDisplayDialogue = false;
				}
			}
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
		}
			break;
		case "Merchant_EndDialogue":
		{
			GameObject[] gObjs = GameObject.FindGameObjectsWithTag("Merchant");
			foreach(GameObject g in gObjs)
			{
				g.GetComponent<NPC_ArmorMerchantScript>().ActivateMerchantScreen();
			}
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
		}
			break;
		case "Blacksmith_EndDialogue":
		{
			Debug.Log("event");
			GameObject blackSmith = GameObject.Find("Blacksmith");
			blackSmith.GetComponent<NPC_BlacksmithScript>().ActivateScreen();
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
			{
				player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
			}
		}
			break;
		default:
			break;
		}
	}

	void BeginDialogue(int iter)
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if(player)
		{
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
		}
		GameObject messageSystem = GameObject.Find("Female Dancer");
		if(messageSystem)
		{
			messageSystem.GetComponent<MessageHandler>().BeginDialogue(iter);
		}
	}

	override public void WaypointTriggered(Collider c)
	{
		switch(c.name)
		{
		case "Waypoint1":
			c.enabled = false;
			ds.m_dStoryFlagField.Add("Inon_CrossedBridge", 1);
			break;
		default:
			break;
		}
	}

}