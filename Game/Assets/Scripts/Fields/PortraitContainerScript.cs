using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortraitContainerScript : MonoBehaviour 
{
	public Dictionary<string,Sprite> m_dPortraits = new Dictionary<string, Sprite>();
	public List<Sprite> m_lBriolPortraits;
	public List<Sprite> m_lCallanPortraits;
	public List<Sprite> m_lMattachPortraits;
	// Use this for initialization
	void Start () 
	{
		for(int i = 0; i < m_lBriolPortraits.Count; ++i)
			m_dPortraits.Add("Briol"+i.ToString(), m_lBriolPortraits[i]);
		for(int i = 0; i < m_lCallanPortraits.Count; ++i)
			m_dPortraits.Add("Callan"+i.ToString(), m_lCallanPortraits[i]);
		for(int i = 0; i < m_lMattachPortraits.Count; ++i)
			m_dPortraits.Add("Mattach"+i.ToString(), m_lMattachPortraits[i]);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
