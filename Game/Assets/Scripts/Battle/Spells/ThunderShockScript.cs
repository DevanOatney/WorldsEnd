using UnityEngine;
using System.Collections;

public class ThunderShockScript : MonoBehaviour {

	public GameObject m_goOwner;
	public AnimationClip m_acShockAnim;
	float m_fDuration;
	// Use this for initialization
	void Start () 
	{
		m_fDuration = m_acShockAnim.length;
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_fDuration -= Time.deltaTime;
		if(m_fDuration <= 0.0f)
		{
			GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");
			foreach(GameObject ally in Allies)
				ally.GetComponent<UnitScript>().AdjustHP(m_goOwner.GetComponent<UnitScript>().GetTempSTR());
			m_goOwner.GetComponent<UnitScript>().m_aAnim.SetBool("m_bIsCasting", false);
			GameObject.Find("TurnWatcher").GetComponent<TurnWatcherScript>().MyTurnIsOver(m_goOwner);
			Destroy(gameObject);

		}
	}
}
