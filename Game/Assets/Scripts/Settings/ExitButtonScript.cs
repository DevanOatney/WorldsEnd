using UnityEngine;
using System.Collections;

public class ExitButtonScript : MonoBehaviour 
{
	private float m_fMasterVolume = 0.5f;
	private float m_fMusicVolume = 0.5f;
	private float m_fSFXVolume = 0.5f;
	private float m_fVoiceVolume = 0.5f;
	private float m_fBrightness = 4.0f;
	private bool m_bToUseBattleAnimations = true;
	public bool m_bSwitching = false;
	public int m_nTextSpeed = 2;
	Vector3 pos;
	public GUISkin skin;
	public AudioClip m_aSelectMenu;
	void Start()
	{
		pos = transform.position;
		pos = Camera.main.WorldToScreenPoint(pos);
		pos.y = Screen.height - pos.y;
		
		
		GameObject GO = GameObject.Find("PersistantData");
		if(GO != null)
		{
			DCScript script = GO.GetComponent<DCScript>();
			m_fMasterVolume = script.m_fMasterVolume;
			m_fMusicVolume = script.m_fMusicVolume;
			m_fSFXVolume = script.m_fSFXVolume;
			m_fVoiceVolume = script.m_fVoiceVolume;
			m_fBrightness = script.m_fBrightness; 
			m_bToUseBattleAnimations = script.m_bToUseBattleAnimations;
			m_nTextSpeed = script.m_nTextSpeed;
		}
	}
    void OnGUI() 
	{
        
    }
	void ChangeScreen()
	{
		GameObject GO = GameObject.Find("PersistantData");
		if(GO != null)
		{
			GO.GetComponent<DCScript>().m_fMasterVolume = m_fMasterVolume;
			GO.GetComponent<DCScript>().m_fMusicVolume = m_fMusicVolume;
			GO.GetComponent<DCScript>().m_fSFXVolume = m_fSFXVolume;
			GO.GetComponent<DCScript>().m_fVoiceVolume = m_fVoiceVolume;
			GO.GetComponent<DCScript>().m_fBrightness = m_fBrightness;
			GO.GetComponent<DCScript>().m_bToUseBattleAnimations = m_bToUseBattleAnimations;
			GO.GetComponent<DCScript>().m_nTextSpeed = m_nTextSpeed;
			GO.GetComponent<DCScript>().AdjustValues();
			GO.GetComponent<DCScript>().SetMasterVolume();
		}
		Application.LoadLevel("IntroMenu_Scene");
	}

	void OnMouseDown()
	{
		GameObject GO = GameObject.Find("PersistantData");
		if(GO != null)
		{
			DCScript script = GO.GetComponent<DCScript>();
			GetComponent<AudioSource>().PlayOneShot(m_aSelectMenu, script.m_fSFXVolume);
		}
		m_bSwitching = true;
		Invoke("ChangeScreen", 3.9f);
		Camera.main.SendMessage("fadeOut");
		FadeInOutSound obj = gameObject.GetComponent<FadeInOutSound>();
		StartCoroutine(obj.FadeAudio(4.0f, FadeInOutSound.Fade.Out));
	}
}
