using UnityEngine;
using System.Collections;

public class AlphaScript : MonoBehaviour {
	public float m_fAlphaValue;
	// Use this for initialization
	void Start () 
	{
		GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r,GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, m_fAlphaValue);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
