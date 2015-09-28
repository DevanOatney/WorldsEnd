using UnityEngine;
using System.Collections;

public class PortraitAnimationScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void EndAnimation()
	{
		GameObject.Find("Menu Screen").GetComponent<MenuScreenScript>().AnimationEnded();
	}
}
