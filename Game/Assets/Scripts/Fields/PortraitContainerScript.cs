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
		for(int i = 0; i < m_lBriolPortraits.Count; ++i)
			m_dPortraits.Add("Briol"+i.ToString(), m_lBriolPortraits[i]);
		for(int i = 0; i < m_lCallanPortraits.Count; ++i)
			m_dPortraits.Add("Callan"+i.ToString(), m_lCallanPortraits[i]);	             
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
