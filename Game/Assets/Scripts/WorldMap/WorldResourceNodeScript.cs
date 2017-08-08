using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldResourceNodeScript : MonoBehaviour, IPointerClickHandler
{
	public string m_szNodeLocationName;
	GameObject m_goRoot;
	// Use this for initialization
	void Start () 
	{
		m_goRoot = gameObject.transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}


	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData)
	{
		m_goRoot.GetComponent<WorldMissionMapScript> ().ActivateRoster ();
	}
	#endregion
}
