using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class DialogueScriptLoaderScript : MonoBehaviour 
{
	public enum DLGType {NORMAL=1, HERO, EVENT}
	public TextAsset m_szFile;
	private StreamReader sr1;
	public class Choice
	{
		public string Line = null;
		public string TextIDToGoTo = null;
	}
	public class dlg
	{
		public string TextID = null;
		public int SpecialCaseFlag = -1;
	}
	public class nrmlDlg :  dlg
	{
		public string CharacterName = null;
		public string DialogueFilePath = null;
		public string Line = null;
		public int BustID = -1;
		public string TextIDToGoTo = null;
	}
	public class heroDlg : dlg
	{
		public int BustID = -1;
		public int NumberOfChoices = -1;
		public List<Choice> choices;
	}
	public class cncldDlg : dlg 
	{
		public string EventIDToGoTo = null;
	}
	public List<dlg> dlgList = new List<dlg>();
	
	void Awake ()
	{
		if(m_szFile != null)
		{
			StartReadFile();
		}
	}
	// Use this for initialization
	void Start () 
	{ 
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void StartReadFile() 
	{
		string[] DialogueLines = m_szFile.text.Split('\n');
		foreach(string line in DialogueLines)
		{
			string[] piece = line.Split('|');
			int spcCase = int.Parse(piece[1]);
			switch(spcCase)
			{
			case (int)DLGType.NORMAL:
			{
				//NORMAL
				//id|case|Character|soundDir|The line| BustID| idToGoTo
				nrmlDlg newDlg = new nrmlDlg();
				newDlg.TextID = piece[0].Trim();
				newDlg.SpecialCaseFlag = spcCase;
				newDlg.CharacterName = piece[2].Trim();
				newDlg.DialogueFilePath = piece[3].Trim();
				newDlg.Line = piece[4].Trim();
				newDlg.BustID = int.Parse(piece[5]);
				newDlg.TextIDToGoTo = piece[6].Trim();
				dlgList.Add(newDlg);

			}
				break;
			case (int)DLGType.HERO:
			{
				//HERO
				//id|case|BustID|NumberOfChoices|Line|idToGoTo....
				heroDlg newDlg = new heroDlg();

				newDlg.TextID = piece[0].Trim();
				newDlg.SpecialCaseFlag = spcCase;
				newDlg.BustID = int.Parse(piece[2].Trim());
				newDlg.NumberOfChoices = int.Parse(piece[3].Trim());
				if(newDlg.NumberOfChoices > 0)
					newDlg.choices = new List<Choice>();
				for(int i = 4; i -1 < newDlg.NumberOfChoices + 4; i += 2)
				{
					Choice c = new Choice();
					c.Line = piece[i].Trim();
					c.TextIDToGoTo = piece[i+1].Trim();
					newDlg.choices.Add(c);
				}
				dlgList.Add(newDlg);
			}
				break;
			case (int)DLGType.EVENT:
			{
				//EVENT
				//id|case|EventToGoTo
				cncldDlg newDlg = new cncldDlg();
				newDlg.TextID = piece[0].Trim();
				newDlg.SpecialCaseFlag = spcCase;
				newDlg.EventIDToGoTo = piece[2].Trim();
				dlgList.Add(newDlg);

			}
				break;
			}
		}

		//typing the stuff below was crazy time consuming.. but was having issues with running the exe outside of the root folder.. sooo abandoned it.. but it took way too long for
		//me to get rid of it xD I've grown attached to it, lol.
		/*
		sr1 = new StreamReader(_fileName, Encoding.Default);
		if(sr1 != null)
		{
			while(sr1.EndOfStream != true)
			{
				char[] buffer = new char[1];
				string textID = "";
				int spcCase = -1;
				while(buffer[0] != '|')
				{
					sr1.Read(buffer,0, 1);  
					if(buffer[0] != '|' && buffer[0] != '\n' && buffer[0] != '\r')
						textID += buffer[0];
				}
				sr1.Read(buffer, 0, 1);
				//get the special case flag
				spcCase = System.Convert.ToInt16(System.Convert.ToString(buffer[0]));
				//clear the tag after the special case
				sr1.Read(buffer, 0, 1);
				buffer[0] = ' '; 
				switch(spcCase)
				{
				case (int)DLGType.Normal:
				{
					nrmlDlg newDlg = new nrmlDlg();
					string characterName = ""; 
					while(buffer[0] != '|')
					{
						sr1.Read(buffer,0, 1);  
						if(buffer[0] != '|')
							characterName += buffer[0];
					}
					newDlg.TextID = textID;
					newDlg.SpecialCaseFlag = spcCase;
					newDlg.CharacterName = characterName;
					//reset the buffer
					buffer[0] = ' '; 
					string voiceFilePath = "";
					while(buffer[0] != '|')
					{
						sr1.Read(buffer, 0, 1);
						if(buffer[0] != '|')
							voiceFilePath += buffer[0];
					}
					newDlg.DialogueFilePath = voiceFilePath;
					//reset buffer
					buffer[0] = ' ';
					string line = "";
					while(buffer[0] != '|')
					{
						sr1.Read(buffer, 0, 1);
						if(buffer[0] != '|')
							line += buffer[0];
					}
					newDlg.Line = line;
					//reset buffer
					int bustID = -1;
					sr1.Read(buffer, 0, 1);
					//get the bust ID
					bustID = System.Convert.ToInt16(System.Convert.ToString(buffer[0]));
					newDlg.BustID = bustID;
					//clear the tag after the BustID
					sr1.Read(buffer, 0, 1);
					buffer[0] = ' '; 
					string TextIDToGoTo = "";
					while(buffer[0] != '\r')
					{
						sr1.Read(buffer, 0, 1);
						if(buffer[0] != '\r')
							TextIDToGoTo += buffer[0];
					}
					newDlg.TextIDToGoTo = TextIDToGoTo;
					dlgList.Add(newDlg);
				
				}
					break;
				case (int)DLGType.Hero:
				{
					heroDlg newDlg = new heroDlg();
					newDlg.TextID = textID;
					newDlg.SpecialCaseFlag = spcCase;
					int bustID = -1;
					sr1.Read(buffer, 0, 1);
					//get the bust ID
					bustID = System.Convert.ToInt16(System.Convert.ToString(buffer[0]));
					newDlg.BustID = bustID;
					//clear the tag after the BustID
					sr1.Read(buffer, 0, 1);
					buffer[0] = ' '; 
					
					int numberOfChoices = -1;
					sr1.Read(buffer, 0 , 1);
					numberOfChoices = System.Convert.ToInt16(System.Convert.ToString(buffer[0]));
					newDlg.NumberOfChoices = numberOfChoices;
					if(numberOfChoices > 0)
						newDlg.choices = new List<Choice>();
					//clear the tag after the number of choices
					sr1.Read(buffer, 0, 1);
					buffer[0] = ' '; 
					for(int i = 0; i < numberOfChoices; ++i)
					{
						Choice choice = new Choice();
						string line = "";
						while(buffer[0] != '|')
						{
							sr1.Read(buffer, 0, 1);
							if(buffer[0] != '|')
								line += buffer[0];
						}
						choice.Line = line;
						buffer[0] = ' ';
						string textIdToGoTo = "";
						while(buffer[0] != '|' && buffer[0] != '\r')
						{
							sr1.Read(buffer, 0, 1);
							if(buffer[0] != '|' && buffer[0] != '\r')
								textIdToGoTo += buffer[0];
						}
						buffer[0] = ' ';
						choice.TextIDToGoTo = textIdToGoTo;
						newDlg.choices.Add(choice);
					}
					
					dlgList.Add(newDlg);
				}
					break;
				case (int)DLGType.Concluding:
				{
					cncldDlg newDlg = new cncldDlg();
					newDlg.TextID = textID;
					newDlg.SpecialCaseFlag = spcCase;
					string eventToGoTo = "";
					while(buffer[0] != '\r' && sr1.EndOfStream == false)
					{
						sr1.Read(buffer, 0, 1);
						if(buffer[0] != '\r')
							eventToGoTo += buffer[0]; 
					}
					newDlg.EventIDToGoTo = eventToGoTo;
					dlgList.Add(newDlg);
					
				}
					break;
				}
				
			}
			sr1.Close();
		}
		*/
	}
}
