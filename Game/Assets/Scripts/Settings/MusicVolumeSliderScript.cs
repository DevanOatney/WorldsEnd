using UnityEngine;
using System.Collections;

public class MusicVolumeSliderScript : MonoBehaviour {

	private float m_fSliderValue = 0.0f;
	public float m_fMinRange = 0.0f;
	public float m_fMaxRange = 1.0f;
	public float m_fBoxXPos = 0.0f;
	public float m_fBoxYPos = 0.0f;
	public float m_fBoxHeight = 10.0f;
	public float m_fBoxWidth = 75;
	public GameObject exitButton;
	// Use this for initialization
	void Start () 
	{
		GameObject GO = GameObject.Find("PersistantData");
		if(GO != null)
		{
			m_fSliderValue = 0.5f + GO.GetComponent<DCScript>().m_fMusicVolume;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
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
			gos = GameObject.Find("Master Volume");
			if(gos)
			{
				gos.GetComponent<AudioSource>().volume = m_fSliderValue;
			}
			GameObject GO = GameObject.Find("PersistantData");
			if(GO != null)
			{
				GO.GetComponent<DCScript>().m_fMusicVolume = m_fSliderValue - 0.5f;
				GO.GetComponent<DCScript>().AdjustValues();
			}
		}
	}
}
