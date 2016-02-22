using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellLibrary
{
	static SpellLibrary m_sInstance = null;
	public SpellLibrary GetInstance()
	{
		if(m_sInstance == null)
			m_sInstance = this;
		return m_sInstance;
	}
	public class cSpellData
	{
		public string m_szSpellName;
		public int m_nElementType;
		public int m_nMPCost;
		public int m_nHPMod;
		public int m_nMPMod;
		public int m_nPOWMod;
		public int m_nDEFMod;
		public int m_nSPDMod;
		public int m_nHITMod;
	}
	public List<cSpellData> m_lAllSpells = new List<cSpellData>();


	public void LoadSpells(TextAsset _ta)
	{
		string[] Spells = _ta.text.Split('\n');
		foreach(string line in Spells)
		{
			//command for a commented line, ignores this line and moves to the next.
			if(line[0] == '/')
			{
				continue;
			}
			//This splits each segment of data
			string[] pieces = line.Split(',');
			List<string> _lData = new List<string>();
			foreach(string p in pieces)
			{
				//This is to ignore anything on the left side of :
				string[] data = p.Split(':');
				_lData.Add(data[data.Length-1].Trim());
			}
			cSpellData newSpell = new cSpellData();
			newSpell.m_szSpellName = _lData[0].Trim();
			newSpell.m_nElementType = int.Parse(_lData[1].Trim());
			newSpell.m_nMPCost = int.Parse(_lData[2].Trim());
			newSpell.m_nHPMod = int.Parse(_lData[3].Trim());
			newSpell.m_nMPMod = int.Parse(_lData[4].Trim());
			newSpell.m_nPOWMod = int.Parse(_lData[5].Trim());
			newSpell.m_nDEFMod = int.Parse(_lData[6].Trim());
			newSpell.m_nSPDMod = int.Parse(_lData[7].Trim());
			newSpell.m_nHITMod = int.Parse(_lData[8].Trim());
			m_lAllSpells.Add(newSpell);
		}
	}

}
