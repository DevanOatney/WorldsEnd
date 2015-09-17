using UnityEngine;
using System.Collections;

public class IntroSceneScript : MonoBehaviour 
{
	public GameObject m_goAudioPlayer;

	// Use this for initialization
	void Start () 
	{
		if(CAudioHelper.Instance == null)
		{
			GameObject audioPlayer = (GameObject)Instantiate(m_goAudioPlayer);
			DontDestroyOnLoad(audioPlayer);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		Application.LoadLevel("IntroMenu_Scene");
	}
}
