using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DCScript : MonoBehaviour 
{
	public float m_fMasterVolume = 0.5f;
	public float m_fMusicVolume = 0.5f;
	public float m_fSFXVolume = 0.5f;
	public float m_fVoiceVolume = 0.5f;
	public float m_fBrightness = 0.0f;
	public bool m_bToUseBattleAnimations = true;
	public int m_nTextSpeed = 2;

	//flag field that is toggled depending on what the players actions are in the game
	//["Name"] - 1 if activated 								  : Place Created
	public Dictionary<string, int> m_dStoryFlagField = new Dictionary<string, int>();
	//-------
	//FIELD INFORMATION

	//for encounter information
	List<string> m_lEnemies;
	public List<string> GetEnemyNames() {return m_lEnemies;}
	public void SetEnemyNames(List<string> e) {m_lEnemies = e;}

	//The name of the scene that the player was on before the encounter started.  Needs to be set at the field just before the battle is started.
	//Also using this for loading/saving to be the current scene that the player is in when they save.
	string m_szPreviousField;
	public string GetPreviousFieldName() {return m_szPreviousField;}
	public void SetPreviousFieldName(string s) {m_szPreviousField = s;}
	//The position of the player on the field before the encounter starts.  Set before battle is loaded, and before a field is loaded.
	Vector3 m_vPreviousPosition = new Vector3();
	public Vector3 GetPreviousPosition() {return m_vPreviousPosition;}
	public void SetPreviousPosition(Vector3 v) {m_vPreviousPosition = v;}
	//The direction in which the player was previously facing
	int m_nFacingDirection = 0;
	public int GetPreviousFacingDirection() {return m_nFacingDirection;}
	public void SetPreviousFacingDirection(int fd) {m_nFacingDirection = fd;}
	//The starting point that the character should go to (used from screen switching)
	string m_szNextStartingPosition;
	public void SetStartingPos(string strtPos) {m_szNextStartingPosition = strtPos;}
	public string GetStartingPos() {return m_szNextStartingPosition;}


	//amount of cash that the player has
	public int m_nGold;

	public int m_nMusicIter;

	//The amount of time that the player has played for.
	public float m_fTimePlayed = 0.0f;

	public ItemLibrary m_lItemLibrary = new ItemLibrary();


	public class StatusEffect
	{
		public string m_szName;
		public int m_nCount;
		public List<string> m_lEffectedMembers = new List<string>();
	}
	//list of status effects that are inflicting the party
	List<StatusEffect> m_lStatusEffects = new List<StatusEffect>();
	public List<StatusEffect> GetStatusEffects() {return m_lStatusEffects;}
	public void SetStatusEffects(List<StatusEffect> l) {m_lStatusEffects.Clear(); m_lStatusEffects = l;}
	public void AddStatusEffect(StatusEffect se)
	{
		int catchIter = IsStatusEffectInList(se.m_szName);
		if(catchIter != -1)
		{
			foreach(string c in se.m_lEffectedMembers)
			{
				bool inList = false;
				//for each effected unit, check all effected units, if you're not in it.. add this unit to the effected units
				foreach(string s in m_lStatusEffects[catchIter].m_lEffectedMembers)
				{
					if(s == c)
						inList = true;
				}
				if(inList == false)
					m_lStatusEffects[catchIter].m_lEffectedMembers.Add(c);


				if(m_lStatusEffects[catchIter].m_nCount < se.m_nCount)
					m_lStatusEffects[catchIter].m_nCount = se.m_nCount;
			}
		}
		else
		{
			m_lStatusEffects.Add (se);
		}
	}
	public void RemoveMeFromStatus(string szName, int iter)
	{
		m_lStatusEffects[iter].m_lEffectedMembers.Remove(szName);
		if(m_lStatusEffects[iter].m_lEffectedMembers.Count <= 0)
			m_lStatusEffects.RemoveAt(iter);
	}
	public int IsStatusEffectInList(string szName)
	{
		int counter = 0;
		foreach(StatusEffect se in m_lStatusEffects)
		{
			if(se.m_szName == szName)
				return counter;
			counter++;
		}
		return -1;
	}

	public class CharacterData
	{
		public string m_szCharacterName;
		public int m_nMaxHP, m_nCurHP, m_nSTR, m_nDEF, m_nSPD, m_nEVA, m_nHIT, m_nLevel, m_nCurrentEXP;

		public string m_szWeaponName;
		public int m_nWeaponDamageModifier;
		public int m_nWeaponLevel;
		public string m_szWeaponModifierName;

		public ItemLibrary.ArmorData m_idChestSlot;
		public ItemLibrary.ArmorData m_idLegSlot;
		public ItemLibrary.ArmorData m_idBeltSlot;
		public ItemLibrary.ArmorData m_idShoulderSlot;
		public ItemLibrary.ArmorData m_idHelmSlot;
		public ItemLibrary.ArmorData m_idGloveSlot;

		public ItemLibrary.ItemData m_idTrinket1;
		public ItemLibrary.ItemData m_idTrinket2;

		public List<string> m_lSpellsKnown;

		public string m_szCharacterRace;
		public string m_szCharacterClassType;
		public string m_szCharacterBio;
		public int m_nFormationIter;

	}

	//List of characters in party
	List<CharacterData> m_lPartyMembers = new List<CharacterData>();
	public List<CharacterData> GetParty() {return m_lPartyMembers;}
	public void SetParty(List<CharacterData> p) {m_lPartyMembers = p;}
	public void AddPartyMember(CharacterData character) {m_lPartyMembers.Add(character);}
	public void RemovePartyMember(CharacterData character) {m_lPartyMembers.Remove(character);}
	public CharacterData GetCharacter(string szName)
	{
		foreach(CharacterData c in m_lPartyMembers)
		{
			if(szName == c.m_szCharacterName)
				return c;
		}
		return null;
	}
	//iter to which background to load during battle (DOES NOT NEED TO BE SAVED/LOADED)
	int m_nBattleFieldBackgroundIter = 0;
	public int GetBattleFieldBackgroundIter() {return m_nBattleFieldBackgroundIter;}
	public void SetBattleFieldBackgroundIter(int iter) {m_nBattleFieldBackgroundIter = iter;}



	public class cModifier
	{
		public string m_szModifierName;
		public string m_szModifierDesc;
		public int m_nModCost;
	}
	List<cModifier> m_lModifiers = new List<cModifier>();
	public List<cModifier> GetModifierList() {return m_lModifiers;}
	public void SetModifierList(List<cModifier> lst) {m_lModifiers = lst;}



	public class LevelingWeapon
	{
		public string m_szWeaponName;
		public int m_nLevel;
		public int m_nDamage;
	}
	public class WeaponData
	{
		public string m_szOwnerName;
		public List<LevelingWeapon> m_lLevels = new List<LevelingWeapon>();
	}
	List<WeaponData> m_lWeapons = new List<WeaponData>();
	public List<WeaponData> GetWeaponList() {return m_lWeapons;}
	public void SetWeaponList(List<WeaponData> wpns) {m_lWeapons = wpns;}


	public TextAsset m_taStatProgression;
	public List<ClassStats> m_lClassStatProgressions = new List<ClassStats>();
	public ClassStats GetClassType(string szName)
	{
		foreach(ClassStats c in m_lClassStatProgressions)
		{
			if(c.m_szClassName == szName)
				return c;
		}
		return null;
	}
	public class ClassStats
	{
		public string m_szClassName;
		public int m_nHPProg, m_nStrProg, m_nDefProg, m_nSpdProg, m_nEvaProg, m_nHitProg;
	}

	void Awake () 
	{
        DontDestroyOnLoad (gameObject);
    }
	// Use this for initialization
	void Start () 
	{
		m_fTimePlayed = PlayerPrefs.GetFloat("Time_Played", 0.0f);
		StartCoroutine(IncrementTimePlayed());
		AdjustValues(); 
		m_nGold = 100;
		LoadStatProgressions();
	}

	public IEnumerator  IncrementTimePlayed () 
	{
		while(true)
		{
			m_fTimePlayed +=1;
			yield return new WaitForSeconds(1);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void LoadStatProgressions()
	{
		string[] classes = m_taStatProgression.ToString().Split('\n');
		foreach(string szClass in classes)
		{
			ClassStats cClass = new ClassStats();
			string[] szpieces = szClass.Split(',');
			cClass.m_szClassName = szpieces[0].Trim();
			cClass.m_nHPProg = int.Parse(szpieces[1].Trim());
			cClass.m_nStrProg = int.Parse(szpieces[2].Trim());
			cClass.m_nDefProg = int.Parse(szpieces[3].Trim());
			cClass.m_nSpdProg = int.Parse(szpieces[4].Trim());
			cClass.m_nEvaProg = int.Parse(szpieces[5].Trim());
			cClass.m_nHitProg = int.Parse(szpieces[6].Trim());
			m_lClassStatProgressions.Add(cClass);
		}
	}

	public void RestoreParty()
	{
		foreach(CharacterData character in m_lPartyMembers)
		{
			character.m_nCurHP = character.m_nMaxHP;
		}
		foreach(StatusEffect se in m_lStatusEffects)
		{
			GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().RemoveStatusEffect(se.m_szName);
		}
		m_lStatusEffects.Clear();
	}

	public void AdjustValues()
	{
		if(m_fMasterVolume < 0.0f)
			m_fMasterVolume = 0.0f;
		else if(m_fMasterVolume > 1.0f)
			m_fMasterVolume = 1.0f;
		if(m_fBrightness < -2.0f)
			m_fBrightness = -2.0f;
		else if(m_fBrightness > 2.0f)
			m_fBrightness = 2.0f;
		if(m_fSFXVolume < -0.5f)
			m_fSFXVolume = -0.5f;
		else if(m_fSFXVolume > 0.5f)
			m_fSFXVolume = 0.5f;
		if(m_fMusicVolume < -0.5f)
			m_fMusicVolume = -0.5f;
		else if(m_fMusicVolume > 0.5f)
			m_fMusicVolume = 0.5f;
		if(m_fVoiceVolume < -0.5f)
			m_fVoiceVolume = -0.5f;
		else if(m_fVoiceVolume > 0.5f)
			m_fVoiceVolume = 0.5f;
		if(m_nTextSpeed > 3)
			m_nTextSpeed = 3;
		else if(m_nTextSpeed < 1)
			m_nTextSpeed = 1;
	}
	
	public void SetMasterVolume()
	{
		AudioListener.volume = m_fMasterVolume;
	}
}
