using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SaveButtonScript : MonoBehaviour 
{

	public AudioClip m_aSelectMenu;
	public GameObject exitButton;
	void ChangeScreen()
	{
        SceneManager.LoadScene("IntroMenu_Scene");
	}

	public void SaveSelected()
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

