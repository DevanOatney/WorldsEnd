﻿using UnityEngine;
using System.Collections;

public class BackgroundScript : MonoBehaviour 
{
	public GameObject[] m_sBackgrounds;
	void Awake()
	{
	}
	// Use this for initialization
	void Start () 
	{
		SetBackground(GameObject.Find("PersistantData").GetComponent<DCScript>().GetBattleFieldBackgroundIter());
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public void SetBackground(int i)
	{
		if (i >= m_sBackgrounds.Length)
			i = m_sBackgrounds.Length - 1;
		m_sBackgrounds[i].SetActive(true);
	}
}
