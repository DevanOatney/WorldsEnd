using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MasterVolumeSliderScript : MonoBehaviour {
	
	private float m_fSliderValue = 0.5f;
	public GameObject exitButton;
	public GameObject m_goMusicVolume;
	public GameObject m_goMasterVolumeSlider;
	// Use this for initialization
	void Start () 
	{
		GameObject GO = GameObject.Find("PersistantData");
		if(GO != null)
		{
			m_fSliderValue = GO.GetComponent<DCScript>().m_fMasterVolume;
		}
		m_goMasterVolumeSlider.GetComponent<Slider>().value = m_fSliderValue;
	}
	
	public void ValueChanged(float _fValue)
	{
		GetComponent<AudioSource>().volume = m_goMusicVolume.GetComponent<MusicVolumeSliderScript>().m_fSliderValue;
		GameObject GO = GameObject.Find("PersistantData");
		if(GO != null)
		{
			if(exitButton.GetComponent<ExitButtonScript>().m_bSwitching == false)
			{
				GO.GetComponent<DCScript>().m_fMasterVolume = _fValue;
				GO.GetComponent<DCScript>().AdjustValues();
				GO.GetComponent<DCScript>().SetMasterVolume();
			}
		}
	}
}
