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
			newCharacter.GetComponent<CAllyBattleScript>().SetUnitStats();

		}
	}
}
