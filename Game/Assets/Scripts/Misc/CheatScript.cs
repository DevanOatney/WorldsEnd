using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CheatScript : MonoBehaviour 
{
	bool m_bShowCheatLog = false;
	string m_szCheatString = string.Empty;
	List<string> m_lChatLog = new List<string>();
	Vector3 m_vScrollPos = Vector2.zero;
	bool m_bEnterPressed = false;
	bool m_bEscapePressed = false;
	//List of all of the prefaced cheat codes
	List<string> m_lCheatEntries = new List<string>();
	bool m_bAbleToFindCommand = false;

	DCScript dc;
	// Use this for initialization
	void Start () 
	{
		dc = GameObject.Find("PersistantData").GetComponent<DCScript>();
		m_lCheatEntries.Add("event.");
		m_lCheatEntries.Add("player.");
		m_lCheatEntries.Add ("party.");
		m_lCheatEntries.Add ("recruit.");
		m_lCheatEntries.Add ("base.");
		m_lCheatEntries.Add("nocombat");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.BackQuote))
		{
			GameObject player = GameObject.Find("Player");
			if(player.GetComponent<FieldPlayerMovementScript>().GetAllowInput() == true)
			{
				m_bShowCheatLog = true;
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
			}
		}
	}

	void ParseCheatString(string _cheatCommand)
	{
		if(_cheatCommand.Length <= 4)
		{
			m_lChatLog.Add("Unable to find command " + _cheatCommand);
			_cheatCommand = string.Empty;
		}
		else
		{
			//User actually typed something into the cheat log, parse it and see if it's an identifiable cheat code.
			foreach(string command in m_lCheatEntries)
			{
				if(_cheatCommand.Contains(command))
				{

					string[] pieces = _cheatCommand.Split('.');
					switch(pieces[0].Trim())
					{
						case "event":
							{
								switch(pieces[1].Trim())
								{
									case "add":
										{
											//Add an event to the list of events
											if(pieces.Length == 4)
											{
												int result = 0;
												if(dc.m_dStoryFlagField.TryGetValue(pieces[2].Trim(), out result))
												{
													if(result == int.Parse(pieces[3].Trim()))
													{
														m_lChatLog.Add("Cannot add event, event already in list.");
														m_bAbleToFindCommand = true;
													}
													else
													{
														//The ID differs from the one in the list, adjust it
														m_lChatLog.Add("Added event: " + pieces[2].Trim() + " with ID: " + pieces[3].Trim());
														dc.m_dStoryFlagField.Remove(pieces[2].Trim());
														dc.m_dStoryFlagField.Add(pieces[2].Trim(), int.Parse(pieces[3].Trim()));
														//reset the waypoints in the current eventsystem to handle this change.
														GameObject.Find("Event system").GetComponent<BaseEventSystemScript>().SetWaypoints();
														m_bAbleToFindCommand = true;
													}
												}
												else
												{
													//this event is not in the list, add it and the ID
													m_lChatLog.Add("Added event: " + pieces[2].Trim() + " with ID: " + pieces[3].Trim());
													dc.m_dStoryFlagField.Add(pieces[2].Trim(), int.Parse(pieces[3].Trim()));
													m_bAbleToFindCommand = true;
												}
											}
										}
										break;
								}

							}
							break;
						case "player":
							{
								switch(pieces[1].Trim())
								{
									case "move":
										{
											//need one more command at least, or you know it's invalid.
											if(pieces.Length == 3)
											{
												GameObject targetLocation = GameObject.Find(pieces[2].Trim());
												if(targetLocation != null)
												{
													//we found the game object to move to, so move there!
													GameObject.Find("Player").transform.position = targetLocation.transform.position;
													m_lChatLog.Add("Moving to " + targetLocation.name + "'s location.");
													m_bAbleToFindCommand = true;
												}
											}
										}
										break;
									case"additem":
										{
											//needs two commands, or it's invalid.
											if(pieces.Length == 4)
											{
												//make sure this is a valid item.
												ItemLibrary.ItemData item = dc.m_lItemLibrary.GetItemFromDictionary(pieces[2].Trim());
												if(item != null)
												{
													int amount;
													if(int.TryParse(pieces[3].Trim(), out amount))
													{
														ItemLibrary.CharactersItems characterItem = new ItemLibrary.CharactersItems(item.m_szItemName, item.m_nItemType, amount);
														dc.m_lItemLibrary.AddItem(characterItem);
														m_lChatLog.Add("Adding " + characterItem.m_nItemCount + " " + characterItem.m_szItemName + "  to inventory.");
														m_bAbleToFindCommand = true;
													}
												}
											}
										}
										break;

									case "stats":
										{
											switch (pieces [2].Trim ()) 
											{
												case "runspeed":
													{
														switch (pieces [3].Trim ()) 
														{
															case "double":
																{
																	GameObject.Find ("Player").GetComponent<FieldPlayerMovementScript> ().m_fRunningSpeed += GameObject.Find ("Player").GetComponent<FieldPlayerMovementScript> ().m_fRunningSpeed;
																	m_lChatLog.Add ("Doubling the player's running speed");
																	m_bAbleToFindCommand = true;
																}
																break;
															case "half":
																{
																	GameObject.Find ("Player").GetComponent<FieldPlayerMovementScript> ().m_fRunningSpeed -= GameObject.Find ("Player").GetComponent<FieldPlayerMovementScript> ().m_fRunningSpeed;
																	m_lChatLog.Add ("Halfing the player's running speed");
																	m_bAbleToFindCommand = true;
																}
																break;
														}

													}
													break;
											}
										}
										break;
								}
							}
							break;
						case "party":
							{
								string _secondPiece = pieces [1].Trim ();
								if (_secondPiece == "add")
								{
									string _recruitName = pieces [2].Trim ();
									DCScript.CharacterData _charData = dc.GetComponent<DCScript> ().GetRosteredCharacterData (_recruitName);
									if (_charData != null)
									{
										m_bAbleToFindCommand = true;
										dc.AddPartyMember (_charData);
										m_lChatLog.Add (_recruitName + " has been added into the party.");
									}
								}
								else
									if (_secondPiece == "remove")
									{

										string _recruitName = pieces [2].Trim ();
										DCScript.CharacterData _charData = dc.GetComponent<DCScript> ().GetRosteredCharacterData (_recruitName);
										if (_charData != null)
										{
											m_bAbleToFindCommand = true;
											dc.RemovePartyMember (_charData);
											m_lChatLog.Add (_recruitName + " has been removed from the party.");
										}
									}
							}
							break;
						case "recruit":
							{
								m_bAbleToFindCommand = true;
								string _recruitName = pieces [1].Trim ();
								//GameObject newRecruit = Resources.Load<GameObject> ("Units/Ally/" + _recruitName + "/" + _recruitName);
								DCScript.CharacterData _charData = dc.GetComponent<DCScript> ().GetRosteredCharacterData (_recruitName);
								if (_charData != null)
								{
									if (_charData.m_bHasBeenRecruited == false)
									{
										_charData.m_bHasBeenRecruited = true;
										m_lChatLog.Add (_recruitName + " has been recruited.");
									}
									else
									{
										m_lChatLog.Add (_recruitName + " has already been recruited.");
									}
								}
								else
								{
									m_lChatLog.Add (_recruitName + " wasn't found as an available character to recruit.");
								}

							}
							break;
						case "base":
							{
								string _nextPiece = pieces [1].Trim ();
								switch (_nextPiece)
								{
									case "blacksmith":
										{
											m_bAbleToFindCommand = true;
											int _level = int.Parse( pieces [2].Trim ());
											if (dc.GetComponent<DCScript> ().m_dBaseFlagField.ContainsKey ("BlacksmithLevel"))
											{
												dc.m_dBaseFlagField.Remove ("BlacksmithLevel");
												dc.m_dBaseFlagField.Add ("BlacksmithLevel", _level);
											}
											else
											{
												dc.m_dBaseFlagField.Add ("BlacksmithLevel", _level);
											}
											m_lChatLog.Add ("Changed Blacksmith tier to level " + _level);
											GameObject.Find ("Event system").GetComponent<BaseEventSystemScript> ().AdjustBuildings ();
										}
										break;
									case "resource":
										{
											m_bAbleToFindCommand = true;
											string _location = pieces [2].Trim ();
											dc.m_lFieldResourceLocationsFound.Add (_location);
											m_lChatLog.Add ("Added resource location : " + _location);
										}
										break;
								}

							}
							break;
						case "nocombat":
							{
								gameObject.GetComponent<EncounterGroupLoaderScript>().m_bEncountersHappen = false;
								m_bAbleToFindCommand = true;
								m_lChatLog.Add("Combat disabled");
							}
							break;
						default:
							m_lChatLog.Add("Unable to find command " + _cheatCommand);
							m_bAbleToFindCommand = true;
							break;
					}

				}

			}
			if(m_bAbleToFindCommand == false)
			{
				m_lChatLog.Add("Unable to find command " + _cheatCommand);
			}
		}
	}

	void OnGUI()
	{
		if(m_bShowCheatLog == true)
		{
			
			Event e = Event.current;
			if (e.keyCode == KeyCode.Return && e.type == EventType.KeyUp)
			{
				e.Use();
				if(m_bEnterPressed == false)
				{
					if (m_szCheatString == "Bundle.ResourceGathering")
					{
						ParseCheatString ("nocombat");
						ParseCheatString ("event.add.ToAEvent.5");
						ParseCheatString ("recruit.Domin");
						ParseCheatString ("base.resource.Inon Forest");
						ParseCheatString ("player.move.InfrontOfWorldMapTable");
					}
					else
					{
						ParseCheatString (m_szCheatString);
					}
					m_bEnterPressed = true;
				}
			}
			else
				m_bEnterPressed = false;
			if(e.keyCode == KeyCode.Escape && e.type == EventType.KeyUp)
			{
				if(m_bEscapePressed == false)
				{
					m_bShowCheatLog = false;
					GameObject player = GameObject.Find("Player");
					player.GetComponent<FieldPlayerMovementScript>().ReleaseBind();
					m_bEscapePressed = true;
				}
			}
			else
			{
				m_bEscapePressed = false;
			}
			GUI.SetNextControlName("Cheat Log");
			m_szCheatString = GUI.TextField(new Rect(10, Screen.height - Screen.height * .8f, 200, 20), m_szCheatString);
			GUI.FocusControl("Cheat Log");
				
			GUILayout.BeginVertical();
			m_vScrollPos = GUILayout.BeginScrollView(m_vScrollPos, GUILayout.MaxWidth(500), GUILayout.MinWidth(500),GUILayout.MaxHeight(100), GUILayout.MinHeight(100)); 
			
			foreach(string ent in m_lChatLog){
				GUILayout.BeginHorizontal();
				GUI.skin.label.wordWrap = true;
				GUILayout.Label(ent);
				GUILayout.EndHorizontal();
				GUILayout.Space(1);
			}
			
			GUILayout.EndScrollView();
			m_vScrollPos.y = float.PositiveInfinity;
			m_bAbleToFindCommand = false;
		}
	}
}
