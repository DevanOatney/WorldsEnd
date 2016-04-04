using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MessageHandler : MonoBehaviour 
{
	public List<DialogueScriptLoaderScript.dlg> dialogueEvents;
	public bool m_bShouldDisplayDialogue = false;
	public int m_nCurrentDialogueIter = -1;

	//Text stuff
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


	GameObject m_goDialogueCanvas;
	GameObject m_goDialogueBox;
	GameObject m_goDialoguePortrait;
	GameObject m_goDialogueNameplate1;
	GameObject m_goDialogueNameplate2;

	/****************************************************************/                                           
	// Use this for initialization
	void Awake()
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

	void Start () 
	{
		m_goDialogueCanvas = GameObject.Find("Canvas_Dialogue");
		if(m_goDialogueCanvas)
		{
			m_goDialogueBox = m_goDialogueCanvas.transform.FindChild("DialogueBox").gameObject;
			m_goDialoguePortrait = m_goDialogueCanvas.transform.FindChild("DialoguePortrait").gameObject;
			m_goDialogueNameplate1 = m_goDialogueCanvas.transform.FindChild("Nameplate1").gameObject;
			m_goDialogueNameplate2 = m_goDialogueCanvas.transform.FindChild("Nameplate2").gameObject;
		}
		else
		{
			Debug.Log(gameObject.transform.parent.gameObject.name + " Skipped");
		}
		DisableUI();
	}

	void DisableUI()
	{
		DisableUIObject(m_goDialogueBox);
		DisableUIObject(m_goDialogueNameplate1);
		DisableUIObject(m_goDialogueNameplate2);
		DisableUIObject(m_goDialoguePortrait);
	}

	void DisableUIObject(GameObject _uiObj)
	{
		Image img = _uiObj.GetComponent<Image>();
		if(img != null)
		{
			img.enabled = false;
		}
		Text[] txtField = _uiObj.GetComponentsInChildren<Text>();
		foreach(Text txt in txtField)
		{
			txt.enabled = false;
		}
	}

	void EnableUIObject(GameObject _uiObj)
	{
		Image img = _uiObj.GetComponent<Image>();
		if(img != null)
		{
			img.enabled = true;
		}
		Text[] txtField = _uiObj.GetComponentsInChildren<Text>();
		foreach(Text txt in txtField)
		{
			txt.enabled = true;
		}
	}

	void HandleNormalDialogue()
	{
		EnableUIObject(m_goDialogueBox);

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
				GetComponent<MorseCodePlayer>().StopMorseCodeMessage();
				DisableUI();
				return;
			}
			else
			{
				GetComponent<MorseCodePlayer>().StopMorseCodeMessage();
				line = ((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).Line;
				textIter = line.Length;
			}
		}

		timer += Time.deltaTime;
		//increment the text based on the players text speed
		if(timer >= speed)
		{
			if(textIter == 0)
			{
				string filePath = ((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).DialogueFilePath; 
				if(filePath != "null" && filePath.Length > 1)
				{

					GetComponent<MorseCodePlayer>().StopMorseCodeMessage();
					GetComponent<MorseCodePlayer>().PlayMorseCodeMessage(((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).Line);
				}
			}
			if(textIter < ((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).Line.Length)
			{
				line += ((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).Line[textIter];
				textIter++;
			}
			else
				GetComponent<MorseCodePlayer>().StopMorseCodeMessage();
			timer = 0.0f;
		}
		Texture2D tBust;
		string szName = ((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).CharacterName;
		int bustID = ((DialogueScriptLoaderScript.nrmlDlg)dialogueEvents[m_nCurrentDialogueIter]).BustID;
		if(GameObject.Find("Portraits Container").GetComponent<PortraitContainerScript>().m_dPortraits.TryGetValue(
			szName+bustID.ToString() , out tBust))
		{
			//This dialogue has a portrait!! Draw things
			EnableUIObject(m_goDialoguePortrait);
			m_goDialoguePortrait.GetComponent<Image>().sprite = Sprite.Create(tBust, 
				new Rect(0, 0, tBust.width,
					tBust.height), new Vector2(0.5f, 0.5f));
			DisableUIObject(m_goDialogueNameplate1);
			EnableUIObject(m_goDialogueNameplate2);
			m_goDialogueNameplate2.GetComponentInChildren<Text>().text = szName;
		}
		else
		{
			//no bust, just have their name?
			DisableUIObject(m_goDialoguePortrait);
			EnableUIObject(m_goDialogueNameplate1);
			m_goDialogueNameplate1.GetComponentInChildren<Text>().text = szName;
			DisableUIObject(m_goDialogueNameplate2);
		}
		m_goDialogueBox.GetComponentInChildren<Text>().text = line;

	}


	void HandleHeroDialogue()
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
			GetComponent<AudioSource>().Stop();
			DisableUI();
			return;
		}
		EnableUIObject(m_goDialogueBox);
		DisableUIObject(m_goDialogueNameplate1);
		DisableUIObject(m_goDialoguePortrait);
		DisableUIObject(m_goDialogueNameplate2);
		line = "";
		for(int i = 0; i < ((DialogueScriptLoaderScript.heroDlg)dialogueEvents[m_nCurrentDialogueIter]).NumberOfChoices; ++ i)
		{
			line += ((DialogueScriptLoaderScript.heroDlg)dialogueEvents[m_nCurrentDialogueIter]).choices[i].Line + '\n';
		}
		m_goDialogueBox.GetComponentInChildren<Text>().text = line;
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
					HandleNormalDialogue();
				}
					break;
				case (int)DialogueScriptLoaderScript.DLGType.HERO:
				{
					HandleHeroDialogue();
				}
					break;
				case (int)DialogueScriptLoaderScript.DLGType.EVENT:
				{
					m_bShouldDisplayDialogue = false;
					GameObject.Find("Event system").GetComponent<BaseEventSystemScript>().HandleEvent(((DialogueScriptLoaderScript.cncldDlg)dialogueEvents[m_nCurrentDialogueIter]).EventIDToGoTo);
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
				break;
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
 