using UnityEngine;
using System.Collections;

public class TreasureChestScript : MonoBehaviour {
	public AnimationClip m_acOpeningAnim;
	float m_fAnimLength;
	bool m_bIsOpening = false;
	public void SetIsOpening( bool b) { m_bIsOpening = b;}
	public bool m_bIsOpened = false;

	ItemLibrary.CharactersItems m_ciItemHeld;
	public ItemLibrary.CharactersItems GetItemHeld() {return m_ciItemHeld;}
	public void SetItemHeld(ItemLibrary.CharactersItems item){m_ciItemHeld = item;}
	//the amount of the item held within to give to the player
	public int m_nAmountofItem = 1;

	//Temp for testing
	public string m_szItemName;
	public AudioClip m_acOpeningSound;
	// Use this for initialization
	void Start () 
	{
		m_ciItemHeld = new ItemLibrary.CharactersItems();
		m_ciItemHeld.m_szItemName = m_szItemName;
		m_ciItemHeld.m_nItemCount = m_nAmountofItem;

		m_fAnimLength = m_acOpeningAnim.length;
		int result;
		if(GameObject.Find("PersistantData").GetComponent<DCScript>().m_dStoryFlagField.TryGetValue(name, out result))
		{
			m_bIsOpened = true;
			Animator chestAnim = GetComponent<Animator>();
			chestAnim.SetBool("m_bAlreadyOpened", true);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bIsOpening == true && m_bIsOpened == false)
		{
			m_fAnimLength -= Time.deltaTime;
			if(m_fAnimLength <= 0)
			{
				m_bIsOpened = true;
				if(m_ciItemHeld.m_szItemName != "Spyr")
				{
					GameObject.Find("PersistantData").GetComponent<DCScript>().m_lItemLibrary.AddItem(m_ciItemHeld);
					GameObject.Find("PersistantData").GetComponent<DCScript>().m_dStoryFlagField.Add(name, 1);
				}
				else
				{
					GameObject.Find("PersistantData").GetComponent<DCScript>().m_nGold += m_ciItemHeld.m_nItemCount;
					GameObject.Find("PersistantData").GetComponent<DCScript>().m_dStoryFlagField.Add(name, 1);
				}
				string szMessage = m_ciItemHeld.m_nItemCount + " " + m_ciItemHeld.m_szItemName;
				GameObject.Find("UI_Alerts").GetComponent<UIAlertWindowScript>().ActivateWindow(UIAlertWindowScript.MESSAGEID.eITEMREWARD, szMessage, null);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if(c.name == "Action Box(Clone)")
		{
			if(m_bIsOpened == false)
			{
				Animator chestAnim = GetComponent<Animator>();
				if(chestAnim.GetBool("m_bIsOpening") == false)
					chestAnim.SetBool("m_bIsOpening", true);
				m_bIsOpening = true;
				if(m_acOpeningSound != null)
					GetComponent<AudioSource>().PlayOneShot(m_acOpeningSound, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
			}
		}
	}
}
