using UnityEngine;
using System.Collections;

public class VoiceVolumeSliderScript : MonoBehaviour {

	private float m_fSliderValue = 0.0f;
	public float m_fMinRange = 0.0f;
	public float m_fMaxRange = 1.0f;
	public float m_fBoxXPos = 0.0f;
	public float m_fBoxYPos = 0.0f;
	public float m_fBoxHeight = 10.0f;
	public float m_fBoxWidth = 75;
	public float m_fOldSliderValue;
	public AudioClip m_aSFXSound;
	public GameObject exitButton;
	// Use this for initialization
	void Start () 
	{
		GameObject GO = GameObject.Find("PersistantData");
		if(GO != null)
		{
			m_fSliderValue = 0.5f + GO.GetComponent<DCScript>().m_fVoiceVolume;
		}
		m_fOldSliderValue = m_fSliderValue;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_fOldSliderValue != m_fSliderValue)
		{
			if(Input.GetMouseButtonUp(0))
			{
				m_fOldSliderValue = m_fSliderValue;
				GetComponent<AudioSource>().Stop();
				GameObject GO = GameObject.Find("PersistantData");
				if(GO != null)
				{
					GetComponent<AudioSource>().PlayOneShot(m_aSFXSound, 0.5f + GO.GetComponent<DCScript>().m_fVoiceVolume);
				}
			}
		}
	}
	void OnGUI()
	{
		Transform _transform = transform;
		Vector3 _pos = _transform.position;
		_pos = Camera.main.WorldToScreenPoint(_pos);
		float x = _pos.x + m_fBoxXPos;
		float y = Screen.height - _pos.y + m_fBoxYPos;
		m_fSliderValue = GUI.HorizontalSlider(new Rect(x, y, m_fBoxWidth, m_fBoxHeight), m_fSliderValue, m_fMinRange, m_fMaxRange);
		
		if(exitButton.GetComponent<ExitButtonScript>().m_bSwitching == false)
		{
			GameObject gos;
			gos = GameObject.Find("SFX Volume Slider");
			if(gos)
			{
				gos.GetComponent<AudioSource>().volume = m_fSliderValue;
			}
			GameObject GO = GameObject.Find("PersistantData");
			if(GO != null)
			{
				GO.GetComponent<DCScript>().m_fVoiceVolume = m_fSliderValue - 0.5f;
				GO.GetComponent<DCScript>().AdjustValues();
			}
		}
		
	}
}
