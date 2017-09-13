using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class InonEventHandler : BaseEventSystemScript
{
    DCScript ds;

    public GameObject[] Phase1_waypoints;//waypoints to keep the player stuck in his room
    public GameObject[] Phase2_waypoints;//waypoint to keep the player from going to the ritual until they go talk to the guy in the guild hall and the blacksmith
    public GameObject[] Phase3_waypoints;//waypoints inside of the ritual event
    public GameObject[] Phase4_waypoints;//waypoints inside ritual after player returns from fight
    public GameObject[] Phase5_waypoints;//waypoints still in town even after the ritual
    public GameObject m_goBoar;
    public GameObject m_goDeadBoar;
    public Sprite m_t2dDeadBoarWithoutTusk;
    public GameObject m_goForestLine;

    bool m_bUpDir = false, m_bDownDir = false, m_bLeftDir = false, m_bRightDir = false;


    //Name of background music to play during this scene
    string m_szBackgroundMusicName = "Inon_BGM";

    // Use this for initialization
    void Start()
    {
        ds = GameObject.Find("PersistantData").GetComponent<DCScript>();
        GameObject _goAudioHelper = GameObject.Find("AudioHelper");
        _goAudioHelper.GetComponent<CAudioHelper>().vPlayMusic(_goAudioHelper.GetComponent<CAudioHelper>().eFromName(m_szBackgroundMusicName), true, true);

        UpdateWaypoints();
    }

    void UpdateWaypoints()
    {
        int result;
        if (ds.m_dStoryFlagField.TryGetValue("Inon_KeyEvents", out result) == false)
        {
            //This is the introduction, play it yo!
            GameObject player = GameObject.Find("Player");
            player.GetComponent<FieldPlayerMovementScript>().BindInput();
            player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
            player.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
            ds.m_dStoryFlagField.Add("Inon_KeyEvents", 0);
            foreach (GameObject wpnt in Phase1_waypoints)
                wpnt.GetComponent<BoxCollider2D>().enabled = true;
            foreach (GameObject wpnt in Phase2_waypoints)
                wpnt.GetComponent<BoxCollider2D>().enabled = true;
            foreach (GameObject wpnt in Phase3_waypoints)
                wpnt.GetComponent<BoxCollider2D>().enabled = true;
            foreach (GameObject wpnt in Phase4_waypoints)
                wpnt.GetComponent<BoxCollider2D>().enabled = true;
            foreach (GameObject wpnt in Phase5_waypoints)
                wpnt.GetComponent<BoxCollider2D>().enabled = true;
            GameObject.Find("Briol").GetComponent<SpriteRenderer>().enabled = true;
            GameObject.Find("Mattach").GetComponent<SpriteRenderer>().enabled = true;
            GameObject.Find("NoteFromFamily").GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            switch (result)
            {
                //0: Player has started the game, but hasn't yet even talked to the guy in the guild hall.
                case 0:
                    {
                        //Haven't started the ritual yet.
                        foreach (GameObject wpnt in Phase1_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = false;
                        foreach (GameObject wpnt in Phase2_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = true;
                        foreach (GameObject wpnt in Phase3_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = true;
                        foreach (GameObject wpnt in Phase4_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = true;
                        foreach (GameObject wpnt in Phase5_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = true;
                        GameObject.Find("Briol").GetComponent<SpriteRenderer>().enabled = true;
                        GameObject.Find("Mattach").GetComponent<SpriteRenderer>().enabled = true;
                    }
                    break;
                //1: Player has talked to the guy in the guild hall, but not the blacksmith
                //2: Player has talked to the guy in the guild hall, the blacksmith, but not met with his dad/sister
                case 2:
                    {
                        foreach (GameObject wpnt in Phase1_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = false;
                        foreach (GameObject wpnt in Phase2_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = false;
                        foreach (GameObject wpnt in Phase3_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = true;
                        foreach (GameObject wpnt in Phase4_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = true;
                        foreach (GameObject wpnt in Phase5_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = true;
                        GameObject.Find("Briol").GetComponent<SpriteRenderer>().enabled = true;
                        GameObject.Find("Mattach").GetComponent<SpriteRenderer>().enabled = true;
                    }
                    break;
                //3: Player has met with the dad and sister, but not completed the ritual yet.
                case 3:
                    {
                        GameObject player = GameObject.Find("Player");
                        player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
                        player.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 0);
                        player.GetComponent<FieldPlayerMovementScript>().BindInput();
                        GameObject.Find("Mattach").GetComponentInChildren<MessageHandler>().BeginDialogue("B1");
                        ds.m_dStoryFlagField.Remove("Inon_RitualBattleComplete");
                        foreach (GameObject wpnt in Phase1_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = false;
                        foreach (GameObject wpnt in Phase2_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = false;
                        foreach (GameObject wpnt in Phase3_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = false;
                        foreach (GameObject wpnt in Phase4_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = true;
                        foreach (GameObject wpnt in Phase5_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = true;
                        GameObject.Find("Briol").GetComponent<SpriteRenderer>().enabled = true;
                        GameObject.Find("Mattach").GetComponent<SpriteRenderer>().enabled = true;
                        m_goDeadBoar.SetActive(true);
                    }
                    break;
                //4: Player has completed the initial ritual event and is free to leave town.
                case 4:
                    {
                        //The player has finished all of the intro events in Inon.
                        foreach (GameObject wpnt in Phase1_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = false;
                        foreach (GameObject wpnt in Phase2_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = false;
                        foreach (GameObject wpnt in Phase3_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = false;
                        foreach (GameObject wpnt in Phase4_waypoints)
                            wpnt.GetComponent<BoxCollider2D>().enabled = false;
                        GameObject.Find("Briol").GetComponent<SpriteRenderer>().enabled = false;
                        GameObject.Find("Mattach").GetComponent<SpriteRenderer>().enabled = true;
                    }
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int result;
        if (ds.m_dStoryFlagField.TryGetValue("Inon_HasMoved", out result) == false)
        {
            //player hasn't yet moved in all of the cardinal directions
            GameObject player = GameObject.Find("Player");
            if (player.GetComponent<FieldPlayerMovementScript>().GetAllowInput() == true)
            {
				if (Input.GetKey (KeyCode.UpArrow) && m_bUpDir == false) 
				{
					if(player.GetComponent<FieldPlayerMovementScript>().m_nFacingDir == 3)
						m_bUpDir = true;
				}
				else if (Input.GetKey (KeyCode.DownArrow) && m_bDownDir == false) 
				{
					if(player.GetComponent<FieldPlayerMovementScript>().m_nFacingDir == 0)
						m_bDownDir = true;
				}
				else if (Input.GetKey (KeyCode.LeftArrow) && m_bLeftDir == false) 
				{
					if(player.GetComponent<FieldPlayerMovementScript>().m_nFacingDir == 1)
						m_bLeftDir = true;
				}
				else if (Input.GetKey (KeyCode.RightArrow) && m_bRightDir == false) 
				{
					if(player.GetComponent<FieldPlayerMovementScript>().m_nFacingDir == 2)
						m_bRightDir = true;
				}
                else if(m_bUpDir == true && m_bRightDir == true && m_bLeftDir == true && m_bDownDir == true)
                {
                    //player has moved in all of the directions
                    ds.m_dStoryFlagField.Add("Inon_HasMoved", 1);
                    player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
                    player.GetComponent<FieldPlayerMovementScript>().BindInput();
                    player.GetComponentInChildren<MessageHandler>().BeginDialogue("B1");
                    GameObject.Find("NoteFromFamily").GetComponent<BoxCollider2D>().enabled = true;
                }
            }
        }
    }

	override public void HandleEvent(string eventID)
    {
        switch (eventID)
        {
            case "Callan_Movement":
                {
                    //For when callan can begin moving
                    GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
                }
                break;
            case "NoteInterractedWith":
                {
                    //Callan is now looking at the note
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player)
                    {
                        player.GetComponent<FieldPlayerMovementScript>().BindInput();
                        player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
                        player.GetComponentInChildren<MessageHandler>().BeginDialogue("D1");
                        GameObject.Find("NoteFromFamily").GetComponent<BoxCollider2D>().enabled = false;
                    }
                }
                break;
            case "ReadNote":
                {
                    //Callan has read the note from his family and can now leave the room.
                    GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
                    foreach (GameObject go in Phase1_waypoints)
                        go.SetActive(false);
                    GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
                    GameObject.Find("Player").GetComponentInChildren<MessageHandler>().BeginDialogue("C1");

                }
                break;
            case "Cytheria":
                {
                    GameObject messageSystem = GameObject.Find("Cytheria");
                    if (messageSystem)
                    {
                        int cythRes = -1;
                        if (ds.m_dStoryFlagField.TryGetValue("Inon_Cytheria", out cythRes) == false)
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                        else
                        {
                            if (cythRes == 1)
                                messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("B0");
                            else if (cythRes == 2)
                                messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("C0");
                        }
                    }
                }
                break;
            case "Cytheria_EndDialogue1":
                {
                    //Player told Cytheria to tell the boy she likes how she feels
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player)
                    {
                        player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
                    }
                    ds.m_dStoryFlagField.Add("Inon_Cytheria", 1);
                }
                break;
            case "Cytheria_EndDialogue2":
                {
                    //Player told Cytheria to keep her feelings hidden from the boy she likes.
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player)
                    {
                        player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
                    }
                    ds.m_dStoryFlagField.Add("Inon_Cytheria", 2);
                }
                break;
            case "Delaria":
                {
                    GameObject messageSystem = GameObject.Find("Delaria");
                    if (messageSystem)
                    {
                        int delRes = -1;
                        if (ds.m_dStoryFlagField.TryGetValue("Inon_Delaria", out delRes) == false)
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                        else
                        {
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("B0");
                        }
                    }
                }
                break;
            case "Delaria_EndDialogue":
                {
                    //Player has finished his first conversation with Delaria, mark it so she never says the same thing again
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player)
                    {
                        player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
                    }
                    ds.m_dStoryFlagField.Add("Inon_Delaria", 1);
                }
                break;
            case "Timmy":
                {
                    GameObject messageSystem = GameObject.Find("Timmy");
                    if (messageSystem)
                    {
                        int timRes = -1;
                        if (ds.m_dStoryFlagField.TryGetValue("Inon_Timmy", out timRes) == false)
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                        else
                        {
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("A3");
                        }
                    }
                }
                break;
            case "Matthew":
                {
                    GameObject messageSystem = GameObject.Find("Matthew");
                    if (messageSystem)
                    {

                        int mattRes = -1;
                        if (ds.m_dStoryFlagField.TryGetValue("Inon_CeremonyComplete", out mattRes) == false)
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                        else
                        {
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("A3");
                        }
                    }
                }
                break;
            case "MockWarBattle":
                {
                    //So, this is just a sample of the war battle, and will probably need to be removed for the actual game (if it hasn't been removed before that, lol)
                    List<FightSceneControllerScript.cWarUnit> _lUnits = ds.GetWarUnits();
                    ds.m_szWarBattleDataPath = "Test_Map";
                    if (_lUnits.Count <= 0)
                    {
                        //So, if we're in here, the player has never done a war battle, so, give him some default units to play with.
                        GameObject _Callan = Resources.Load<GameObject>("Units/WarUnits/" + "Callan");
                        GameObject _Soldier = Resources.Load<GameObject>("Units/WarUnits/" + "Human_Soldier");
                        GameObject _Archer = Resources.Load<GameObject>("Units/WarUnits/" + "Human_Archer");
                        _lUnits.Add(_Callan.GetComponent<TRPG_UnitScript>().m_wuUnitData);
                        _lUnits.Add(_Soldier.GetComponent<TRPG_UnitScript>().m_wuUnitData);
                        _lUnits.Add(_Archer.GetComponent<TRPG_UnitScript>().m_wuUnitData);
                        
                    }
                    else
                    {
                        //So, the player has said yes before.. not sure if we need to do anything here...
                        Debug.Log("In here");
                    }
                    ds.SetPreviousFieldName(SceneManager.GetActiveScene().name);
                    SceneManager.LoadScene("War_Scene");
                }
                break;

            case "Marcus":
                {
                    GameObject messageSystem = GameObject.Find("Marcus");
                    if (messageSystem)
                    {
                        messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                    }
                }

                break;
            case "Bedrest":
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player)
                    {
                        player.GetComponent<FieldPlayerMovementScript>().BindInput();
                    }
                    GameObject messageSystem = GameObject.Find("Bedrest");
                    if (messageSystem)
                    {
                        messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                    }
                }

                break;
            case "Briar":
                {
                    GameObject messageSystem = GameObject.Find("Briar");
                    if (messageSystem)
                    {
                        int briRes = -1;
                        if (ds.m_dStoryFlagField.TryGetValue("Inon_KeyEvents", out briRes))
                        {
                            switch (briRes)
                            {
                                //0: Player has started the game, but hasn't yet even talked to the guy in the guild hall.
                                case 0:
                                    {
                                        messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                                    }
                                    break;
                                //1: Player has talked to the guy in the guild hall, but not the blacksmith
                                case 1:
                                    {
                                        messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("B0");
                                    }
                                    break;
                                //2: Player has talked to the guy in the guild hall, the blacksmith, but not met with his dad/sister
                                default:
                                    {
                                        messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("C0");
                                    }
                                    break;
                            }
                        }
                    }
                }
                break;
            //This is after talking to the blacksmith and getting directions to where your father/sister are for the ritual
            case "Briar_EndDialogue":
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player)
                    {
                        player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
                    }
                    ds.m_dStoryFlagField["Inon_KeyEvents"] = 2;
                    UpdateWaypoints();
                }
                break;
            case "OldTuck":
                {
                    GameObject messageSystem = GameObject.Find("Old Tuck");
                    if (messageSystem)
                    {

                        int oldRes = -1;
                        if (ds.m_dStoryFlagField.TryGetValue("Inon_CeremonyComplete", out oldRes) == false)
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                        else
                        {
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("A3");
                        }
                    }
                }
                break;
            case "Cassandra":
                {
                    GameObject messageSystem = GameObject.Find("Cassandra");
                    if (messageSystem)
                    {
                        int cassRes = -1;
                        if (ds.m_dStoryFlagField.TryGetValue("Inon_Cassandra", out cassRes) == false)
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                        else
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("A5");
                    }
                }
                break;
            case "Cassandra_EndDialogue":
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player)
                    {
                        player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
                    }
                    ds.m_dStoryFlagField.Add("Inon_Cassandra", 1);
                }
                break;
            case "Lydia":
                {
                    GameObject messageSystem = GameObject.Find("Lydia");
                    if (messageSystem)
                    {
                        int lydRes = -1;
                        if (ds.m_dStoryFlagField.TryGetValue("Inon_Lydia", out lydRes))
                        {
                            if (lydRes == 12)
                            {
                                messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("E1");
                            }
                            else if (lydRes == 11)
                            {
                                //TODO: check player inventory for mushrooms
                                ItemLibrary.CharactersItems mushroom = ds.m_lItemLibrary.GetItemFromInventory("Rare Mushroom");
                                if (mushroom != null)
                                {
                                    if (mushroom.m_nItemCount >= 5)
                                    {
                                        //has gotten all of the items.
                                        ds.m_lItemLibrary.RemoveItemAll(mushroom);
                                        ds.m_dStoryFlagField["Inon_Lydia"] = 12;
                                        messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("D0");
                                    }
                                    else
                                        messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("C0");
                                }
                                else
                                    messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("C0");
                            }
                            else if (lydRes == 10)
                            {
                                lydRes++;
                                ds.m_dStoryFlagField["Inon_Lydia"] = lydRes;
                                messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("B0");
                            }
                            else if (lydRes < 10)
                            {
                                lydRes++;
                                ds.m_dStoryFlagField["Inon_Lydia"] = lydRes;
                                messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                            }
                        }
                        else
                        {
                            ds.m_dStoryFlagField.Add("Inon_Lydia", 1);
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                        }

                    }
                }
                break;
            case "Lydia_EndDialogue":
                {
                    //turn off all dialogues happening, release bind on input.. umn.. i think that's it?
                    GameObject[] gObjs = GameObject.FindObjectsOfType<GameObject>();
                    foreach (GameObject g in gObjs)
                    {
                        if (g.GetComponent<MessageHandler>() != null)
                        {
                            if (g.GetComponent<NPCScript>() != null)
                                g.GetComponent<NPCScript>().m_bIsBeingInterractedWith = false;
                            g.GetComponentInChildren<MessageHandler>().m_bShouldDisplayDialogue = false;
                        }
                    }
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player)
                    {
                        player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
                    }
                    ds.m_nGold += 100;
                    GameObject.Find("UI_Alerts").GetComponent<UIAlertWindowScript>().ActivateWindow(UIAlertWindowScript.MESSAGEID.eITEMREWARD, "100 Spyr", null);
                }
                break;
            case "Bartholomew":
                {
                    GameObject messageSystem = GameObject.Find("Bartholomew");
                    if (messageSystem)
                    {

                        int bartRes = -1;
                        if (ds.m_dStoryFlagField.TryGetValue("Inon_KeyEvents", out bartRes))
                        {
                            if (bartRes == 0)
                            {
                                messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                            }
                            else
                            {
                                messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("A4");
                            }
                        }
                    }
                }
                break;
            case "Bartholomew_EndDialogue":
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player)
                    {
                        player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
                    }
                    ds.m_dStoryFlagField["Inon_KeyEvents"] = 1;
                }
                break;
            case "Constantinople":
                {
                    GameObject messageSystem = GameObject.Find("Constantinople");
                    if (messageSystem)
                    {

                        int constRes = -1;
                        if (ds.m_dStoryFlagField.TryGetValue("Inon_CeremonyComplete", out constRes) == false)
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                        else
                        {
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("A3");
                        }
                    }
                }
                break;
            case "Mattach":
                {
                    GameObject messageSystem = GameObject.Find("Mattach");
                    if (messageSystem)
                    {

                        int mattachResult = -1;
                        if (ds.m_dStoryFlagField.TryGetValue("Inon_KeyEvents", out mattachResult) == false)
                        {

                        }
                        else
                        {
                            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue("E1");
                        }
                    }
                }
                break;
            case "NPC_Dancer":
                {
                    GameObject messageSystem = GameObject.Find("NPC_Dancer");
                    if (messageSystem)
                    {
                        messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                    }
                }
                break;
            case "Inon_Merchant1":
                {
                    GameObject messageSystem = GameObject.Find("NPC_Merchant1");
                    if (messageSystem)
                    {
                        messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(0);
                    }
                }

                break;
            case "End_Marcus":
                {
                    GameObject[] gObjs = GameObject.FindObjectsOfType<GameObject>();
                    foreach (GameObject g in gObjs)
                    {
                        if (g.GetComponentInChildren<MessageHandler>() != null)
                        {
                            g.GetComponentInChildren<MessageHandler>().m_bShouldDisplayDialogue = false;
                        }
                    }
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player)
                    {
                        player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
                    }
                }
                break;
            case "Merchant_EndDialogue":
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player)
                    {
                        player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
                    }
                    GameObject[] gObjs = GameObject.FindGameObjectsWithTag("Merchant");
                    foreach (GameObject g in gObjs)
                    {
                        g.GetComponent<NPC_ArmorMerchantScript>().ActivateMerchantScreen();
                    }

                }
                break;
            case "InnKeeper_Sleep":
                {
                    GameObject[] keepers = GameObject.FindGameObjectsWithTag("InnKeeper");
                    foreach (GameObject keeper in keepers)
                    {
                        if (keeper.GetComponent<NPCScript>().m_bIsBeingInterractedWith == true)
                        {
                            //found the game object that is the innkeeper, check if the player can afford it, if not.. ?   if you can, go to sleep after deducting the cost
                            if (ds.m_nGold - keeper.GetComponent<NPCScript>().m_nCost >= 0)
                            {
                                ds.m_nGold = ds.m_nGold - keeper.GetComponent<NPCScript>().m_nCost;
                                GameObject.Find("Inn Keeper").GetComponent<InnKeeperScript>().BeginFade();
                            }
                            else
                            {
                                HandleEvent("EndDialogue");
                            }
                        }
                    }

                }
                break;

            case "ItemShoppe":
                {
                    GameObject[] gObjs = GameObject.FindObjectsOfType<GameObject>();
                    foreach (GameObject g in gObjs)
                    {
                        if (g.GetComponentInChildren<MessageHandler>() != null)
                        {
                            g.GetComponentInChildren<MessageHandler>().m_bShouldDisplayDialogue = false;
                        }
                    }
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player)
                    {
                        player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
                    }
                }
                break;
            case "Inon_Blacksmith1":
                {
                    GameObject blackSmith = GameObject.Find("Briar");
                    blackSmith.GetComponent<NPC_BlacksmithScript>().ActivateScreen();

                }
                break;
            case "BoarTutorial":
                {
					GameObject player = GameObject.FindGameObjectWithTag("Player");
					if (player)
					{
						player.GetComponent<FieldPlayerMovementScript> ().DHF_StopMovingFaceDirection (2);
					}
                    m_goForestLine.GetComponent<EdgeCollider2D>().enabled = false;
                    m_goBoar.SetActive(true);
                    m_goBoar.GetComponent<BoarRitualScript>().ActivateBoar();

                }
                break;
            case "StartBoarBattle":
                {
                    //battles stuff
                    ds.m_dStoryFlagField["Inon_KeyEvents"] = 3;
                    ds.m_dStoryFlagField.Add("Battle_ReadMessage", 1);
                    StartBossBattle();
                }
                break;
            case "RetrieveTusks":
                {
                    GameObject.Find("Briol").GetComponent<NPCScript>().DHF_NPCMoveToGameobject(Phase4_waypoints[0], false, 0, false);
                }
                break;
            case "BriolArriveAtRitual":
                {
                    //This event is for Briol having arrived next to the boar during the first ritual event to retrieve the boar tusk.
                    foreach (GameObject wpnt in Phase4_waypoints)
                        wpnt.GetComponent<BoxCollider2D>().enabled = false;
                    m_goDeadBoar.GetComponent<SpriteRenderer>().sprite = m_t2dDeadBoarWithoutTusk;
                    GameObject.Find("UI_Alerts").GetComponent<UIAlertWindowScript>().ActivateWindow(UIAlertWindowScript.MESSAGEID.eITEMREWARD, "1 Boar Tusk", gameObject);

                }
                break;
            case "RITUALEVENT_ObtainedBoarTusk":
                {
                    //Player has acquired the boar tusk, now have Briol move into the player to join the team.
                    ds.m_dStoryFlagField["Inon_KeyEvents"] = 4;
                    m_goDeadBoar.SetActive(false);
                    GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
                    GameObject.Find("Briol").GetComponent<NPCScript>().DHF_NPCMoveIntoPlayer();
                    GameObject.Find("Briol").GetComponent<Collider2D>().enabled = false;
                    Invoke("RecruitBriol", 2.0f);
                }
                break;
            case "RitualEnd":
                {

                    UpdateWaypoints();
                    GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseAllBinds();
                }
                break;
			default:
				{
					base.HandleEvent (eventID);
				}
				break;
        }
    }

    void MessageWindowDeactivated()
    {
        int result = 0;
        if (ds.m_dStoryFlagField.TryGetValue("Inon_KeyEvents", out result))
        {
            if (result == 3)
            {
                HandleEvent("RITUALEVENT_ObtainedBoarTusk");
            }
            else if (result == 4)
            {
                //Briol has now moved into the player and they've seen the window for her having been recruited.
                GameObject.Find("Mattach").GetComponentInChildren<MessageHandler>().BeginDialogue("D0");
            }
        }
    }

    void RecruitBriol()
    {
        ds.AddPartyMember("Briol");
        GameObject.Find("UI_Alerts").GetComponent<UIAlertWindowScript>().ActivateWindow(UIAlertWindowScript.MESSAGEID.eRECRUITED, "Briol", gameObject);
    }

    void BeginDialogue(int iter)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            player.GetComponent<FieldPlayerMovementScript>().BindInput();
        }
        GameObject messageSystem = GameObject.Find("Female Dancer");
        if (messageSystem)
        {
            messageSystem.GetComponentInChildren<MessageHandler>().BeginDialogue(iter);
        }
    }

    override public void WaypointTriggered(Collider2D c)
    {
		if (c.name.Contains ("StepBack"))
		{
			GameObject player = GameObject.Find ("Player");
			player.GetComponent<FieldPlayerMovementScript> ().SetState ((int)FieldPlayerMovementScript.States.eIDLE);
			player.GetComponent<FieldPlayerMovementScript> ().DHF_StopMovingFaceDirection (player.GetComponent<FieldPlayerMovementScript> ().m_nFacingDir);
			int result;
			if (ds.m_dStoryFlagField.TryGetValue ("Inon_KeyEvents", out result))
			{
				bool _found = false;
				switch (result)
				{
					//0: Player has started the game, but hasn't yet even talked to the guy in the guild hall.
					case 0:
						{
							//Okay it's possible that you're still in the main room still.. so check first to see if you've moved in each direction
							if (ds.m_dStoryFlagField.TryGetValue ("Inon_HasMoved", out result) == false || GameObject.Find ("NoteFromFamily"))
							{
								GameObject.Find ("Player").GetComponentInChildren<MessageHandler> ().BeginDialogue ("F0");
								_found = true;

							}
							else
							{
								GameObject.Find ("Player").GetComponentInChildren<MessageHandler> ().BeginDialogue ("F1");
								_found = true;
							}
						}
						break;
						//1: Player has talked to the guy in the guild hall, but not the blacksmith
					case 1:
						{
							GameObject.Find ("Player").GetComponentInChildren<MessageHandler> ().BeginDialogue ("F2");
							_found = true;
						}
						break;
						//2: Player has talked to the guy in the guild hall, the blacksmith, but not met with his dad/sister
					case 2:
						{
							GameObject.Find ("Player").GetComponentInChildren<MessageHandler> ().BeginDialogue ("F3");
							_found = true;
						}
						break;
					default:
						{
							Debug.Log ("hit");
							GameObject.Find ("Player").GetComponentInChildren<MessageHandler> ().BeginDialogue ("I can't go this way yet!", "Callan", 1);
							_found = true;
						}
						break;
				}

				if (_found == true)
					c.enabled = false;
			}
		}

        switch (c.name)
        {
            case "IntoHallwayCheck":
                {
                    //The player needs to stay in this room, disable input and have him begin walking downwards, will get caught by child waypoint to release input and stop movement.
                    GameObject player = GameObject.Find("Player");
                    if (player)
                    {
                        player.GetComponent<FieldPlayerMovementScript>().BindInput();
                        player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eWALKDOWN);
                        player.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bMoveDown", true);
                        player.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetBool("m_bRunButtonIsPressed", false);
                        player.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", 0);
                        player.GetComponent<FieldPlayerMovementScript>().SetIsRunning(false);
                        GameObject.Find("StepBackWaypoint").GetComponent<BoxCollider2D>().enabled = true;
                    }
                }
                break;
            case "TowardRitualCheck":
                {
                    //Stops the player from going up to the ritual site before doing the previous events.
                    GameObject player = GameObject.Find("Player");
                    if (player)
                    {
						GameObject _wypnt = GameObject.Find("StepBackPoint");
						_wypnt.GetComponent<Collider2D> ().enabled = true;
                        player.GetComponent<FieldPlayerMovementScript>().BindInput();
						player.GetComponent<FieldPlayerMovementScript> ().DHF_PlayerMoveToGameObject (_wypnt, false, 3);
                    }
                }
                break;
            case "StartArriveAtRitual":
                {

                    //player has gotten close enough to the ritual for us to take over, first we need to make sure that the player is in the right x alignment, move toward that waypoint depending on the direction
                    GameObject.Find("StartArriveAtRitual").GetComponent<BoxCollider2D>().enabled = false;
                    GameObject src = GameObject.Find("Player");
                    src.GetComponent<FieldPlayerMovementScript>().BindInput();
					GameObject _wypnt = GameObject.Find ("ArrivedAtRitualWaypoint");
					src.GetComponent<FieldPlayerMovementScript> ().DHF_PlayerMoveToGameObject (_wypnt, false, 3);
					_wypnt.GetComponent<BoxCollider2D>().enabled = true;
                }
                break;
            case "ArrivedAtRitualWaypoint":
                {
                    GameObject player = GameObject.Find("Player");
                    player.GetComponent<FieldPlayerMovementScript>().SetState((int)FieldPlayerMovementScript.States.eIDLE);
                    player.GetComponent<FieldPlayerMovementScript>().ResetAnimFlagsExcept(-1);
                    GameObject.Find("Mattach").GetComponentInChildren<MessageHandler>().BeginDialogue("A1");
                    GameObject.Find("ArrivedAtRitualWaypoint").GetComponent<BoxCollider2D>().enabled = false;
                }
                break;
            case "BoarArriveAtRitual":
                {
                    m_goBoar.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    m_goBoar.GetComponent<Animator>().SetBool("m_bAttack", true);
                }
                break;
            case "LeaveFromSouthWaypoint":
                {
                    GameObject src = GameObject.Find("Player");
                    src.GetComponent<FieldPlayerMovementScript>().BindInput();
                    src.GetComponent<FieldPlayerMovementScript>().DHF_PlayerMoveToGameObject(GameObject.Find("StepBackSouth"), false);
                    GameObject.Find("StepBackSouth").GetComponent<BoxCollider2D>().enabled = true;

                }
                break;
            case "LeaveFromNorthWaypoint":
                {
                    GameObject src = GameObject.Find("Player");
                    src.GetComponent<FieldPlayerMovementScript>().BindInput();
                    src.GetComponent<FieldPlayerMovementScript>().DHF_PlayerMoveToGameObject(GameObject.Find("StepBackNorth"), false);
                    GameObject.Find("StepBackNorth").GetComponent<BoxCollider2D>().enabled = true;
                }
                break;

            default:
                break;
        }
    }

    void StartBossBattle()
    {
        GameObject dc = GameObject.Find("PersistantData");
        if (dc)
        {
            List<EncounterGroupLoaderScript.cEnemyData> bossEncounter = new List<EncounterGroupLoaderScript.cEnemyData>();
            EncounterGroupLoaderScript.cEnemyData enemy = new EncounterGroupLoaderScript.cEnemyData();
            enemy.m_szEnemyName = "Boar";
            enemy.m_nFormationIter = 4;
            bossEncounter.Add(enemy);

            //Set the names of the list of enemies the player is about to fight
            dc.GetComponent<DCScript>().SetEnemyNames(bossEncounter);
            //Set the position of the player before the battle starts
            GameObject go = GameObject.Find("PersistantData");
            GameObject m_goPlayer = GameObject.Find("Player");
            go.GetComponent<DCScript>().SetPreviousPosition(m_goPlayer.transform.position);
            go.GetComponent<DCScript>().SetPreviousFacingDirection(m_goPlayer.GetComponent<FieldPlayerMovementScript>().m_nFacingDir);
            go.GetComponent<DCScript>().SetPreviousFieldName(SceneManager.GetActiveScene().name);
            go.GetComponent<DCScript>().SetBattleFieldBackgroundIter(1);

            GameObject Briol = Resources.Load<GameObject>("Units/Ally/Briol/Briol");
            Briol.GetComponent<CAllyBattleScript>().SetUnitStats();

            SceneManager.LoadScene("Battle_Scene");
        }
    }

}