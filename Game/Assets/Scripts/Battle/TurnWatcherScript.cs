using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TurnWatcherScript : MonoBehaviour 
{
	public Dictionary<string,int> m_dEventTriggers = new Dictionary<string, int>();
	public List<GameObject> m_goUnits;
	public int m_nOrderIter = 0;
	// Use this for initialization
	DCScript ds;
	[HideInInspector]
	public GameObject m_goActionSelector;

	//The amount of enemies on the field
	int m_nEnemyCount = 0;
	//The amount of allies on the field
	int m_nAllyCount = 0;
	public int GetAllyCount() {return m_nAllyCount;}
	string m_szFirstPart = "Units/Enemy/";
	bool m_bIsBattleOver = false;
	//tag to only allow the player to end Victory screen once
	bool m_VictoryScreenOneShot = false;
	//the time it takes to fade out after the battle is over
	float m_fFadeDuration = 1.0f;

	//bool so that after everything is instantiated we can set up a turn order
	bool m_bHasStarted = false;

	//to keep the screen from fading before death animations happen
	public List<AudioClip>   m_lACMuicFiles = new List<AudioClip> ();

	public Texture2D m_t2dSelector;

	public AudioClip m_acVictoryFanfare;

	//list of levels of enemies that are defeated
	List<int> m_lExperienceToAward = new List<int>();

	//The previous amount of experience each character had before they level.
	Dictionary<string, int> m_lPreviousExperience = new Dictionary<string, int>();

	//The new amount of experience each character is gaining
	Dictionary<string, int> m_lNewExperienceTotal = new Dictionary<string, int>();

	//The levels that the characters previously were
	Dictionary<string, int> m_lPreviousLevels = new Dictionary<string, int>();

	//speed of the counter showing the characters current exp
	int m_fExpTickSpeed = 1;
	float m_fExpBucket = 0.01f;
	bool[] m_bHasEnteredEXP = {false, false, false, false, false, false};


	//for leveling up characters
	public GameObject m_gLevelUpObj;
	//flag for if the players has already pressed return during the victory screen
	bool m_bHasPressedEnter = false;
	[HideInInspector]
	float m_fChanceToEscape = 60.0f;
	public float GetChanceToEscape() {return m_fChanceToEscape;}

	void Awake()
	{
		GameObject pdata = GameObject.Find("PersistantData");
		m_goActionSelector = GameObject.Find("Actions");
		m_goActionSelector.SetActive(false);

		if(pdata == null)
		{
			//This is a debug play then.   Create a data canister, and put the main character in the party
			pdata = Instantiate(Resources.Load("Misc/PersistantData", typeof(GameObject))) as GameObject;
			pdata.name = pdata.name.Replace("(Clone)", "");
			GameObject.Find("PersistantData").GetComponent<DCScript>().GetParty().Clear();
			GameObject Callan = Resources.Load<GameObject>("Units/Ally/Callan/Callan");
			Callan.GetComponent<CAllyBattleScript>().SetUnitStats();
			GameObject Briol = Resources.Load<GameObject>("Units/Ally/Briol/Briol");
			Briol.GetComponent<CAllyBattleScript>().SetUnitStats();
			List<string> lEnemies = new List<string>();
			lEnemies.Add("Boar");
			lEnemies.Add("KillerBee");
			pdata.GetComponent<DCScript>().SetEnemyNames(lEnemies);
			ds = GameObject.Find("PersistantData").GetComponent<DCScript>();
			List<DCScript.CharacterData> party = ds.GetParty();
			foreach(DCScript.CharacterData ally in party)
			{
				if(ally.m_szCharacterName == "Briol")
					ally.m_nFormationIter = 3;
				else
					ally.m_nFormationIter = 4;
			}
			ds.SetParty(party);
		}
		else
			ds = GameObject.Find("PersistantData").GetComponent<DCScript>();


	}

	void Start () 
	{
		Camera.main.GetComponent<AudioSource>().volume = 0.5f + ds.m_fMusicVolume;
		//load enemies
		List<string> enemies = ds.GetEnemyNames();
		int FormationCounter = 0;
		foreach(string s in enemies)
		{
			string fullPath = m_szFirstPart + s + "/" + s;
			GameObject loadedEnemy = Resources.Load<GameObject>(fullPath);
			GameObject enemy = Instantiate(loadedEnemy) as GameObject;
			//remove the "(clone)"
			enemy.name = loadedEnemy.name;
			//Set it's position on the field accordingly
			enemy.GetComponent<UnitScript>().FieldPosition = FormationCounter;
			enemy.GetComponent<UnitScript>().SetUnitLevel(loadedEnemy.GetComponent<UnitScript>().GetUnitLevel());
			//if(enemy.name == "CharacterReference")



			//e the stats of the unit to the object it instantiated from
			enemy.GetComponent<UnitScript>().SetMaxHP(loadedEnemy.GetComponent<UnitScript>().GetMaxHP());
			enemy.GetComponent<UnitScript>().SetCurHP(loadedEnemy.GetComponent<UnitScript>().GetCurHP());
			enemy.GetComponent<UnitScript>().SetSTR(loadedEnemy.GetComponent<UnitScript>().GetSTR());
			enemy.GetComponent<UnitScript>().SetDEF(loadedEnemy.GetComponent<UnitScript>().GetDEF());
			enemy.GetComponent<UnitScript>().SetSPD(loadedEnemy.GetComponent<UnitScript>().GetSPD());

			FormationCounter++;
		}
		FormationCounter = 0;
		List<DCScript.CharacterData> allies = ds.GetParty();
		foreach(DCScript.CharacterData g in allies)
		{
			GameObject Ally = Instantiate(Resources.Load<GameObject>("Units/Ally/" + g.m_szCharacterName + "/" + g.m_szCharacterName)) as GameObject;
			//Set the name so (Clone) isn't in the name.
			Ally.name = g.m_szCharacterName;
			//because if it's not 0 it means this is just debugging
			if(Ally.GetComponent<UnitScript>().FieldPosition == 0)
			{
				Ally.GetComponent<UnitScript>().FieldPosition = g.m_nFormationIter;
				Ally.GetComponent<CAllyBattleScript>().UpdatePositionOnField();
			}
			//Set the stats of the unit to the object it instantiated from
			Ally.GetComponent<UnitScript>().SetMaxHP(g.m_nMaxHP);
			Ally.GetComponent<UnitScript>().SetCurHP(g.m_nCurHP);
			Ally.GetComponent<UnitScript>().SetSTR(g.m_nSTR);
			Ally.GetComponent<UnitScript>().SetDEF(g.m_nDEF);
			Ally.GetComponent<UnitScript>().SetSPD(g.m_nSPD);
			Ally.GetComponent<UnitScript>().SetUnitLevel(g.m_nLevel);
			Ally.GetComponent<CAllyBattleScript>().m_szClassName = g.m_szCharacterClassType;
			Ally.GetComponent<CAllyBattleScript>().m_nCurrentExperience = g.m_nCurrentEXP;
			Ally.GetComponent<CAllyBattleScript>().m_lSpellList = g.m_lSpellsKnown;
			Ally.GetComponent<CAllyBattleScript>().m_idHelmSlot = g.m_idHelmSlot;
			Ally.GetComponent<CAllyBattleScript>().m_idShoulderSlot = g.m_idShoulderSlot;
			Ally.GetComponent<CAllyBattleScript>().m_idChestSlot = g.m_idChestSlot;
			Ally.GetComponent<CAllyBattleScript>().m_idGloveSlot = g.m_idGloveSlot;
			Ally.GetComponent<CAllyBattleScript>().m_idBeltSlot = g.m_idBeltSlot;
			Ally.GetComponent<CAllyBattleScript>().m_idLegSlot = g.m_idLegSlot;
			Ally.GetComponent<CAllyBattleScript>().m_idTrinket1 = g.m_idTrinket1;
			Ally.GetComponent<CAllyBattleScript>().m_idTrinket2 = g.m_idTrinket2;
		}


		m_bHasStarted = false;
		ds.SetMasterVolume();
		GetComponent<AudioSource>().PlayOneShot(m_lACMuicFiles[ds.m_nMusicIter], 0.5f + ds.m_fMusicVolume); 
	}


	IEnumerator  GainExp (GameObject gUnit) 
	{
		while(m_lNewExperienceTotal[gUnit.name] > 0)
		{

			
			m_lPreviousExperience[gUnit.name] += m_fExpTickSpeed;
			m_lNewExperienceTotal[gUnit.name] -= m_fExpTickSpeed;
			if(m_lPreviousExperience[gUnit.name] >= gUnit.GetComponent<CAllyBattleScript>().m_nExperienceToLevel)
			{
				//Level!
				m_lPreviousExperience[gUnit.name] = 0;
				m_lPreviousLevels[gUnit.name] += 1;
				
				
				GameObject Catch = Instantiate(m_gLevelUpObj) as GameObject;
				Vector3 pos = gUnit.transform.position;
				pos.z -= 0.1f;
				Catch.transform.position = pos;
				Destroy(Catch, 1.35f);
			}
			yield return new WaitForSeconds( m_fExpBucket);
		}

	}
	
	
	void SortTurnOrder()
	{
		//sort units based on turn order (make sure this is set to highest goes first
		m_goUnits.Sort(delegate(GameObject a, GameObject b){return (a.GetComponent<UnitScript>().GetSPD().CompareTo(b.GetComponent<UnitScript>().GetSPD()));});
		m_goUnits.Reverse();
		//Activate the first unit in the turn order
		m_goUnits[m_nOrderIter].GetComponent<UnitScript>().m_bIsMyTurn = true;

		/*
		//Check to  make sure this isn't the second part of the boss battle, if it is...
		int result;
		if(ds.m_dStoryFlagField.TryGetValue("Battle_ReadMessage", out result))
		{
			switch(result)
			{
			case 1:
			{
				//Tutorial dialogue for the intro
				//stop combat for a second
				m_goUnits[m_nOrderIter].GetComponent<UnitScript>().m_bIsMyTurn = false;
				GameObject messageWindow = GameObject.Find("MessageWindow");
				messageWindow.GetComponent<SpriteRenderer>().enabled = true;
				GameObject.Find("TextOnWindow").SetActive(true);
				messageWindow.GetComponent<MessageWindowScript>().BeginMessage("Defeat this boar with attacks, use healing items if your health gets low!");
			}
				break;
			case 2:
			{
				//Dialogue for the boss fight against the boar in the temple of azyre
			}
				break;
			}


		}
		else
		{
			m_goUnits[m_nOrderIter].GetComponent<UnitScript>().m_bIsMyTurn = true;
		}
		*/

	}

	public void PlayMessage(string p_szMessage)
	{
		m_goUnits[m_nOrderIter].GetComponent<UnitScript>().m_bIsMyTurn = false;
		GameObject messageWindow = GameObject.Find("MessageWindow");
		messageWindow.GetComponent<SpriteRenderer>().enabled = true;
		GameObject.Find("TextOnWindow").SetActive(true);
		messageWindow.GetComponent<MessageWindowScript>().BeginMessage(p_szMessage);
	}

	void DoneReadingMessage()
	{
		ds.m_dStoryFlagField.Remove("Battle_ReadMessage");
		//the event window is done displaying it's message
		GameObject.Find("MessageWindow").GetComponent<SpriteRenderer>().enabled = false;
		GameObject.Find("TextOnWindow").SetActive(false);
		m_goUnits[m_nOrderIter].GetComponent<UnitScript>().m_bIsMyTurn = true;
	}

	// Update is called once per frame
	void Update () 
	{
		if(m_bHasStarted == false)
		{
			m_bHasStarted = true;
			SortTurnOrder();
		}
	}

	void OnGUI()
	{
		//Check to see if we should be displaying the exp award screen
		if(m_bIsBattleOver == true)
		{
			//get a list of all of the allies on the map
			GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");

			if(Input.GetKeyUp(KeyCode.Return) && m_VictoryScreenOneShot == false)
			{
				if(m_bHasPressedEnter == false)
				{
					m_bHasPressedEnter = true;

					
					foreach(GameObject Ally in Allies)
					{
						m_lPreviousExperience[Ally.name] = Ally.GetComponent<CAllyBattleScript>().m_nExperienceToLevel;
						m_lPreviousLevels[Ally.name] = Ally.GetComponent<CAllyBattleScript>().GetUnitLevel();
						m_lNewExperienceTotal[Ally.name] = 0;
					}
				}
				else
				{
					//just move on
					m_VictoryScreenOneShot = true;
					BeginFading();
					FadeInOutSound obj = Camera.main.GetComponent<FadeInOutSound>();
					StartCoroutine(obj.FadeAudio(1.0f, FadeInOutSound.Fade.Out));
					Invoke("Finish", 1.0f);
				}
			}

			GUI.BeginGroup(new Rect(Screen.width * 0.3f, Screen.height * 0.3f, 200, 200));
			GUI.Box(new Rect(0, 0, 200, 200), "");
			//For incrementing down the screen to display each character
			float fYAdjust = 15.0f;
			//For shifting to the right as things are drawn
			float fXAdjust = 0.0f;
			foreach(GameObject Ally in Allies)
			{
				//base the y position as to the units position on the field so it correlates correctly
				switch(Ally.GetComponent<UnitScript>().FieldPosition)
				{
				case 0:
				{
					fYAdjust = 15.0f + 25.0f;
				}
					break;
				case 1:
				{
					fYAdjust = 15.0f;
				}
					break;
				case 2:
				{
					fYAdjust = 15.0f + 50.0f;
				}
					break;
				}

				//Draw a background box for fun
				GUI.Box(new Rect(0, fYAdjust, 200, 25.0f), "");
				//Display their name on the left of the box
				GUI.Box(new Rect(fXAdjust, fYAdjust, 100, 25.0f), Ally.name);
				fXAdjust += 105.0f;
				//GUI.Label(new Rect(fXAdjust, fYAdjust, 40.0f, 25.0f),"Lv: " +  m_lPreviousLevels[Ally.name].ToString());
				fXAdjust += 50.0f;


				//Start the coroutine for the unit to gain exp
				//TODO: Set this up so it's only called once per character
				if(m_bHasEnteredEXP[Ally.GetComponent<UnitScript>().FieldPosition] == false)
				{
					m_bHasEnteredEXP[Ally.GetComponent<UnitScript>().FieldPosition] = true;
					StartCoroutine("GainExp", Ally);
				}


				//Display their current exp just to the right
				//int digitCount = (int)(Mathf.Log10( m_lPreviousExperience[Ally.name]) +1);
				//GUI.Label(new Rect(fXAdjust, fYAdjust, 30.0f, 25.0f), m_lPreviousExperience[Ally.name].ToString());



				//reset the x adjustment for the next character
				fXAdjust = 0.0f;
			}



			GUI.EndGroup();
		}
	}

	public void MyTurnIsOver(GameObject go)
	{
		if(m_bIsBattleOver == true)
			return;
		if(go == m_goUnits[m_nOrderIter])
		{
			m_nOrderIter++;
			if(m_nOrderIter >= m_goUnits.Count)
				m_nOrderIter = 0;
			m_goUnits[m_nOrderIter].GetComponent<UnitScript>().m_bIsMyTurn = true;
		}
	}
	public void RemoveMeFromList(GameObject go, float deathDuration)
	{
		//add the 
		m_lExperienceToAward.Add(go.GetComponent<UnitScript>().GetUnitLevel());
		//quickly grab the index of the object about to be removed
		int whereOnList = m_goUnits.IndexOf(go);
		//remove the unit
		if(m_goUnits.Remove(go) == true)
		{
			//if the unit that was removed is before the current person, decrement the iter
			if(m_nOrderIter > whereOnList)
			{
				m_nOrderIter--;
			}
		}
		//Adjust the amount of enemies/allies on the map to check for win/loss cases
		//TODO: perhaps set up a scenario where if a specific enemy/ally is defeated the fight ends
		if(go.GetComponent<UnitScript>().m_nUnitType > (int)UnitScript.UnitTypes.NPC)
		{
			//removing an enemy
			m_nEnemyCount--;

			if(m_nEnemyCount <= 0)
			{
				int result;
				if(ds.m_dStoryFlagField.TryGetValue("Primus_ReturnFromFixFight", out result))
				{
					ds.m_dStoryFlagField.Remove("Primus_ReturnFromFixFight");
					ds.m_dStoryFlagField.Add("DefeatedApplicant", 1);
				}
				m_bIsBattleOver  = true;
				Win();
			}
		}
		else
		{
			m_nAllyCount--;
			if(m_nAllyCount <= 0)
			{
				Invoke("BeginFading", m_fFadeDuration);
				int result;
				if(ds.m_dStoryFlagField.TryGetValue("FixedFight", out result))
				{
					if(!ds.m_dStoryFlagField.TryGetValue("Primus_ReturnFromFixFight", out result))
					{
							ds.m_dStoryFlagField.Remove("FixedFight");
							ds.m_dStoryFlagField.Add("Primus_ReturnFromFixFight", 1);
							Invoke ("ReturnToField", 3.0f);
					}
					else
					{
						Invoke("Lose",3.0f + m_fFadeDuration);
					}
				}
				else
				{
					Invoke("Lose", 3.0f + m_fFadeDuration);
				}
			}
		}
	}
	public void AddMeToList(GameObject go)
	{
		//TODO  perhaps adjust incase the list needs alteration when someone joins the fight?
		m_goUnits.Add(go);
		if(go.GetComponent<UnitScript>().m_nUnitType > (int)UnitScript.UnitTypes.ALLY_RANGED)
		{
			//adding an enemy
			m_nEnemyCount++;
		}
		else
		{
			m_nAllyCount++;
		}
	}

	void BeginFading()
	{
		Camera.main.SendMessage("fadeOut");
	}

	void ReturnToField()
	{
		//called when the battle was scripted and is now over, handle any dictionary adding/removal beforehand
		string previousField = ds.GetPreviousFieldName();
		ds.SetPreviousFieldName(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(previousField);
	}

	void Win()
	{
		//Play victory fanfare
		GetComponent<AudioSource>().Stop();
		GetComponent<AudioSource>().PlayOneShot(m_acVictoryFanfare, 0.5f + ds.m_fSFXVolume);
		//TODO: award experience to the allies on field.
		List<DCScript.CharacterData> party = ds.GetParty();
		foreach(DCScript.CharacterData ally in party)
		{
			GameObject foundAlly = GameObject.Find(ally.m_szCharacterName);
			
			//Catch the previous amount of xp the character had incase they level
			int prevExp = foundAlly.GetComponent<CAllyBattleScript>().m_nCurrentExperience;
			m_lPreviousLevels.Add(ally.m_szCharacterName, ally.m_nLevel);
			//Award experience (their script will check if it levels and return a bool .. maybe I want to do some level up effect?  Not sure..
			int nExp = foundAlly.GetComponent<CAllyBattleScript>().AwardExperience(m_lExperienceToAward);
			//add the previous exp of this unit to the list
			m_lPreviousExperience.Add(ally.m_szCharacterName, prevExp);
			//add the total experience the character is going to gain to the list
			m_lNewExperienceTotal.Add(ally.m_szCharacterName, nExp);

			if(nExp + prevExp >= foundAlly.GetComponent<CAllyBattleScript>().m_nExperienceToLevel)
			{
				//the character leveled up
				ally.m_nCurHP = foundAlly.GetComponent<CAllyBattleScript>().GetMaxHP();

			}
			else
			{
				ally.m_nCurHP = foundAlly.GetComponent<CAllyBattleScript>().GetCurHP();
			}
			ally.m_nMaxHP = foundAlly.GetComponent<CAllyBattleScript>().GetMaxHP();
			ally.m_nSTR = foundAlly.GetComponent<CAllyBattleScript>().GetSTR();
			ally.m_nDEF = foundAlly.GetComponent<CAllyBattleScript>().GetDEF();
			ally.m_nSPD = foundAlly.GetComponent<CAllyBattleScript>().GetSPD();
			ally.m_nLevel = foundAlly.GetComponent<CAllyBattleScript>().GetUnitLevel();
			ally.m_nCurrentEXP = foundAlly.GetComponent<CAllyBattleScript>().m_nCurrentExperience;
			ally.m_idHelmSlot = foundAlly.GetComponent<CAllyBattleScript>().m_idHelmSlot;
			ally.m_idShoulderSlot = foundAlly.GetComponent<CAllyBattleScript>().m_idShoulderSlot;
			ally.m_idChestSlot = foundAlly.GetComponent<CAllyBattleScript>().m_idChestSlot;
			ally.m_idGloveSlot = foundAlly.GetComponent<CAllyBattleScript>().m_idGloveSlot;
			ally.m_idBeltSlot = foundAlly.GetComponent<CAllyBattleScript>().m_idBeltSlot;
			ally.m_idLegSlot = foundAlly.GetComponent<CAllyBattleScript>().m_idLegSlot;
			ally.m_idTrinket1 = foundAlly.GetComponent<CAllyBattleScript>().m_idTrinket1;
			ally.m_idTrinket2 = foundAlly.GetComponent<CAllyBattleScript>().m_idTrinket2;
			ally.m_lSpellsKnown = foundAlly.GetComponent<CAllyBattleScript>().m_lSpellList;
		}
		ds.SetParty(party);

	}

	public void Escape()
	{
		BeginFading();
		FadeInOutSound obj = Camera.main.GetComponent<FadeInOutSound>();
		StartCoroutine(obj.FadeAudio(1.0f, FadeInOutSound.Fade.Out));
		Invoke("Finish", 1.0f);
	}

	void Finish()
	{
		//grab each status effect and add it back to the list in the data canister, if the status effect already exists, add the character to the list of effected characters.
		GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");
		foreach(GameObject Ally in Allies)
		{
			foreach(GameObject effect in Ally.GetComponent<UnitScript>().m_lStatusEffects)
			{
				Debug.Log (effect.name);
				int catchIter = ds.IsStatusEffectInList(effect.GetComponent<BattleBaseEffectScript>().name);
				if( catchIter != -1)
				{
					//effect already exists in list
					bool bAlreadyInList = false;
					foreach(string name in ds.GetStatusEffects()[catchIter].m_lEffectedMembers)
					{
						if(name == Ally.name)
							bAlreadyInList = true;
					}
					if(bAlreadyInList == false)
					{
						//character is not on the list of effected characters
						ds.GetStatusEffects()[catchIter].m_lEffectedMembers.Add(Ally.name);
					}

					if(effect.GetComponent<BattleBaseEffectScript>().m_nAmountOfTicks > ds.GetStatusEffects()[catchIter].m_nCount)
						ds.GetStatusEffects()[catchIter].m_nCount = effect.GetComponent<BattleBaseEffectScript>().m_nAmountOfTicks;
					Debug.Log (ds.GetStatusEffects()[catchIter].m_nCount = effect.GetComponent<BattleBaseEffectScript>().m_nAmountOfTicks);
				}
				else
				{
					//effect needs to be added
					DCScript.StatusEffect se = new DCScript.StatusEffect();
					se.m_szName =  effect.GetComponent<BattleBaseEffectScript>().name;
					se.m_nCount =  effect.GetComponent<BattleBaseEffectScript>().m_nAmountOfTicks;
					se.m_lEffectedMembers.Add(Ally.name);
				}
			}
		}
		//TODO: chance to win items based on enemies defeated/set items that are won during that specific battle
		string previousField = ds.GetPreviousFieldName();
		ds.SetPreviousFieldName(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(previousField);
	}
	void Lose()
	{
        SceneManager.LoadScene("GameOver_Scene");
	}

	public void ActionSelected(int p_nIndex)
	{
		foreach(GameObject unit in m_goUnits)
		{
			if(unit.GetComponent<UnitScript>().m_bIsMyTurn == true)
			{
				//assuming it's an ally.. because if you're in the action menu selecting an action.. it must be an allies turn
				m_goActionSelector.SetActive(false);
				unit.GetComponent<CAllyBattleScript>().HandleActionSelected(p_nIndex);
			}
		}
	}

	public void Enemy_TargetSelected(int p_nIndex)
	{
		GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject e in _enemies)
		{
			if(e.GetComponent<UnitScript>().FieldPosition == p_nIndex)
			{
				if(e.GetComponent<UnitScript>().GetCurHP() > 0)
				{
					foreach(GameObject unit in m_goUnits)
					{
						if(unit.GetComponent<UnitScript>().m_bIsMyTurn == true)
						{
							unit.GetComponent<CAllyBattleScript>().ChangeEnemyTarget(p_nIndex);
							return;
						}
					}
				}
			}
		}
	}

	public void Ally_TargetSelected(int p_nIndex)
	{
		GameObject[] _allies = GameObject.FindGameObjectsWithTag("Ally");
		foreach(GameObject a in _allies)
		{
			if(a.GetComponent<UnitScript>().FieldPosition == p_nIndex)
			{
				foreach(GameObject b in _allies)
				{
					if(b.GetComponent<UnitScript>().m_bIsMyTurn == true)
					{
						
					}
				}
			}
		}
	}

}
