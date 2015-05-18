using UnityEngine;
using System.Collections;

public class CharacterRefScript : MonoBehaviour {

	public GameObject m_gThunderShock;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool ItsMyTurn()
	{
		int dice = Random.Range(0, 3);
		if( dice == 1)
		{
			//thundershock baby!
			GetComponent<AudioSource>().PlayOneShot(GetComponent<BeserkEnemyScript>().m_acAttackAudio, 0.5f + GameObject.Find("PersistantData").GetComponent<DCScript>().m_fSFXVolume);
			GameObject thunder = Instantiate(m_gThunderShock) as GameObject;
			thunder.GetComponent<ThunderShockScript>().m_goOwner = gameObject;
			return true;
		}
		else
			//normal attack logic, target weakest enemy
			return false;
	}
}
