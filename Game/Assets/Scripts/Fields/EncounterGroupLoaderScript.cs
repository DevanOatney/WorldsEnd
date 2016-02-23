using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class EncounterGroupLoaderScript : MonoBehaviour 
{
	public bool m_bEncountersHappen = true;
	//This string is for if there's different types of enemy groups that change throughout the course of the game  NOTE: default to EncounterGroups
	public TextAsset[] m_szEncounters = null;
	//iter to change if it's a different group
	public int m_nEncounterGroupIter = 0;

	//Listception! lawl
	List<List<cEnemyData>> m_lDayEncounterGroups = new List<List<cEnemyData>>();
	public List<List<cEnemyData>> GetDayEncounterGroups() {return m_lDayEncounterGroups;}

	List<List<cEnemyData>> m_lNightEncounterGroups = new List<List<cEnemyData>>();
	public List<List<cEnemyData>> GetNightEncounterGroups() {return m_lNightEncounterGroups;}

	//flag for if we should now be adding the monsters into the night category instead of the day one
	bool m_bNightTime = false;
	public List<cEnemyData> m_lEnemies = new List<cEnemyData>();

	public class cEnemyData
	{
		public string m_szEnemyName;
		public int m_nFormationIter;
	}

	//flag for if we're now loading the night time enemy group (if there is none, the overwatcher needs to just access the daytime one
	// Use this for initialization
	void Start () 
	{
		if(m_nEncounterGroupIter < m_szEncounters.Length)
		{
			if(m_szEncounters[m_nEncounterGroupIter] != null)
			{
				StartReadFile();
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
			
	}

	void StartReadFile()
	{
		string[] lines = m_szEncounters[m_nEncounterGroupIter].text.Split('\n');
		foreach(string group in lines)
		{
			if(group.Trim() == "NIGHT")
			{
				m_bNightTime = true;
				continue;
			}
			string[] enemies = group.Split(',');
			foreach(string e in enemies)
			{
				string[] pieces = e.Split(':');
				cEnemyData enemy = new cEnemyData();
				enemy.m_szEnemyName = pieces[0].Trim();
				enemy.m_nFormationIter = int.Parse(pieces[1].Trim());
				m_lEnemies.Add(enemy);
			}
			if(m_bNightTime == false)
				m_lDayEncounterGroups.Add(m_lEnemies);
			else
				m_lNightEncounterGroups.Add(m_lEnemies);
			m_lEnemies = new List<cEnemyData>();
		}
	}

}
