using UnityEngine;
using System.Collections;

public class RosterLoaderScript : MonoBehaviour
{
	public TextAsset m_taRosterList;
	// Use this for initialization
	void Start () 
	{
		LoadRoster();
		gameObject.GetComponent<DCScript>().AddPartyMember("Callan");
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void LoadRoster()
	{
		string[] Characters = m_taRosterList.text.Split('\n');
		foreach(string line in Characters)
		{
			//command for a commented line, ignores this line and moves to the next.
			if(line[0] == '/')
			{
				continue;
			}
			GameObject newCharacter = Resources.Load<GameObject>("Units/Ally/" + line.Trim() + "/" + line.Trim());
			if (newCharacter != null)
				newCharacter.GetComponent<CAllyBattleScript> ().SetUnitStats ();
			else
			{
				//This is a character that doesn't have combat stats.
				DCScript.cNonCombatAlly c = new DCScript.cNonCombatAlly();
				c.m_szCharacterName = line.Trim ();
				c.m_bCombatCharacter = false;
				TextAsset _stats = Resources.Load<TextAsset> ("Units/Ally/" + line.Trim () + "/" + "NonCombatStats");
				string[] Lines = _stats.text.Split('\n');
				c.m_szCharacterRace = Lines [0].Split (':') [1].Trim ();
				c.m_szCharacterClassType = Lines [1].Split (':') [1].Trim ();
				c.m_szCharacterBio = Lines [2].Split (':') [1].Trim ();
				c.m_szSupportAbility = Lines [3].Split (':') [1].Trim ();
				gameObject.GetComponent<DCScript> ().SetRosteredCharacterData (c);
			}

		}
	}
}
