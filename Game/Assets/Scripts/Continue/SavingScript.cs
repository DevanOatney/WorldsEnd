using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;


public class SavingScript : MonoBehaviour 
{
	[Serializable]
	public class cOutputData
	{
		public float m_fMasterVol, m_fMusicVol, m_fSFXVol, m_fVoiceVol, m_fBrightness;
		public bool m_bToUseBattleAnimations;
		public int m_nTextSpeed;
		public List<string> m_lFlagKeys = new List<string>();
		public List<int> m_lFlagValues = new List<int>();
		public List<string> m_lResourceLocationNames = new List<string>();
		public List<string> m_lResourceLocationCharacterNames = new List<string> ();
		public List<string> m_lBaseUpgradeNames = new List<string> ();
		public List<int> m_lBaseUpgradeValues = new List<int> ();
		public List<string> m_lResourceLocationsUnlocked = new List<string> ();
		public int m_nGold;
		public List<DCScript.CharacterData> m_lParty;
		public List<DCScript.CharacterData> m_lRoster;
        public List<FightSceneControllerScript.cWarUnit> m_lAllyUnits;
		public List<DCScript.StatusEffect> m_lStatusEffects;
		public List<ItemLibrary.CharactersItems> m_lInventory;
		public string m_szSaveName;
		public string m_szSceneName;
		public vVector3 m_vStartingPosition;
		public int m_nFacingDir;
		public int m_nLevel;
		public float m_fTimePlayed;
	}
	[Serializable]
	public class vVector3
	{
		public float _fX, _fY, _fZ;
	}
	cOutputData WriteOutData()
	{
		cOutputData newData = new cOutputData();
		GameObject Canister = GameObject.Find("PersistantData");
		DCScript dcs = Canister.GetComponent<DCScript>();

		newData.m_fMasterVol = dcs.m_fMasterVolume;
		newData.m_fMusicVol = dcs.m_fMusicVolume;
		newData.m_fSFXVol = dcs.m_fSFXVolume;
		newData.m_fVoiceVol = dcs.m_fVoiceVolume;
		newData.m_fBrightness = dcs.m_fBrightness;
		newData.m_bToUseBattleAnimations = dcs.m_bToUseBattleAnimations;
		newData.m_nTextSpeed = dcs.m_nTextSpeed;
		foreach(KeyValuePair<string, int> entry in dcs.m_dStoryFlagField)
		{
			newData.m_lFlagKeys.Add(entry.Key);
			newData.m_lFlagValues.Add(entry.Value);
		}
		foreach (KeyValuePair<string, string> entry in dcs.m_dUnitsGatheringResources)
		{
			newData.m_lResourceLocationNames.Add (entry.Key);
			newData.m_lResourceLocationCharacterNames.Add (entry.Value);
		}
		foreach (KeyValuePair<string, int> entry in dcs.m_dBaseFlagField)
		{
			newData.m_lBaseUpgradeNames.Add (entry.Key);
			newData.m_lBaseUpgradeValues.Add (entry.Value);
		}
		foreach (string _locationUnlocked in dcs.m_lFieldResourceLocationsFound)
		{
			newData.m_lResourceLocationsUnlocked.Add (_locationUnlocked);
		}
		newData.m_nGold = dcs.m_nGold;
		newData.m_lParty = dcs.GetParty();

		foreach (DCScript.CharacterData character in newData.m_lParty)
		{
			if (character.m_szCharacterName == "Callan")
			{
				newData.m_nLevel = character.m_nLevel;
			}
		}
		newData.m_lRoster = dcs.GetRoster();
        newData.m_lAllyUnits = dcs.GetWarUnits();
		newData.m_fTimePlayed = dcs.m_fTimePlayed;
		newData.m_lStatusEffects = dcs.GetStatusEffects();
		newData.m_lInventory = dcs.m_lItemLibrary.m_lInventory;
		newData.m_szSaveName = dcs.m_szSaveLocationName;dcs.GetPreviousFieldName ();
		newData.m_szSceneName = dcs.GetPreviousFieldName ();
		newData.m_vStartingPosition = new vVector3();
		newData.m_vStartingPosition._fX = dcs.GetPreviousPosition().x;
		newData.m_vStartingPosition._fY = dcs.GetPreviousPosition().y;
		newData.m_vStartingPosition._fZ = dcs.GetPreviousPosition().z;
		newData.m_nFacingDir = dcs.GetPreviousFacingDirection();

		return newData;
	}


	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	//iter is for 1 of the 3 save files that can be written
	public void Save(int iter)
	{
		cOutputData newData = WriteOutData();
		BinaryFormatter bf = new BinaryFormatter();
		if(!Directory.Exists(Application.dataPath + "/Resources/Save Files"))
		{    
			//if it doesn't, create it
			Directory.CreateDirectory(Application.dataPath + "/Resources/Save Files");

		}
		FileStream _fFile = File.Create(Application.dataPath + "/Resources/Save Files/" + iter.ToString() + ".dat");
		bf.Serialize(_fFile, newData);
		_fFile.Close();
	}
}
