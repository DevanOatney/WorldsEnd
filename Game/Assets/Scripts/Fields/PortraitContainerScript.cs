using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortraitContainerScript : MonoBehaviour 
{
	public Dictionary<string,Texture2D> m_dPortraits = new Dictionary<string, Texture2D>();
	public List<Texture2D> m_lBriolPortraits;
	public List<Texture2D> m_lCallanPortraits;
	// Use this for initialization
	void Start () 
	{
		for(int i = 1; i < m_lBriolPortraits.Count+1; ++i)
			m_dPortraits.Add("Briol"+i.ToString(), m_lBriolPortraits[i-1]);
		for(int i = 1; i < m_lCallanPortraits.Count+1; ++i)
			m_dPortraits.Add("Callan"+i.ToString(), m_lCallanPortraits[i-1]);	             
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
