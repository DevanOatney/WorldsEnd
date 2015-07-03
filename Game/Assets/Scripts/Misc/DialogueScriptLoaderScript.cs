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
	}
}
