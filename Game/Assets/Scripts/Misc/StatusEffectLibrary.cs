using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatusEffectLibrary 
{
	public List<cStatusEffectData> m_lStatusEffectData = new List<cStatusEffectData>();

	public class cStatusEffectData
	{
		public string m_szEffectName;
		public int m_nEffectType; //0 - Poison, 1 - Paralyze, 2 - Stone
		public int m_nEffectCount;
		public int m_nHPMod;
		public int m_nMPMod;
		public int m_nPOWMod;
		public int m_nDEFMod;
		public int m_nSPDMod;
		public int m_nHITMod;
		public int m_nEVAMod;
	}

	public void LoadStatusEffects(TextAsset _ta)
	{
		string[] _szEffects = _ta.text.Split('\n');
		foreach(string effect in _szEffects)
		{
			string[] _szData = effect.Split(',');
			List<string> _lData = new List<string>();
			foreach(string data in _szData)
			{
				string[] _szPieces = data.Split(':');
				_lData.Add(_szPieces[_szPieces.Length-1].Trim());
			}
			cStatusEffectData newEffect = new cStatusEffectData();
			newEffect.m_szEffectName = _lData[0];
			newEffect.m_nEffectType = int.Parse(_lData[1].Trim());
			newEffect.m_nEffectCount = int.Parse(_lData[2].Trim());
			newEffect.m_nHPMod = int.Parse(_lData[3].Trim());
			newEffect.m_nMPMod = int.Parse(_lData[4].Trim());
			newEffect.m_nPOWMod = int.Parse(_lData[5].Trim());
			newEffect.m_nDEFMod = int.Parse(_lData[6].Trim());
			newEffect.m_nSPDMod = int.Parse(_lData[7].Trim());
			newEffect.m_nHITMod = int.Parse(_lData[8].Trim());
			newEffect.m_nEVAMod = int.Parse(_lData[9].Trim());
			m_lStatusEffectData.Add(newEffect);
		}
	}

	public DCScript.StatusEffect ConvertToDCStatusEffect(string effectName)
	{
		DCScript.StatusEffect newEffect = new DCScript.StatusEffect();
		foreach(cStatusEffectData effect in m_lStatusEffectData)
		{
			if(effect.m_szEffectName == effectName)
			{
				newEffect.m_szEffectName = effect.m_szEffectName;
				newEffect.m_nEffectType = effect.m_nEffectType;
				newEffect.m_nStartingTickCount = effect.m_nEffectCount;
				newEffect.m_nHPMod = effect.m_nHPMod;
				newEffect.m_nMPMod = effect.m_nMPMod;
				newEffect.m_nPOWMod = effect.m_nPOWMod;
				newEffect.m_nDEFMod = effect.m_nDEFMod;
				newEffect.m_nSPDMod = effect.m_nSPDMod;
				newEffect.m_nHITMod = effect.m_nHITMod;
				newEffect.m_nEVAMod = effect.m_nEVAMod;
				return newEffect;
			}
		}
		return null;
	}
}
