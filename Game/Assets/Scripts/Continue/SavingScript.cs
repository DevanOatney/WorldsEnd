using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;


public class SavingScript : MonoBehaviour 
{


	List<string> m_lOutputData = new List<string>();

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
		m_lOutputData.Clear();
		//Get the scrip that has the data
		GameObject Canister = GameObject.Find("PersistantData");
		DCScript dcs = Canister.GetComponent<DCScript>();
		//User settings
		m_lOutputData.Add(dcs.m_fMasterVolume.ToString());
		m_lOutputData.Add(dcs.m_fMusicVolume.ToString());
		m_lOutputData.Add(dcs.m_fSFXVolume.ToString());
		m_lOutputData.Add(dcs.m_fVoiceVolume.ToString());
		m_lOutputData.Add(dcs.m_fBrightness.ToString());
		m_lOutputData.Add(dcs.m_bToUseBattleAnimations.ToString());
		m_lOutputData.Add(dcs.m_nTextSpeed.ToString());

		//The flags
		//Write out the amount of pairs in the flag field
		m_lOutputData.Add(dcs.m_dStoryFlagField.Count.ToString());
		foreach(KeyValuePair<string, int> kvp in dcs.m_dStoryFlagField)
		{
			m_lOutputData.Add(kvp.Key);
			m_lOutputData.Add(kvp.Value.ToString());
		}

		//The amount of gold the player has
		m_lOutputData.Add(dcs.m_nGold.ToString());

		//The amount of characters written out
		List<DCScript.CharacterData> party = dcs.GetParty();
		m_lOutputData.Add(party.Count.ToString());
		foreach(DCScript.CharacterData member in party)
		{
			//name
			m_lOutputData.Add(member.m_szCharacterName);
			//race
			m_lOutputData.Add(member.m_szCharacterRace);
			//class
			m_lOutputData.Add(member.m_szCharacterClassType);
			//bio
			m_lOutputData.Add(member.m_szCharacterBio);
			//max hp
			m_lOutputData.Add(member.m_nMaxHP.ToString());
			//cur hp
			m_lOutputData.Add(member.m_nCurHP.ToString());
			//str
			m_lOutputData.Add(member.m_nSTR.ToString());
			//def
			m_lOutputData.Add(member.m_nDEF.ToString());
			//spd
			m_lOutputData.Add(member.m_nSPD.ToString());
			//eva
			m_lOutputData.Add(member.m_nEVA.ToString());
			//hit
			m_lOutputData.Add(member.m_nHIT.ToString());
			//Level
			m_lOutputData.Add(member.m_nLevel.ToString());
			//Current Experience
			m_lOutputData.Add(member.m_nCurrentEXP.ToString());
			//Weapon Name
			m_lOutputData.Add(member.m_szWeaponName);
			//Weapon Level
			m_lOutputData.Add(member.m_nWeaponLevel.ToString());
			//Weapon Damage Mod
			m_lOutputData.Add(member.m_nWeaponDamageModifier.ToString());
			//Weapon Modifier Name
			m_lOutputData.Add(member.m_szWeaponModifierName);

			//Helm Armor
			if(member.m_idHelmSlot != null)
				m_lOutputData.Add(member.m_idShoulderSlot.m_szItemName);
			else
				m_lOutputData.Add("NULL");
			//Shoulder Armor
			if(member.m_idShoulderSlot != null)
				m_lOutputData.Add(member.m_idShoulderSlot.m_szItemName);
			else
				m_lOutputData.Add("NULL");
			//Chest Armor
			if(member.m_idChestSlot != null)
				m_lOutputData.Add(member.m_idChestSlot.m_szItemName);
			else
				m_lOutputData.Add("NULL");
			//Glove Armor
			if(member.m_idGloveSlot != null)
				m_lOutputData.Add(member.m_idGloveSlot.m_szItemName);
			else
				m_lOutputData.Add("NULL");
			//Belt Armor
			if(member.m_idBeltSlot != null)
				m_lOutputData.Add(member.m_idBeltSlot.m_szItemName);
			else
				m_lOutputData.Add("NULL");
			//Leg Armor
			if(member.m_idLegSlot != null)
				m_lOutputData.Add(member.m_idLegSlot.m_szItemName);
			else
				m_lOutputData.Add("NULL");
			//Trinket1
			if(member.m_idTrinket1 != null)
				m_lOutputData.Add(member.m_idTrinket1.m_szItemName);
			else
				m_lOutputData.Add("NULL");
			//Trinket2
			if(member.m_idTrinket2 != null)
				m_lOutputData.Add(member.m_idTrinket2.m_szItemName);
			else
				m_lOutputData.Add("NULL");

			//amount of spells the character knows
			m_lOutputData.Add(member.m_lSpellsKnown.Count.ToString());
			foreach(string s in member.m_lSpellsKnown)
			{
				m_lOutputData.Add(s);
			}
		}
		//amount of status effects the party has
		m_lOutputData.Add(dcs.GetStatusEffects().Count.ToString());
		foreach(DCScript.StatusEffect se in dcs.GetStatusEffects())
		{
			//name of the status effect
			m_lOutputData.Add(se.m_szName);
			//amount of ticks remaining on the effect
			m_lOutputData.Add(se.m_nCount.ToString());
			//the amount of units effected
			m_lOutputData.Add(se.m_lEffectedMembers.Count.ToString());
			foreach(string s in se.m_lEffectedMembers)
				m_lOutputData.Add(s);
		}

		//Amount of items in inventory
		m_lOutputData.Add(dcs.m_lItemLibrary.m_lInventory.Count.ToString());
		//Characters inventory
		foreach(ItemLibrary.CharactersItems item in dcs.m_lItemLibrary.m_lInventory)
		{
			m_lOutputData.Add (item.m_szItemName);
			m_lOutputData.Add(item.m_nItemCount.ToString());
			m_lOutputData.Add(item.m_nItemType.ToString());
		}
		//The scene to load
		//sw.WriteLine(Application.loadedLevelName);
		m_lOutputData.Add(SceneManager.GetActiveScene().name);
		//The position of the player on that field
		//sw.WriteLine(dcs.GetPreviousPosition());
		m_lOutputData.Add(dcs.GetPreviousPosition().ToString());
		//The Direction in which the player was facing during save
		//sw.WriteLine(dcs.GetPreviousFacingDirection());
		m_lOutputData.Add(dcs.GetPreviousFacingDirection().ToString());
		string output = "";
		for(int i=0; i<m_lOutputData.Count; ++i)
		{
			output += m_lOutputData[i]+'\n';
		}
		System.IO.File.Delete(Application.dataPath + "/Resources/Save Files/" + iter.ToString() + ".txt");
		System.IO.File.WriteAllText(Application.dataPath + "/Resources/Save Files/" + iter.ToString() + ".txt", output);

	}
}
