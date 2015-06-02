using UnityEngine;
using System.Collections;

public class TreasureChestScript : MonoBehaviour {
	public AnimationClip m_acOpeningAnim;
	float m_fAnimLength;
	bool m_bIsOpening = false;
	public void SetIsOpening( bool b) { m_bIsOpening = b;}
	bool m_bIsOpened = false;

	DCScript.CharactersItems m_ciItemHeld;
	public DCScript.CharactersItems GetItemHeld() {return m_ciItemHeld;}
	public void SetItemHeld(DCScript.CharactersItems item){m_ciItemHeld = item;}
	//the amount of the item held within to give to the player
	public int m_nAmountofItem = 1;
	Vector2 m_vSizeOfMessage;

	Vector3 positionOfPlayer = new Vector3();

	float m_fBoxOpacity = 1.0f;
	bool m_bMessagePopped = false;
	//Temp for testing
	public string m_szItemName;
	public AudioClip m_acOpeningSound;
	// Use this for initialization
	void Start () 
	{
		//TEMP FOR TESTING
		m_ciItemHeld = new DCScript.CharactersItems();
		m_ciItemHeld.m_szItemName = m_szItemName;
		m_ciItemHeld.m_nItemCount = Random.Range(1, 4);
		//

		m_fAnimLength = m_acOpeningAnim.length;
		m_vSizeOfMessage = new Vector2(m_ciItemHeld.m_szItemName.Length * 17, 25);
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
				//TODO : Perhaps change this if there is a limited bag space.
				GameObject.Find("PersistantData").GetComponent<DCScript>().AddItem(m_ciItemHeld);
				GameObject.Find("PersistantData").GetComponent<DCScript>().m_dStoryFlagField.Add(name, 1);
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
				GetComponent<AudioSource>().PlayOneShot(m_acOpeningSound, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
				GameObject player = c.GetComponent<InterractingBoxScript>().GetOwner();
				positionOfPlayer = player.transform.position;
				positionOfPlayer.x = player.GetComponent<BoxCollider>().bounds.min.x;
				positionOfPlayer.y += player.GetComponent<BoxCollider>().bounds.size.y * 0.9f;

			}
		}
	}

	void OnGUI()
	{
		if(m_bIsOpened == true && m_bMessagePopped == false)
		{
			m_fBoxOpacity *= 0.9985f;
			GUI.color = new Color(1.0f, 1.0f, 1.0f, m_fBoxOpacity); //0.5 is half opacity 
			Vector2 pos = Camera.main.WorldToScreenPoint(positionOfPlayer);
			pos.x = (pos.x - m_vSizeOfMessage.x * 0.5f) + GameObject.Find("Player").GetComponent<BoxCollider>().bounds.size.x;
			pos.y = Screen.height - pos.y;
			GUI.Box(new Rect(pos.x, pos.y, m_vSizeOfMessage.x + 50.0f, m_vSizeOfMessage.y), "You have found " + m_nAmountofItem + " " + m_ciItemHeld.m_szItemName);
			GUI.color = Color.white;

			if(m_fBoxOpacity <= 0.1f)
				m_bMessagePopped = true;
		}
	}
}
