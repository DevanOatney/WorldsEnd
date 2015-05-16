using UnityEngine;
using System.Collections;

public class ContinueStatPageScript : MonoBehaviour {
	public string m_szFieldName = "";
	// Use this for initialization
	
	void Awake()
	{
		GameObject GO = GameObject.Find("PersistantData");
		if(GO)
		{
			GetComponent<AudioSource>().volume = GO.GetComponent<DCScript>().m_fMusicVolume;
		}
		
	}
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	void OnGUI()
	{
	}
}
