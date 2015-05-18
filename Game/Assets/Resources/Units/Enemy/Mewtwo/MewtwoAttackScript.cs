using UnityEngine;
using System.Collections;

public class MewtwoAttackScript : MonoBehaviour 
{
	public GameObject m_goBeamData;
	GameObject m_goBeam;
	public AnimationClip m_acBeamAnimation;
	float m_fAnimLength;


	float timer = 0.0f;
	bool m_bBeginFiring = false;
	GameObject m_goTarget;

	// Use this for initialization
	void Start () 
	{
		m_fAnimLength = m_acBeamAnimation.length;
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bBeginFiring == true)
		{
			timer += Time.deltaTime;
			if(timer >= m_fAnimLength)
			{
				timer = 0.0f;
				m_bBeginFiring = false;
				Destroy(m_goBeam);
				m_goTarget.GetComponent<UnitScript>().AdjustHP(gameObject.GetComponent<UnitScript>().GetSTR());
				m_goTarget = null;
				GameObject.Find("TurnWatcher").GetComponent<TurnWatcherScript>().MyTurnIsOver(gameObject);
				gameObject.GetComponent<BeserkEnemyScript>().GetAnim().SetBool("m_bIsCasting", false);
			}
		}
	}
	public void BeginFiring(GameObject target)
	{
		timer = 0.0f;
		m_bBeginFiring = true;
		Vector3 dir = target.transform.position - transform.position;
		dir = target.transform.InverseTransformDirection(dir);
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		m_goBeam = Instantiate(m_goBeamData, transform.position, Quaternion.identity) as GameObject;
		m_goBeam.transform.Rotate(new Vector3(0,0,1), angle);
		m_goTarget = target;
	}
}
