using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MessageHandler : MonoBehaviour 
{
	public List<DialogueScriptLoaderScript.dlg> dialogueEvents;
	public bool m_bShouldDisplayDialogue = false;
	public int m_nCurrentDialogueIter = -1;
	private Vector3 pos;
	
	private int width = Screen.width;
	private int height = Screen.height;
	/// <summary>
	/// Text Stuff
	/// </summary>
	private float speed = 0.5f;
	private string line;
	private float timer = 0.0f;
	private int textIter = 0;
	///
	//for hero dialogue
	private int selectedIndex = 0;
	public Texture2D selectedTexture;
	float bufferedInputTimer = 0.0f;
	float bufferedInputBucket = 0.2f;
	
	//pointer to the event handler of the current scene, since events could have the same ID but are different per what scene is currently being played.
	public GameObject eventHandler;
	/****************************************************************/                                           
	// Use this for initialization
	void Start () 
	{
		DialogueScriptLoaderScript GO = gameObject.GetComponent<DialogueScriptLoaderScript>();
		if(GO)
		{
			dialogueEvents = GO.dlgList;
		}
		
		GameObject data = GameObject.Find("PersistantData");
		if(data)
		{
			speed = speed / ((Mathf.Pow(data.GetComponent<DCScript>().m_nTextSpeed, 2)+1)); 
		}
		else
			speed = 0.1f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//increment the buffered input timer for if users are holding down a button
		bufferedInputTimer += Time.deltaTime;
		
		
		//if dialogue is running.
		if(m_bShouldDisplayDialogue == true)
		{
			
			switch(dialogueEvents[m_nCurrentDialogueIter].SpecialCaseFlag)
			{
				case (int)DialogueScriptLoaderScript.DLGType.NORMAL:
				{
					if(Input.GetKeyDown(KeyCode.Return))
					{
						//if the current line is done
						if(line.Length >= ((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).Line.Length)
						{
							string textID = ((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).TextIDToGoTo.ToString();
							for(int i = 0; i < dialogueEvents.Count; ++i)
							{
								if(textID == dialogueEvents[i].TextID)
								{
									m_nCurrentDialogueIter = i;
									break;
								}
							}
							line = "";
							timer = 0.0f;
							textIter = 0; 
						}
						else
						{
							line = ((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).Line;
							textIter = line.Length;
						}
					}
				}
					break;
				case (int)DialogueScriptLoaderScript.DLGType.HERO:
				{
					if(Input.GetKey(KeyCode.DownArrow))
					{
						if(bufferedInputTimer >= bufferedInputBucket)
						{
							selectedIndex++;
							if(selectedIndex >= ((DialogueScriptLoaderScript.heroDlg)dialogueEvents[m_nCurrentDialogueIter]).NumberOfChoices)
								selectedIndex = 0;
							bufferedInputTimer = 0.0f;
						}
					}
					else if(Input.GetKey(KeyCode.UpArrow))
					{
						if(bufferedInputTimer >= bufferedInputBucket)
						{
							selectedIndex--;
							if(selectedIndex < 0)
								selectedIndex = ((DialogueScriptLoaderScript.heroDlg)dialogueEvents[m_nCurrentDialogueIter]).NumberOfChoices-1;
							bufferedInputTimer = 0.0f;
						}
					}
					else if(Input.GetKeyDown(KeyCode.Return))
					{
						string textID = ((DialogueScriptLoaderScript.heroDlg)dialogueEvents[m_nCurrentDialogueIter]).choices[selectedIndex].TextIDToGoTo;
						for(int i = 0; i < dialogueEvents.Count; ++i)
						{
							if(textID == dialogueEvents[i].TextID)
							{
								m_nCurrentDialogueIter = i;
								break;
							}
						}
						line = "";
					    timer = 0.0f;
						textIter = 0;
						selectedIndex = 0;
					}
				}
					break;
				case (int)DialogueScriptLoaderScript.DLGType.EVENT:
				{
					//Application.LoadLevel("Game_Introduction_Scene");
						
				}
					break;
					
			}
		}
	}
	void OnGUI()
	{
		if(m_bShouldDisplayDialogue == true)
		{
			//display the background box
			
			GUI.Box(new Rect(width/20,height/2, width, (height - height/50) -(height/2)), "");
			switch(dialogueEvents[m_nCurrentDialogueIter].SpecialCaseFlag)
			{
			case (int)DialogueScriptLoaderScript.DLGType.NORMAL:
			{
				timer += Time.deltaTime;
				//increment the text based on the players text speed
				if(timer >= speed)
				{
					if(textIter == 0)
					{
						string filePath = ((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).DialogueFilePath; 
						if(filePath != "null" && filePath.Length > 1)
						{
							AudioClip clip = (AudioClip)Resources.Load(filePath);
							if(clip)
							{
								GameObject GO = GameObject.Find("PersistantData");
								if(GO)
								{
									GetComponent<AudioSource>().Stop();
									GetComponent<AudioSource>().PlayOneShot(clip, 0.5f + GO.GetComponent<DCScript>().m_fVoiceVolume);
								}
							}
							
						}
					}
					if(textIter < ((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).Line.Length)
					{
						line += ((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).Line[textIter];
						textIter++;
					}
					timer = 0.0f;
				}
				Texture2D tBust;
				string szName = ((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).CharacterName;
				int bustID = ((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).BustID;
				if(GameObject.Find("Portraits Container").GetComponent<PortraitContainerScript>().m_dPortraits.TryGetValue(
																		szName+bustID.ToString() , out tBust))
				{
					//This dialogue has a portrait!! Draw things
					GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
					GUI.Box(new Rect(0, height/2-96, 96, 96), tBust);
					int catchFont = GUI.skin.box.fontSize;
					GUI.skin.box.fontSize = 20;
					GUI.Box(new Rect(105, height/2-28, 100, 28), szName);
					GUI.skin.box.fontSize = catchFont;
				}
				else
				{
					//no bust, just have their name?
					int catchFont = GUI.skin.box.fontSize;
					GUI.skin.box.fontSize = 20;
					GUI.Box(new Rect(2, height/2-28, 10 * szName.Length + 10, 28), szName);
					GUI.skin.box.fontSize = catchFont;
				}
				GUI.Label(new Rect(width/20,height/2, width - (width/20), (height - height/50) -(height/2)), line);
				
			}
				break;
			case (int)DialogueScriptLoaderScript.DLGType.HERO:
			{
				
				for(int i = 0; i < ((DialogueScriptLoaderScript.heroDlg)dialogueEvents[m_nCurrentDialogueIter]).NumberOfChoices; ++ i)
				{
					line = ((DialogueScriptLoaderScript.heroDlg)dialogueEvents[m_nCurrentDialogueIter]).choices[i].Line;
					GUI.Label(new Rect(width/20,height/2+ (15*i), width, (height - height/50) -(height/2)), line);
					line = ""; 
				}
				int lengthOfText = ((DialogueScriptLoaderScript.heroDlg)dialogueEvents[m_nCurrentDialogueIter]).choices[selectedIndex].Line.Length * 10;
				GUIStyle myStyle = new GUIStyle(GUI.skin.box);
				myStyle.normal.background = selectedTexture;
				//draw the selector box for the dialogue choice
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
				GUI.Box((new Rect(width/20-2, height/2 + (15*selectedIndex), lengthOfText, 17)), "",myStyle);

				string szName = "Callan";
				int catchFont = GUI.skin.box.fontSize;
				GUI.skin.box.fontSize = 20;
				GUI.Box(new Rect(0, height/2-28, 75, 28), szName);
				GUI.skin.box.fontSize = catchFont;

			}
				break;
			case (int)DialogueScriptLoaderScript.DLGType.EVENT:
			{
				m_bShouldDisplayDialogue = false;
				eventHandler.GetComponent<BaseEventSystemScript>().HandleEvent(((DialogueScriptLoaderScript.cncldDlg)dialogueEvents[m_nCurrentDialogueIter]).EventIDToGoTo);
			}
				break;
			}
			
		}
	}
	public void BeginDialogue(int iter)
	{
		m_bShouldDisplayDialogue = true;
		m_nCurrentDialogueIter = iter;
	}
	public void BeginDialogue(string id)
	{
		int c = 0;
		foreach(DialogueScriptLoaderScript.dlg dlg in dialogueEvents)
		{
			if(id == dlg.TextID)
			{
				m_bShouldDisplayDialogue = true;
				m_nCurrentDialogueIter = c;
			}
			c++;
		}
	}
	public void ChangeDialogueEvent(string EventToGoTo)
	{
		for(int i = 0; i < dialogueEvents.Count; ++i)
		{
			if(EventToGoTo == dialogueEvents[i].TextID)
			{
				m_nCurrentDialogueIter = i;
				break;
			}
		}
		line = "";
		timer = 0.0f;
		textIter = 0; 
	}
}
 