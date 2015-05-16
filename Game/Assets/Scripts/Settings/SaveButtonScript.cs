using UnityEngine;
using System.Collections;

public class SaveButtonScript : MonoBehaviour {

	Vector3 pos;
	public GUISkin skin;
	public AudioClip m_aSelectMenu;
	public GameObject exitButton;
	void Start()
	{
		pos = transform.position; 
		pos = Camera.main.WorldToScreenPoint(pos);
		pos.y = Screen.height - pos.y;
		
	}
    void OnGUI() 
	{
        
    }
	void ChangeScreen()
	{
		Application.LoadLevel("IntroMenu_Scene");
	}

	void OnMouseDown()
	{
		if(exitButton.GetComponent<ExitButtonScript>().m_bSwitching == false)
		{
			GameObject GO = GameObject.Find("PersistantData");
			if(GO != null)
			{
				GetComponent<AudioSource>().PlayOneShot(m_aSelectMenu, GO.GetComponent<DCScript>().m_fSFXVolume);
			}
			Invoke("ChangeScreen", 3.9f);
			Camera.main.SendMessage("fadeOut");
			FadeInOutSound obj = gameObject.GetComponent<FadeInOutSound>();
			StartCoroutine(obj.FadeAudio(4.0f, FadeInOutSound.Fade.Out));
		}
	}
}

