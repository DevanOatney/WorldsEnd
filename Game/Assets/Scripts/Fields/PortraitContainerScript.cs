using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortraitContainerScript : MonoBehaviour 
{
	public Dictionary<string,Texture2D> m_dPortraits = new Dictionary<string, Texture2D>();
	public List<Texture2D> m_lDevanPortraits;
	public List<Texture2D> m_lMattPortraits;
	// Use this for initialization
	void Start () 
	{
		for(int i = 1; i < m_lDevanPortraits.Count+1; ++i)
			m_dPortraits.Add("Devan"+i.ToString(), m_lDevanPortraits[i-1]);
		for(int i = 1; i < m_lMattPortraits.Count+1; ++i)
			m_dPortraits.Add("Matt"+i.ToString(), m_lMattPortraits[i-1]);	             
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
