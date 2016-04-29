using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenSwitchScript : MonoBehaviour 
{
	//name of scene to switch to, if instead it's just a different part of this scene to teleport to, set this string to NULL
	public string m_szSceneName = "";
	//If it's not a scene switch, this is the location the player will go to, send camera a fade out call, invoke a delayed call to fade back in and change the player position
	//to the new location and set the correct facing
	public GameObject m_goNextLocation;
	//New direction to face, default to -1 if no new facing
	public int m_nNewFacingDir = -1;
	public AudioClip m_acMusic = null;
	//If we're scene switching, which starting point should we go to?
	public string m_szStartingLocToGoTo = "";
	bool m_bHasJustAppeared = false;
	public void SetAppearFlag(bool flag) {m_bHasJustAppeared = flag;}
	void TurnOffFlag() {m_bHasJustAppeared = false;}


	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if(m_bHasJustAppeared == false)
		{
			if(c.name == "Player")
			{
				Camera.main.SendMessage("fadeOut");
				Invoke("ChangePlaces", 3.0f);
				GameObject player = GameObject.Find("Player");
				FieldPlayerMovementScript fpmScript = player.GetComponent<FieldPlayerMovementScript>();
				//Suspend input while fading
				player.GetComponent<FieldPlayerMovementScript>().BindInput();
				fpmScript.SetState((int)FieldPlayerMovementScript.States.eIDLE);
				fpmScript.GetAnimator().SetBool("m_bRunButtonIsPressed", false);
				fpmScript.SetIsRunning(false);
				fpmScript.ResetAnimFlagsExcept(-1);

			}
		}
	}

	void ChangePlaces()
	{
		if(m_szSceneName != "")
		{
			DCScript dcs = GameObject.Find("PersistantData").GetComponent<DCScript>();
			if(m_nNewFacingDir != -1)
				dcs.SetPreviousFacingDirection(m_nNewFacingDir);
			dcs.SetStartingPos(m_szStartingLocToGoTo);

			GameObject player = GameObject.Find("Player");
			dcs.GetStatusEffects().Clear();
			foreach(GameObject status in player.GetComponent<FieldPlayerMovementScript>().m_lStatusEffects)
			{
				DCScript.StatusEffect se = new DCScript.StatusEffect();
				se.m_szEffectName = status.name;
				se.m_nHPMod = status.GetComponent<FieldBaseStatusEffectScript>().m_nMod;
				se.m_lEffectedMembers = status.GetComponent<FieldBaseStatusEffectScript>().m_lEffectedUnits;
				dcs.AddStatusEffect(se);
			}

            SceneManager.LoadScene(m_szSceneName);
		}
		else
		{
			if(m_acMusic != null)
			{
				Camera.main.GetComponent<AudioSource>().Stop();
				Camera.main.GetComponent<AudioSource>().clip = m_acMusic;
				Camera.main.GetComponent<AudioSource>().Play();
			}
			m_goNextLocation.GetComponent<ScreenSwitchScript>().SetAppearFlag(true);
			m_goNextLocation.GetComponent<ScreenSwitchScript>().Invoke("TurnOffFlag", 0.1f);
			GameObject player = GameObject.Find("Player");
			if(m_nNewFacingDir != -1)
				player.GetComponent<FieldPlayerMovementScript>().m_nFacingDir = m_nNewFacingDir;
			player.GetComponent<FieldPlayerMovementScript>().GetAnimator().SetInteger("m_nFacingDir", m_nNewFacingDir);
			Vector3 newPos = m_goNextLocation.transform.position;
			switch(m_nNewFacingDir)
			{
			case 0:
				newPos.y -= player.GetComponent<BoxCollider2D>().bounds.size.y;
				break;
			case 1:
				newPos.x -= player.GetComponent<BoxCollider2D>().bounds.size.x;
				break;
			case 2:
				newPos.x += player.GetComponent<BoxCollider2D>().bounds.size.x;
				break;
			case 3:
				newPos.y += player.GetComponent<BoxCollider2D>().bounds.size.y *2;
				break;
			}
			player.transform.position = newPos;
			Camera.main.SendMessage("fadeIn");
			GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
		}
	}
}
