using UnityEngine;
using System.Collections;

public class InnKeeperScript : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public void BeginFade()
	{
		Camera.main.GetComponent<FadeInFadeOut>().fadeOut();
		Invoke("FadeBackIn", 2.0f);
	}
	
	void FadeBackIn()
	{
		GameObject.Find("PersistantData").GetComponent<DCScript>().RestoreParty();
		Camera.main.SendMessage("fadeIn");
		GameObject.Find("Event System").GetComponent<BaseEventSystemScript>().HandleEvent("EndDialogue");
	}

}
