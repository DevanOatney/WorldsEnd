using UnityEngine;
using System.Collections;

public class IntroMenuHighlightInput : MonoBehaviour 
{
	public GameObject m_cCamera;
	public GameObject m_gNewGame;
	public GameObject m_gContinue;
	public GameObject m_gSettings;
	public GameObject m_gExit;

	private GameObject[] m_gGameObjects = new GameObject[4];
	private int m_nHighlightedIndex;
	public void SetHighlightedIndex(int index) {m_nHighlightedIndex = index;}
	float musicFadeTime = 4.0f;
	public AudioClip m_aMoveHighlight;
	public AudioClip m_aSelectMenu;
	public AnimationClip m_acHighlightDraw;
	float m_fDrawLength;
	float m_fDrawTimer = 0.0f;
	public bool m_bAllowInput = true;
	void Awake()
	{
		GameObject GO = GameObject.Find("PersistantData");
		if(GO)
		{
			GetComponent<AudioSource>().volume = 0.5f + GO.GetComponent<DCScript>().m_fMusicVolume;
		}
	}
	// Use this for initialization
	void Start () 
	{
		m_nHighlightedIndex = 0;
		m_gGameObjects[0] = m_gNewGame;
		m_gGameObjects[1] = m_gContinue;
		m_gGameObjects[2] = m_gSettings;
		m_gGameObjects[3] = m_gExit;
		
		Vector3 position = new Vector3( m_gGameObjects[m_nHighlightedIndex].transform.position.x,  m_gGameObjects[m_nHighlightedIndex].transform.position.y,  m_gGameObjects[m_nHighlightedIndex].transform.position.z - 0.1f);
		gameObject.transform.position = position;
		FadeInOutSound obj = gameObject.GetComponent<FadeInOutSound>();
		StartCoroutine(obj.FadeAudio(2.0f, FadeInOutSound.Fade.In));

		gameObject.GetComponent<Animator>().SetBool("m_bShouldDraw", true);
		m_fDrawLength = m_acHighlightDraw.length;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bAllowInput == true)
		{
			if(Input.GetKeyUp(KeyCode.DownArrow))
			{
				++m_nHighlightedIndex;
				if(m_nHighlightedIndex > 3)
					m_nHighlightedIndex = 0;
				ChangeHighlightedPosition();
				gameObject.GetComponent<Animator>().SetBool("m_bShouldDraw", true);
				m_fDrawTimer = 0.0f;
				GetComponent<AudioSource>().PlayOneShot(m_aMoveHighlight, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
			}
			else if(Input.GetKeyUp(KeyCode.UpArrow))
			{
				--m_nHighlightedIndex;
				if(m_nHighlightedIndex < 0)
					m_nHighlightedIndex = 3;
				ChangeHighlightedPosition();
				gameObject.GetComponent<Animator>().SetBool("m_bShouldDraw", true);
				m_fDrawTimer = 0.0f;
				GetComponent<AudioSource>().PlayOneShot(m_aMoveHighlight, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
			}
			else if(Input.GetKeyUp(KeyCode.Return))
			{
				GetComponent<AudioSource>().PlayOneShot(m_aSelectMenu, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
				Invoke("ChangeScreen", musicFadeTime - 0.1f);
				Camera.main.SendMessage("fadeOut");
				FadeInOutSound obj = gameObject.GetComponent<FadeInOutSound>();
				StartCoroutine(obj.FadeAudio(musicFadeTime, FadeInOutSound.Fade.Out));
				m_bAllowInput = false;
			}
			
			
			if(gameObject.GetComponent<Animator>().GetBool("m_bShouldDraw") == true)
			{
				m_fDrawTimer += Time.deltaTime;
				if(m_fDrawTimer >= m_fDrawLength)
				{
					gameObject.GetComponent<Animator>().SetBool("m_bShouldDraw", false);
					m_fDrawTimer = 0.0f;
				}
			}
		}
	}
	
	public void ChangeHighlightedPosition()
	{

		Vector3 position = new Vector3( m_gGameObjects[m_nHighlightedIndex].transform.position.x,  m_gGameObjects[m_nHighlightedIndex].transform.position.y,  m_gGameObjects[m_nHighlightedIndex].transform.position.z - 0.1f);
		gameObject.transform.position = position;
	}
	
	void ChangeScreen()
	{
		m_gGameObjects[m_nHighlightedIndex].GetComponent<MenuSelectScript>().Enter();
	}
	
	public void MouseSelection(int index)
	{
		if(m_bAllowInput == true)
		{
			m_bAllowInput = false;
			m_nHighlightedIndex = index;
			GetComponent<AudioSource>().PlayOneShot(m_aSelectMenu, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
			Invoke("ChangeScreen", musicFadeTime - 0.1f);
			Camera.main.SendMessage("fadeOut");
			FadeInOutSound obj = gameObject.GetComponent<FadeInOutSound>();
			StartCoroutine(obj.FadeAudio(musicFadeTime, FadeInOutSound.Fade.Out));
		}
	}
}
