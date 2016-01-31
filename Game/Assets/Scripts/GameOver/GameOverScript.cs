using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		Input.ResetInputAxes();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.anyKeyDown)
		{
			GameObject dc = GameObject.Find("PersistantData");
			if(dc)
			{
				Destroy(dc);
			}
            SceneManager.LoadScene("Intro_Scene");
		}
	}
}
