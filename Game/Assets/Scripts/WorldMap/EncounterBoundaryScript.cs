using UnityEngine;
using System.Collections;

public class EncounterBoundaryScript : MonoBehaviour 
{
	public TextAsset[] m_goEncounters;
	GameObject m_goWatcher;
	// Use this for initialization
	void Start () {
		m_goWatcher = GameObject.Find ("OverWatcher");
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if (c.gameObject.name == "Player")
		{
			m_goWatcher.GetComponent<EncounterGroupLoaderScript> ().m_szEncounters = m_goEncounters;
		}
	}
}
