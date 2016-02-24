using UnityEngine;
using System.Collections;

public class StatusEffectLoaderScript : MonoBehaviour 
{
	public TextAsset m_taStatusEffects;
	// Use this for initialization
	void Start () 
	{
		gameObject.GetComponent<DCScript>().m_lStatusEffectLibrary.LoadStatusEffects(m_taStatusEffects);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
