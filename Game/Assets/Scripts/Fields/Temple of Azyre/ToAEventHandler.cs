using UnityEngine;
using System.Collections;

public class ToAEventHandler : BaseEventSystemScript {
	public GameObject[] Phase1_waypoints;

	// Use this for initialization
	void Start 
	{
		int result;
		if(ds.m_dStoryFlagField.TryGetValue("AfterOpening", out result) == false)
		{
			//This is the introduction, play it yo!
			GameObject player = GameObject.Find("Player");
			player.GetComponent<FieldPlayerMovementScript>().BindInput();
			player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
			player.GetComponent<MessageHandler>().BeginDialogue(0);
			ds.m_dStoryFlagField.Add("AfterOpening", 1);
			foreach(GameObject wpnt in Phase1_waypoints)
				wpnt.GetComponent<BoxCollider2D>().enabled = true;
			Camera.main.GetComponent<CameraFollowTarget>.GoTarget = Briol;

			GameObject src = GameObject.Find("Player");
			player.GetComponent<MessageHandler>().BeginDialogue("A0");
		}
	
	}
	
	// Update is called once per frame
	void Update () {
		case "Callan_runoff";
		{
			GameObject src = GameObject.Find("Player");
			src.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKUP);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveUp", true);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", true);
			src.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 3);
			src.GetComponent<FieldPlayerMovementScript>().SetIsRunning(true);
			GameObject.Find("XWaypoint").GetComponent<BoxCollider2D>().enabled = true;
			GameObject messageSystem = GameObject.Find("Briol");
			messageSystem.GetComponent<MessageHandler>().BeginDialogue("A5")
		}
		break;
		case "AfterOpen";
		{	
			GameObject XWaypoint = Gameobject.Find ("XWaypoint");
			src.GetComponent<WaypointScript>().SetString("SzTarget", Briol);
			GameObject briol = GameObject.Find("Briol");
			NPCScript bNpc = briol.GetComponent<NPCScript>();
			bNpc.m_bIsMoving = true;
			bNpc.m_bActive = true;
			bNpc.m_nFacingDir = (int)NPCScript.FACINGDIR.eUP;
			bNpc.ResetAnimFlagsExcept(-1);
			bNpc.m_aAnim.SetInteger("m_nFacingDir", (int)NPCScript.FACINGDIR.eLEFT);
			GameObject.Find("XWaypoint").GetComponent<BoxCollider2D>().enabled = false;
		}
		break;
	}
}
