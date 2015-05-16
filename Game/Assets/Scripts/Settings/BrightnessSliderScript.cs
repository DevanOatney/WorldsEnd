using UnityEngine;
using System.Collections;

public class BrightnessSliderScript : MonoBehaviour {

private float m_fSliderValue = 0.0f;
	public float m_fMinRange = -2.0f;
	public float m_fMaxRange = 2.0f;
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
			m_fSliderValue = GO.GetComponent<DCScript>().m_fBrightness;
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
		
		GameObject GO = GameObject.Find("PersistantData");
		if(GO != null)
		{
			if(exitButton.GetComponent<ExitButtonScript>().m_bSwitching == false)
			{
				GO.GetComponent<DCScript>().m_fBrightness = m_fSliderValue;
				GO.GetComponent<DCScript>().AdjustValues();
				Camera.main.GetComponent<Light>().intensity = GO.GetComponent<DCScript>().m_fBrightness + 1;

			}
		}
	}
}
