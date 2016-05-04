using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MusicVolumeSliderScript : MonoBehaviour {

	public float m_fSliderValue = 0.0f;
	public GameObject exitButton;
	public GameObject m_goSlider;
	// Use this for initialization
	void Start () 
	{
		m_fSliderValue = 0.5f;
		GameObject GO = GameObject.Find("PersistantData");
		if(GO != null)
		{
			m_fSliderValue = 0.5f + GO.GetComponent<DCScript>().m_fMusicVolume;
		}
		m_goSlider.GetComponent<Slider>().value = m_fSliderValue;
	}

	public void ValueChanged(float _fValue)
	{		
		if(exitButton.GetComponent<ExitButtonScript>().m_bSwitching == false)
		{
			GameObject gos;
			gos = GameObject.Find("Master Volume");
			if(gos)
			{
				gos.GetComponent<AudioSource>().volume = _fValue;
			}
			GameObject GO = GameObject.Find("PersistantData");
			if(GO != null)
			{
				GO.GetComponent<DCScript>().m_fMusicVolume = _fValue - 0.5f;
				GO.GetComponent<DCScript>().AdjustValues();
			}
		}
	}
}
