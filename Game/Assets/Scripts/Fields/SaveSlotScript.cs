﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SaveSlotScript : MonoBehaviour , IPointerClickHandler
{
	public int m_nSlotIndex;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		Object.FindObjectOfType<SavePointOrbScript> ().SaveFile (m_nSlotIndex);
	}

	#endregion
}
