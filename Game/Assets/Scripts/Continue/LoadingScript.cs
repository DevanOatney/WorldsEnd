using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;


public class LoadingScript : MonoBehaviour 
{
	string m_szFileName;
	StreamReader sr;
	// Use this for initialization
	void Awake()
	{
		m_szFileName = Application.dataPath + "/Resources/Save Files/";
	}
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	//check to see if file exists
	public bool CheckFile(int iter)
	{
		try
		{
			sr = new StreamReader(m_szFileName + iter.ToString() + ".dat");
			sr.Close();
			return true;
		}
		catch(Exception e)
		{
			Debug.Log(e.Message);
			return false;
		}
	}

	public ContinueHighlightInputScript.SaveDataInformation GetSaveData(int iter)
	{
		if(File.Exists(m_szFileName + iter.ToString() + ".dat") == false)
			return null;
		ContinueHighlightInputScript.SaveDataInformation saveData = new ContinueHighlightInputScript.SaveDataInformation();
		saveData.m_szName = "Callan";

		BinaryFormatter bf = new BinaryFormatter();
		FileStream _fFile = File.Open(m_szFileName + iter.ToString() + ".dat", FileMode.Open);
		SavingScript.cOutputData fileData = (SavingScript.cOutputData)bf.Deserialize(_fFile);
		_fFile.Close();

		//Amount of gold the player has
		saveData.m_nGold = fileData.m_nGold;
		saveData.m_nLevel = fileData.m_nLevel;
		saveData.m_szFieldName = fileData.m_szSaveName;
		saveData.m_szSceneName = fileData.m_szSceneName;
		saveData.m_fTimePlayed = fileData.m_fTimePlayed;

		return saveData;
	}

	//iter is for 1 of the 3 save files that can be written
	public void Load(int iter)
	{
		if(File.Exists(m_szFileName + iter.ToString() + ".dat") == false)
			return;
		DCScript NewData = GameObject.Find("PersistantData").GetComponent<DCScript>();
		
		BinaryFormatter bf = new BinaryFormatter();
		FileStream _fFile = File.Open(m_szFileName + iter.ToString() + ".dat", FileMode.Open);
		SavingScript.cOutputData fileData = (SavingScript.cOutputData)bf.Deserialize(_fFile);
		_fFile.Close();


		NewData.m_fMasterVolume = fileData.m_fMasterVol;
		NewData.m_fMusicVolume = fileData.m_fMusicVol;
		NewData.m_fSFXVolume = fileData.m_fSFXVol;
		NewData.m_fVoiceVolume = fileData.m_fVoiceVol;
		NewData.m_fBrightness = fileData.m_fBrightness;
		NewData.m_bToUseBattleAnimations = fileData.m_bToUseBattleAnimations;
		NewData.m_nTextSpeed = fileData.m_nTextSpeed;

		for(int i = 0; i < fileData.m_lFlagKeys.Count; ++i)
		{
			NewData.m_dStoryFlagField.Add(fileData.m_lFlagKeys[i], fileData.m_lFlagValues[i]);
		}
		for (int i = 0; i < fileData.m_lResourceLocationNames.Count; ++i)
		{
			NewData.m_dUnitsGatheringResources.Add (fileData.m_lResourceLocationNames [i], fileData.m_lResourceLocationCharacterNames [i]);
		}
		for (int i = 0; i < fileData.m_lBaseUpgradeNames.Count; ++i)
		{
			NewData.m_dBaseFlagField.Add (fileData.m_lBaseUpgradeNames [i], fileData.m_lBaseUpgradeValues [i]);
		}
		NewData.m_lFieldResourceLocationsFound = fileData.m_lResourceLocationsUnlocked;
		NewData.m_nGold = fileData.m_nGold;
		NewData.m_fTimePlayed = fileData.m_fTimePlayed;
		NewData.SetRoster(fileData.m_lRoster);
		NewData.SetParty(fileData.m_lParty);
        NewData.SetWarUnits(fileData.m_lAllyUnits);
		NewData.SetStatusEffects(fileData.m_lStatusEffects);
		NewData.m_lItemLibrary.m_lInventory = fileData.m_lInventory;
		NewData.SetPreviousFieldName(fileData.m_szSceneName);
		NewData.m_szSaveLocationName = fileData.m_szSaveName;
		Vector3 startingPos = new Vector3(fileData.m_vStartingPosition._fX, fileData.m_vStartingPosition._fY, fileData.m_vStartingPosition._fZ);
		NewData.SetPreviousPosition(startingPos);
		NewData.SetPreviousFacingDirection(fileData.m_nFacingDir);
	}

}
