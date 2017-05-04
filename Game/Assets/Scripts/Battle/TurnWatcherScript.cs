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
	bool m_bOneShotAfterVictory = false;

	public List<AudioClip>   m_lACMuicFiles = new List<AudioClip> ();
	public Texture2D m_t2dSelector;
	public AudioClip m_acVictoryFanfare;

	//list of levels of enemies that are defeated
	List<int> m_lExperienceToAward = new List<int>();

	//The previous amount of experience each character will have at the end.
	Dictionary<string, int> m_lNextExperience = new Dictionary<string, int>();

	//The new amount of experience each character is gaining
	Dictionary<string, int> m_lNewExperienceTotal = new Dictionary<string, int>();

	//The levels that the characters will be at the end of the experience tally
	Dictionary<string, int> m_lNextLevel = new Dictionary<string, int>();

	//List of the items that will be able to drop at the end of battle
	List<StandardEnemyScript.DroppedItem> m_lItemsThatCanDrop = new List<StandardEnemyScript.DroppedItem>();

	//Amount of gold that you'll get at the end of the fight
	int m_nGoldDropped = 0;

	//List for how many items are won at the end of the fight.
	List<ItemChances> m_dnItemCountChances = new List<ItemChances>();
	class ItemChances
	{
		public ItemChances(int amnt, int chnc) {m_nAmountOfItems = amnt;m_nChance = chnc; }
		public int m_nAmountOfItems;
		public int m_nChance;
	}
	//List of the items that were won
	List<ItemLibrary.CharactersItems> m_lItemsWon = new List<ItemLibrary.CharactersItems>();

	//speed of the counter showing the characters current exp
	int m_fExpTickSpeed = 3;
	float m_fExpBucket = 0.001f;



	//for leveling up characters
	public GameObject m_gLevelUpObj;
	public List<GameObject> m_lStatusEffectList;
	//flag for if the players has already pressed return during the victory screen
	bool m_bHasPressedEnter = false;
	[HideInInspector]
	float m_fChanceToEscape = 60.0f;
	public float GetChanceToEscape() {return m_fChanceToEscape;}
	List<GameObject> m_lPartyPanels = new List<GameObject>();

	void Awake()
	{
		m_dnItemCountChances.Add(new ItemChances(0, 55));
		m_dnItemCountChances.Add(new ItemChances(1, 25));
		m_dnItemCountChances.Add(new ItemChances(2, 10));
		m_dnItemCountChances.Add(new ItemChances(3, 8));
		m_dnItemCountChances.Add(new ItemChances(4, 2));
		GameObject pdata = GameObject.Find("PersistantData");
		m_goActionSelector = GameObject.Find("Actions");
		m_goActionSelector.SetActive(false);


		if(pdata == null)
		{
			//This is a debug play then.   Create a data canister, and put the main character in the party
			pdata = Instantiate(Resources.Load("Misc/PersistantData", typeof(GameObject))) as GameObject;
			pdata.name = pdata.name.Replace("(Clone)", "");
			ds = pdata.GetComponent<DCScript>();
			ds.GetParty().Clear();
			GameObject Callan = Resources.Load<GameObject>("Units/Ally/Callan/Callan");
			Callan.GetComponent<CAllyBattleScript>().SetUnitStats();
			ds.AddPartyMember("Callan");
			GameObject Briol = Resources.Load<GameObject>("Units/Ally/Briol/Briol");
			Briol.GetComponent<CAllyBattleScript>().SetUnitStats();
			ds.AddPartyMember("Briol");
			GameObject Illuiyani = Resources.Load<GameObject>("Units/Ally/Illuiyani/Illuiyani");
			Illuiyani.GetComponent<CAllyBattleScript>().SetUnitStats();
			ds.AddPartyMember("Illuiyani");
			List<EncounterGroupLoaderScript.cEnemyData> lEnemies = new List<EncounterGroupLoaderScript.cEnemyData>();
			EncounterGroupLoaderScript.cEnemyData enemy = new EncounterGroupLoaderScript.cEnemyData();
			enemy.m_szEnemyName = "Slug Slinger";
			enemy.m_nFormationIter = 3;
			lEnemies.Add(enemy);
			pdata.GetComponent<DCScript>().SetEnemyNames(lEnemies);
		}
		else
			ds = GameObject.Find("PersistantData").GetComponent<DCScript>();


	}

	void InitializePortraits(CharacterPanelContainer _cpc, CAllyBattleScript _c)
	{
		if(_cpc == null || _c == null)
			return;
		_cpc.m_goCharacterName.GetComponent<Text>().text = _c.name;
		Texture2D sprText = _c.TextureFromSprite (_c.m_tLargeBust);
		_cpc.m_goCharacterPortrait.GetComponent<Image> ().sprite = Sprite.Create (sprText,
			new Rect (0, 0, sprText.width, sprText.height),
			new Vector2 (0.5f, 0.5f));
	}

	void Start () 
	{
		Camera.main.GetComponent<AudioSource>().volume = 0.5f + ds.m_fMusicVolume;
		//load enemies
		List<EncounterGroupLoaderScript.cEnemyData> enemies = ds.GetEnemyNames();
		int FormationCounter = 0;
		foreach(EncounterGroupLoaderScript.cEnemyData e in enemies)
		{
			string fullPath = m_szFirstPart + e.m_szEnemyName + "/" + e.m_szEnemyName;
			GameObject loadedEnemy = Resources.Load<GameObject>(fullPath);
			GameObject enemy = Instantiate(loadedEnemy) as GameObject;
			//remove the "(clone)"
			enemy.name = loadedEnemy.name;
			//Set it's position on the field accordingly
			enemy.GetComponent<UnitScript>().FieldPosition = e.m_nFormationIter;
			enemy.GetComponent<UnitScript>().SetUnitLevel(loadedEnemy.GetComponent<UnitScript>().GetUnitLevel());
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
			Ally.GetComponent<UnitScript>().SetMaxMP(g.m_nMaxMP);
			Ally.GetComponent<UnitScript>().SetCurMP(g.m_nCurMP);
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
			FormationCounter++;
		}
		for(int i = 0; i < allies.Count; ++i)
		{
			m_lPartyPanels.Add(GameObject.Find("Character"+i));
			InitializePortraits(m_lPartyPanels[i].GetComponent<CharacterPanelContainer>(), GameObject.Find(allies[i].m_szCharacterName).GetComponent<CAllyBattleScript>());
		}
		for(;FormationCounter < 6; ++FormationCounter)
		{
			GameObject.Find("Character"+FormationCounter).SetActive(false);
		}

		m_bHasStarted = false;
		ds.SetMasterVolume();
		GetComponent<AudioSource>().PlayOneShot(m_lACMuicFiles[ds.m_nMusicIter], 0.5f + ds.m_fMusicVolume); 
	}

	void UpdateCharacterPanel(CharacterPanelContainer _cpc, CAllyBattleScript _c)
	{
		if(_cpc == null || _c == null)
			return;

		_cpc.m_goCharacterName.GetComponent<Text>().text = _c.name;
		_cpc.m_goCharacterLevel.FindChild("Text").GetComponent<Text>().text = _c.GetUnitLevel().ToString();
		_cpc.m_goCharacterEXP.FindChild("Text").GetComponent<Text>().text = _c.m_nCurrentExperience.ToString();
		_cpc.m_goCharacterMaxHP.GetComponent<Text>().text = _c.GetMaxHP().ToString();
		//The rest is the mess that is figuring out the current hp in regards to digits and displaying the cur health value in a color coded manner... I really never want to look at this code again, lol
		#region HealthMess
		float fPercentHealthLeft = (float)(_c.GetCurHP() / _c.GetMaxHP());
		Color cHealthColor = Color.green;
		if(fPercentHealthLeft < 0.3f)
			cHealthColor = Color.red;
		else if(fPercentHealthLeft < 0.6f)
			cHealthColor = Color.yellow;
		int nDigitCount = (int)(Mathf.Log10(_c.GetCurHP()) +1);

		for(int i = 0; i < nDigitCount; ++i)
		{
			Transform _tDigit = null;
			switch(i)
			{
			case 0:
				{
					_tDigit = _cpc.m_goCharacterCurHP.FindChild("Single");
				}
				break;
			case 1:
				{
					_tDigit = _cpc.m_goCharacterCurHP.FindChild("Ten");
				}
				break;
			case 2:
				{
					_tDigit = _cpc.m_goCharacterCurHP.FindChild("Hundred");
				}
				break;
			case 3:
				{
					_tDigit = _cpc.m_goCharacterCurHP.FindChild("Thousand");
				}
				break;
			}
			_tDigit.GetComponent<Text>().text = _c.GetCurHP().ToString()[_c.GetCurHP().ToString().Length-i-1].ToString();
			_tDigit.GetComponent<Text>().color = cHealthColor;
		}
		for(int i = nDigitCount+1; i < 5; ++i)
		{
			switch(i)
			{
			case 2:
				{
					Transform _tDigit = _cpc.m_goCharacterCurHP.FindChild("Ten");
					_tDigit.GetComponent<Text>().text = "";
				}
				break;
			case 3:
				{
					Transform _tDigit = _cpc.m_goCharacterCurHP.FindChild("Hundred");
					_tDigit.GetComponent<Text>().text = "";
				}
				break;
			case 4:
				{
					Transform _tDigit = _cpc.m_goCharacterCurHP.FindChild("Thousand");
					_tDigit.GetComponent<Text>().text = "";
				}
				break;
			}
		}
		#endregion

	}

	IEnumerator  GainExp (GameObject gUnit) 
	{
		while(m_lNewExperienceTotal[gUnit.name] > 0)
		{
			int remainder = m_lNewExperienceTotal[gUnit.name] - m_fExpTickSpeed;
			if(remainder < 0)
			{
				gUnit.GetComponent<CAllyBattleScript>().m_nCurrentExperience += remainder + m_fExpTickSpeed;
				m_lNewExperienceTotal[gUnit.name] -= m_fExpTickSpeed;
			}
			else
			{
				gUnit.GetComponent<CAllyBattleScript>().m_nCurrentExperience += m_fExpTickSpeed;
				m_lNewExperienceTotal[gUnit.name] -= m_fExpTickSpeed;
			}
			if(gUnit.GetComponent<CAllyBattleScript>().m_nCurrentExperience >= gUnit.GetComponent<CAllyBattleScript>().m_nExperienceToLevel)
			{
				//Level!
				gUnit.GetComponent<CAllyBattleScript>().LevelUp();
				gUnit.GetComponent<CAllyBattleScript>().m_nCurrentExperience = 0;
				
				
				GameObject Catch = Instantiate(m_gLevelUpObj) as GameObject;
				Vector3 pos = gUnit.transform.position;
				pos.z -= 0.1f;
				Catch.transform.position = pos;
				Destroy(Catch, 1.35f);
			}
			foreach (GameObject panel in m_lPartyPanels) {
				if (panel.transform.FindChild ("Character Name").GetComponent<Text> ().text == gUnit.name) {
					UpdateCharacterPanel (panel.GetComponent<CharacterPanelContainer> (), gUnit.GetComponent<CAllyBattleScript> ());
				}
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
		messageWindow.GetComponent<MessageWindowScript>().AddMessage(p_szMessage);
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

			if(m_bOneShotAfterVictory == false)
			{
				m_bOneShotAfterVictory = true;
				GameObject.Find("Party").GetComponent<Animator>().Play("PartyRoster_SlideIn");
				GameObject ItemsWon = GameObject.Find("Items Won");
				GameObject GoldDropped = ItemsWon.transform.FindChild("GoldWon").gameObject;
				GoldDropped.GetComponentInChildren<Text>().text = "Gold dropped: " + m_nGoldDropped.ToString();
				ItemsWon.GetComponent<Animator>().Play("ItemsWon_SlideIn");
				if(m_lItemsWon.Count > 0)
				{
					int _nItemCount = 1;
					Transform itemsWonPanel = ItemsWon.transform.FindChild("Panel");
					foreach(ItemLibrary.CharactersItems ciItem in m_lItemsWon)
					{
						Transform itemObject = itemsWonPanel.FindChild("Item"+_nItemCount.ToString());
						itemObject.FindChild("ItemName").GetComponent<Text>().text = ciItem.m_szItemName;
						itemObject.FindChild("ItemCount").GetComponent<Text>().text = ciItem.m_nItemCount.ToString();
						++_nItemCount;
					}
				}

				foreach(GameObject Ally in Allies)
				{
					if(Ally.name.Contains("Clone") == false)
					{
						StartCoroutine("GainExp", Ally);
					}
				}
			}

			if((Input.GetKeyUp(KeyCode.Return) || Input.GetMouseButtonDown(0)) && m_VictoryScreenOneShot == false)
			{
				//If this is the first time the player hits the button during the victory screen, just stop animating things and show the final results
				if(m_bHasPressedEnter == false)
				{
					m_bHasPressedEnter = true;
					foreach(GameObject Ally in Allies)
					{
						Ally.GetComponent<CAllyBattleScript>().m_nCurrentExperience = m_lNextExperience[Ally.name];
						int remainder = m_lNextLevel[Ally.name] - Ally.GetComponent<CAllyBattleScript>().GetUnitLevel();
						while(remainder > 0)
						{
							Ally.GetComponent<CAllyBattleScript>().LevelUp();
							remainder--;
						}
						m_lNewExperienceTotal[Ally.name] = 0;
						foreach(GameObject panel in m_lPartyPanels)
						{
							if(panel.transform.FindChild("Character Name").GetComponent<Text>().text == Ally.name)
							{
								UpdateCharacterPanel(panel.GetComponent<CharacterPanelContainer>(), Ally.GetComponent<CAllyBattleScript>());
							}
						}
					}
				}
				//If this is the second time the player hits the button(or if the animations are over), fade and end the battle scene
				else
				{
					//just move on
					m_VictoryScreenOneShot = true;
					BeginFading();
					FadeInOutSound obj = Camera.main.GetComponent<FadeInOutSound>();
					StartCoroutine(obj.FadeAudio(1.0f, FadeInOutSound.Fade.Out));
					Invoke("Finish", 1.0f);
				}
				Input.ResetInputAxes();
			}
				//GUI.Label(new Rect(fXAdjust, fYAdjust, 40.0f, 25.0f),"Lv: " +  m_lPreviousLevels[Ally.name].ToString());
				//Display their current exp just to the right
				//int digitCount = (int)(Mathf.Log10( m_lPreviousExperience[Ally.name]) +1);
				//GUI.Label(new Rect(fXAdjust, fYAdjust, 30.0f, 25.0f), m_lPreviousExperience[Ally.name].ToString());
		}
	}

	public void MyTurnIsOver(GameObject go)
	{
		if(m_bIsBattleOver == true)
			return;
		//do a premeditated check to see if the battle is going to end (i.e. if everyone's hp is at 0, that way we don't start another persons turn for no reason.
		int aliveEnemies = 0;
		int aliveAllies = 0;
		foreach(GameObject unit in m_goUnits)
		{
			if(unit.GetComponent<UnitScript>().m_nUnitType <= UnitScript.UnitTypes.NPC)
			{
				if(unit.GetComponent<UnitScript>().GetCurHP() > 0)
				{
					aliveAllies++;
					continue;
				}
			}
			if(unit.GetComponent<UnitScript>().m_nUnitType > UnitScript.UnitTypes.NPC)
			{
				if(unit.GetComponent<UnitScript>().GetCurHP() > 0)
				{
					aliveEnemies++;
					continue;
				}
			}

		}
		if(aliveAllies == 0 || aliveEnemies == 0)
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
		if(go.GetComponent<UnitScript>().m_bIsMyTurn == true)
		{
			m_nOrderIter++;
			if(m_nOrderIter >= m_goUnits.Count)
				m_nOrderIter = 0;
			m_goUnits[m_nOrderIter].GetComponent<UnitScript>().m_bIsMyTurn = true;
		}
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
		if(go.GetComponent<UnitScript>().m_nUnitType > UnitScript.UnitTypes.NPC)
		{
			//add the enemies level to the list of levels that have been defeated.
			m_lExperienceToAward.Add(go.GetComponent<UnitScript>().GetUnitLevel());
			//add any items this enemy can drop to the list of items that can drop
			foreach(StandardEnemyScript.DroppedItem item in go.GetComponent<StandardEnemyScript>().m_lItemsThatCanDrop)
			{
				m_lItemsThatCanDrop.Add(item);
			}
			//removing an enemy
			m_nEnemyCount--;
			m_nGoldDropped += go.GetComponent<StandardEnemyScript>().m_nGoldDropped;
			if(m_nEnemyCount <= 0)
			{
				DisableCharacterHealthPanels();
				RollForItems();
				m_bIsBattleOver  = true;
				m_goActionSelector.SetActive(false);
				Win();
			}
		}
		else
		{
			m_nAllyCount--;
			if(m_nAllyCount <= 0)
			{
				DisableCharacterHealthPanels();
				m_goActionSelector.SetActive(false);
				Invoke("BeginFading", m_fFadeDuration);
				Invoke("Lose", 3.0f + m_fFadeDuration);
			}
		}
	}

	void DisableCharacterHealthPanels()
	{
		GameObject[] panels = GameObject.FindGameObjectsWithTag("Finish");
		panels[0].transform.parent.gameObject.SetActive(false);
		foreach(GameObject go in panels)
			go.SetActive(false);
	}

	public void AddMeToList(GameObject go)
	{
		//TODO  perhaps adjust incase the list needs alteration when someone joins the fight?
		m_goUnits.Add(go);
		if(go.GetComponent<UnitScript>().m_nUnitType > UnitScript.UnitTypes.ALLY_RANGED)
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

	void RollForItems()
	{
		List<StandardEnemyScript.DroppedItem> itemsWon = new List<StandardEnemyScript.DroppedItem>();
		int roll = Random.Range(0, 100);
		int _nItemsWon = 0;
		foreach(ItemChances chance in m_dnItemCountChances)
		{
			if(roll <= chance.m_nChance)
			{
				_nItemsWon = chance.m_nAmountOfItems;
				break;
			}
			else
			{
				roll -= chance.m_nChance;
			}
		}
		for(int i = 0; i < _nItemsWon; ++i)
		{
			//100% for each unit that was defeated.
			int totalChance = m_lExperienceToAward.Count * 100;
			roll = Random.Range(1, totalChance);
			
			foreach(StandardEnemyScript.DroppedItem item in m_lItemsThatCanDrop)
			{
				if(roll <= item.m_nChance)
				{
					//found the item that was won
					itemsWon.Add(item);
					break;
				}
				else
					roll -= item.m_nChance;
			}

		}
		foreach(StandardEnemyScript.DroppedItem item in itemsWon)
		{
			if(item.m_szItemName == "")
			{
				//nothing was won
				Debug.Log("Won nothing");
			}
			else
			{
				ItemLibrary.ItemData idItem = ds.m_lItemLibrary.GetItemFromDictionary(item.m_szItemName);
				ItemLibrary.CharactersItems newItem = new ItemLibrary.CharactersItems();
				newItem.m_szItemName = idItem.m_szItemName;
				newItem.m_nItemType = idItem.m_nItemType;
				newItem.m_nItemCount = item.m_nAmount;
				ds.m_lItemLibrary.AddItem(newItem);
				m_lItemsWon.Add(newItem);
			}
		}
	}

	void Win()
	{
		//Play victory fanfare
		GetComponent<AudioSource>().Stop();
		GetComponent<AudioSource>().PlayOneShot(m_acVictoryFanfare, 0.5f + ds.m_fSFXVolume);
		//award experience to the allies on field.
		ds.m_nGold += m_nGoldDropped;
		List<DCScript.CharacterData> party = ds.GetParty();
		foreach(DCScript.CharacterData ally in party)
		{
			GameObject foundAlly = GameObject.Find(ally.m_szCharacterName);

			//Award experience (their script will check if it levels and return a bool .. maybe I want to do some level up effect?  Not sure..
			int nextExp = 0;
			int nextLvl = 0;
			int nExp = foundAlly.GetComponent<CAllyBattleScript>().AwardExperience(m_lExperienceToAward, ref nextExp, ref nextLvl);
			m_lNextExperience.Add(ally.m_szCharacterName, nextExp);
			m_lNextLevel.Add(ally.m_szCharacterName, nextLvl);
			//add the total experience the character is going to gain to the list
			m_lNewExperienceTotal.Add(ally.m_szCharacterName, nExp);

		}
		//ds.SetParty(party);

	}

	public void Escape()
	{
		BeginFading();
		FadeInOutSound obj = Camera.main.GetComponent<FadeInOutSound>();
		StartCoroutine(obj.FadeAudio(1.0f, FadeInOutSound.Fade.Out));
		Invoke("Finish", 1.0f);
	}
	//Called just before finishing this scene, updates the data canister's party to reflect any leveling, status effects, or equipment changes.
	void UpdatePartyStatsForScreenSwitch()
	{
		List<DCScript.CharacterData> party = ds.GetParty();
		foreach(DCScript.CharacterData ally in party)
		{
			GameObject foundAlly = GameObject.Find(ally.m_szCharacterName);
			ally.m_nMaxHP = foundAlly.GetComponent<CAllyBattleScript>().GetMaxHP();
			ally.m_nSTR = foundAlly.GetComponent<CAllyBattleScript>().GetSTR();
			ally.m_nDEF = foundAlly.GetComponent<CAllyBattleScript>().GetDEF();
			ally.m_nSPD = foundAlly.GetComponent<CAllyBattleScript>().GetSPD();
			if(ally.m_nLevel != foundAlly.GetComponent<CAllyBattleScript>().GetUnitLevel())
			{
				ally.m_nCurHP = ally.m_nMaxHP;
				ally.m_nCurMP = ally.m_nMaxMP;
			}
			else
			{
				ally.m_nCurHP = foundAlly.GetComponent<CAllyBattleScript>().GetCurHP();
				ally.m_nCurMP = foundAlly.GetComponent<CAllyBattleScript>().GetCurMP();
			}
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
		//ds.SetParty(party);
	}
	void Finish()
	{
		UpdatePartyStatsForScreenSwitch();
		//grab each status effect and add it back to the list in the data canister, if the status effect already exists, add the character to the list of effected characters.
		GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");
		foreach(GameObject Ally in Allies)
		{
			foreach(GameObject effect in Ally.GetComponent<UnitScript>().m_lStatusEffects)
			{
				int catchIter = ds.IsStatusEffectInList(effect.GetComponent<BattleBaseEffectScript>().m_nEffectType);
				if( catchIter != -1)
				{
					//effect already exists in list
					bool bAlreadyInList = false;
					foreach(DCScript.StatusEffect.cEffectedMember _member in ds.GetStatusEffects()[catchIter].m_lEffectedMembers)
					{
						if(_member.m_szCharacterName == Ally.name)
						{
							_member.m_nTicksLeft = effect.GetComponent<BattleBaseEffectScript>().m_nAmountOfTicks;
							bAlreadyInList = true;
						}
					}
					if(bAlreadyInList == false)
					{
						//character is not on the list of effected characters
						ds.GetStatusEffects()[catchIter].m_lEffectedMembers.Add(new DCScript.StatusEffect.cEffectedMember(Ally.name, effect.GetComponent<BattleBaseEffectScript>().m_nAmountOfTicks));
					}
					effect.GetComponent<BattleBaseEffectScript>().RefreshEffect(ds.GetStatusEffects()[catchIter]);
					ds.AddStatusEffect(effect.GetComponent<BattleBaseEffectScript>().ConvertToDCStatusEffect());
				}
				else
				{
					//effect needs to be added
					DCScript.StatusEffect se = new DCScript.StatusEffect();
					BattleBaseEffectScript e = effect.GetComponent<BattleBaseEffectScript>();
					se.m_szEffectName = e.name;
					se.m_nEffectType = e.m_nEffectType;
					se.m_nPOWMod = e.m_nPOWMod;
					se.m_nDEFMod = e.m_nDEFMod;
					se.m_nSPDMod = e.m_nSPDMod;
					se.m_nHITMod = e.m_nHITMod;
					se.m_nEVAMod = e.m_nEVAMod;
					se.m_lEffectedMembers.Add(new DCScript.StatusEffect.cEffectedMember(Ally.name, e.m_nAmountOfTicks));
					ds.AddStatusEffect(se);
				}
			}
		}
		string previousField = ds.GetPreviousFieldName();
		ds.SetPreviousFieldName(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(previousField);
	}
	void Lose()
	{
        SceneManager.LoadScene("GameOver_Scene");
	}

	public GameObject FindStatusEffect(string szName)
	{
		foreach(GameObject go in m_lStatusEffectList)
		{
			if(go.name == szName)
				return go;
		}
		return null;
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
				break;
			}
		}
	}

	public void Enemy_TargetHighlighted(int p_nIndex)
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
							if(unit.GetComponent<UnitScript>().m_nUnitType <= UnitScript.UnitTypes.NPC)
								unit.GetComponent<CAllyBattleScript>().ChangeEnemyTarget(p_nIndex);
							return;
						}
					}
				}
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
							if(unit.GetComponent<UnitScript>().m_nUnitType <= UnitScript.UnitTypes.NPC)
								unit.GetComponent<CAllyBattleScript>().EnemyToAttackSelected(p_nIndex);
							return;
						}
					}
				}
			}
		}
	}

	public void Ally_TargetHighlighted(int p_nIndex)
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
						if(b.GetComponent<UnitScript>().m_nState == (int)CAllyBattleScript.ALLY_STATES.ITEM_PICKED_SINGLEHEAL)
						{
							if(a.GetComponent<UnitScript>().GetCurHP() == 0)
							{
								if(b.GetComponent<CAllyBattleScript>().m_goItemBeingUsed.GetComponent<BaseItemScript>().m_bCanTargetDeadUnits == true)
								{
									//in here it means that you're using an item that can target a dead person, and you've highlighted over a dead person.. so.. target them ;)
									b.GetComponent<CAllyBattleScript>().ChangeAllyTarget(p_nIndex);
								}
							}
							else
							{
								b.GetComponent<CAllyBattleScript>().ChangeAllyTarget(p_nIndex);
							}
						}
						else if(b.GetComponent<UnitScript>().m_nState == (int)CAllyBattleScript.ALLY_STATES.SPELL_PICKED_SINGLEHEAL)
						{
							if(a.GetComponent<UnitScript>().GetCurHP() == 0)
							{
								//If the unit is dead, check to see if this spell can target dead people. haha.. can you see dead people?
							}
							else
							{
								b.GetComponent<CAllyBattleScript>().ChangeAllyTarget(p_nIndex);
							}
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
						b.GetComponent<CAllyBattleScript>().AllyToActSelected(p_nIndex);
					}
				}
			}
		}
	}

}
