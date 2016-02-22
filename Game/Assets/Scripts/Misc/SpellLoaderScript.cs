using UnityEngine;
using System.Collections;

public class SpellLoaderScript : MonoBehaviour 
{
	public TextAsset m_taSpellList;
	// Use this for initialization
	void Start () 
	{
		GameObject.Find("PersistantData").GetComponent<DCScript>().m_lSpellLibrary.LoadSpells(m_taSpellList);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
