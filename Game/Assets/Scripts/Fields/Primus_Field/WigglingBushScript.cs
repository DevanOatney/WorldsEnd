using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WigglingBushScript : MonoBehaviour 
{
	bool m_bHasBeenInterractedWith = false;
	public List<AudioClip> m_Greetings;
	public List<AudioClip> m_Farewells;
	public AudioClip m_FinalAudio;
	public AudioClip m_acCoin;
	public AudioClip m_acFailToBuy;

	bool m_bHasFinishedGreetings = false;
	bool m_bHasFinishedFarewells = false;
	bool m_bHasPlayedLastAudio = false;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider c)
	{
		if(c.name == "Action Box(Clone)")
		{
			c.enabled = false;
			//toggle flag to display the GUI

			if(m_bHasPlayedLastAudio == false)
			{
				if(m_bHasFinishedGreetings == true && m_bHasFinishedFarewells == true)
				{
					//Play the last audio then flip the flag
					GetComponent<AudioSource>().Stop();
					GetComponent<AudioSource>().PlayOneShot(m_FinalAudio, 0.5f+ GameObject.Find("PersistantData").GetComponent<DCScript>().m_fVoiceVolume);
					m_bHasPlayedLastAudio = true;
					m_bHasBeenInterractedWith = true;

				}
				else
				{
					GetComponent<AudioSource>().Stop();
					m_bHasBeenInterractedWith = true;
					PlayGreeting();
				}

				
			}
		}
	}

	void OnGUI()
	{
		if(m_bHasBeenInterractedWith == true)
		{
			//not sure yet.... maybe make this a vendor?
			//Make sure no other input is allowed from the player
			FieldPlayerMovementScript fpm = GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>();

			Vector2 size = new Vector2(200, 200);
			//This stuff is going to be in the middle of the screen, just below the character.. whatever it is, lol
			GUI.BeginGroup(new Rect(Screen.width * 0.5f - size.x * 0.5f, Screen.height * 0.7f - size.y * 0.5f, size.x, size.y));
			//Let's draw a background gui stuff.. hm, I kind of want to use a texture
			GUI.Box(new Rect(0, 0, size.x, size.y), "");
			int labelSize = GUI.skin.label.fontSize;
			int boxLabelSize = GUI.skin.box.fontSize;
			GUI.skin.label.fontSize = 16;
			GUI.skin.button.fontSize = 20;
			GUI.Label(new Rect(size.x * 0.5f-50, size.y * 0.2f, 100, 75.0f), "Would you like to buy a shrubbery for      10 exp?");
			if(GUI.Button(new Rect(size.x * 0.2f, size.y * 0.8f, 50, 25), "Yes"))
			{
				GetComponent<AudioSource>().Stop();
				//purchase a shubbery, close the window, play farewell dialogue  !!! IF HE CAN AFFORD IT !!!
				DCScript dcs = GameObject.Find("PersistantData").GetComponent<DCScript>();
				foreach(DCScript.CharacterData partyMember in dcs.GetParty())
				{
					if(partyMember.m_szCharacterName == "Matt")
					{
						if(partyMember.m_nCurrentEXP >= 10)
						{
							//Player can afford the -10 experience, subtract it, add the item to the inventory, and play a little "Cha-Ching" sfx to show it worked.
							partyMember.m_nCurrentEXP = partyMember.m_nCurrentEXP - 10;
							AddItemToPlayersInventory("Shrubbery", 1);


							PlayFarewell();
							fpm.ReleaseBind();
							m_bHasBeenInterractedWith = false;
						}
						else
						{
							//Don't add item or subtract exp.   Play a little buzzer sound to show it isn't working and remain on the screen.
						}
					}
				}

			}
			if(GUI.Button(new Rect(size.x * 0.6f, size.y * 0.8f, 50, 25), "No"))
			{
				GetComponent<AudioSource>().Stop();
				//close the window, play farewell dialogue
				PlayFarewell();
				fpm.ReleaseBind();
				m_bHasBeenInterractedWith = false;
			}
			GUI.EndGroup();

			GUI.skin.label.fontSize = labelSize;
			GUI.skin.box.fontSize = boxLabelSize;

		}
	}

	void AddItemToPlayersInventory(string szItemName, int nAmount)
	{
		DCScript dcs = GameObject.Find("PersistantData").GetComponent<DCScript>();
		foreach(KeyValuePair<string, ItemLibrary.ItemData> kvp in dcs.m_lItemLibrary.GetItemDictionary())
		{
			if(dcs.m_lItemLibrary.GetItemFromDictionary(szItemName) != null)
			{
				//Great success, add that amount of that item to the inventory
				ItemLibrary.CharactersItems m_ciItemHeld = new ItemLibrary.CharactersItems();
				m_ciItemHeld.m_szItemName = kvp.Key;
				m_ciItemHeld.m_nItemCount = nAmount;
				dcs.m_lItemLibrary.AddItem(m_ciItemHeld);
			}
			else
			{
				//Item isn't there.. um.. 
				Debug.Log ("Errawr retrieving item data from dictionary");
			}
		}
	}

	void PlayFarewell()
	{
		if(m_bHasFinishedFarewells == false)
		{
			int diceRoll = Random.Range(0, m_Farewells.Count);
			GetComponent<AudioSource>().Stop();
			GetComponent<AudioSource>().PlayOneShot(m_Farewells[diceRoll], 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fVoiceVolume);
			m_Farewells.RemoveAt(diceRoll);
			if(m_Farewells.Count <= 0)
			{
				m_bHasFinishedFarewells = true;
			}

		}
	}

	void PlayGreeting()
	{
		if(m_bHasFinishedGreetings == false)
		{
			int diceRoll = Random.Range(0, m_Greetings.Count);
			GetComponent<AudioSource>().Stop();
			GetComponent<AudioSource>().PlayOneShot(m_Greetings[diceRoll], 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fVoiceVolume);
			m_Greetings.RemoveAt(diceRoll);
			if(m_Greetings.Count <= 0)
			{
				m_bHasFinishedGreetings = true;
			}
		}
	}
}
