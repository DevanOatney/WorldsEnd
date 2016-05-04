using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VoiceVolumeSliderScript : MonoBehaviour {

	private float m_fSliderValue = 0.0f;
	public AudioClip m_aSFXSound;
	public GameObject exitButton;
	public GameObject m_goSlider;
	bool m_bHasChanged = false;
	// Use this for initialization
	void Start () 
	{
		m_fSliderValue = 0.5f;
		GameObject GO = GameObject.Find("PersistantData");
		if(GO != null)
		{
			m_fSliderValue = 0.5f + GO.GetComponent<DCScript>().m_fVoiceVolume;
		}
		m_goSlider.GetComponent<Slider>().value = m_fSliderValue;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bHasChanged)
		{
			if(Input.GetMouseButtonUp(0))
			{
				m_bHasChanged = false;
				GetComponent<AudioSource>().Stop();
				GameObject GO = GameObject.Find("PersistantData");
				if(GO != null)
				{
					GetComponent<AudioSource>().PlayOneShot(m_aSFXSound, 0.5f + GO.GetComponent<DCScript>().m_fVoiceVolume);
				}
			}
		}
	}
	public void ValueChanged(float _fValue)
	{
		if(exitButton.GetComponent<ExitButtonScript>().m_bSwitching == false)
		{
			m_bHasChanged = true;
			GameObject gos;
			gos = GameObject.Find("SFX Volume Slider");
			if(gos)
			{
				gos.GetComponent<AudioSource>().volume = _fValue;
			}
			GameObject GO = GameObject.Find("PersistantData");
			if(GO != null)
			{
				GO.GetComponent<DCScript>().m_fVoiceVolume = _fValue - 0.5f;
				GO.GetComponent<DCScript>().AdjustValues();
			}
		}
		
	}
}
